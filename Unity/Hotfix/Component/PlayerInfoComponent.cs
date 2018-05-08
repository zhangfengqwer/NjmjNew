using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    //public class PlayerInfoComponentSystem : AwakeSystem<PlayerInfoComponent>
    //{
    //    public override void Awake(PlayerInfoComponent self)
    //    {
            
    //    }
    //}
    
    public class PlayerInfoComponent : Component
    {
        public long uid;//用户UID
        private PlayerInfo playerInfo;

        public void SetPlayerInfo(PlayerInfo playerInfo)
        {
            this.playerInfo = playerInfo;
        }

        public PlayerInfo GetPlayerInfo()
        {
            return playerInfo;
        }
    }
}
