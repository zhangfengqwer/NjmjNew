namespace ETModel
{
    public class GameControllerComponent : Component
    {
        public RoomConfig RoomConfig { get; set; }

        public RoomName RoomName { get; set; }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();
            RoomName = RoomName.None;
        }
    }
}
