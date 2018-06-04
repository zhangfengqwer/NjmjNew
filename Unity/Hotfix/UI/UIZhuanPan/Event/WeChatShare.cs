using ETModel;

namespace ETHotfix
{
	[Event(ETModel.EventIdType.WeChatShare)]
	public class WeChatShare : AEvent
    {
        public override void Run()
        {
            UIZhuanPanComponent.Instance.RequestShare();
        }
    }
}
