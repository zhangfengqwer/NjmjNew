using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

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

				// 加载热更配置
				ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
				Game.Scene.AddComponent<ConfigComponent>();
				ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");

				UnitConfig unitConfig = (UnitConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), 1001);
				Log.Debug($"config {JsonHelper.ToJson(unitConfig)}");

				Game.EventSystem.Run(EventIdType.InitSceneStart);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}

            {
                // 发牌
                {
                    List<MahjongInfo> zhuangjia = new List<MahjongInfo>();
                    List<MahjongInfo> other1 = new List<MahjongInfo>();
                    List<MahjongInfo> other2 = new List<MahjongInfo>();
                    List<MahjongInfo> other3 = new List<MahjongInfo>();
                    List<MahjongInfo> rest = new List<MahjongInfo>();

                    Logic_NJMJ.getInstance().FaMahjong(zhuangjia, other1, other2, other3, rest);
                }

                // 胡牌
                {
                    List<MahjongInfo> list = new List<MahjongInfo>();
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_2));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_3));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_3));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_4));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_4));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_5));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_6));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_6));

                    bool b = Logic_NJMJ.getInstance().isHuPai(list);
                    Debug.Log(b);
                }
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