using System;
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
		public override async void Start(UILoadResComponent self)
		{
            LoadRes();

            Game.EventSystem.Run(EventIdType.LoadingFinish);
        }

        // 加载资源
        public static void LoadRes()
        {
            string fileName = "";
            try
            {
                string versionPath = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();

                VersionConfig localVersionConfig = JsonHelper.FromJson<VersionConfig>(File.ReadAllText(versionPath));

                foreach (var data in localVersionConfig.FileInfoDict)
                {
                    fileName = data.Value.File;
                    if ((fileName.Equals("Version.txt")) ||
                        (fileName.Equals("StreamingAssets")))
                    {
                        continue;
                    }

                    resourcesComponent.LoadBundle(fileName);
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
