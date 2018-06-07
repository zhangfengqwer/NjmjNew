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
                DuanwuTreasureInfo treasure = await proxyComponent.Query<DuanwuTreasureInfo>(message.UId);
                List<DuanwuTreasureInfo> treasureInfoList = await proxyComponent.QueryJson<DuanwuTreasureInfo>($"{{UId:{message.UId},EmailId:{message.TreasureId}}}");
                await changeDuanwuDataWithStr(message.UId, message.Reward);

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

                int id = int.Parse(list2[0]);
                int num = int.Parse(list2[1]);

                await ChangeDuanwuRewardData(uid, id, num);
            }
        }

        /// <param name="uid"></param>
        /// <param name="propId"></param>
        /// <param name="propNum"></param>
        public static async Task ChangeDuanwuRewardData(long uid, int propId, int propNum)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
            switch (propId)
            {
                // 金币
                case 1:
                    {
                        playerBaseInfos[0].GoldNum += propNum;
                        await proxyComponent.Save(playerBaseInfos[0]);
                    }
                    break;
                //头像（端午活动头像）
                case 555:
                    {
                        playerBaseInfos[0].Icon = propNum.ToString();
                        //之后要在数据库添加属于玩家的头像
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
                            itemInfo.BagId = propId;
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
}
