using System;
using ETModel;

namespace ETHotfix
{
	public static class Log
	{
		public static void Warning(string msg)
		{
		    if (!NetConfig.getInstance().isFormal)
                ETModel.Log.Warning(msg);
		}

		public static void Info(string msg)
		{
		    if (!NetConfig.getInstance().isFormal)
                ETModel.Log.Info(msg);
		}

		public static void Error(Exception e)
		{
		    if (!NetConfig.getInstance().isFormal)
                ETModel.Log.Error(e.ToStr());
		}

	    public static void Error(string msg)
		{
		    if (!NetConfig.getInstance().isFormal)
                ETModel.Log.Error(msg);
        }

		public static void Debug(string msg)
		{
		    if (!NetConfig.getInstance().isFormal)
                ETModel.Log.Debug(msg);
        }
	}
}