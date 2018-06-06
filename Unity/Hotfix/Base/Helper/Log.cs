using System;
using ETModel;

namespace ETHotfix
{
	public static class Log
	{
		public static void Warning(string msg)
		{
			ETModel.Log.Warning(msg);
		}

		public static void Info(string msg)
		{
			ETModel.Log.Info(msg);
		}

		public static void Error(Exception e)
		{
			ETModel.Log.Error(e.ToStr());
		    //HttpReqUtil.Req($"http://10.224.4.158:8080/GetClientError?data=" + e.ToStr());
		}

	    public static void Error(string msg)
		{
		    ETModel.Log.Error(msg);
        }

		public static void Debug(string msg)
		{
			ETModel.Log.Debug(msg);
		}
	}
}