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
        public string userName;//用户名称
        public int goldNum;//金币数量
        public int wingNum;//元宝数量
    }
}
