namespace ETModel
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

	    /// <summary>
	    /// 6位房间id
	    /// </summary>
	    public int FriendRoomId { get; set; }

	    /// <summary>
	    /// 局数
	    /// </summary>
	    public int JuCount { get; set; }

	    /// <summary>
	    /// 开房userid
	    /// </summary>
	    public long MasterUserId { get; set; }

	    /// <summary>
	    /// 是否公开
	    /// </summary>
	    public bool IsPublic { get; set; }

        /// <summary>
        /// 房间钥匙
        /// </summary>
	    public int KeyCount { get; set; }
	}
}
