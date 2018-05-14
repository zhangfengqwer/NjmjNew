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
        private List<TaskInfo> taskInfoList;//任务信息

        public void SetPlayerInfo(PlayerInfo playerInfo)
        {
            this.playerInfo = playerInfo;
        }

        public PlayerInfo GetPlayerInfo()
        {
            return playerInfo;
        }

        public void SetInfoList(List<ShopInfo> shopInfoList,List<TaskInfo> taskInfoList)
        {
            this.shopInfoList = shopInfoList;
            this.taskInfoList = taskInfoList;
        }

        public List<ShopInfo> GetShopInfoList()
        {
            return shopInfoList;
        }

        public List<TaskInfo> GetTaskInfoList()
        {
            return taskInfoList;
        }

        public void Awake()
        {
            Instance = this;
        }
    }
}
