using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerReconnectHandler : AMHandler<Actor_GamerReconnet>
    {
        protected override async void Run(Session session, Actor_GamerReconnet message)
        {
            try
            {
                Log.Info($"断线重连:" + JsonHelper.ToJson(message));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
