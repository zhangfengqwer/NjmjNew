using System.Collections.Generic;

namespace ETModel
{
	[Config(AppType.Client)]
	public partial class TaskConfigCategory : ACategory<TaskConfig>
	{
	}

    public class TaskData
    {
        static TaskData s_instance = null;

        List<TaskConfig> listData = new List<TaskConfig>();

        public static TaskData getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new TaskData();
            }

            return s_instance;
        }

        public List<TaskConfig> getDataList()
        {
            return listData;
        }

        public TaskConfig GetDataByTaskId(long taskId)
        {
            for (int i = 0; i < listData.Count; ++i)
            {
                if (listData[i].Id == taskId)
                    return listData[i];
            }
            return null;
        }
    }

    public class TaskConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public string Desc;
		public int Reward;
		public int Target;
	}
}
