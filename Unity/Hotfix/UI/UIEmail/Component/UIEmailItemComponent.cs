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
        private Button readBtn;
        private Text date;
        private GameObject flag;
        private Email email;
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
            readBtn = rc.Get<GameObject>("ReadBtn").GetComponent<Button>();
            rewardItem = CommonUtil.getGameObjByBundle(UIType.UIRewardItem);
            readBtn.onClick.Add(() =>
            {
                //ReadEmail();
            });
            get.onClick.Add(() =>
            {
                GetItem();
            });
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
            G2C_GetItem g2cGetItem = (G2C_GetItem)await SessionWrapComponent.Instance
                .Session.Call(new C2G_GetItem
                {
                    UId = PlayerInfoComponent.Instance.uid,
                    InfoList = itemList,
                    MailId = email.EId
                });
            ToastScript.createToast("领取成功");
            get.gameObject.SetActive(false);
            GameUtil.changeDataWithStr(email.RewardItem, ',');
            flag.SetActive(false);
        }

        public void SetEmailData(Email email)
        {
            this.email = email;
            title.text = email.EmailTitle;
            content.text = email.Content;
            date.text = email.Date;
            state = email.State;
            string reward = email.RewardItem;
            flag.SetActive(state == 0);
            if (reward != null && !reward.Equals(""))
            {
                get.gameObject.SetActive(state == 0);
                string[] rewardArr = reward.Split(';');
                for(int i = 0;i< rewardArr.Length; ++i)
                {
                    int itemId = CommonUtil.splitStr_Start(rewardArr[i], ',');
                    int rewardNum = CommonUtil.splitStr_End(rewardArr[i], ',');
                    RewardStruct rewardStruct = new RewardStruct();
                    rewardStruct.itemId = itemId;
                    rewardStruct.rewardNum = rewardNum;
                    rewardList.Add(rewardStruct);
                }
                SetRewardItemInfo();
            }
        }

        private void SetRewardItemInfo()
        {
            GameObject obj = null;
            for (int i = 0;i< rewardList.Count; ++i)
            {
                if (i < rewardItemList.Count)
                    obj = rewardItemList[i];
                else
                {
                    obj = GameObject.Instantiate(rewardItem);
                    obj.transform.SetParent(itemGrid.transform);
                    obj.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIRewardItemComponent>();
                    rewardItemList.Add(obj);
                    uiList.Add(ui);
                }
                uiList[i].GetComponent<UIRewardItemComponent>().SetRewardInfo(rewardList[i].itemId.ToString(), rewardList[i].rewardNum);
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
