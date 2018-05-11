using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
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
        private Text date;
        private GameObject flag;
        private Email email;
        private bool isRead;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            title = rc.Get<GameObject>("Title").GetComponent<Text>();
            content = rc.Get<GameObject>("Content").GetComponent<Text>();
            itemGrid = rc.Get<GameObject>("ItemGrid");
            date = rc.Get<GameObject>("Date").GetComponent<Text>();
            flag = rc.Get<GameObject>("Flag");
        }

        public void SetEmailData(Email email)
        {
            title.text = email.EmailTitle;
            content.text = email.Content;
            date.text = email.Date;
            isRead = email.IsRead;
            string reward = email.RewardItem;
            flag.SetActive(isRead);
            if (!reward.Equals(""))
            {
                string[] rewardArr = reward.Split(';');
                for(int i = 0;i< rewardArr.Length; ++i)
                {
                    
                }
            }
        }
    }
}
