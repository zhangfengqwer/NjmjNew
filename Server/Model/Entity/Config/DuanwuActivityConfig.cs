namespace ETModel
{
	[Config(AppType.Client)]
	public partial class DuanwuActivityConfigCategory : ACategory<DuanwuActivityConfig>
	{
	}

	public class DuanwuActivityConfig: IConfig
	{
		public long Id { get; set; }
		public string Desc;
		public int Reward;
		public int Target;
	}
}
