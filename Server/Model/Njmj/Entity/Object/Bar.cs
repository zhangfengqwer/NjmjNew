using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class PengOrBar : ComponentWithId
    {
        public int Weight { get; set; }
        public long UserId { get; set; }
        public BarType BarType { get; set; }
        public OperateType OperateType { get; set; }
    }

    public enum BarType
    {
        DarkBar,
        LightBar,
        PengBar
    }

    public enum OperateType
    {
        Peng,
        Bar
    }
}
