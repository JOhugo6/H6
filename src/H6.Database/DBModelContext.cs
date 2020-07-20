using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace H6.Database
{
  public abstract class DBModelContext : DbContext
  {
    private readonly ILogger<DBModelContext> _logger;
    
    

    public DBModelContext(ILogger<DBModelContext> logger, DbContextOptions options) : base(options)
    {
      _logger = logger;
    }

    private static List<Type> _entitiesToRegister = null;

    private static Threading.LockerManager _lockerManager = new Threading.LockerManager();

    protected abstract HashSet<string> GetProjectsWithDBEntities();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      var projectsWithEntities = GetProjectsWithDBEntities();
      if(projectsWithEntities != null && projectsWithEntities.Count>0)
      {
        foreach (var i in projectsWithEntities) RegisterEntitiesToContext(modelBuilder, i);
      }
    }

    private static Dictionary<string, List<Type>> _typesToRegistration = new Dictionary<string, List<Type>>();
    private List<Type> EnsureTypesForRegistation(string assembly)
    {
      { if (_typesToRegistration.TryGetValue(assembly, out List<Type> types)) return types; }
      
      lock (_lockerManager.GetLocker(assembly))
      {
        if (_typesToRegistration.TryGetValue(assembly, out List<Type> types)) return types;

        var typesToRegister = Assembly.Load(assembly)
                                      .GetTypes()
                                      .Where(type => !String.IsNullOrEmpty(type.Namespace)
                                                      && type.BaseType != null 
                                                      && type.BaseType.IsGenericType
                                                      && type.BaseType.GetGenericTypeDefinition() == typeof(Entities.DBEntityBase<,>))
                                      .ToList();
        _typesToRegistration[assembly] = typesToRegister;
        return typesToRegister;
      }
    }

    private void RegisterEntitiesToContext(ModelBuilder modelBuilder, string assemblyName)
    {
      var typesToRegister = EnsureTypesForRegistation(assemblyName);
      if (typesToRegister != null && typesToRegister.Count > 0) RegisterTypesToContext(modelBuilder, typesToRegister);
    }

    private void RegisterTypesToContext(ModelBuilder modelBuilder, IEnumerable<Type> types)
    {
      foreach (var type in types)
      {
        var mi = EnsureMethodInfo(type);
        mi.Item2.Invoke(mi.Item1, new object[] { modelBuilder });
      }

    }

    private static Dictionary<string, Tuple<object,MethodInfo>> _allMethodsOnModelCreating = new Dictionary<string, Tuple<object, MethodInfo>>();

    private Tuple<object, MethodInfo> EnsureMethodInfo(Type type)
    {
      var assemblyName = type.Assembly.FullName;
      var typeName = type.FullName;
      var key = $"{assemblyName}+{typeName}";
      {
        if (_allMethodsOnModelCreating.TryGetValue(key, out Tuple<object, MethodInfo> method)) return method;
      }
      lock (_lockerManager.GetLocker(key))
      {
        if (_allMethodsOnModelCreating.TryGetValue(key, out Tuple<object, MethodInfo> method)) return method;

        var mi = type.GetMethod("OnModelCreating", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly);
        
        var cnti = type.GetConstructor(Type.EmptyTypes);
        var o = cnti.Invoke(new object[] { });
        
        var t = new Tuple<object, MethodInfo>(o, mi);

        _allMethodsOnModelCreating[key] = t;
        return t;
      }
    }

    public override void Dispose()
    {
      if (this.Database.CurrentTransaction != null)
      {
        this.Database.CurrentTransaction.Dispose();   //rollback if needed
      }
      base.Dispose();
    }

    private StringBuilder HandleDbUpdateException(DbUpdateException ex)
    {
      var log = new StringBuilder();

      var dataSnapshot = MakeSnapshot();
      log.AppendLine("DB Context data snapshot:");
      log.AppendLine(dataSnapshot.ToString());
      return log;
    }

    private Dictionary<string, string> GetValues(EntityEntry entityEntry, IEnumerable<IProperty> properties)
    {
      var values = new Dictionary<string, string>();

      if (properties == null) properties = entityEntry.CurrentValues.Properties.OrderBy(p => p.Name);

      foreach (var entityProperty in properties)
      {
        var valueObject = entityProperty.PropertyInfo.GetValue(entityEntry.Entity);
        var value = string.Empty;

        if (valueObject == null) value = "null";
        else
        {
          if (entityProperty.ClrType.IsArray && entityProperty.ClrType.IsAssignableFrom(typeof(byte[])))
          {
            var valueArray = (byte[])entityProperty.PropertyInfo.GetValue(entityEntry.Entity);

            if (valueArray.Length <= 8000)
              value = Convert.ToBase64String(valueArray);
            else
              value = $"Size: {valueArray.Length} B.";
          }
          else
            value = entityProperty.PropertyInfo.GetValue(entityEntry.Entity).ToString();
        }
        values.Add(entityProperty.Name, value);
      }
      return values;
    }

    private StringBuilder MakeSnapshot()
    {
      var log = new StringBuilder();

      // get added entities
      var dbContextEntities = this.ChangeTracker.Entries()
                                      .Where(e => e.State == EntityState.Added)
                                      .OrderBy(e => e.Entity.GetType().Name)
                                      .ToList();

      {
        foreach (var entityEntry in dbContextEntities)
        {
          var values = GetValues(entityEntry, null);
          var entityLog = $"Create -> [ Entity: {entityEntry.Entity.GetType().Name} ; Data: {System.Text.Json.JsonSerializer.Serialize(values)} ]";
          log.AppendLine(entityLog);
        }
      }



      // get modified entities
      var modifiedEntitiesToTrace = this.ChangeTracker.Entries()
                                        .Where(e => e.State == EntityState.Modified)
                                        .OrderBy(e => e.Entity.GetType().Name)
                                        .ToList();

      {
        foreach (var entityEntry in modifiedEntitiesToTrace)
        {
          var modifiedProperties = entityEntry.CurrentValues.Properties
                                                            .Where(i =>
                                                                        {
                                                                          var original = entityEntry.OriginalValues[i.Name];
                                                                          var current = entityEntry.CurrentValues[i.Name];

                                                                          if (!Equals(original, current))
                                                                          {
                                                                            return true;
                                                                          }

                                                                          return false;
                                                                        })
                                                            .ToList();

          if (modifiedProperties.Count > 0)
          {
            string entityRecordId = GetEntityRecordId(entityEntry.Entity);

            var values = GetValues(entityEntry,modifiedProperties);

            var entityLog = $"Update -> [ Entity: {entityEntry.Entity.GetType().Name} ; Record ID: {entityRecordId} ; Data: {System.Text.Json.JsonSerializer.Serialize(values)} ]";

            log.AppendLine(entityLog);
          }
        }
      }
      return log;
    }

    private string GetEntityRecordId(object entity)
    {
      if (entity is Entities.IPrimaryKeyAsId<IComparable>) return (entity as Entities.IPrimaryKeyAsId<IComparable>).Id.ToString();

      var propertyInfo = entity.GetType().GetProperty("Id");
      if (propertyInfo != null)
      {
       var value = propertyInfo.GetValue(entity);
        if (value != null) return value.ToString();
      }
      return "0";
    }

    public override int SaveChanges()
    {
      ChangeTracker.DetectChanges();

      ChangeTracker.Entries<Entities.IModifiedOn>()
          .Where(e => e.State == EntityState.Modified && !e.Property("ModifiedOn").IsModified).ToList()
          .ForEach(e => e.Property("ModifiedOn").CurrentValue = DateTime.UtcNow);

      var logMsg = new StringBuilder();
      try
      {
        return base.SaveChanges();
      }
      catch (DbUpdateException dbUpdateException)
      {
        try
        {
          var exceptionDetail = HandleDbUpdateException(dbUpdateException).ToString();
          logMsg.AppendLine(exceptionDetail);
        }
        catch (Exception ex)
        {
          logMsg.AppendLine(ex.ToString());
        }

        _logger.LogError(dbUpdateException, logMsg.ToString());
        throw dbUpdateException;
      }
      catch (Exception ex)
      {
        logMsg.AppendLine(ex.Message);
        _logger.LogError(ex, logMsg.ToString());
        throw ex;
      }
    }
  }
}
