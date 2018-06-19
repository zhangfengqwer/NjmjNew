using System.Linq;
using System.Collections.Generic;
using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
//    [ObjectSystem]
//    public class HeartBeatSystem: UpdateSystem<HeartBeatComponent>
//    {
//        public override void Update(HeartBeatComponent self)
//        {
//            self.Update();
//        }
//    }

    public class HeartBeatComponent: Component
    {
        /// <summary>
        /// 心跳包间隔
        /// </summary>
        public float SendInterval = 2f;

        /// <summary>
        /// 记录时间
        /// </summary>
        private float RecordDeltaTime = 0f;

        public async void Update()
        {
            // 如果还没有建立Session直接返回、或者没有到达发包时间
            if ((Time.time - RecordDeltaTime < SendInterval)) return;
            // 记录当前时间
            this.RecordDeltaTime = Time.time;
            // 开始发包
            try
            {
                G2C_HeartBeat g2cHeartBeat = (G2C_HeartBeat) await SessionWrapComponent.Instance.Session.Call(new C2G_HeartBeat { });
            }
            catch
            {
                // 执行断线后的操作 Debug.Log(“断线了”);
                UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("提示", "与服务器断开连接，请重新登录。");
                script.setOnClickOkEvent(() =>
                {
                    Game.Scene.GetComponent<UIComponent>().RemoveAll();
                    Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);
                });

                script.setOnClickCloseEvent(() =>
                {
                    Game.Scene.GetComponent<UIComponent>().RemoveAll();
                    Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);
                });
            }
        }
    }
}