using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public class Consts
    {
        public enum MahjongWeight
        {
            // 万：1-9
            // 条：11-19
            // 筒：21-29
            // 风：东(31)  南(33)  西(35)  北(37)
            // 红中：41
            // 发财：43
            // 白板：45
            // 春：51
            // 夏：53
            // 秋：55
            // 冬：57
            // 梅：61
            // 兰：63
            // 竹：65
            // 菊：67

            // 万
            Wan_1 = 1,
            Wan_2,
            Wan_3,
            Wan_4,
            Wan_5,
            Wan_6,
            Wan_7,
            Wan_8,
            Wan_9,

            // 条
            Tiao_1 = 11,
            Tiao_2,
            Tiao_3,
            Tiao_4,
            Tiao_5,
            Tiao_6,
            Tiao_7,
            Tiao_8,
            Tiao_9,

            // 筒
            Tong_1 = 21,
            Tong_2,
            Tong_3,
            Tong_4,
            Tong_5,
            Tong_6,
            Tong_7,
            Tong_8,
            Tong_9,

            // 风
            Feng_Dong = 31,
            Feng_Xi = 33,
            Feng_Nan = 35,
            Feng_Bei = 37,

            // 花：普通
            Hua_HongZhong = 41,
            Hua_FaCai = 43,
            Hua_BaiBan = 45,

            // 花：春夏秋冬
            Hua_Spring = 51,
            Hua_Summer = 53,
            Hua_Autumn = 55,
            Hua_Winter = 57,

            // 花：梅兰竹菊
            Hua_MeiHua = 61,
            Hua_LanHua = 63,
            Hua_ZhuZi = 65,
            Hua_JuHua = 67,
        }
    }
}
