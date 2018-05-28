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

            NoticeBtn.onClick.Add(() =>
            {
                PlayerPrefs.SetInt(info.id.ToString(), 1);
                Debug.Log("===");
                Flag.SetActive(false);
            });

        }

        public void SetText(NoticeInfo info)
        {
            this.info = info;
            Content.text = info.content;
            int state = PlayerPrefs.GetInt(info.id.ToString());
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
