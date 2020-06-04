using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace H6.Threading
{
  public sealed class LockerManager
  {
		/// <summary>
		/// List object to which is applied lock
		/// </summary>
		private Dictionary<string, object> _lockerCollection = new Dictionary<string, object>(255);

		#region public object GetLocker(string key)
		/// <summary>
		/// Returns an object to which to apply the lock
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns></returns>
		public object GetLocker(string key)
		{
			object o;

			if (!_lockerCollection.TryGetValue(key, out o))
			{
				lock (((ICollection)_lockerCollection).SyncRoot)
				{
					if (!_lockerCollection.TryGetValue(key, out o))
					{
						o = new object();
						_lockerCollection[key] = o;
					}
				}
			}
			return o;
		}
		#endregion
	}
}
