namespace ETModel
{
	[Config(AppType.Client)]
	public partial class TaskConfigCategory : ACategory<TaskConfig>
	{
	}

	public class TaskConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public string Desc;
		public int Reward;
		public int Target;
	}
}
