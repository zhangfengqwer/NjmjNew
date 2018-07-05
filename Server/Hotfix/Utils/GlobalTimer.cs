﻿
using ETModel;

namespace ETHotfix
{
    public class GlobalTimer
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

        static async void onTimer(object data)
        {
            int year = CommonUtil.getCurYear();
            int month = CommonUtil.getCurMonth();
            int day = CommonUtil.getCurDay();
            int hour = CommonUtil.getCurHour();
            int min = CommonUtil.getCurMinute();
            int sec = CommonUtil.getCurSecond();

            //// 每日零点
            //if ((hour == 0) && (min == 0) && (sec == 0))
            //{
            //    Log.Info("刷新数据库");  
            //    NetOuterComponent netOuterComponent = Game.Scene.GetComponent<NetOuterComponent>();

            //    Log.Warning("当前服务器session数量：" + netOuterComponent.sessions.Count);


            //    // 刷新任务
            //    // 刷新签到
            //}

            #region TaskTest
            if ((sec == 0))
            {
                DBHelper.RefreshGameRank();
                //Game.Scene.AddComponent<DBOperatorComponet>();
            }
            if((sec == 30))
            {
                DBHelper.RefreshWealthRank();
            }
            #endregion

            #region 每周一刷新周排行榜数据
            if (CommonUtil.IsMonday())
            {
                //周一零点刷新数据
                if ((hour == 0) && (min == 0) && (sec == 0))
                {
                    if ((sec == 0))
                    {
                        DBHelper.RefreshGameRank();
                        //Game.Scene.AddComponent<DBOperatorComponet>();
                    }
                    if ((sec == 30))
                    {
                        DBHelper.RefreshWealthRank();
                    }
                    await DBCommonUtil.AccountWeekData();
                }
            }
            #endregion

            //// 每日报表
            //if (sec == 0)
            ////if ((min == 10) && (sec == 0))
            //{
            //    for (int i = 0; i < DataStatistics.channelList.Count; i++)
            //    {
            //        await DataStatistics.Start(DataStatistics.channelList[i]);
            //    }
            //}
        }
    }
}