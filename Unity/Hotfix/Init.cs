using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ETModel;
using UnityEngine;
using static ETHotfix.Consts;

namespace ETHotfix
{
	public static class Init
	{
		public static void Start()
		{
            try
			{
				Game.Scene.ModelScene = ETModel.Game.Scene;

				// 注册热更层回调
				ETModel.Game.Hotfix.Update = () => { Update(); };
				ETModel.Game.Hotfix.LateUpdate = () => { LateUpdate(); };
				ETModel.Game.Hotfix.OnApplicationQuit = () => { OnApplicationQuit(); };
				
				Game.Scene.AddComponent<UIComponent>();
				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatherComponent>();

				Game.Scene.AddComponent<PlayerInfoComponent>();
                Game.Scene.AddComponent<UIIconComponent>();
                Game.Scene.AddComponent<SessionWrapComponent>();

                // 加载热更配置
                ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
				Game.Scene.AddComponent<ConfigComponent>();
				ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");

				UnitConfig unitConfig = (UnitConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), 1001);
//				Log.Debug($"config {JsonHelper.ToJson(unitConfig)}");

				Game.EventSystem.Run(EventIdType.InitSceneStart);

			    Application.targetFrameRate = 45;
			    // 永不息屏
			    Screen.sleepTimeout = SleepTimeout.NeverSleep;

            }
            catch (Exception e)
			{
				Log.Error(e);
			}
        }

		public static void Update()
		{
			try
			{
				Game.EventSystem.Update();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void LateUpdate()
		{
			try
			{
				Game.EventSystem.LateUpdate();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void OnApplicationQuit()
		{
			Game.Close();
		}
	}
}