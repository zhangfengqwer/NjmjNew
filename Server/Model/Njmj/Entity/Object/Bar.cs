using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class PengOrBar : ComponentWithId
    {
        public int Weight { get; set; }
        /// <summary>
        /// 杠的那个人的Uid
        /// </summary>
        public long UserId { get; set; }
        public BarType BarType { get; set; }
        public OperateType OperateType { get; set; }
    }

    public enum BarType
    {
        None,
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
