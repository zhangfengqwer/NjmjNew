using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_CheatHandler : AMActorHandler<Gamer, Actor_GamerCheat>
    {
        protected override async Task Run(Gamer gamer, Actor_GamerCheat message)
        {
            try
            {
                Log.Info("收到作弊：" + JsonHelper.ToJson(message));
                if (string.IsNullOrEmpty(message.Info)) return;
                string[] split = message.Info.Split(' ');
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);

                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();

                if (handCardsComponent == null)
                {
                    handCardsComponent = gamer.AddComponent<HandCardsComponent>();
                }

                if (split.Length == 13)
                {
                    List<MahjongInfo> mahjongInfos = new List<MahjongInfo>();
                    foreach (var item in split)
                    {
                        mahjongInfos.Add(new MahjongInfo()
                        {
                            m_weight = (Consts.MahjongWeight)Convert.ToInt32(item),
                            weight = (byte) Convert.ToInt32(item)
                        });
                    }

                    handCardsComponent.library = mahjongInfos;

                    Logic_NJMJ.getInstance().SortMahjong(handCardsComponent.library);

                }
                else if (split.Length == 1)
                {
                    int num = Convert.ToInt32(split[0]);
                    room.NextGrabCard = new MahjongInfo()
                    {
                        m_weight = (Consts.MahjongWeight) num,
                        weight = (byte) num
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            await Task.CompletedTask;
        }
    }
}
