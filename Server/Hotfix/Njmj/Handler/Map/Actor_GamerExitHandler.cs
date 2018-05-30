using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerExitHandler : AMActorHandler<Gamer, Actor_GamerExitRoom>
	{

	    protected override async Task Run(Gamer gamer, Actor_GamerExitRoom message)
	    {
	        await Task.CompletedTask;

            try
            {
	            Log.Info($"玩家{gamer.UserID}退出房间");
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);

	            if (room.State == RoomState.Game)
	            {
	                gamer.isOffline = true;
                    //玩家断开添加自动出牌组件
                    //if (gamer.GetComponent<TrusteeshipComponent>() == null)
                    //gamer.AddComponent<TrusteeshipComponent>();
	                gamer.EndTime = DateTime.Now;
	                TimeSpan span = gamer.EndTime - gamer.StartTime;
	                int totalSeconds = (int) span.TotalSeconds;
	                DBCommonUtil.RecordGamerTime(gamer.EndTime, false,gamer.UserID);
	                DBCommonUtil.RecordGamerInfo(gamer.UserID, totalSeconds);

                    Log.Info($"玩家{gamer.UserID}断开，切换为自动模式");

                }
                else
	            {
	                //人满了
	                if (room.seats.Count == 4)
	                {
	                    roomComponent.readyRooms.Remove(room.Id);
	                    roomComponent.idleRooms.Add(room);
	                }

	                //玩家主动退出 通知gate
	                if (message.IsFromClient)
	                {
	                    ActorProxyComponent proxyComponent = Game.Scene.GetComponent<ActorProxyComponent>();
	                    ActorProxy actorProxy = proxyComponent.Get(gamer.PlayerID);
	                    actorProxy.Send(new M2G_Actor_GamerExitRoom());

	                    //消息广播给其他人
	                    room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
	                    //房间移除玩家
	                    room.Remove(gamer.UserID);
	                }
	                else //游戏崩溃
	                {
	                    //房间移除玩家
	                    room.Remove(gamer.UserID);
	                    //消息广播给其他人
	                    room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
	                }
	                gamer.Dispose();
	                //房间没人就释放
	                if (room.seats.Count == 0)
	                {
	                    Log.Debug($"房间释放:{room.Id}");
	                    roomComponent.RemoveRoom(room);
	                    room?.Dispose();
                    }
                }
            }
	        catch (Exception e)
	        {
	            Log.Error(e);
            }
	    }
	}
}