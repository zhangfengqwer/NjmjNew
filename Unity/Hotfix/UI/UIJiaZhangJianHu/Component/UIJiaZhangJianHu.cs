using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiJiaZhangJianHuComponentSystem : AwakeSystem<UIJiaZhangJianHuComponent>
	{
		public override void Awake(UIJiaZhangJianHuComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIJiaZhangJianHuComponent : Component
	{
        private Button btn_close;

        public void Awake()
		{
            ToastScript.clear();

            initData();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            btn_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            btn_close.onClick.Add(onClick_Close);

            CommonUtil.SetTextFont(btn_close.transform.parent.gameObject);
            UIAnimation.ShowLayer(btn_close.transform.parent.gameObject);
        }

        public void onClick_Close()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIJiaZhangJianHu);
        }
    }
}
