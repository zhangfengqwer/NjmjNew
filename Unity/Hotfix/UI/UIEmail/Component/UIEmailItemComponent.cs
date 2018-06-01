using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class RewardStruct
    {
        public int itemId;
        public int rewardNum;
    }

    [ObjectSystem]
    public class UIEmailItemSystem : AwakeSystem<UIEmailItemComponent>
    {
        public override void Awake(UIEmailItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIEmailItemComponent : Component
    {
        private Text title;
        private Text content;
        private GameObject itemGrid;
        private Button get;
        private Button Delete;
        private Button readBtn;
        private Text date;
        private GameObject flag;
        public Email email;
        private int state;
        private GameObject rewardItem;
        private List<GameObject> rewardItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private List<RewardStruct> rewardList = new List<RewardStruct>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            title = rc.Get<GameObject>("Title").GetComponent<Text>();
            content = rc.Get<GameObject>("Content").GetComponent<Text>();
            itemGrid = rc.Get<GameObject>("ItemGrid");
            date = rc.Get<GameObject>("Date").GetComponent<Text>();
            flag = rc.Get<GameObject>("Flag");
            get = rc.Get<GameObject>("Get").GetComponent<Button>();
            Delete = rc.Get<GameObject>("Delete").GetComponent<Button>();
            readBtn = rc.Get<GameObject>("ReadBtn").GetComponent<Button>();
            rewardItem = CommonUtil.getGameObjByBundle(UIType.UIRewardItem);
            readBtn.onClick.Add(() =>
            {
                //ReadEmail();
            });

            //获得奖励
            get.onClick.Add(() =>
            {
                GetItem();
            });

            //删除邮件
            Delete.onClick.Add(() =>
            {
                DeleteMail();
            });
        }

        private async void DeleteMail()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_EmailOperate g2cGetItem = (G2C_EmailOperate)await SessionWrapComponent.Instance
                .Session.Call(new C2G_EmailOperate
                {
                    Uid = PlayerInfoComponent.Instance.uid,
                    EmailId = (int)email.EId,
                    state = 2
                });
            UINetLoadingComponent.closeNetLoading();
            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIEmail).GetComponent<UIEmailComponent>().RefreshMailUI();
            ToastScript.createToast("删除邮件成功!");
        }

        private async void GetItem()
        {
            List<GetItemInfo> itemList = new List<GetItemInfo>();
            for(int i = 0;i< rewardList.Count; ++i)
            {
                GetItemInfo itemInfo = new GetItemInfo();
                itemInfo.ItemID = rewardList[i].itemId;
                itemInfo.Count = rewardList[i].rewardNum;
                itemList.Add(itemInfo);
            }
            UINetLoadingComponent.showNetLoading();
            G2C_EmailOperate g2cGetItem = (G2C_EmailOperate)await SessionWrapComponent.Instance
                .Session.Call(new C2G_EmailOperate
                {
                    Uid = PlayerInfoComponent.Instance.uid,
                    InfoList = itemList,
                    EmailId = (int)email.EId,
                    state = 1
                });
            UINetLoadingComponent.closeNetLoading();
            ToastScript.createToast("领取成功");
            get.gameObject.SetActive(false);
            GameUtil.changeDataWithStr(email.RewardItem);
            flag.SetActive(false);
            Delete.gameObject.SetActive(true);
        }

        public void SetEmailData(Email email)
        {
            this.email = email;
            title.text = email.EmailTitle;
            content.text = email.Content;
            date.text = email.Date.Substring(0, 10);
            state = email.State;
            string reward = email.RewardItem;
            flag.SetActive(state == 0);
            if (state == 0)
                Delete.gameObject.SetActive(false);
            Delete.gameObject.SetActive(state == 1);
            rewardList.Clear();
            if (reward != null && !reward.Equals(""))
            {
                get.gameObject.SetActive(state == 0);
                string[] rewardArr = reward.Split(';');
                for(int i = 0;i< rewardArr.Length; ++i)
                {
                    int itemId = CommonUtil.splitStr_Start(rewardArr[i], ':');
                    int rewardNum = CommonUtil.splitStr_End(rewardArr[i], ':');
                    RewardStruct rewardStruct = new RewardStruct();
                    rewardStruct.itemId = itemId;
                    rewardStruct.rewardNum = rewardNum;
                    rewardList.Add(rewardStruct);
                }
                SetRewardItemInfo();
            }
            else
            {
                if(rewardItemList.Count > 0)
                {
                    SetMoreItemHide(0);
                    get.gameObject.SetActive(false);
                }
            }
        }

        private void SetRewardItemInfo()
        {
            GameObject obj = null;
            for (int i = 0;i< rewardList.Count; ++i)
            {
                if (i < rewardItemList.Count)
                {
                    rewardItemList[i].SetActive(true);
                    obj = rewardItemList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(rewardItem);
                    obj.transform.SetParent(itemGrid.transform);
                    obj.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    obj.transform.localPosition = Vector3.zero;
                    rewardItemList.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIRewardItemComponent>();
                    uiList.Add(ui);
                }
                uiList[i].GetComponent<UIRewardItemComponent>().SetRewardInfo(rewardList[i].itemId.ToString(), rewardList[i].rewardNum);
            }
            SetMoreItemHide(rewardList.Count);
        }

        private void SetMoreItemHide(int index)
        {
            for(int i = index;i< rewardItemList.Count; ++i)
            {
                rewardItemList[i].SetActive(false);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            rewardItemList.Clear();
            uiList.Clear();
            rewardList.Clear();
        }
    }
}
