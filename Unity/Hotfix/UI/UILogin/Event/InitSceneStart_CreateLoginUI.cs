using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.InitSceneStart)]
	public class InitSceneStart_CreateLoginUI: AEvent
	{
		public override void Run()
		{
#if GM
            UI ui = CommonUtil.ShowUI(UIType.UIGMLogin);
#else
            UI ui = CommonUtil.ShowUI(UIType.UILogin);
#endif
        }
	}
}
