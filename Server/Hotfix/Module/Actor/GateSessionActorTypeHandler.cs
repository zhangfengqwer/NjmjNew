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
			    if (actorMessage is Actor_GamerCanOperation)
			    {
			        User user = session.GetComponent<SessionUserComponent>().User;

			        Log.Info($"收到{user.UserID}map传了的：" + JsonHelper.ToJson(actorMessage));
                }

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
