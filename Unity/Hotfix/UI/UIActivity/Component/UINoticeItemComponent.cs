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
        private NoticeInfo info;

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
            Debug.Log(info.id);
            Content.text = info.content;
            int state = PlayerPrefs.GetInt(info.id.ToString());
            Flag.SetActive(!(state == 1));
            if (Flag.activeInHierarchy)
                this.GetParent<UI>().GameObject.transform.SetAsFirstSibling();
        }

        public int GetTextRow()
        {
            return ((int)Content.GetComponent<RectTransform>().rect.height - 28) / 34;
        }

        public void SetLine()
        {
            Debug.Log(GetTextRow());
            Line.transform.localPosition = new Vector3(Line.transform.localPosition.x, -56 - 34 * GetTextRow(), 0);
            Debug.Log(Line.transform.localPosition);
        }
    }
}
