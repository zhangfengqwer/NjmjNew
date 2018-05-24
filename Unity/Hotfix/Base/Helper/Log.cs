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
		}

		public static void Error(string msg)
		{
			ETModel.Log.Error(msg);

		    using (UnityWebRequestAsync webRequestAsync = ETModel.ComponentFactory.Create<UnityWebRequestAsync>())
		    {

//                webRequestAsync.DownloadAsync(versionUrl);
		    }
        }

		public static void Debug(string msg)
		{
			ETModel.Log.Debug(msg);
		}
	}
}