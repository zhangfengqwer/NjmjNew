using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIDuanwuTreasureSystem : AwakeSystem<UIDuanwuTreasureComponent>
    {
        public override void Awake(UIDuanwuTreasureComponent self)
        {
            self.Awake();
        }
    }

    public class UIDuanwuTreasureComponent : Component
    {
        private Text HLimit;
        private Text JLimit;
        private Text PLimit;
        private Text HPrice;
        private Text JPrice;
        private Text PPrice;
        private Button Close;
        private GameObject Grid;
        private GameObject RewardGrid;
        private Button DuiBtn;
        private GameObject CommonReward;
        private Button CloseReward;
        private Text Price;
        private Text Title;

        private GameObject treasureItem = null;
        private GameObject rewardItem = null;
        private List<GameObject> treasureItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private List<TreasureInfo> treasureInfoList = new List<TreasureInfo>();
        private List<RewardStruct> rewardList = new List<RewardStruct>();
        private List<GameObject> rewardItemList = new List<GameObject>();
        private List<UI> rewardUIList = new List<UI>();

        private TreasureInfo pInfo;
        private TreasureInfo jInfo;
        private TreasureInfo hInfo;
        private List<DuanwuTreasureLogInfo> duanwuTreasureLogInfoList = new List<DuanwuTreasureLogInfo>();
        private TreasureInfo curTreasureInfo;
        private UIDuanwuTreasureItemComponent curDuanwuTreasureItemCoponent;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            HLimit = rc.Get<GameObject>("HLimit").GetComponent<Text>();
            JLimit = rc.Get<GameObject>("JLimit").GetComponent<Text>();
            PLimit = rc.Get<GameObject>("PLimit").GetComponent<Text>();
            HPrice = rc.Get<GameObject>("HPrice").GetComponent<Text>();
            PPrice = rc.Get<GameObject>("PPrice").GetComponent<Text>();
            JPrice = rc.Get<GameObject>("JPrice").GetComponent<Text>();
            Grid = rc.Get<GameObject>("Grid");
            Close = rc.Get<GameObject>("Close").GetComponent<Button>();
            RewardGrid = rc.Get<GameObject>("RewardGrid");
            DuiBtn = rc.Get<GameObject>("DuiBtn").GetComponent<Button>();
            CloseReward = rc.Get<GameObject>("CloseReward").GetComponent<Button>();
            CommonReward = rc.Get<GameObject>("CommonReward");
            Price = rc.Get<GameObject>("Price").GetComponent<Text>();
            Title = rc.Get<GameObject>("Title").GetComponent<Text>();

            treasureItem = CommonUtil.getGameObjByBundle(UIType.UITreasure);
            rewardItem = CommonUtil.getGameObjByBundle(UIType.UIDuanwuRewardItem);

            Close.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIDuanwuTreasure);
            });

            DuiBtn.onClick.Add(() =>
            {
            //ToastScript.createToast("未到活动时间");
            //return;
            //string curTime = CommonUtil.getCurTimeNormalFormat();
            //if (string.CompareOrdinal(curTime, componen) >= 0
            //    && string.CompareOrdinal(curTime, duanwuData.EndTime) < 0)
            //{
                try
                {
                    DuanwuTreasureLogInfo treasureLogInfo = GetTreasureLogById(curTreasureInfo.TreasureId);
                    if (treasureLogInfo.buyCount < 10)
                    {
                        if( UIDuanwuActivityComponent.Instance.duanwuData.ZongziCount >= curTreasureInfo.Price)
                        {
                            //可以购买
                            BuyTreasure();
                        }
                        else
                        {
                            ToastScript.createToast("粽子数量不够，请完成活动获取更多粽子");
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            });

            CloseReward.onClick.Add(() =>
            {
                CommonReward.SetActive(false);
            });
            
            GetTreasureData();
        }

        private async void BuyTreasure()
        {
            G2C_BuyDuanwuTreasure g2c = (G2C_BuyDuanwuTreasure)await Game.Scene.GetComponent<SessionComponent>().Session.Call(new C2G_BuyDuanwuTreasure
            {
                UId = PlayerInfoComponent.Instance.uid,
                Reward = curTreasureInfo.Reward,
                Price = curTreasureInfo.Price,
                TreasureId = curTreasureInfo.TreasureId,
                LimitCount = curTreasureInfo.LimitCount
            });
            if(g2c.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2c.Message);
            }
            else
            {
                ToastScript.createToast("兑换成功");
                //刷新数据
                await SetPlayerInfo();
                curDuanwuTreasureItemCoponent.RefreshUI(g2c.Info);
                UIDuanwuActivityComponent.Instance.RefreshUI(g2c.ZongZiCount);
            }
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        private async Task SetPlayerInfo()
        {
            long uid = PlayerInfoComponent.Instance.uid;

            G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo)await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = uid });
            if(g2CPlayerInfo == null)
            {
                Log.Debug("用户信息错误");
                return;
            }
            PlayerInfoComponent.Instance.SetPlayerInfo(g2CPlayerInfo.PlayerInfo);
            PlayerInfoComponent.Instance.ownIcon = g2CPlayerInfo.OwnIcon;
            GameUtil.changeData(1, 0);
        }

        private async void GetTreasureLogInfo()
        {
            G2C_GetDuanwuTreasureInfo g2ctreasure = (G2C_GetDuanwuTreasureInfo)await Game.Scene.GetComponent<SessionComponent>().Session.Call(new C2G_GetDuanwuTreasureInfo { UId = PlayerInfoComponent.Instance.uid });
            duanwuTreasureLogInfoList = g2ctreasure.Treasures;
            CreateTreasureList();
        }

        private void Init()
        {
            HLimit.text = $"每个限购<color=#810000FF>{hInfo.LimitCount}</color>次";
            JLimit.text = $"每个限购<color=#810000FF>{jInfo.LimitCount}</color>次";
            PLimit.text = $"每个限购<color=#810000FF>{pInfo.LimitCount}</color>次";
            HPrice.text = hInfo.Price.ToString();
            JPrice.text = jInfo.Price.ToString();
            PPrice.text = pInfo.Price.ToString();
        }

        private DuanwuTreasureLogInfo GetTreasureLogById(int treasureId)
        {
            for (int i = 0; i < duanwuTreasureLogInfoList.Count; ++i)
            {
                if (duanwuTreasureLogInfoList[i].TreasureId == treasureId)
                {
                    return duanwuTreasureLogInfoList[i];
                }
            }
            return null;
        }

        private async void GetTreasureData()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_DuanwuTreasure g2ctreasure = (G2C_DuanwuTreasure)await SessionComponent.Instance.Session.Call(new C2G_DuanwuTreasure {  UId = PlayerInfoComponent.Instance.uid});
            UINetLoadingComponent.closeNetLoading();
            treasureInfoList = g2ctreasure.TreasureInfoList;
            GetTreasureLogInfo();
            SetTreasureInfo();
            Init();
        }

        private void CreateTreasureList()
        {
            GameObject obj = null;
            try
            {
                for (int i = 0; i < treasureInfoList.Count; ++i)
                {
                    TreasureInfo info = treasureInfoList[i];
                    if (i < treasureItemList.Count)
                    {
                        obj = treasureItemList[i];
                    }
                    else
                    {
                        obj = GameObject.Instantiate(treasureItem);
                        obj.transform.SetParent(Grid.transform);
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localScale = Vector3.one;
                        treasureItemList.Add(obj);
                        UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                        ui.AddComponent<UIDuanwuTreasureItemComponent>();
                        uiList.Add(ui);
                    }
                    DuanwuTreasureLogInfo treasureLogInfo = GetTreasureLogById(info.TreasureId);
                    uiList[i].GetComponent<UIDuanwuTreasureItemComponent>().SetItemInfo(info, treasureLogInfo.buyCount);
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }

        private void SetTreasureInfo()
        {
            for (int i = 0; i < treasureInfoList.Count; ++i)
            {
                if(i < 3)
                {
                    pInfo = treasureInfoList[i];
                }
                else if(i < 6)
                {
                    jInfo = treasureInfoList[i];
                }
                else
                {
                    hInfo = treasureInfoList[i];
                }
            }
        }

        public void SetReward(UIDuanwuTreasureItemComponent component, TreasureInfo info)
        {
            try
            {
                curDuanwuTreasureItemCoponent = component;
                curTreasureInfo = info;
                Price.text = info.Price.ToString();
                Title.text = info.Name;
                rewardList.Clear();
                CommonReward.gameObject.SetActive(true);
                string[] rewards = info.Reward.Split(';');
                for (int i = 0; i < rewards.Length; ++i)
                {
                    string[] strArr = rewards[i].Split(':');
                    string iconName = strArr[0];
                    string rewardNum = strArr[1];
                    RewardStruct rewardStruct = new RewardStruct();
                    rewardStruct.iconName = iconName;
                    rewardStruct.rewardNum = int.Parse(rewardNum);
                    rewardList.Add(rewardStruct);
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
            CreateRewards();
        }

        private void CreateRewards()
        {
            GameObject obj = null;
            for(int i = 0;i< rewardList.Count; ++i)
            {
                if (i < rewardItemList.Count)
                {
                    obj = rewardItemList[i];
                    rewardItemList[i].SetActive(true);
                }
                else
                {
                    obj = GameObject.Instantiate(rewardItem);
                    obj.transform.SetParent(RewardGrid.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    rewardItemList.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIDuanwuRewardItemComponent>();
                    rewardUIList.Add(ui);
                }
                rewardUIList[i].GetComponent<UIDuanwuRewardItemComponent>().SetItemInfo(rewardList[i]);
            }
            SetMoreHide(rewardList.Count);
        }

        private void SetMoreHide(int index)
        {
            for(int i = index;i< rewardItemList.Count; ++i)
            {
                rewardItemList[i].SetActive(false);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            uiList.Clear();
            treasureItemList.Clear();
            treasureInfoList.Clear();
            rewardUIList.Clear();
            rewardList.Clear();
            rewardItemList.Clear();
            duanwuTreasureLogInfoList.Clear();
        }
    }
}
