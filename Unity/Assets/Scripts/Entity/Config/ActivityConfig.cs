namespace ETModel
{
	[Config(AppType.Client)]
	public partial class ActivityConfigCategory : ACategory<ActivityConfig>
	{
	}

	public class ActivityConfig: IConfig
	{
		public long Id { get; set; }
		public string Title;
	}
}
