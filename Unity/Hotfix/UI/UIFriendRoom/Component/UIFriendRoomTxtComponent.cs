using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFriendRoomTxtComponentSystem: AwakeSystem<UIFriendRoomTxtComponent>
    {
        public override void Awake(UIFriendRoomTxtComponent self)
        {
            self.Awake();
        }
    }

    public class UIFriendRoomTxtComponent: Component
    {
        private Text AllScoreTxt;
        private Text NameTxt;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            AllScoreTxt = rc.Get<GameObject>("AllScoreTxt").GetComponent<Text>();
            NameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);
        }

        public void SetInfo(string name, List<int> list)
        {
            int score = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                score += list[i];
            }

            if (score >= 0)
            {
                AllScoreTxt.text = "+" + score;
            }
            else if (score < 0)
            {
                AllScoreTxt.text = score.ToString();
            }

            if (name.Length > 2)
            {
                NameTxt.text = name.Substring(0, 2) + "...";
            }
            else
            {
                NameTxt.text = name;
            }
        }
    }
}