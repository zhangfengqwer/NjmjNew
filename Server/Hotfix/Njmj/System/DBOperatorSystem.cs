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

        public static void JudgeTime(this DBOperatorComponet componet)
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
                Log.Info("刷新数据库");
                // 刷新任务
                DBHelper.RefreshDB();
                // 刷新签到
            }

            if ((sec == 0))
            {
                DBHelper.RefreshRankFromDB();
            }
        }
    }
}
