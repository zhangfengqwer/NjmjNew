using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIExpressSystem : AwakeSystem<UIExpressionComponent>
    {
        public override void Awake(UIExpressionComponent self)
        {
            self.Awake();
        }
    }
    public class UIExpressionComponent : Component
    {
        private GameObject UIExpression;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIExpression = rc.Get<GameObject>("UIExpression");
        }

        public void SetExpression(string expresstion)
        {
            
        }
    }
}
