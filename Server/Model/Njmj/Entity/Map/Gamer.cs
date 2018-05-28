namespace ETModel
{
    [ObjectSystem]
    public class GamerAwakeSystem : AwakeSystem<Gamer,long>
    {
        public override void Awake(Gamer self, long id)
        {
            self.Awake(id);
        }
    }

    /// <summary>
    /// 房间玩家对象
    /// </summary>
    public sealed class Gamer : Entity
    {
        //用户ID（唯一）
        public long UserID { get; private set; }

        //玩家GateActorID
        public long PlayerID { get; set; }

        //玩家所在房间ID
        public long RoomID { get; set; }

        //是否准备
        public bool IsReady { get; set; }
        
        //是否赢
        public bool IsWinner { get; set; }

        //是否离线
        public bool isOffline { get; set; }

        // 是否是刚发完牌
        public bool isGangFaWanPai = false;

        // 是否是刚发完牌就听牌
        public bool isFaWanPaiTingPai = false;

        // 是否是杠完补牌
        public bool isGangEndBuPai = false;

        // 是否是抓到硬花补牌
        public bool isGetYingHuaBuPai = false; 
        
        // 是否是自摸
        public bool isZimo = false;
        
        // 是否是自摸
        public bool isFangPao = false;

        public int ReadyTimeOut = 0;

        public bool IsCanPeng { get; set; }
        public bool IsCanGang { get; set; }
        public bool IsCanHu { get; set; }

        public void Awake(long id)
        {
            this.UserID = id;
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.UserID = 0;
            this.PlayerID = 0;
            this.RoomID = 0;
            this.IsReady = false;
            this.isOffline = false;
        }
    }
}
