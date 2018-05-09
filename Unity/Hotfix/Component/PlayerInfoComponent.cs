using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    [ObjectSystem]
    public class PlayerInfoComponentSystem : AwakeSystem<PlayerInfoComponent>
    {
        public override void Awake(PlayerInfoComponent self)
        {
            self.Awake();
        }
    }
    
    public class PlayerInfoComponent : Component
    {
        public static PlayerInfoComponent Instance;

        public long uid;//用户UID
        private PlayerInfo playerInfo;
        private List<ShopInfo> shopInfoList;//商店信息

        public void SetPlayerInfo(PlayerInfo playerInfo)
        {
            this.playerInfo = playerInfo;
        }

        public PlayerInfo GetPlayerInfo()
        {
            return playerInfo;
        }

        public void SetShopInfoList(List<ShopInfo> shopInfoList)
        {
            this.shopInfoList = shopInfoList;
        }

        public List<ShopInfo> GetShopInfoList()
        {
            return shopInfoList;
        }

        public void Awake()
        {
            Instance = this;
        }
    }
}
