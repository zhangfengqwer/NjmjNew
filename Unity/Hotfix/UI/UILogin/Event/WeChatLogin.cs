using ETModel;

namespace ETHotfix
{
	[Event(ETModel.EventIdType.WeChatLogin)]
	public class WeChatLogin : AEvent<string, string, string>
    {
        public override void Run(string thirdId, string channelName, string response)
        {
//            UILoginComponent.Instance
        }
    }
}
