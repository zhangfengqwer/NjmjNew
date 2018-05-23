using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class ChengjiuInfo : EntityDB
    {
        public long UId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Reward { get; set; }
        public int TaskId { get; set; }
        public int CurProgress { get; set; }
        public int Target { get; set; }
        public bool IsComplete { get; set; }
        public bool IsGet { get; set; }
    }
}
