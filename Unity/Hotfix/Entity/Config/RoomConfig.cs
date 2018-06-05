using ETModel;

namespace ETHotfix
{
	[Config(AppType.Client)]
	public partial class RoomConfigCategory : ACategory<RoomConfig>
	{
	}

	public class RoomConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public int ServiceCharge;
		public int Multiples;
		public int MinThreshold;
	}
}
