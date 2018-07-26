using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiSetComponentSystem : AwakeSystem<UISetComponent>
	{
		public override void Awake(UISetComponent self)
		{
			self.Awake();
		}
	}
	
	public class UISetComponent : Component
	{
        private GameObject Text_shengying_kai;
        private GameObject Text_shengying_guan;
        private GameObject Text_nan1;
        private GameObject Text_nan2;
        private GameObject Text_nv1;
        private GameObject Text_nv2;

        private Button Button_close;
        private Button Button_OK;

        public void Awake()
		{
            ToastScript.clear();
            initData();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Text_shengying_kai = rc.Get<GameObject>("Text_shengying_kai");
            Text_shengying_guan = rc.Get<GameObject>("Text_shengying_guan");
            Text_nan1 = rc.Get<GameObject>("Text_nan1");
            Text_nan2 = rc.Get<GameObject>("Text_nan2");
            Text_nv1 = rc.Get<GameObject>("Text_nv1");
            Text_nv2 = rc.Get<GameObject>("Text_nv2");

            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();
            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();

            Text_shengying_kai.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_shengyin_kai);
            Text_shengying_guan.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_shengyin_guan);
            Text_nan1.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_nan1);
            Text_nan2.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_nan2);
            Text_nv1.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_nv1);
            Text_nv2.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_nv2);

            Button_close.onClick.Add(onClick_close);
            Button_OK.onClick.Add(onClick_OK);

            if (PlayerPrefs.GetInt("isOpenSound", 1) == 1)
            {
                setShengYin(true);
            }
            else
            {
                setShengYin(false);
            }

            setYinSe(PlayerInfoComponent.Instance.GetPlayerInfo().PlayerSound);

            CommonUtil.SetTextFont(Button_OK.transform.parent.gameObject);
            UIAnimation.ShowLayer(Button_OK.transform.parent.gameObject);
        }

        public void onClick_close()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UISet);
        }

        public void onClick_OK()
        {
            // 音量开关
            {
                if (Text_shengying_kai.transform.Find("Button/Image").transform.localScale.x == 1)
                {
                    PlayerPrefs.SetInt("isOpenSound",1);
                    SoundsHelp.Instance.IsOpenSound(true);
                }
                else
                {
                    PlayerPrefs.SetInt("isOpenSound", 0);
                    SoundsHelp.Instance.IsOpenSound(false);
                }
            }

            // 音色
            {
                int playerSoung = 1;
                if (Text_nan1.transform.Find("Button/Image").transform.localScale.x == 1)
                {
                    playerSoung = 1;
                }
                else if (Text_nan2.transform.Find("Button/Image").transform.localScale.x == 1)
                {
                    playerSoung = 2;
                }
                else if (Text_nv1.transform.Find("Button/Image").transform.localScale.x == 1)
                {
                    playerSoung = 3;
                }
                else if (Text_nv2.transform.Find("Button/Image").transform.localScale.x == 1)
                {
                    playerSoung = 4;
                }

                RequestSetPlayerSound(playerSoung);
            }
        }

        public void onClick_shengyin_kai()
        {
            setShengYin(true);
        }

        public void onClick_shengyin_guan()
        {
            setShengYin(false);
        }

        public void onClick_nan1()
        {
            setYinSe(1);

            SoundsHelp.Instance.playSound_Nan1_BuHua();
        }

        public void onClick_nan2()
        {
            setYinSe(2);

            SoundsHelp.Instance.playSound_Nan2_BuHua();
        }

        public void onClick_nv1()
        {
            setYinSe(3);

            SoundsHelp.Instance.playSound_Nv1_BuHua();
        }

        public void onClick_nv2()
        {
            setYinSe(4);

            SoundsHelp.Instance.playSound_Nv2_BuHua();
        }

        void setShengYin(bool isOpen)
        {
            if (isOpen)
            {
                Text_shengying_kai.transform.Find("Button/Image").transform.localScale = new Vector3(1,1,1);
                Text_shengying_guan.transform.Find("Button/Image").transform.localScale = Vector3.zero;
            }
            else
            {
                Text_shengying_kai.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_shengying_guan.transform.Find("Button/Image").transform.localScale = new Vector3(1, 1, 1);
            }
        }

        void setYinSe(int playerSound)
        {
            if (playerSound == 1)
            {
                Text_nan1.transform.Find("Button/Image").transform.localScale = new Vector3(1, 1, 1);
                Text_nan2.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nv1.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nv2.transform.Find("Button/Image").transform.localScale = Vector3.zero;
            }
            else if (playerSound == 2)
            {
                Text_nan1.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nan2.transform.Find("Button/Image").transform.localScale = new Vector3(1, 1, 1);
                Text_nv1.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nv2.transform.Find("Button/Image").transform.localScale = Vector3.zero;
            }
            else if (playerSound == 3)
            {
                Text_nan1.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nan2.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nv1.transform.Find("Button/Image").transform.localScale = new Vector3(1, 1, 1);
                Text_nv2.transform.Find("Button/Image").transform.localScale = Vector3.zero;
            }
            else if (playerSound == 4)
            {
                Text_nan1.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nan2.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nv1.transform.Find("Button/Image").transform.localScale = Vector3.zero;
                Text_nv2.transform.Find("Button/Image").transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private async void RequestSetPlayerSound(int PlayerSound)
        {
            UINetLoadingComponent.showNetLoading();
            G2C_SetPlayerSound g2cSetPlayerSound = (G2C_SetPlayerSound)await SessionComponent.Instance.Session.Call(new C2G_SetPlayerSound { Uid = PlayerInfoComponent.Instance.uid, PlayerSound = PlayerSound });
            UINetLoadingComponent.closeNetLoading();

            if (g2cSetPlayerSound.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cSetPlayerSound.Message);

                return;
            }

            PlayerInfoComponent.Instance.GetPlayerInfo().PlayerSound = PlayerSound;

            ToastScript.createToast("设置成功");
        }
    }
}
