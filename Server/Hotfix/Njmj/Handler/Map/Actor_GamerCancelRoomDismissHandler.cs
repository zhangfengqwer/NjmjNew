using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_GamerCancelRoomDismissHandler : AMActorHandler<Gamer, Actor_GamerCancelRoomDismiss>
    {

        protected override async Task Run(Gamer gamer, Actor_GamerCancelRoomDismiss message)
        {
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);
                if (room == null || !room.IsFriendRoom || gamer.DismissState != DismissState.None)
                {
                    return;
                }

                gamer.DismissState = DismissState.Cancel;
                room.Broadcast(new Actor_GamerCancelRoomDismiss() {UserId = gamer.UserID});

                Gamer[] gamers = room.GetAll();

                if (gamers.Count(g => g.DismissState == DismissState.Cancel) == 2)
                {
                    room.roomDismissTokenSource?.Cancel();
                    foreach (var _gamer in gamers)
                    {
                        _gamer.DismissState = DismissState.None;
                    }
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