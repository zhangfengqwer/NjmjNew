using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{ 
    public class GamerOnlineTime : ComponentWithId
    {
        public string CreateTime { set; get; }
        public long UId { set; get; }
        public int Type { set; get; }
    }
}
