using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
	public static class BundleHelper
	{
		public static async Task DownloadBundle()
		{
		    Game.EventSystem.Run(EventIdType.LoadingBegin);
		    Log.Debug("等待之前");
		    await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1500);
		    PlatformHelper.SetIsFormal(NetConfig.getInstance().isFormal ? "0" : "1");
		    Log.Debug("等待之后");
            await StartDownLoadResources();
            Game.EventSystem.Run(EventIdType.LoadRes);
        }

        public static async Task StartDownLoadResources()
		{
			if (Define.IsAsync)
			{
				try
				{
					using (BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.AddComponent<BundleDownloaderComponent>())
					{
					  
					    Log.Debug("开始下载");
                        await bundleDownloaderComponent.StartAsync();
					    Log.Debug("结束下载");
                    }
					Game.Scene.GetComponent<ResourcesComponent>().LoadOneBundle("StreamingAssets");
					ResourcesComponent.AssetBundleManifestObject = (AssetBundleManifest)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("StreamingAssets", "AssetBundleManifest");
                }
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
    }
}
