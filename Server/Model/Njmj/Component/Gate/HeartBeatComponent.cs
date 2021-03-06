﻿using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    [ObjectSystem]
    public class HeartBeatComponentSystem : UpdateSystem<HeartBeatComponent>
    {
        public override void Update(HeartBeatComponent self)
        {
            self.Update();
        }
    }

    public class HeartBeatComponent : Component
    {
        /// <summary>
        /// 更新间隔
        /// </summary>
        public long UpdateInterval = 5;

        /// <summary>
        /// 超出时间
        /// </summary>
        /// <remarks>如果跟客户端连接时间间隔大于在服务器上删除该Session</remarks>
        public long OutInterval = 60;

        /// <summary>
        /// 记录时间
        /// </summary>
        private long _recordDeltaTime = 0;

        /// <summary>
        /// 当前Session连接时间
        /// </summary>
        public long CurrentTime = 0;

        public void Update()
        {
            // 如果没有到达发包时间、直接返回
            if (!(TimeHelper.ClientNowSeconds() - _recordDeltaTime > this.UpdateInterval) || CurrentTime == 0) return;

            // 记录当前时间
            this._recordDeltaTime = TimeHelper.ClientNowSeconds();

            if (TimeHelper.ClientNowSeconds() - CurrentTime > OutInterval)
            {
                // 移除Session
                Log.Info("移除Session:" + this.Parent.InstanceId);
                Game.Scene.GetComponent<NetOuterComponent>().Remove(this.Parent.InstanceId);
                Game.Scene.GetComponent<NetInnerComponent>().Remove(this.Parent.InstanceId);
                Game.Scene.GetComponent<NjmjGateSessionKeyComponent>().Remove(this.Parent.InstanceId);
                this.Parent.Dispose();
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            CurrentTime = 0;
            _recordDeltaTime = 0;
        }
    }
}