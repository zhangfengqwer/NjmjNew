using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public class Gamer : Entity
    {
        //玩家打牌数据
        public GamerData gameData;

        //用户ID（唯一）
        public long UserID { get; set; }

        //是否准备
        public bool IsReady { get; set; }

        //是否离线
        public bool isOffline { get; set; }

        //是否是庄家
        public bool IsBanker { get; set; }
        
        //玩家信息
        public PlayerInfo PlayerInfo { get; set; }
    }
}
