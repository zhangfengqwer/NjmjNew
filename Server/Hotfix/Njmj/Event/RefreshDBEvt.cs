using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.RefreshDB)]
    public class RefreshDBEvt : AEvent
    {
        public override void Run()
        {
            DBHelper.RefreshDB();
        }
    }
}

