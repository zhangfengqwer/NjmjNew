using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_GamerAgreeRoomDismissHandler : AMActorHandler<Gamer, Actor_GamerAgreeRoomDismiss>
    {

        protected override async Task Run(Gamer gamer, Actor_GamerAgreeRoomDismiss message)
        {
            await GamerAgreeRoomDismiss(gamer);
        }

        public static async Task GamerAgreeRoomDismiss(Gamer gamer)
        {
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);
                if (room == null || !room.IsFriendRoom || gamer.DismissState != DismissState.None)
                {
                    return;
                }

                gamer.DismissState = DismissState.Agree;
                room.Broadcast(new Actor_GamerAgreeRoomDismiss() {UserId = gamer.UserID});

                Gamer[] gamers = room.GetAll();

                if (gamers.Count(g => g.DismissState == DismissState.Agree) == 3)
                {
                    room.roomDismissTokenSource?.Cancel();
                    room.Broadcast(new Actor_GamerReadyTimeOut()
                    {
                        Message = "房间已解散"
                    });
                    GameHelp.RoomDispose(room);
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