using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_ChatHandler : AMActorHandler<Gamer, Actor_Chat>
    {
        protected override async Task Run(Gamer gamer, Actor_Chat message)
        {
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);
                room.Broadcast(new Actor_Chat { ChatType = message.ChatType, Value = message.Value });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            await Task.CompletedTask;
        }
    }
}
