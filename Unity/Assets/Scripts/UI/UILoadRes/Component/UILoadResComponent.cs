using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
	[ObjectSystem]
	public class UiLoadResComponentAwakeSystem : AwakeSystem<UILoadResComponent>
	{
		public override void Awake(UILoadResComponent self)
		{
			//self.text = self.GetParent<UI>().GameObject.Get<GameObject>("Text").GetComponent<Text>();
		}
	}

	[ObjectSystem]
	public class UiLoadResComponentStartSystem : StartSystem<UILoadResComponent>
	{
        static List<string> fileList = new List<string>()
        {
            "uiaccountbind.unity3d",
            "uibindphone.unity3d",
            "uichangename.unity3d",
            "uichat.unity3d",
            "uichatitem.unity3d",
            "uishop.unity3d",
            "uiactivity.unity3d",
            "uiactivity_101.unity3d",
            "uitask.unity3d",
            "uichengjiu.unity3d",
            "uichengjiuitem.unity3d",
            "uibag.unity3d",
            "uibagitem.unity3d",
            "uizhuanpan.unity3d",
            "uidaily.unity3d",
            "uiemail.unity3d",
            "uihelp.unity3d",
            "uiplayerinfo.unity3d",
            "uiplayericon.unity3d",
            "uiemail.unity3d",
            "uiemailitem.unity3d",
            "uiexpression.unity3d",
            "uigameresult.unity3d",
            "uigolditem.unity3d",
            "uiicon.unity3d",
            "uijiazhangjianhu.unity3d",
            "uilobby.unity3d",
            "uilogin.unity3d",
            "uimain.unity3d",
            "uineterror.unity3d",
            "uinoticeitem.unity3d",
            "uipropitem.unity3d",
            "uirank.unity3d",
            "uirankitem.unity3d",
            "uirealname.unity3d",
            "uirewarditem.unity3d",
            "uiset.unity3d",
            "uitaskitem.unity3d",
            "uiusehuafei.unity3d",
            "uiuselaba.unity3d",
            "uivip.unity3d",
            "uivipitem.unity3d",
            "uiwingitem.unity3d",

            "image_daily.unity3d",
            "image_gameresult.unity3d",
            "image_help.unity3d",
            "image_login.unity3d",
            "image_main.unity3d",
            "image_shop.unity3d",
            "image_task.unity3d",
            "image_zhuanpan.unity3d",
            "playericon.unity3d",
            "uichengjiuicon.unity3d",

            "uiroom.unity3d",
            "uiready.unity3d",
            "uichatshow.unity3d",
            "ui.unity3d",
        };

		public override async void Start(UILoadResComponent self)
		{
          
            // 检测apk更新
            if (true)
            {
                using (UnityWebRequestAsync webRequestAsync = ETModel.ComponentFactory.Create<UnityWebRequestAsync>())
                {
                    await webRequestAsync.DownloadAsync("http://fwdown.hy51v.com/njmj/online/files/versionconfig.json");
                    string data = webRequestAsync.Request.downloadHandler.text;
                    ApkVersionConfig.getInstance().init(data);
                    Log.Debug("channel_name = " + PlatformHelper.GetChannelName());
                    Log.Debug("VersionName = " + PlatformHelper.GetVersionName());
                    VersionInfo versionInfo = ApkVersionConfig.getInstance().getDataById(PlatformHelper.GetChannelName());
                    if (versionInfo != null)
                    {
                        // apk更新
                        if (versionInfo.version.CompareTo(PlatformHelper.GetVersionName()) > 0)
                        {
                            DownApkScript.create();
                            
                            return;
                        }
                    }
                }
            }
            
            await LoadRes();

            ToastScript.createToast("加载完毕");

            Game.EventSystem.Run(EventIdType.LoadingFinish);
        }

        // 加载资源
        public static async Task LoadRes()
        {
            string fileName = "";
            try
            {
                string versionPath = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();

                VersionConfig localVersionConfig = JsonHelper.FromJson<VersionConfig>(File.ReadAllText(versionPath));

                //foreach (var data in localVersionConfig.FileInfoDict)
                //{
                //    fileName = data.Value.File;
                //    if ((fileName.Equals("Version.txt")) ||
                //        (fileName.Equals("StreamingAssets")))
                //    {
                //        continue;
                //    }

                //    await resourcesComponent.LoadBundleAsync(fileName);
                //}

                foreach (var str in fileList)
                {
                    fileName = str;
                    if ((fileName.Equals("Version.txt")) ||
                        (fileName.Equals("StreamingAssets")))
                    {
                        continue;
                    }

                    await resourcesComponent.LoadBundleAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("LoadRes异常：" + ex + "----" + fileName);
            }
        }
    }

	public class UILoadResComponent : Component
	{
		public Text text;
	}
}
