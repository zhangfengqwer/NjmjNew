namespace ETModel
{
    public class GameControllerComponent : Component
    {
        //全场倍率
        public int Multiples { get; set; }

        //最低入场门槛
        public long MinThreshold { get; set; }

        //服务费
        public long ServiceCharge { get; set; }

        public RoomConfig RoomConfig { get; set; }

        public RoomName RoomName { get; set; }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.Multiples = 0;
            this.MinThreshold = 0;
            this.ServiceCharge = 0;
            RoomName = RoomName.None;
        }
    }
}
