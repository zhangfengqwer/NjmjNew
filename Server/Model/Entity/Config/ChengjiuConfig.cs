namespace ETModel
{
	[Config(AppType.Client)]
	public partial class ChengjiuConfigCategory : ACategory<ChengjiuConfig>
	{
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
