using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_RankUpdate : AMHandler<Actor_RankUpdate>
    {
        protected override void Run(Session session, Actor_RankUpdate message)
        {
            
        }
    }
}
