namespace ETModel
{
	[Config(AppType.Client)]
	public partial class ChatConfigCategory : ACategory<ChatConfig>
	{
	}

	public class ChatConfig: IConfig
	{
		public long Id { get; set; }
		public string Content;
	}
}
