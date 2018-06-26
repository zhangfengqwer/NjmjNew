using System.Linq;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    public class ThirdSDKQuitComponent : Component
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

        private void Awake()
        {
            
        }

        private void Update()
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
//                    if (exitGameObject == null)
//                    {
//                        exitGameObject = ExitGamePanelScript.create();
//                    }
                }
            }
        }

    }
}
