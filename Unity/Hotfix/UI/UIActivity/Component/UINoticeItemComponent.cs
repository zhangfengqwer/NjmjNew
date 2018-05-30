using ETModel;
using Hotfix;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UINoticeItemSystem : AwakeSystem<UINoticeItemComponent>
    {
        public override void Awake(UINoticeItemComponent self)
        {
            self.Awake();   
        }
    }

    public class UINoticeItemComponent : Component
    {
        private Text Content;
        private Text Title;
        private GameObject Line;
        private GameObject Flag;
        private Button NoticeBtn;
        public NoticeInfo info;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Content = rc.Get<GameObject>("Content").GetComponent<Text>();
            Line = rc.Get<GameObject>("Line");
            Flag = rc.Get<GameObject>("Flag");
            NoticeBtn = rc.Get<GameObject>("NoticeBtn").GetComponent<Button>();
            Title = rc.Get<GameObject>("Title").GetComponent<Text>();
            NoticeBtn.onClick.Add(() =>
            {
                string key = $"{PlayerInfoComponent.Instance.uid}{info.id}";
                PlayerPrefs.SetInt(key, 1);
                Flag.SetActive(false);
            });

        }

        public void SetText(NoticeInfo info)
        {
            this.info = info;
            Title.text = info.title;
            Content.text = info.content;
            string key = $"{PlayerInfoComponent.Instance.uid}{info.id}";
            int state = PlayerPrefs.GetInt(key);
            Flag.SetActive(!(state == 1));
        }

        public float GetTextHeight()
        {
            return Content.preferredHeight - 34;
        }

        public void SetLine()
        {
            Line.transform.localPosition = new Vector3(Line.transform.localPosition.x, -62 - (GetTextHeight()), 0);
        }
    }
}
