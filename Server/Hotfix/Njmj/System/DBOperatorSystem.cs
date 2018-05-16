using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class DBOperatorSystem : UpdateSystem<DBOperatorComponet>
    {
        public override void Update(DBOperatorComponet self)
        {
            self.JudgeTime();
        }
    }

    public static class DBOepration
    {
        public static void Refresh()
        {
            DBHelper.RefreshDB();
        }

        public static void JudgeTime(this DBOperatorComponet componet)
        {
            int year = CommonUtil.getCurYear();
            int month = CommonUtil.getCurMonth();
            int day = CommonUtil.getCurDay();
            int hour = CommonUtil.getCurHour();
            int min = CommonUtil.getCurMinute();
            int sec = CommonUtil.getCurSecond();

            // 每日零点
            if ((hour == 17) && (min == 0) && (sec == 0))
            {
                // 刷新任务
                Log.Info("刷新数据库");
                Refresh();
                // 刷新签到
            }
        }
    }
}
