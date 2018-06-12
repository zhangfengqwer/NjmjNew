using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
	[ObjectSystem]
	public class UiLoadingComponentAwakeSystem : AwakeSystem<UILoadingComponent>
	{
		public override void Awake(UILoadingComponent self)
		{
			self.text = self.GetParent<UI>().GameObject.Get<GameObject>("Text").GetComponent<Text>();
			self.button = self.GetParent<UI>().GameObject.Get<GameObject>("Button").GetComponent<Button>();

		    self.button.onClick.Add(() =>
		    {
		        if (++NetConfig.getInstance().clickCount == 3)
		        {
		            NetConfig.getInstance().isFormal = false;
		            ToastScript.createToast("test");
		        }
		    });
		}
	}

	[ObjectSystem]
	public class UiLoadingComponentStartSystem : StartSystem<UILoadingComponent>
	{
		public override async void Start(UILoadingComponent self)
		{
			TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

			while (true)
			{
				await timerComponent.WaitAsync(1000);

				if (self.IsDisposed)
				{
                    return;
				}

				BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.GetComponent<BundleDownloaderComponent>();
				if (bundleDownloaderComponent == null)
				{
					continue;
				}

				self.text.text = "正在更新资源 " + $"{bundleDownloaderComponent.Progress}%";
			}
        }
    }

	public class UILoadingComponent : Component
	{
		public Text text;
	    public Button button;
	}
}
