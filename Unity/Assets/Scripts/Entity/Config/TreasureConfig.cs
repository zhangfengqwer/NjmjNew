namespace ETModel
{
	[Config(AppType.Client)]
	public partial class TreasureConfigCategory : ACategory<TreasureConfig>
	{
	}

	public class TreasureConfig: IConfig
	{
		public long Id { get; set; }
		public int Reward;
		public int TotalTime;
	}
}
