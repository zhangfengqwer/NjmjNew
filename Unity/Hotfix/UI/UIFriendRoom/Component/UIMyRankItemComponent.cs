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
    public class UIMyRankItemSystem : AwakeSystem<UIMyRankItem>
    {
        public override void Awake(UIMyRankItem self)
        {
            self.Start();
        }
    }

    public class UIMyRankItem : Component
    {
        private Button View;
        private Text TimeTxt;
        private Text DateTxt;
        private GameObject AccountGrid;
        private Text RoomIdTxt;
        private Text ScoreTxt;
        private Image Result;

        private FriendRoomRecordInfo info;
        private List<GameObject> objList = new List<GameObject>();
        private GameObject item = null;
        private int index;
        private List<UI> uiList = new List<UI>();
        private Dictionary<string, List<int>> scoreDic = new Dictionary<string, List<int>>();

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            View = rc.Get<GameObject>("View").GetComponent<Button>();
            AccountGrid = rc.Get<GameObject>("AccountGrid");
            TimeTxt = rc.Get<GameObject>("TimeTxt").GetComponent<Text>();
            DateTxt = rc.Get<GameObject>("DateTxt").GetComponent<Text>();
            RoomIdTxt = rc.Get<GameObject>("RoomIdTxt").GetComponent<Text>();
            ScoreTxt = rc.Get<GameObject>("ScoreTxt").GetComponent<Text>();
            Result = rc.Get<GameObject>("Result").GetComponent<Image>();

            item = CommonUtil.getGameObjByBundle(UIType.UIFriendRoomTxt);

            View.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIFriendRoomDetail);
                if(GameUtil.GetComponentByType<UIFriendRoomDetailComponent>(UIType.UIFriendRoomDetail) != null)
                {
                    GameUtil.GetComponentByType<UIFriendRoomDetailComponent>(UIType.UIFriendRoomDetail).SetInfo(info);
                }
            });
        }

        private void AccountAllScore()
        {
            for (int j = 0; j < info.gameList.Count; ++j)
            {
                for (int k = 0; k < info.gameList[j].nameList.Count; ++k)
                {
                    if (!scoreDic.ContainsKey(info.gameList[j].nameList[k]))
                    {
                        scoreDic.Add(info.gameList[j].nameList[k], new List<int>());
                    }
                    scoreDic[info.gameList[j].nameList[k]].Add(info.gameList[j].scoreList[k]);
                }
            }
        }

        private List<int> MyScore(string name)
        {
            if (scoreDic.ContainsKey(name))
            {
                return scoreDic[name];
            }
            return null;
        }

        public void SetInfo(FriendRoomRecordInfo info,int id)
        {
            this.info = info;
            this.index = id;
            AccountAllScore();

            int allScore = 0;
            List<int> myList = MyScore(PlayerInfoComponent.Instance.GetPlayerInfo().Name);
            if(myList != null)
            {
                for(int i = 0;i< myList.Count; ++i)
                {
                    allScore += myList[i];
                }
            }

            if (allScore >= 0)
            {
                ScoreTxt.text = "+" + allScore.ToString();
            }
            else
            {
                ScoreTxt.text = allScore.ToString();
            }
            Result.sprite = CommonUtil.getSpriteByBundle("image_main", allScore >= 0 ? "1" : "2");

            RoomIdTxt.text = info.roomNum.ToString();
            TimeTxt.text = info.time.ToString();

            GameObject obj = null;
            int index = 0;
            foreach (var score in scoreDic)
            {
                if (index < objList.Count)
                {
                    obj = objList[index];
                }
                else
                {
                    obj = GameObject.Instantiate(item, AccountGrid.transform);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIFriendRoomTxtComponent>();
                    uiList.Add(ui);
                    objList.Add(obj);
                }
                uiList[index].GetComponent<UIFriendRoomTxtComponent>().SetInfo(score.Key, score.Value);
                index++;
            }
        }
    }
}
