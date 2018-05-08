using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ETModel;
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
                
                // 测试
                {
                    List<MahjongInfo> list = new List<MahjongInfo>();
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_2));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Wan_2));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Tong_6));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Tong_6));
                    list.Add(new MahjongInfo(Consts.MahjongWeight.Tong_6));

                    // 判断是否胡牌
                    bool b = Logic_NJMJ.getInstance().isHuPai(list);
                    if (b)
                    {
                        HuPaiNeedData huPaiNeedData = new HuPaiNeedData();
                        huPaiNeedData.restMahjongCount = 100;
                        huPaiNeedData.isSelfZhuaPai = true;
                        huPaiNeedData.isZhuangJia = true;
                        huPaiNeedData.isGangFaWanPai = true;

                        // 最新的牌
                        huPaiNeedData.my_lastMahjong = new MahjongInfo(Consts.MahjongWeight.Tong_6);

                        // 碰的牌
                        huPaiNeedData.my_pengList.Add(new MahjongInfo(MahjongWeight.Tong_6));
                        huPaiNeedData.my_pengList.Add(new MahjongInfo(MahjongWeight.Tong_6));
                        huPaiNeedData.my_pengList.Add(new MahjongInfo(MahjongWeight.Tong_6));

                        // 获取胡牌类型
                        List<HuPaiType> list2 = Logic_NJMJ.getInstance().getHuPaiType(list, huPaiNeedData);
                        for (int i = 0; i < list2.Count; i++)
                        {
                            Log.Debug(list2[i].ToString());
                        }
                    }
                    else
                    {
                        Log.Debug("无法胡牌");
                    }
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