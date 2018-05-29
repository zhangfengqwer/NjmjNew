using System.Collections.Generic;

namespace ETModel
{
    [Config(AppType.Client)]
    public partial class ShopConfigCategory : ACategory<ShopConfig>
    {
    }

    public class ShopData
    {
        static ShopData s_instance = null;

        List<ShopConfig> listData = new List<ShopConfig>();

        public static ShopData getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ShopData();
            }

            return s_instance;
        }

        public List<ShopConfig> getDataList()
        {
            return listData;
        }

        public ShopConfig GetDataByShopId(long shopId)
        {
            for(int i = 0;i< listData.Count; ++i)
            {
                if (listData[i].Id == shopId)
                    return listData[i];
            }
            return null;
        }
    }
    
	public class ShopConfig: IConfig
	{
		public long Id { get; set; }
		public int shopType;
		public string Name;
		public string Desc;
		public int CurrencyType;
		public int Price;
		public string Items;
		public string Icon;
        public int VipPrice;
    }
}
