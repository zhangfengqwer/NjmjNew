using System.Collections.Generic;

namespace ETModel
{
	[Config(AppType.Client)]
	public partial class ChengjiuConfigCategory : ACategory<ChengjiuConfig>
	{
	}

    public class ChengjiuData
    {
        static ChengjiuData s_instance = null;

        List<ChengjiuConfig> listData = new List<ChengjiuConfig>();

        public static ChengjiuData getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ChengjiuData();
            }

            return s_instance;
        }

        public List<ChengjiuConfig> getDataList()
        {
            return listData;
        }

        public ChengjiuConfig GetDataByChengjiuId(long taskId)
        {
            for (int i = 0; i < listData.Count; ++i)
            {
                if (listData[i].Id == taskId)
                    return listData[i];
            }
            return null;
        }
    }

    public class ChengjiuConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public string Desc;
		public int Reward;
		public int Target;
	}
}
