using ETModel;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    class FriendRoomRecord
    {
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
                public List<string> nameList = new List<string>();
                public List<int> scoreList = new List<int>();
                public string time;

                public ResultDetails(List<string> _gameInfoList, string _time)
                {
                    time = _time;

                    for (int i = 0; i < _gameInfoList.Count; i++)
                    {
                        List<string> list = new List<string>();
                        CommonUtil.splitStr(_gameInfoList[i], list, ';');

                        if (list.Count == 3)
                        {
                            nameList.Add(list[1]);
                            scoreList.Add(int.Parse(list[2]));
                        }
                        else
                        {
                            nameList.Add("信息缺失");
                            scoreList.Add(0);
                        }
                    }
                }
            }
        }

        public static async Task<string> getRecord(long uid)
        {
            string jsonData = "";

            List<FriendRoomRecordInfo> listData = new List<FriendRoomRecordInfo>();

            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();

            string time = CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(),-90);
            var filter1 = (Builders<Log_Game>.Filter.Gt("CreateTime", time) & Builders<Log_Game>.Filter.Eq("RoomName", "好友房")) & (Builders<Log_Game>.Filter.Eq("Player1_uid", uid) | Builders<Log_Game>.Filter.Eq("Player2_uid", uid)
                     | Builders<Log_Game>.Filter.Eq("Player3_uid", uid) | Builders<Log_Game>.Filter.Eq("Player4_uid", uid));
            List<Log_Game> list = await dbComponent.GetDBDataCollection<Log_Game>(typeof(Log_Game).Name).Find(filter1).ToListAsync();
            Log.Debug(JsonHelper.ToJson(list) + "=====");
            int roomNum = -1;
            FriendRoomRecordInfo FriendRoomRecordInfo = null;
            for (int i = list.Count - 1; i >= 0 ; i--)
            {
                // 如果房间号跟之前的不一样，说明这是另一局的
                if (list[i].RoomNum != roomNum)
                {
                    roomNum = list[i].RoomNum;
                    FriendRoomRecordInfo = new FriendRoomRecordInfo();
                    listData.Add(FriendRoomRecordInfo);

                    FriendRoomRecordInfo.result = 1;
                    FriendRoomRecordInfo.allScore = 100;
                    FriendRoomRecordInfo.roomNum = list[i].RoomNum;
                    FriendRoomRecordInfo.time = list[i].CreateTime;
                }

                // 每一小局对局信息
                {
                    FriendRoomRecordInfo.gameList.Add(new FriendRoomRecordInfo.ResultDetails(new List<string>() { list[i].Player1_info, list[i].Player2_info, list[i].Player3_info, list[i].Player4_info, }, list[i].CreateTime));
                }
            }

            jsonData = JsonConvert.SerializeObject(listData);
            Log.Info("------------------" + jsonData);
            return jsonData;
        }
    }
}
