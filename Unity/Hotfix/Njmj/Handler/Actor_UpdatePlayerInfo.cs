using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_UpdatePlayerInfo:AMHandler<Actor_UpDateData>
    {
        protected override void Run(Session session, Actor_UpDateData message)
        {
            try
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>()
                    .UpDatePlayerInfo(message.playerInfo);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
