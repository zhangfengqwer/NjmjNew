using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class GlobalTimer
    {
        static GlobalTimer s_instance = null;

        System.Threading.Timer m_timer;

        public static GlobalTimer getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new GlobalTimer();
            }

            return s_instance;
        }

        public void start()
        {
            m_timer = new System.Threading.Timer(onTimer, "", 1000, 1000);
        }

        static void onTimer(object data)
        {
            int year = CommonUtil.getCurYear();
            int month = CommonUtil.getCurMonth();
            int day = CommonUtil.getCurDay();
            int hour = CommonUtil.getCurHour();
            int min = CommonUtil.getCurMinute();
            int sec = CommonUtil.getCurSecond();

            // 每日零点
            if ((hour == 0) && (min == 0) && (sec == 0))
            {
                // 刷新任务

                // 刷新签到
            }

            #region TaskTest
            if((hour == 14) && (min == 59) && (sec == 0))
            {
                Game.Scene.AddComponent<DBOperatorComponet>();
            }
            #endregion
        }
    }
}
