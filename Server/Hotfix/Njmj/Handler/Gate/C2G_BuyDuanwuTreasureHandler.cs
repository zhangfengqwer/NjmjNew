using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_BuyDuanwuTreasureHandler : AMRpcHandler<C2G_BuyDuanwuTreasure, G2C_BuyDuanwuTreasure>
    {
        protected override async void Run(Session session, C2G_BuyDuanwuTreasure message, Action<G2C_BuyDuanwuTreasure> reply)
        {
            G2C_BuyDuanwuTreasure response = new G2C_BuyDuanwuTreasure();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<DuanwuTreasureInfo> treasureInfoList = await proxyComponent.QueryJson<DuanwuTreasureInfo>($"{{UId:{message.UId},TreasureId:{message.TreasureId}}}");

                if(treasureInfoList.Count > 0)
                {
                    if(treasureInfoList[0].BuyCount == message.LimitCount)
                    {
                        //购买该宝箱已到达上限
                        response.Error = ErrorCode.ERR_Exception;
                        response.Message = "该宝箱购买已经达到上限";
                        reply(response);
                    }
                    else
                    {
                        List<DuanwuDataBase> dataBases = await proxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{message.UId}}}");
                        if (dataBases[0].ZongziCount <= 0)
                        {
                            dataBases[0].ZongziCount = 0;
                            response.Error = ErrorCode.ERR_Exception;
                            response.Message = "粽子数量不够";
                            reply(response);
                            return;
                        }
                        dataBases[0].ZongziCount -= message.Price;
                        await proxyComponent.Save(dataBases[0]);
                        treasureInfoList[0].BuyCount += 1;
                        DuanwuTreasureLogInfo info = new DuanwuTreasureLogInfo();
                        info.TreasureId = message.TreasureId;
                        info.buyCount = treasureInfoList[0].BuyCount;
                        response.Info = info;
                        response.ZongZiCount = dataBases[0].ZongziCount;
                        await proxyComponent.Save(treasureInfoList[0]);
                        await changeDuanwuDataWithStr(message.UId, message.Reward);
                    }
                }
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        public static async Task changeDuanwuDataWithStr(long uid, string reward)
        {
            List<string> list1 = new List<string>();
            CommonUtil.splitStr(reward, list1, ';');

            for (int i = 0; i < list1.Count; i++)
            {
                List<string> list2 = new List<string>();
                CommonUtil.splitStr(list1[i], list2, ':');

                string id = list2[0];
                int num = int.Parse(list2[1]);

                await ChangeDuanwuRewardData(uid, id, num);
            }
        }

        /// <param name="uid"></param>
        /// <param name="propId"></param>
        /// <param name="propNum"></param>
        public static async Task ChangeDuanwuRewardData(long uid, string propId,int propNum)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
            //头像（端午活动头像）
            if (propId.Length >= 7)
            {
                List<OtherData> otherDatas = await proxyComponent.QueryJson<OtherData>($"{{UId:{uid}}}");
                //之后要在数据库添加属于玩家的头像
                if(otherDatas.Count > 0)
                {
                    List<string> str_list = new List<string>();
                    CommonUtil.splitStr(otherDatas[0].OwnIcon, str_list, ';');
                    if (!IsContains(str_list, propId))
                    {
                        otherDatas[0].OwnIcon += $"{propId};";
                        await proxyComponent.Save(otherDatas[0]);
                    }
                    else
                    {
                        Log.Debug("已经拥有此头像");
                    }
                }
                else
                {
                    OtherData otherData = ComponentFactory.CreateWithId<OtherData>(IdGenerater.GenerateId());
                    otherData.UId = uid;
                    otherData.OwnIcon = $"{propId};";
                    await proxyComponent.Save(otherData);
                }
            }
            else
            {
                int id = int.Parse(propId);
                switch (id)
                {
                    // 金币
                    case 1:
                        {
                            playerBaseInfos[0].GoldNum += propNum;
                            await proxyComponent.Save(playerBaseInfos[0]);
                        }
                        break;
                    // 其他道具
                    default:
                        {
                            List<UserBag> userBags = await proxyComponent.QueryJson<UserBag>($"{{UId:{uid},BagId:{propId}}}");
                            if (userBags.Count == 0)
                            {
                                UserBag itemInfo = new UserBag();
                                itemInfo.BagId = id;
                                itemInfo.UId = uid;
                                itemInfo.Count = propNum;
                                DBHelper.AddItemToDB(itemInfo);
                            }
                            else
                            {
                                userBags[0].Count += propNum;
                                if (userBags[0].Count < 0)
                                {
                                    userBags[0].Count = 0;
                                }
                                await proxyComponent.Save(userBags[0]);
                            }
                        }
                        break;
                }
            }
        }

        public static bool IsContains(List<string> list,string iconName)
        {
            for(int i = 0;i< list.Count; ++i)
            {
                if (iconName == list[i])
                    return true;
            }
            return false;
        }
    }
}
