namespace ETModel
{
	[Config(AppType.Client)]
	public partial class NoticeConfigCategory : ACategory<NoticeConfig>
	{
	}

	public class NoticeConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public string Content;
	}
}
