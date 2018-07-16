using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_ReliefGoldHandler : AMHandler<Actor_ReliefGold>
    {
        protected override async void Run(ETModel.Session session, Actor_ReliefGold message)
        {
            try
            {
                ToastScript.createToast(message.Reward);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
