using System;
using ETModel;

namespace ETHotfix
{
    [Event(ETModel.EventIdType.ChangeAccount)]
    public class ChangeAccount : AEvent
    {
        public override void Run()
        {
            try
            {
                UIPlayerInfoComponent.onClickChangeAccount();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}