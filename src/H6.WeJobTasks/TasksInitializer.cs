using H6.WeJobTasks.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace H6.WeJobTasks
{
  public class TasksInicializer
  {
    private readonly SettingsOptions _settingsOptions;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly string InstanceId;

    private readonly Dictionary<string, WebJobTask> _initializedTasks = new Dictionary<string, WebJobTask>();

    public System.Threading.Tasks.Task[] AllTasks() => _initializedTasks.Values.Select(i=>i).Where(a => a .Runner != null).Select(a => a.Runner).ToArray();

    private void LogInfo(string message)
    {
      _logger.LogInformation($"{InstanceId}: {message}");
    }

    private void LogCritical(Exception ex, string message ="")
    {
      _logger.LogCritical(ex, $"{InstanceId}: {message}; Ex: {ex}");
    }


    public TasksInicializer(IOptions<SettingsOptions> settingsOptions, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
      _settingsOptions = settingsOptions?.Value;
      _loggerFactory = loggerFactory;
      _logger = loggerFactory.CreateLogger<TasksInicializer>();
      _serviceProvider = serviceProvider;

      InstanceId = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
    }
   
    public async System.Threading.Tasks.Task RunLoop(CancellationToken cancellationToken)
    {
      LogInfo("Start Loop");

      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          InitializeTasks(cancellationToken);
          if (cancellationToken.IsCancellationRequested) break;

          foreach (var key in _initializedTasks.Keys)
          {
            if (cancellationToken.IsCancellationRequested) break;
            var task = _initializedTasks[key];
            if (task.TimeoutExpired)
            {
              LogCritical(null, $"{task.TaskName} - Time for process in task expired. Start: {task.LastStart}, Timeout: {task.Timeout}");
              try
              {
                task.Abort();
              }
              catch (Exception ex)
              {
                LogCritical(ex, $"{task.TaskName} - abort failed");
              }
              continue;
            }

            if (task.Running) continue;

            if (!task.Running && task.NextRun <= DateTime.UtcNow)
            {
              task.Start();
            }

            await System.Threading.Tasks.Task.Delay(100, cancellationToken);
          }
          await System.Threading.Tasks.Task.Delay(500, cancellationToken);
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
          LogInfo("Service correctly aborted");
          await System.Threading.Tasks.Task.Delay(500, cancellationToken);
        }
        catch (Exception ex)
        {
          LogCritical( ex);
          await System.Threading.Tasks.Task.Delay(5000, cancellationToken);
        }
      }

      LogInfo("End Loop");
    }

    private string GetAssembly(string defaultAssembly, string assembly)
    {
      if (string.IsNullOrWhiteSpace(defaultAssembly)) throw new Exception("DefaultAssembly is not set");
      return string.IsNullOrWhiteSpace(assembly) ? defaultAssembly : assembly;
    }

    private string GetNamespace(string defaultNameSpace, string nmespace)
    {
      if (string.IsNullOrWhiteSpace(defaultNameSpace)) throw new Exception("DefaultNameSpace is not set");
      return string.IsNullOrWhiteSpace(nmespace) ? defaultNameSpace : nmespace;
    }

    private string TaskFullName(string defaultAssembly, string defaultNamesace, TaskDefinition taskDefinition)
    {
      var key = $"{GetAssembly(defaultAssembly, taskDefinition.Assembly)}::{GetNamespace(defaultNamesace,taskDefinition.Namespace)}.{taskDefinition.Class}";
      return key;
    }

    /// <summary>
    /// initialize tasks
    /// </summary>
    /// <param name="token"></param>
    private void InitializeTasks(CancellationToken token)
    {
      var tasksDefinitions = _settingsOptions.WeJobTasks;
      foreach (var taskDef in tasksDefinitions.Tasks)
      {
        if (token.IsCancellationRequested) return;
        try
        {
          CreateNewTask(tasksDefinitions.DefaultAssembly, tasksDefinitions.DefaultNamespace, taskDef,token);
        }
        catch (Exception ex)
        {
          LogCritical(ex, $"Not initialized task {taskDef.Class}");
        }
      }
    }

    /// <summary>
    /// create new task and intialize it
    /// </summary>
    /// <param name="defaultAssembly"></param>
    /// <param name="defaultNamespace"></param>
    /// <param name="taskDef"></param>
    /// <param name="token"></param>
    private void CreateNewTask(string defaultAssembly, string defaultNamespace, TaskDefinition taskDef, CancellationToken token)
    {
      var fullName = TaskFullName(defaultAssembly, defaultNamespace, taskDef);

      if (_initializedTasks.ContainsKey(fullName)) return;

      LogInfo($"Start create task - {fullName}");

      var assembly = GetAssembly(defaultAssembly, taskDef.Assembly);
      var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                              .Where(a => a.FullName.ToLower()
                                                                    .StartsWith(assembly.ToLower()))
                                              .ToList();
      if (assemblies.Count == 0)
      {
        var asm = AppDomain.CurrentDomain.Load(assembly);
        if (asm == null) throw new Exception($"for '{assembly}': Unable to load {assembly}.dll");
        assemblies.Add(asm);
      }

      object instance = null;
      var nmespace = GetNamespace(defaultNamespace, taskDef.Namespace);
      var fullClassName = string.Format("{0}.{1}", nmespace, taskDef.Class);

      foreach (var asm in assemblies.OrderBy(a => a.FullName))
      {
        Type type = asm.GetType(fullClassName);
        if (type != null)
        {
          instance = Activator.CreateInstance(type);
          break;
        }
      }
      if (instance == null) throw new Exception($"Unable to create instance of {fullName}");

      var newTask = (WebJobTask)instance;
      newTask.Logger = _loggerFactory.CreateLogger<WebJobTask>();
      newTask.LoggerFactory = _loggerFactory;
      newTask.TaskName = fullName;
      newTask.RunAfterMidnight = taskDef.RunAfterMidnight;
      newTask.NextRun = DateTime.UtcNow.AddMinutes(newTask.RunAfterMidnight);
      newTask.Period = taskDef.Period;
      newTask.Timeout = taskDef.Timeout;
      newTask.InstanceId = this.InstanceId;

      newTask.InitTask(token, _serviceProvider);

      _initializedTasks.Add(fullName, newTask);

      LogInfo($"End create task - {fullName}");
    }
  }
}
