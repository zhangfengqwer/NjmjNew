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
            try
            {
	            Log.Info($"玩家{gamer.UserID}退出房间");
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);
                if (room == null) return;

	            if (room.State == RoomState.Game)
	            {
	                if (gamer.isOffline) return;
	                gamer.isOffline = true;
                    //玩家断开添加自动出牌组件
                    //if (gamer.GetComponent<TrusteeshipComponent>() == null)
                    //gamer.AddComponent<TrusteeshipComponent>();
	                gamer.EndTime = DateTime.Now;
	                TimeSpan span = gamer.EndTime - gamer.StartTime;
	                int totalSeconds = (int) span.TotalSeconds;
	                //await DBCommonUtil.RecordGamerTime(gamer.EndTime, false,gamer.UserID);
	                await DBCommonUtil.RecordGamerInfo(gamer.UserID, totalSeconds);

                    Log.Info($"玩家{gamer.UserID}断开，切换为自动模式");

                }
                else
	            {
	                GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();

                    //好友房还没开局房主掉线,房间解散
                    if (room.IsFriendRoom && room.State == RoomState.Idle && room.CurrentJuCount == 0)
	                {
	                    if (gameControllerComponent.RoomConfig.MasterUserId == gamer.UserID)
	                    {
	                        room.Broadcast(new Actor_GamerReadyTimeOut()
	                        {
	                            Message = "房主解散房间"
	                        });
	                        GameHelp.RoomDispose(room);
	                        return;
                        }
	                }
	                //好友房开局后,掉线后不能退出
                    if (room.IsFriendRoom && room.CurrentJuCount > 0  && room.CurrentJuCount< gameControllerComponent.RoomConfig.JuCount)
                    {
                        Log.Info($"{gamer.UserID} 好友房开局后,掉线后不能退出");
	                    return;
	                }

	                //玩家主动退出 通知gate
	                if (message.IsFromClient)
	                {
	                    ActorMessageSenderComponent actorMessageSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
	                    ActorMessageSender actorMessageSender = actorMessageSenderComponent.Get(gamer.PlayerID);
	                    actorMessageSender.Send(new M2G_Actor_GamerExitRoom());

	                    //消息广播给其他人
	                    room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
	                    //房间移除玩家
	                    Log.Info($"{gamer.UserID}主动退出，移除玩家");
                        room.Remove(gamer.UserID);
	                }
	                else //游戏崩溃
	                {
                        //房间移除玩家
	                    Log.Info($"{gamer.UserID}崩溃退出，移除玩家");
                        room.Remove(gamer.UserID);
	                    //消息广播给其他人
	                    room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
	                }
	                gamer.Dispose();
	                //房间没人就释放
	                if (room.seats.Count == 0)
	                {
	                    roomComponent.RemoveRoom(room);
	                    room?.Dispose();
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