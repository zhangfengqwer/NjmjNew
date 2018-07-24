using ETModel;
using Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFriendDetailSystem : AwakeSystem<UIFriendDetailComponent>
    {
        public override void Awake(UIFriendDetailComponent self)
        {
            self.Awake();
        }
    }

    public class UIFriendDetailComponent : Component
    {
        private Text RoomTxt;
        private Text TimeTxt;
        private Text Player1Txt;
        private Text Player2Txt;
        private Text Player3Txt;
        private Text Player4Txt;
        private Text JuTxt;

        private FriendRoomRecordInfo info;
        private int index;
        private List<Text> textList = new List<Text>();

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            RoomTxt = rc.Get<GameObject>("RoomTxt").GetComponent<Text>();;
            TimeTxt = rc.Get<GameObject>("TimeTxt").GetComponent<Text>();
            Player1Txt = rc.Get<GameObject>("Player1Txt").GetComponent<Text>();
            Player2Txt = rc.Get<GameObject>("Player2Txt").GetComponent<Text>();
            Player3Txt = rc.Get<GameObject>("Player3Txt").GetComponent<Text>();
            Player4Txt = rc.Get<GameObject>("Player4Txt").GetComponent<Text>();

            textList.Add(Player1Txt);
            textList.Add(Player2Txt);
            textList.Add(Player3Txt);
            textList.Add(Player4Txt);
            
            JuTxt = rc.Get<GameObject>("JuTxt").GetComponent<Text>();

        }

        public void SetInfo(string time,int roomId, FriendRoomRecordInfo.ResultDetails result,int index)
        {
            TimeTxt.text = time;
            RoomTxt.text = roomId.ToString();
            JuTxt.text = index.ToString();
            for(int i = 0;i< result.scoreList.Count; ++i)
            {
                if(result.scoreList[i] >= 0)
                {
                    textList[i].text = "+" + result.scoreList[i];
                }
                else
                {
                    textList[i].text = result.scoreList[i].ToString();
                }
            }
        }
    }
}
