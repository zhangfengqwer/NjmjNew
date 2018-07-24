using System;
using ETHotfix;

namespace ETModel
{
    [ObjectSystem]
    public class GamerAwakeSystem : AwakeSystem<Gamer, long>
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
        public long UserID { get; set; }

        //玩家GateActorID,给gate传送消息的id
        public long PlayerID { get; set; }

        //玩家所在房间ID
        public long RoomID { get; set; }

        //是否准备
        public bool IsReady { get; set; }

        //是否准备
        public DismissState DismissState = DismissState.None;

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

        //玩家在线开始的时间
        public DateTime StartTime { get; set; }

        //游戏离线结束的时间
        public DateTime EndTime { get; set; }
        public bool IsTrusteeship { get; set; }
        public int ChangeGold { get; set; }

        public HuPaiNeedData huPaiNeedData;
        public PlayerBaseInfo playerBaseInfo;

        public void Awake(long id)
        {
            this.UserID = id;
            huPaiNeedData = new HuPaiNeedData();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.UserID = 0;
            this.PlayerID = 0;
            this.RoomID = 0;
            this.IsReady = false;
            this.DismissState = DismissState.None;
            this.isOffline = false;
            this.ReadyTimeOut = 0;
            IsTrusteeship = false;
            IsWinner = false;
            isGangFaWanPai = false;
            isFaWanPaiTingPai = false;
            isGangEndBuPai = false;
            isGetYingHuaBuPai = false;
            isZimo = false;
            isFangPao = false;
            IsCanPeng = false;
            IsCanGang = false;
            IsCanHu = false;
            playerBaseInfo = null;
            ChangeGold = 0;
        }
    }
}