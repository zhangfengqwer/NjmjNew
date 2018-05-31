using ETModel;

namespace ETHotfix
{
	[Event(ETModel.EventIdType.WeChatLogin)]
	public class WeChatLogin : AEvent<string, string, string>
    {
        public override void Run(string thirdId, string channelName, string response)
        {

            await UILoginComponent.Instance.onThirdLoginCallback(new ThirdLoginData()
            {
                    third_id = thirdId,
                    channel_name = channelName,
                    response = response
            });
        }
    }
}
