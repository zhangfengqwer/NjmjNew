using ETModel;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public class HeartBeat
    {
        static HeartBeat s_instance = null;

        long m_durTime = 5000;
        bool isStopHeartBeat = false;

        public static HeartBeat getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new HeartBeat();
            }

            return s_instance;
        }

        public async void startHeartBeat()
        {
            isStopHeartBeat = false;

            while (!isStopHeartBeat)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(m_durTime);
                reqHeartBeat();
            }
        }

        public void stopHeartBeat()
        {
            isStopHeartBeat = true;
        }

        public async void reqHeartBeat()
        {
            try
            {
                G2C_HeartBeat g2cHeartBeat = (G2C_HeartBeat)await SessionWrapComponent.Instance.Session.Call(new C2G_HeartBeat { });
            }
            catch (Exception ex)
            {
                stopHeartBeat();
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UINetError);
            }
        }
    }
}