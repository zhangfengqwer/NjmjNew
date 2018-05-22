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

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Content = rc.Get<GameObject>("Content").GetComponent<Text>();
            Line = rc.Get<GameObject>("Line");
        }

        public void SetText(string text)
        {
            Content.text = text;
        }

        public int GetTextRow()
        {
            return ((int)Content.GetComponent<RectTransform>().rect.height - 28) / 34;
        }

        public void SetLine()
        {
            Line.transform.localPosition = new Vector3(Line.transform.localPosition.x, 109 - 34 * GetTextRow(), 0);
        }
    }
}
