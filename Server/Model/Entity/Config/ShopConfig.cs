namespace ETModel
{
	[Config(AppType.Client)]
	public partial class ShopConfigCategory : ACategory<ShopConfig>
	{
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
