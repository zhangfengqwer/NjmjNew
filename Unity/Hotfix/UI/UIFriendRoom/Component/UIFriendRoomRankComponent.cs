using ETModel;
using LitJson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFriendRoomRankSystem : StartSystem<UIFriendRoomRankComponent>
    {
        public override void Start(UIFriendRoomRankComponent self)
        {
            self.Start();
        }
    }

    public class UIFriendRoomRankComponent : Component
    {
        private Button CloseBtn;
        private GameObject UIFriendGrid;
        private Text ScoreTxt;

        private List<GameObject> objList = new List<GameObject>();
        private GameObject item = null;
        private List<UI> itemUIList = new List<UI>();
        private List<FriendRoomRecordInfo> friendRoomRecordInfoList = new List<FriendRoomRecordInfo>();

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            CloseBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();
            UIFriendGrid = rc.Get<GameObject>("UIFriendGrid");
            ScoreTxt = rc.Get<GameObject>("ScoreTxt").GetComponent<Text>();

            item = CommonUtil.getGameObjByBundle(UIType.UIMyRankItem);

            CloseBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIFriendRoomRank);
            });

            GetInfoReq();
            Init();
            UIAnimation.ShowLayer(this.GetParent<UI>().GameObject);
        }

        private void Init()
        {
            ScoreTxt.text = PlayerInfoComponent.Instance.GetPlayerInfo().Score.ToString();   
        }

        private async void GetInfoReq()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_MyFriendRank mr = (G2C_MyFriendRank)await SessionComponent.Instance.Session.Call(new C2G_MyFriendRank()
            {
                UId = PlayerInfoComponent.Instance.uid
            });
            UINetLoadingComponent.closeNetLoading();

            try
            {
                friendRoomRecordInfoList = JsonMapper.ToObject<List<FriendRoomRecordInfo>>(mr.Data);

                JsonData jd = JsonMapper.ToObject(mr.Data);

                Log.Debug(mr.Data);

                #region inv
                //for (int i = 0; i < jd.Count; i++)
                //{
                //    FriendRoomRecordInfo info = new FriendRoomRecordInfo();
                //    info.gameList = new List<FriendRoomRecordInfo.ResultDetails>();

                //    info.result = (int)jd[i]["result"];
                //    info.allScore = (int)jd[i]["allScore"];
                //    info.roomNum = (int)jd[i]["roomNum"];
                //    info.time = (string)jd[i]["time"];
                //    JsonData gameList = jd[i]["gameList"];

                //    for (int j = 0; j < gameList.Count; ++j)
                //    {
                //        List<string> list = new List<string>();
                //        List<int> scoreList = new List<int>();
                //        for (int k = 0; k < gameList[j]["nameList"].Count; ++k)
                //        {
                //            string str = (string)gameList[j]["nameList"][k];
                //            list.Add(str);
                //        }

                //        for (int l = 0; l < gameList[j]["scoreList"].Count; ++l)
                //        {
                //            int score = (int)gameList[j]["scoreList"][l];
                //            scoreList.Add(score);
                //        }

                //        string time = (string)gameList[j]["time"];
                //        FriendRoomRecordInfo.ResultDetails detail = new FriendRoomRecordInfo.ResultDetails(list, scoreList, time);
                //        info.gameList.Add(detail);
                //    }

                //    friendRoomRecordInfoList.Add(info);
                //}
                #endregion

                CreateItems();
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }

        private void CreateItems()
        {
            GameObject obj = null;
            for(int i = 0;i < friendRoomRecordInfoList.Count;++i)
            {
                if (i < objList.Count)
                {
                    obj = objList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(item,UIFriendGrid.transform);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIMyRankItem>();
                    itemUIList.Add(ui);
                    objList.Add(obj);
                }
                itemUIList[i].GetComponent<UIMyRankItem>().SetInfo(friendRoomRecordInfoList[i], i);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            itemUIList.Clear();
            friendRoomRecordInfoList.Clear();
            objList.Clear();
        }
    }

    // 一个对象代表一大局
    public class FriendRoomRecordInfo
    {
        public int result;      // 1赢  2输
        public int allScore;
        public int roomNum;
        public string time;

        // 代表一大局里面的所有小局
        public List<ResultDetails> gameList = new List<ResultDetails>();

        //------------------------------------------------------------------------------

        // 一个对象代表一小局
        public class ResultDetails
        {
            public List<long> uidList = new List<long>();
            public List<string> nameList = new List<string>();
            public List<int> scoreList = new List<int>();
            public string time;

            public ResultDetails(List<string> _gameInfoList, List<int> _scoreList,List<long> _uidList, string _time)
            {
                time = _time;
                nameList = _gameInfoList;
                scoreList = _scoreList;
            }
        }
    }
}
