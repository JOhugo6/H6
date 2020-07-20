using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace H6.Extensions
{
  public static class Type_Extensions
  {
		/// <summary>
		/// Check if type is nullable
		/// </summary>
		/// <param name="typ"></param>
		/// <returns></returns>
    public static bool IsNullable(this Type typ)
    {
      return typ.IsGenericType && typ.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

		/// <summary>
		/// Convert string the typed value.
		/// </summary>
		/// <param name="t">The t.</param>
		/// <param name="inputString">The input string.</param>
		/// <returns></returns>
		public static object ToTypedValue(this Type t, string inputString)
		{
			if (t == (typeof(string)))
			{
				return inputString;
			}
			if (t == typeof(byte))
			{
				return Convert.ToByte(inputString);
			}
			if (t == typeof(int))
			{
				return Convert.ToInt32(inputString);
			}
			if (t == typeof(Int16))
			{
				return Convert.ToInt16(inputString);
			}
			if (t == typeof(Int32))
			{
				return Convert.ToInt32(inputString);
			}
			if (t == typeof(Int64))
			{
				return Convert.ToInt64(inputString);
			}
			if (t == typeof(byte?))
			{
				byte outInt;
				if (!byte.TryParse(inputString, out outInt))
				{
					return null;
				}
				return outInt;
			}
			if (t == typeof(int?))
			{
				int outInt;
				if (!int.TryParse(inputString, out outInt))
				{
					return null;
				}
				return outInt;
			}
			if (t == typeof(Int16?))
			{
				Int16 outInt;
				if (!Int16.TryParse(inputString, out outInt))
				{
					return null;
				}
				return outInt;
			}
			if (t == typeof(Int32?))
			{
				Int32 outInt;
				if (!Int32.TryParse(inputString, out outInt))
				{
					return null;
				}
				return outInt;
			}
			if (t == typeof(Int64?))
			{
				Int64 outInt;
				if (!Int64.TryParse(inputString, out outInt))
				{
					return null;
				}
				return outInt;
			}
			if (t == typeof(bool))
			{
				return Convert.ToBoolean(inputString);
			}
			if (t == typeof(bool?))
			{
				bool outBool;
				if (bool.TryParse(inputString, out outBool))
				{
					return outBool;
				}
				return null;
			}
			if (t == typeof(decimal))
			{
				inputString = ConvertDecimalToEngStyle(inputString);
				return Convert.ToDecimal(inputString, System.Globalization.CultureInfo.InvariantCulture);
			}
			if (t == typeof(decimal?))
			{
				inputString = ConvertDecimalToEngStyle(inputString);
				decimal outDecimal;
				if (!decimal.TryParse(inputString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out outDecimal))
				{
					return null;
				}
				return outDecimal;
			}
			if (t == typeof(DateTime))
			{
				return Convert.ToDateTime(inputString);
			}
			if (t == typeof(DateTime?))
			{
				DateTime outDateTime;
				if (!DateTime.TryParse(inputString, out outDateTime))
				{
					return null;
				}
				return outDateTime;
			}

			return inputString;
		}

		/// <summary>
		/// convert decimal to eng. style
		/// </summary>
		/// <param name="sNumber">number</param>
		/// <returns>decimal in eng. style</returns>
		static string ConvertDecimalToEngStyle(string sNumber)
		{
			string retVal = string.Empty;
			if (sNumber != null)
			{
				retVal = sNumber.Replace(",", ".");
			}
			return retVal;
		}

		/// <summary>
		/// Convert value to type
		/// </summary>
		/// <param name="typ">type of value</param>
		/// <param name="inputObject">input value</param>
		/// <returns>typed value</returns>
		public static object ToTypedValue(this Type typ, object inputObject)
		{
			if (inputObject == null) return null;
			else if (inputObject.GetType() == typ)
			{
				return inputObject;
			}

			else
			{
				if (inputObject.ToString().Trim() == "") return null;

				return ToTypedValue(typ, inputObject.ToString());
			}
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		public static PropertyInfo GetProperty(this Type sourceType, string propertyName)
		{
			MemberInfo[] mia = sourceType.GetMember(propertyName,
				MemberTypes.Property,
				BindingFlags.Public |
				BindingFlags.GetProperty |
				BindingFlags.IgnoreCase |
				BindingFlags.Instance);
			foreach (MemberInfo mi in mia)
			{
				if (mi != null && mi is PropertyInfo)
				{
					PropertyInfo pi = (PropertyInfo)mi;
					if (pi.CanRead)
					{
						return pi;
					}
				}
			}
			return null;
		}

		/// <summary>
		///  Returns a list of all the type of field, and his parents up to the type System.Object
		/// </summary>
		/// <param name="type">Type for which to return all fields</param>
		/// <returns>List of all fields</returns>
		public static FieldInfo[] GetAllFields(this Type type)
		{
			List<FieldInfo> fields = new List<FieldInfo>();
			fields.AddRange(type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public));

			if (!type.BaseType.Equals(typeof(object)))
			{
				fields.AddRange(type.BaseType.GetAllFields());
			}

			return fields.ToArray();
		}


		private static Dictionary<Type, List<PropertyInfo>> _properties = new Dictionary<Type, List<PropertyInfo>>();
		private static Threading.LockerManager _propertiesLocker = new Threading.LockerManager();

		/// <summary>
		/// Return all properties - only public
		/// </summary>
		/// <param name="type">Type of the object.</param>
		/// <param name="onlySelf">Only self properties</param>
		/// <returns></returns>
		public static PropertyInfo[] GetAllProperties(this Type type, bool onlySelf)
		{
			List<PropertyInfo> properties = null;
			if (!_properties.TryGetValue(type, out properties))
			{
				lock (_propertiesLocker.GetLocker(type.FullName))
				{
					if (!_properties.TryGetValue(type, out properties))
					{
						BindingFlags mflags = BindingFlags.Public | BindingFlags.Instance;
						properties = new List<PropertyInfo>();
						properties.AddRange(type.GetProperties(mflags));
						_properties[type] = properties;
					}
				}
			}

			return onlySelf ? properties.Where(p => p.DeclaringType == type).ToArray() : properties.ToArray();
		}

		/// <summary>
		/// Returns all events of type and all his subtypes
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static List<EventInfo> GetAllEvents(this Type type)
		{
			var events = new List<EventInfo>();
			if (type != null)
			{
				events.AddRange(type.GetEvents(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public));

				if (type.BaseType != null
					&& !type.BaseType.Equals(typeof(object)))
				{
					events.AddRange(type.BaseType.GetAllEvents());
				}
			}

			return events;
		}

		/// <summary>
		/// is it value type?
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsValueType(this Type type)
		{
			var retVal = type.IsValueType || type == typeof(string);
			return retVal;
		}

		/// <summary>
		/// Is type of delegate?
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDelegate(this Type type)
		{
			bool retVal = false;

			if (type == typeof(System.Delegate))
				retVal = true;
			else if (type == typeof(object))
				retVal = false;
			else if (type.BaseType == null)
				retVal = false;
			else
				retVal = type.BaseType.IsDelegate();

			return retVal;
		}
	}
}
