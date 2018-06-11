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
		    await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
//		    Log.Debug("GlobalConfigComponent.Instance.GlobalProto.GetUrl()" + GlobalConfigComponent.Instance.GlobalProto.GetUrl());
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
						await bundleDownloaderComponent.StartAsync();
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
