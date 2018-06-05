using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_ChangeTableHandler : AMActorHandler<Gamer,Actor_ChangeTable>
	{
        private static bool locker = true;

	    protected override async Task Run(Gamer gamer, Actor_ChangeTable message)
	    {
            try
	        {
	            Log.Debug("收到换桌");
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room gamerRoom = roomComponent.Get(gamer.RoomID);

                Room idleRoom = null;
	            //获得空闲的房间
	            foreach (var room in roomComponent.idleRooms)
	            {
	                if (room.Id != gamerRoom.Id)
	                {
	                    idleRoom = room;
	                    break;
	                }
	            }

	            if (idleRoom == null)
	            {
                    idleRoom = RoomFactory.Create();
                    roomComponent.Add(idleRoom);
	                Log.Debug("创建房间：" + idleRoom.Id);
                }

                Log.Debug("收到玩家换桌前：" + gamer.RoomID);
                //房间移除玩家
                gamerRoom.Remove(gamer.UserID);
	            //消息广播给其他人
	            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(100);
	            gamerRoom.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });

	            if (gamerRoom.seats.Count == 3)
	            {
	                roomComponent.idleRooms.Add(gamerRoom);
	                roomComponent.readyRooms.Remove(gamerRoom.Id);
                }

	            //房间没人就释放
	            if (gamerRoom.seats.Count == 0)
	            {
	                Log.Debug($"房间释放:{gamerRoom.Id}");
	                roomComponent.RemoveRoom(gamerRoom);
	                gamerRoom?.Dispose();
	            }

                gamer.IsReady = false;
	            gamer.ReadyTimeOut = 0;
                idleRoom.Add(gamer);
	            idleRoom.BroadGamerEnter();

	            if (idleRoom.seats.Count == 4)
	            {
	                roomComponent.readyRooms.Add(idleRoom.Id, idleRoom);
	                roomComponent.idleRooms.Remove(idleRoom);
	            }

                Log.Debug("收到玩家换桌后：" + gamer.RoomID);
            }
	        catch (Exception e)
	        {
	            Log.Error(e);
            }
	        await Task.CompletedTask;
	    }
	}
}