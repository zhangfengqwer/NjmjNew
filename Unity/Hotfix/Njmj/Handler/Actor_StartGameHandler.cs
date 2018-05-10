using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_StartGameHandler : AMHandler<Actor_StartGame>
    {
        protected override async void Run(Session session, Actor_StartGame message)
        {
            try
            {
                Log.Info($"收到开始:{JsonHelper.ToJson(message)}");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
