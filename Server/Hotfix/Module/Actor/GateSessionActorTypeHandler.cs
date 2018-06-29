using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	/// <summary>
	/// gate session收到的actor消息直接转发给客户端
	/// </summary>
	[ActorTypeHandler(AppType.Gate, ActorType.GateSession)]
	public class GateSessionActorTypeHandler : IActorTypeHandler
	{
	    private Actor_GamerOperation _actorGamerOperation;

	    public async Task Handle(Session session, Entity entity, IActorMessage actorMessage)
		{
			ActorResponse actorResponse = new ActorResponse
			{
				RpcId = actorMessage.RpcId
			};
			try
			{
				// 发送给客户端
				Session clientSession = entity as Session;
				actorMessage.ActorId = 0;
			    _actorGamerOperation = actorMessage as Actor_GamerOperation;

			    if (typeof(ETHotfix.Actor_GamerCanOperation).ToString() == actorMessage.GetType().ToString())
                {
                    Log.Info("收到map传了的：" + JsonHelper.ToJson(actorMessage));
                    Log.Info("收到map传了的session：" + session.Id);
                }

                //Log.Info("收到map传了的：" + JsonHelper.ToJson(actorMessage));
                //Log.Info("收到map传了的session：" + session.Id);
                clientSession.Send(actorMessage);

				session.Reply(actorResponse);
				await Task.CompletedTask;
			}
			catch (Exception e)
			{
				actorResponse.Error = ErrorCode.ERR_SessionActorError;
				actorResponse.Message = $"session actor error {e}";
				session.Reply(actorResponse);
				throw;
			}
		}
	}
}
