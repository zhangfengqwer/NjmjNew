using System.Net;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChatSystem:AwakeSystem<UIChatComponent>
    {
        public override void Awake(UIChatComponent self)
        {
            self.Awake();
        }
    }

    public class UIChatComponent : Component 
    {
        public void Awake()
        {

        }
    }
}
