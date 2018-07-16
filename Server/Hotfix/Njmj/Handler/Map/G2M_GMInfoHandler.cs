using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Map)]
    public class G2M_GMInfoHandler : AMRpcHandler<G2M_GMInfo, M2G_GMInfo>
    {
        protected override async void Run(Session session, G2M_GMInfo message, Action<M2G_GMInfo> reply)
        {
            M2G_GMInfo response = new M2G_GMInfo();
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = null;
                Gamer gamer = null;
                switch (message.Type)
                {
                    case 3:
                        foreach (var _room in roomComponent.gameRooms.Values)
                        {
                            room = _room;
                            gamer = room.Get(message.Uid);
                            if (gamer != null)
                            {
                                Log.Info("找到房间:" + _room.Id);
                                roomComponent.RemoveRoom(room, true);
                                room.Dispose();
                                break;
                            }
                        }

                        if (gamer != null)
                        {
                            response.Message = "解散成功";
                        }
                        else
                        {
                            response.Message = "未找到房间";
                        }
                        break;
                    case 6:
                        foreach (var gameRoom in roomComponent.gameRooms.Values)
                        {
                            gamer = gameRoom.Get(message.Uid);
                            if (gamer != null)
                            {
                                response.Type = 2;
                                break;
                            }
                        }

                        foreach (var gameRoom in roomComponent.idleRooms.Values)
                        {
                            gamer = gameRoom.Get(message.Uid);
                            if (gamer != null)
                            {
                                response.Type = 1;
                                break;
                            }
                        }

                        if (gamer == null)
                        {
                            response.Type = 0;
                        }

                        break;
                }

                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }

            await Task.CompletedTask;
        }
    }
}