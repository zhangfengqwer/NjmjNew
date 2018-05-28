using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ActivityHandler : AMRpcHandler<C2G_Activity, G2C_Activity>
    {
        protected override void Run(Session session, C2G_Activity message, Action<G2C_Activity> reply)
        {
            G2C_Activity response = new G2C_Activity();
            try
            {
                List<Activity> activityList = new List<Activity>();
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();

                for (int i = 1; i < configCom.GetAll(typeof(ActivityConfig)).Length + 1; ++i)
                {
                    int id = 100 + i;
                    ActivityConfig config = (ActivityConfig)configCom.Get(typeof(ActivityConfig), id);
                    Activity activity = new Activity();
                    activity.ActivityId = (int)config.Id;
                    activity.Title = config.Content;
                    activityList.Add(activity);
                }

                response.activityList = activityList;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
