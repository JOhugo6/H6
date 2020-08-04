using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Specialized;
using H6.Extensions;

namespace H6.Common
{
	public static partial class ReflectionHelper
	{
		/// <summary>
		/// Set value in object
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="propertyValue">Set value.</param>
		/// <returns></returns>
		public static bool SetValue(object source, string propertyName, object propertyValue)
		{
			if (source == null)
			{
				return false;
			}

			System.Type sourceType = source.GetType();
			BindingFlags mflags = BindingFlags.Public
			| BindingFlags.SetProperty
			| BindingFlags.IgnoreCase
			| BindingFlags.Instance;

			MemberInfo[] mmi = sourceType.GetMember(propertyName,
				mflags);

			foreach(var mfi in mmi.Where(mm=> mm !=null && mm is PropertyInfo).Select(mm=> (PropertyInfo)mm))
      {
				return SetValuePropertyInfo(source, mfi, propertyValue);
			}
			return false;
		}

		/// <summary>
		/// Set value in object
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="mfi">Property in object</param>
		/// <param name="propertyValue">Set value.</param>
		/// <returns></returns>
		public static bool SetValuePropertyInfo(object source, PropertyInfo mfi, object propertyValue)
		{
			if (mfi.CanWrite)
			{
				object hodnota = mfi.PropertyType.ToTypedValue(propertyValue);
				if (hodnota != null)
				{
					Type propertyType = mfi.PropertyType;
					if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						propertyType = Nullable.GetUnderlyingType(propertyType);
						if (propertyType.IsEnum)
						{
							mfi.SetValue(source, Enum.Parse(propertyType, (string)hodnota), null);
						}
						else
						{
							mfi.SetValue(source, hodnota, null);
						}
					}
					else
					{
						if (propertyType.IsEnum)
						{
							mfi.SetValue(source, Enum.Parse(propertyType, (string)hodnota), null);
						}
						else
						{
							mfi.SetValue(source, hodnota, null);
						}
					}
					return true;
				}
				else
				{
					try
					{

						mfi.SetValue(source, hodnota, null);
					}
#pragma warning disable 168
					catch (Exception ex)
					{
						;
					}
#pragma warning restore 168
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		public static object GetPropertyValue(object source, string propertyName)
		{
			bool exists;

			var result = GetPropertyValue(source, propertyName, out exists);
			if (!exists)
				throw new Exception("Not found property " + propertyName + " in object " + source);

			return result;
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="exists">if set to <c>true</c> [exists].</param>
		/// <returns></returns>
		public static object GetPropertyValue(object source, string propertyName, out bool exists)
		{
			BindingFlags mflags = BindingFlags.Public
									| BindingFlags.GetProperty
									| BindingFlags.IgnoreCase
									| BindingFlags.Instance;

			return GetPropertyValue(source, propertyName, mflags, out exists);
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="mflags">Binding Flags</param>
		/// <param name="exists">if set to <c>true</c> [exists].</param>
		/// <returns></returns>
		public static object GetPropertyValue(object source, string propertyName, BindingFlags mflags, out bool exists)
		{
			System.Type sourceType = source.GetType();
			MemberInfo[] mmi;


			foreach (var mfi in sourceType.GetMember(propertyName, mflags).Where(mm => mm != null && mm is PropertyInfo).Select(mm => (PropertyInfo)mm))
			{
				if (mfi.CanRead)
				{
					exists = true;
					return mfi.GetValue(source, null);
				}
			}

			exists = false;
			return null;
		}

		/// <summary>
		/// Clones the object - object must have Serialized attribute.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public static object CloneObject(object item)
		{
			// HACK: handler AssemblyResolve udalosti zamezi vyjimce, ktera vyskakuje
			// pokud se deserializuje objekt tridy dynamicky nactene assembly
			ResolveEventHandler resolveHandler = new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			AppDomain.CurrentDomain.AssemblyResolve += resolveHandler;

			try
			{
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

				using (Stream stream = new MemoryStream())
				{
					formatter.Serialize(stream, item);
					stream.Seek(0, SeekOrigin.Begin);
					return formatter.Deserialize(stream);
				}
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= resolveHandler;
			}
		}

		/// <summary>
		/// Pomaha pri problemech s deserializaci objektu z dynamicky
		/// nactenych assembly. Viz: http://www.pcreview.co.uk/forums/thread-3676040.php
		/// </summary>
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly ayResult = null;
			string sShortAssemblyName = args.Name.Split(',')[0];
			Assembly[] ayAssemblies =
			AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly ayAssembly in ayAssemblies)
			{
				if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
				{
					ayResult = ayAssembly;
					break;
				}
			}
			return ayResult;
		}
	}
}
