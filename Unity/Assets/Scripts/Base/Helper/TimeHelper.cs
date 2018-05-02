using System;

namespace ETModel
{
	public static class TimeHelper
	{
		private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
		/// <summary>
		/// 客户端时间
		/// </summary>
		/// <returns></returns>
		public static long ClientNow()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000;
		}

		public static long ClientNowSeconds()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000000;
		}

		/// <summary>
		/// 登陆前是客户端时间,登陆后是同步过的服务器时间
		/// </summary>
		/// <returns></returns>
		public static long Now()
		{
			return ClientNow();
		}

	    /// <summary>
	    /// 得到当天的日期 格式：yyyy-MM-dd
	    /// </summary>
	    /// <param name="dateTime"></param>
	    /// <returns></returns>
	    public static string GetCurrentDay(this DateTime dateTime)
	    {
	        return dateTime.ToString("yyyy-MM-dd");
	    }

	    /// <summary>
	    /// 得到现在的时间 格式：yyyy-MM-dd HH:mm:ss
	    /// </summary>
	    /// <param name="dateTime"></param>
	    /// <returns></returns>
	    public static string GetCurrentTime(this DateTime dateTime)
	    {
	        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
	    }
    }
}