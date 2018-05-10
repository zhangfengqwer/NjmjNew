using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiHelpComponentSystem : AwakeSystem<UIHelpComponent>
	{
		public override void Awake(UIHelpComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIHelpComponent : Component
	{
        private GameObject panel_guize;
        private GameObject panel_kefu;
        private GameObject panel_guanyu;

        private Button btn_guize;
        private Button btn_kefu;
        private Button btn_guanyu;
	    private Button btn_jiazhangjianhu;

        public void Awake()
		{
            ToastScript.clear();

            initData();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            panel_guize = rc.Get<GameObject>("Panel_guize");
            panel_kefu = rc.Get<GameObject>("Panel_kefu");
            panel_guanyu = rc.Get<GameObject>("Panel_guanyu");

            btn_guize = rc.Get<GameObject>("Button_guize").GetComponent<Button>();
            btn_kefu = rc.Get<GameObject>("Button_kefu").GetComponent<Button>();
            btn_guanyu = rc.Get<GameObject>("Button_guanyu").GetComponent<Button>();
            btn_jiazhangjianhu = rc.Get<GameObject>("Button_jiazhangjianhu").GetComponent<Button>();

            btn_guize.onClick.Add(onClick_guize);
            btn_kefu.onClick.Add(onClick_kefu);
            btn_guanyu.onClick.Add(onClick_guanyu);
            btn_jiazhangjianhu.onClick.Add(onClick_jiazhangjianhu);
        }

        public void onClick_guize()
        {
            showPanel(1);
        }

        public void onClick_kefu()
        {
            showPanel(2);
        }

        public void onClick_guanyu()
        {
            showPanel(3);
        }

        public void onClick_jiazhangjianhu()
        {

        }

        public void showPanel(int tab)
        {
            switch (tab)
            {
                case 1:
                    {
                        panel_guize.transform.localScale = new Vector3(1,1,1);
                        panel_kefu.transform.localScale = new Vector3(0,0,0);
                        panel_guanyu.transform.localScale = new Vector3(0,0,0);

                        btn_guize.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_yxgz_pressed");
                        btn_kefu.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_kefu");
                        btn_guanyu.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_guanyu");
                    }
                    break;

                case 2:
                    {
                        panel_guize.transform.localScale = new Vector3(0,0,0);
                        panel_kefu.transform.localScale = new Vector3(1,1,1);
                        panel_guanyu.transform.localScale = new Vector3(0, 0, 0);

                        btn_guize.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_yxgz");
                        btn_kefu.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_kefu_pressed");
                        btn_guanyu.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_guanyu");
                    }
                    break;

                case 3:
                    {
                        panel_guize.transform.localScale = new Vector3(0,0,0);
                        panel_kefu.transform.localScale = new Vector3(0, 0, 0);
                        panel_guanyu.transform.localScale = new Vector3(1,1,1);

                        btn_guize.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_yxgz");
                        btn_kefu.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_kefu");
                        btn_guanyu.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_help", "btn_guanyu_pressed");
                    }
                    break;
            }
        }
    }
}
