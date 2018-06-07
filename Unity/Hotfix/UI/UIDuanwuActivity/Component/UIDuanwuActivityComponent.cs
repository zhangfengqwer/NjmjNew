using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIDuanwuActivitySystem : StartSystem<UIDuanwuActivityComponent>
    {
        public override void Start(UIDuanwuActivityComponent self)
        {
            self.Start();
        }
    }

    public class UIDuanwuActivityComponent : Component
    {
        private Text Des;
        private Text Reward;
        private Text Progress;
        private Text ActivityFinishTime;
        private Text OwnZongziCount;
        private GameObject TaskGrid;
        private GameObject TreasureGrid;
        private Button RefreshTaskBtn;
        private Button TreasureBtn;
        private GameObject Treasure;

        #region duanwu
        private GameObject taskItem = null;
        private List<GameObject> taskItemList = new List<GameObject>();
        private List<UI> uilist = new List<UI>();
        private List<DuanwuActivity> duanwuActInfoList = new List<DuanwuActivity>();
        #endregion

        #region treasure
        #endregion

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Des = rc.Get<GameObject>("Des").GetComponent<Text>();
            Reward = rc.Get<GameObject>("Reward").GetComponent<Text>();
            Progress = rc.Get<GameObject>("Progress").GetComponent<Text>();
            ActivityFinishTime = rc.Get<GameObject>("ActivityFinishTime").GetComponent<Text>();
            OwnZongziCount = rc.Get<GameObject>("OwnZongziCount").GetComponent<Text>();
        }

        /// <summary>
        /// 向服务器请求端午任务列表
        /// </summary>
        private async void GetDuanwuTaskInfoList()
        {
            G2C_DuanwuActivity g2cduanwu = (G2C_DuanwuActivity)await SessionWrapComponent.Instance.Session.Call(new C2G_DuanwuActivity() { UId = PlayerInfoComponent.Instance.uid });
            duanwuActInfoList = g2cduanwu.DuanwuActivityList;

        }

        /// <summary>
        /// 创建端午活动任务列表
        /// </summary>
        private void CreateDuanwuList()
        {
            //for(int i = 0;i< )
        }
    }
}
