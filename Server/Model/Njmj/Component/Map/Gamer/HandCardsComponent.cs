using System.Collections.Generic;
using ETHotfix;

namespace ETModel
{
    public class HandCardsComponent : Component
    {
        //所有手牌
        public List<MahjongInfo> library = new List<MahjongInfo>();

        //出的牌
        public readonly List<MahjongInfo> PlayCards = new List<MahjongInfo>();

        //硬花牌
        public readonly List<MahjongInfo> FaceCards = new List<MahjongInfo>();

        //硬花杠牌
        public readonly List<MahjongInfo> FaceGangCards = new List<MahjongInfo>();

        //碰
        public readonly List<MahjongInfo> PengCards = new List<MahjongInfo>();

        //杠牌
        public readonly List<MahjongInfo> GangCards = new List<MahjongInfo>();

        //碰后杠
        public readonly List<MahjongInfo> PengGangCards = new List<MahjongInfo>();

        //碰或杠的牌
        public readonly List<PengOrBar> PengOrBars = new List<PengOrBar>();

        //抓的牌
        public MahjongInfo GrabCard;

        //身份
        public bool IsBanker { get; set; }

        //是否托管
        public bool IsTrusteeship { get; set; }

        //手牌数
        public int CardsCount
        {
            get { return library.Count; }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.library.Clear();
            Log.Debug("手牌清空");
            PlayCards.Clear();
            FaceCards.Clear();
            FaceGangCards.Clear();
            PengCards.Clear();
            GangCards.Clear();
            PengGangCards.Clear();
            PengOrBars.Clear();
            IsTrusteeship = false;
            IsBanker = false;
            GrabCard = null;
        }
    }
}