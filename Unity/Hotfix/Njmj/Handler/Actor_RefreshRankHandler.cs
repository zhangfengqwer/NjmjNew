using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_RefreshRankHandler : AMHandler<Actor_RefreshRank>
    {
        protected override void Run(Session session, Actor_RefreshRank message)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
