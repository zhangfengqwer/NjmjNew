using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class RewardStruct
    {
        public string rewardSpriteName;
        public string rewardNum;
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
        private bool isRead;
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
                ReadEmail();
            });
        }

        private async void ReadEmail()
        {
            G2C_UpdateEmail c2gUpdateEmail = (G2C_UpdateEmail)await
                SessionWrapComponent.Instance.Session.Call(new C2G_UpdateEmail() { EId = email.EId, IsRead = false });
            flag.SetActive(false);
        }

        public void SetEmailData(Email email)
        {
            this.email = email;
            title.text = email.EmailTitle;
            content.text = email.Content;
            date.text = email.Date;
            isRead = email.IsRead;
            string reward = email.RewardItem;
            flag.SetActive(isRead);
            get.gameObject.SetActive(false);
            if (reward != null && !reward.Equals(""))
            {
                get.gameObject.SetActive(true);
                string[] rewardArr = reward.Split(';');
                for(int i = 0;i< rewardArr.Length; ++i)
                {
                    string[] itemArr = rewardArr[i].Split(',');
                    RewardStruct rewardStruct = new RewardStruct();
                    rewardStruct.rewardSpriteName = itemArr[0];
                    rewardStruct.rewardNum = itemArr[1];
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
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIRewardItemComponent>();
                    rewardItemList.Add(obj);
                    uiList.Add(ui);
                }
                uiList[i].GetComponent<UIRewardItemComponent>().SetRewardInfo(rewardList[i].rewardSpriteName, rewardList[i].rewardNum);
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
