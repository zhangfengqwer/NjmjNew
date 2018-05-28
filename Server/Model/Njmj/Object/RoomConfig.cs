namespace ETModel
{
    /// <summary>
    /// 房间配置
    /// </summary>
    public struct RoomConfig
    {
        //房间最低门槛
        public long MinThreshold { get; set; }

        //房间等级
        public RoomLevel Level { get; set; }
    }
}
