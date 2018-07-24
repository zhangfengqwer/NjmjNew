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
    public class UIFriendRoomDetailSystem : AwakeSystem<UIFriendRoomDetailComponent>
    {
        public override void Awake(UIFriendRoomDetailComponent self)
        {
            self.Awake();
        }
    }

    public class UIFriendRoomDetailComponent : Component
    {
        private GameObject Grid;
        private Button CloseBtn;
        private Text Player1Txt;
        private Text Player2Txt;
        private Text Player3Txt;
        private Text Player4Txt;

        private List<Text> textList = new List<Text>();
        private List<GameObject> objList = new List<GameObject>();
        private GameObject item = null;
        private List<UI> uiList = new List<UI>();
        private FriendRoomRecordInfo info;
        private int index;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Grid = rc.Get<GameObject>("Grid");
            CloseBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();
            Player1Txt = rc.Get<GameObject>("Player1Txt").GetComponent<Text>();
            Player2Txt = rc.Get<GameObject>("Player2Txt").GetComponent<Text>();
            Player3Txt = rc.Get<GameObject>("Player3Txt").GetComponent<Text>();
            Player4Txt = rc.Get<GameObject>("Player4Txt").GetComponent<Text>();

            textList.Add(Player1Txt);
            textList.Add(Player2Txt);
            textList.Add(Player3Txt);
            textList.Add(Player4Txt);
            item = CommonUtil.getGameObjByBundle(UIType.UIFriendDetail);

            CloseBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIFriendRoomDetail);
            });
        }

        public void SetInfo(FriendRoomRecordInfo info)
        {
            this.info = info;
            info.gameList.Reverse();
            SetPlayerName();
            CreateItems();
        }

        private void SetPlayerName()
        {
            for (int i = 0;i< info.gameList.Count; ++i)
            {
                for(int j = 0;j< info.gameList[i].nameList.Count; ++j)
                {
                    textList[j].text = info.gameList[i].nameList[j];
                }
            }
        }

        private void CreateItems()
        {
            GameObject obj = null;
            for (int i = 0;i < info.gameList.Count; ++i)
            {
                if(i < objList.Count)
                {
                    obj = objList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(item,Grid.transform);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIFriendDetailComponent>();
                    uiList.Add(ui);
                    objList.Add(obj);
                }
                uiList[i].GetComponent<UIFriendDetailComponent>().SetInfo(info.gameList[i].time, info.roomNum, info.gameList[i],i + 1);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            textList.Clear();
            objList.Clear();
            uiList.Clear();
            info.gameList.Reverse();
        }
    }
}
