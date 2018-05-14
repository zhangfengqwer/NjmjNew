
using ETModel;

namespace ETHotfix
{
    public static class DBHelper
    {
        /// <summary>
        /// 添加DB信息
        /// </summary>
        public static async void AddEmailInfoToDB(EmailInfo info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            EmailInfo emailInfo = ComponentFactory.CreateWithId<EmailInfo>(IdGenerater.GenerateId());
            emailInfo = info;
            await proxyComponent.Save(emailInfo);
        }

        public static async void AddTaskProgressInfoToDB(long uId, TaskProgressInfo info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskProgressInfo taskInfo = ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
            taskInfo = info;
            await proxyComponent.Save(taskInfo);
        }
    }
}
