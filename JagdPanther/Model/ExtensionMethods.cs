using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther
{
	public static class ExtensionMethods
	{
		public static DateTime LastTime { get;set; }
		private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long ToUnixTimeSeconds(this DateTimeOffset dateTime)
		{
			double nowTicks = (dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
			return (long)nowTicks;
		}


	}
}
