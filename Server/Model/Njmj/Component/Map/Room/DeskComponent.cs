using System.Collections.Generic;
using ETHotfix;

namespace ETModel
{
    public class DeskComponent : Component
    {
        public List<MahjongInfo> RestLibrary = new List<MahjongInfo>();
       
        //当前出牌玩家
        public long CurrentAuthority { get; set; }

        //当前庄家
        public long ZhuangJia { get; set; }

        //当前出牌
        public MahjongInfo CurrentCard;

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            RestLibrary.Clear();
            this.CurrentAuthority = 0;
            this.ZhuangJia = 0;
        }
    }
}
