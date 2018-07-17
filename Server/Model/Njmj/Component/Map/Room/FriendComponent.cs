using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETModel
{
    public class FriendComponent : ComponentWithId
    {
        /// <summary>
        /// 6位房间id
        /// </summary>
        public int FriendRoomId { get; set; }

        /// <summary>
        /// 局数
        /// </summary>
        public int JuCount { get; set; }

        /// <summary>
        /// 开房userid
        /// </summary>
        public long MasterUserId { get; set; }

        /// <summary>
        /// 花倍率
        /// </summary>
        public int Multiples { get; set; }

        /// <summary>
        /// 最低门槛
        /// </summary>
        public int MinThreshold { get; set; }
        
        /// <summary>
        /// 是否公开
        /// </summary>
        public bool IsPublic { get; set; }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            base.Dispose();
        }
    }
}
