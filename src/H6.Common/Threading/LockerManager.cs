using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace H6.Threading
{
	/// <summary>
	/// Locker Manager contains list of object for lock
	/// </summary>
  public sealed class LockerManager
  {
		/// <summary>
		/// List object to which is applied lock
		/// </summary>
		private Dictionary<string, object> _lockersCollection = new Dictionary<string, object>(255);

		#region public object GetLocker(string key)
		/// <summary>
		/// Returns an object for lock
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns></returns>
		public object GetLocker(string key)
		{
			if (!_lockersCollection.TryGetValue(key, out object o))
			{
				lock (((ICollection)_lockersCollection).SyncRoot)
				{
					if (!_lockersCollection.TryGetValue(key, out o))
					{
						o = new object();
						_lockersCollection[key] = o;
					}
				}
			}
			return o;
		}
		#endregion
	}
}
