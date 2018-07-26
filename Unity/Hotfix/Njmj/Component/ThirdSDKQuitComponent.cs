using System.Linq;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class ThirdSDKQuitComponentAwakeSystem : AwakeSystem<ThirdSDKQuitComponent>
    {
        public override void Awake(ThirdSDKQuitComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class ThirdSDKQuitComponentUpdateSystem : UpdateSystem<ThirdSDKQuitComponent>
    {
        public override void Update(ThirdSDKQuitComponent self)
        {
            self.Update();
        }
    }

    public class ThirdSDKQuitComponent : Component
    {
        public void Awake()
        {
            
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Log.Info("第三方返回:" + PlatformHelper.isThirdSDKQuit());

                if (PlatformHelper.isThirdSDKQuit())
                {
                    PlatformHelper.thirdSDKQuit("AnroidCallBack", "", "");
                }
                else
                {
                    // 不在游戏内，退出整个游戏
                    if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom) == null)
                    {
                        UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("通知", "是否退出游戏？");
                        script.setOnClickOkEvent(() =>
                        {
                            Application.Quit();
                        });

                        script.setOnClickCloseEvent(() =>
                        {
                            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
                        });

                        script.getTextObj().alignment = TextAnchor.MiddleCenter;
                    }
                    // 在游戏内，退回到主界面
                    else
                    {
                        UIRoomComponent.OnExit();
                    }
                }
            }
        }

    }
}
