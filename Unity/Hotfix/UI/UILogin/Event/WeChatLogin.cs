using ETModel;

namespace ETHotfix
{
	[Event(ETModel.EventIdType.WeChatLogin)]
	public class WeChatLogin : AEvent<string, string, string>
    {
        public override void Run(string thirdId, string nickName, string response)
        {
            UILoginComponent.Instance.onThirdLoginCallback(new ThirdLoginData()
            {
                    third_id = thirdId,
                    nick_name = nickName,
                    response = response
            });
        }
    }
}
