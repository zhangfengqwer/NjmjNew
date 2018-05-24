namespace ETModel
{
	[Config(AppType.Client)]
	public partial class ZhuanpanConfigCategory : ACategory<ZhuanpanConfig>
	{
	}

	public class ZhuanpanConfig: IConfig
	{
		public long Id { get; set; }
		public int itemId;
		public int propId;
		public int PropNum;
		public int Probability;
	}
}
