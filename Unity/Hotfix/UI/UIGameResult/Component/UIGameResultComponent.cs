using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiGameResultComponentSystem : AwakeSystem<UIGameResultComponent>
	{
		public override void Awake(UIGameResultComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIGameResultComponent : Component
	{
        private Button Button_close;
        private Button Button_back;
        private Button Button_jixu;

        private GameObject winPlayer;
        private GameObject otherPlayer1;
        private GameObject otherPlayer2;
        private GameObject otherPlayer3;

        bool isTimerEnd = false;
        GameResultNeedData gameResultNeedData = null;

        private Text Text_daojishi;

        public void Awake()
		{
            ToastScript.clear();

            initData();
            startTimer();
        }

        public void initData()
        {
            isTimerEnd = false;

            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();
            Button_back = rc.Get<GameObject>("Button_back").GetComponent<Button>();
            Button_jixu = rc.Get<GameObject>("Button_jixu").GetComponent<Button>();

            winPlayer = rc.Get<GameObject>("winPlayer");
            otherPlayer1 = rc.Get<GameObject>("otherPlayer1");
            otherPlayer2 = rc.Get<GameObject>("otherPlayer2");
            otherPlayer3 = rc.Get<GameObject>("otherPlayer3");

            Text_daojishi = rc.Get<GameObject>("Text_daojishi").GetComponent<Text>();
            
            Button_close.onClick.Add(onClick_close);
            Button_back.onClick.Add(onClick_close);
            Button_jixu.onClick.Add(onClick_jixu);
        }

        public void setData(GameResultNeedData data)
        {
            gameResultNeedData = data;

            if (gameResultNeedData.isZiMo)
            {
                winPlayer.transform.Find("hupaiType").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_gameresult", "gameresult_hu");
            }
        }

        public void onClick_close()
        {
            isTimerEnd = true;
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);
        }

        public void onClick_jixu()
        {
            ToastScript.createToast("暂未开放");
        }

        public async void startTimer()
        {
            int time = 15;
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                --time;

                if (isTimerEnd)
                {
                    return;
                }

                Text_daojishi.text = ("准备倒计时 " + time.ToString());
            }

            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);
        }
    }

    public class GameResultNeedData
    {
        public bool isZiMo = true;
    }
}
