using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ETModel
{
	public class Init : MonoBehaviour
	{
		private readonly OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();

		private async void Start()
		{
			try
			{
				if (Application.unityVersion != "2017.1.3p2")
				{
					Log.Warning($"请使用Unity2017.1.3p2, 下载地址:\n https://beta.unity3d.com/download/744dab055778/UnityDownloadAssistant-2017.1.3p2.exe?_ga=2.42497696.443074145.1521714954-1119432033.1499739574");
				}

                SynchronizationContext.SetSynchronizationContext(this.contex);

				DontDestroyOnLoad(gameObject);

			    Application.targetFrameRate = 45;
			    // 永不息屏
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
			    // 禁止多点触摸
			    Input.multiTouchEnabled = false;

                Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

                Game.Scene.AddComponent<GlobalConfigComponent>();
				Game.Scene.AddComponent<NetOuterComponent>();
				Game.Scene.AddComponent<ResourcesComponent>();
				Game.Scene.AddComponent<BehaviorTreeComponent>();
				Game.Scene.AddComponent<PlayerComponent>();
				Game.Scene.AddComponent<UnitComponent>();
				Game.Scene.AddComponent<ClientFrameComponent>();
				Game.Scene.AddComponent<UIComponent>();
                Game.Scene.AddComponent<SessionComponent>();

                //新增Component
                Game.Scene.AddComponent<SoundComponent>();

			    // 获取网络配置表
			    await NetConfig.getInstance().Req("http://fwdown.hy51v.com/njmj/online/files/netconfig.json");
//#if GM
//                //不加载AB包
//#else
//                    // 下载ab包
//                    await BundleHelper.DownloadBundle();
//#endif
                // 下载ab包
                await BundleHelper.DownloadBundle();
                Game.Hotfix.LoadHotfixAssembly();

				// 加载配置
				Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
				Game.Scene.AddComponent<ConfigComponent>();
				Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatherComponent>();

				Game.Hotfix.GotoHotfix();

                Game.EventSystem.Run(EventIdType.TestHotfixSubscribMonoEvent, "TestHotfixSubscribMonoEvent");

            }
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private void Update()
		{
			this.contex.Update();
			Game.Hotfix.Update?.Invoke();
			Game.EventSystem.Update();
		}

		private void LateUpdate()
		{
			Game.Hotfix.LateUpdate?.Invoke();
			Game.EventSystem.LateUpdate();
		}

		private void OnApplicationQuit()
		{
			Game.Hotfix.OnApplicationQuit?.Invoke();
			Game.Close();
		}
	}
}