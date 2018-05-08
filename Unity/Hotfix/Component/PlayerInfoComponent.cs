using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
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
        public string userName;//用户名称
        public int goldNum;//金币数量
        public int wingNum;//元宝数量

        public void Awake()
        {
            Instance = this;
        }
    }
}
