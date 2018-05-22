using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using ProtoBuf;
using static ETHotfix.Consts;

namespace ETHotfix
{
    [ProtoContract]
    public class MahjongInfo
    {
        [ProtoMember(1)]
        public byte weight;

        public MahjongWeight m_weight;

        public MahjongInfo(MahjongWeight weight)
        {
            m_weight = weight;
        }

        public MahjongInfo(int weight)
        {
            m_weight = (MahjongWeight)weight;
        }

        //pb反序列化必须需要无参数的构造函数
        public MahjongInfo()
        {

        }

    }

    public class HuPaiNeedData
    {
        // 自己最新抓的牌或是别人刚出的牌
        public MahjongInfo my_lastMahjong = null;

        // 剩余麻将数
        public int restMahjongCount = 100;

        // 最后这张牌是否是自己抓的
        public bool isSelfZhuaPai = false;

        // 自己是否是庄家
        public bool isZhuangJia = false;

        // 是否是刚发完牌
        public bool isGangFaWanPai = false;

        // 是否是刚发完牌就听牌
        public bool isFaWanPaiTingPai = false;

        // 是否是杠完补牌
        public bool isGangEndBuPai = false;

        // 是否是抓到硬花补牌
        public bool isGetYingHuaBuPai = false;

        // 硬花
        public List<MahjongInfo> my_yingHuaList = new List<MahjongInfo>();

        // 自己碰掉的牌:碰掉的三张只要add一个就行
        public List<MahjongInfo> my_pengList = new List<MahjongInfo>();

        // 其他人碰掉的牌
        public List<MahjongInfo> other1_pengList = new List<MahjongInfo>();
        public List<MahjongInfo> other2_pengList = new List<MahjongInfo>();
        public List<MahjongInfo> other3_pengList = new List<MahjongInfo>();

        // 自己杠掉的牌:杠掉的四张只要add一个就行
        public List<MahjongInfo> my_gangList = new List<MahjongInfo>();

    }

    public class Logic_NJMJ
    {
        static Logic_NJMJ s_instance = null;
        public Dictionary<HuPaiType, int> HuPaiHuaCount = new Dictionary<HuPaiType, int>();

        List<MahjongInfo> m_mahjongList = new List<MahjongInfo>();
        List<MahjongInfo> m_differenceMahjongList = new List<MahjongInfo>();

        public static Logic_NJMJ getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new Logic_NJMJ();
                s_instance.initMahjongList();
                s_instance.initHuaCount();
            }

            return s_instance;
        }

        private void initHuaCount()
        {
            HuPaiHuaCount.Add(HuPaiType.Normal, 1);
            HuPaiHuaCount.Add(HuPaiType.MenQing, 1);
            HuPaiHuaCount.Add(HuPaiType.HunYiSe, 1);
            HuPaiHuaCount.Add(HuPaiType.QingYiSe, 1);
            HuPaiHuaCount.Add(HuPaiType.DuiDuiHu, 1);
            HuPaiHuaCount.Add(HuPaiType.QuanQiuDuDiao, 1);
            HuPaiHuaCount.Add(HuPaiType.QiDui_Normal, 1);
            HuPaiHuaCount.Add(HuPaiType.QiDui_HaoHua, 1);
            HuPaiHuaCount.Add(HuPaiType.QiDui_ChaoHaoHua, 1);
            HuPaiHuaCount.Add(HuPaiType.QiDui_ChaoChaoHaoHua, 1);
            HuPaiHuaCount.Add(HuPaiType.YaJue, 1);
            HuPaiHuaCount.Add(HuPaiType.WuHuaGuo, 1);
            HuPaiHuaCount.Add(HuPaiType.GangHouKaiHua_Small, 1);
            HuPaiHuaCount.Add(HuPaiType.GangHouKaiHua_Big, 1);
            HuPaiHuaCount.Add(HuPaiType.TianHu, 1);
            HuPaiHuaCount.Add(HuPaiType.DiHu, 1);
            HuPaiHuaCount.Add(HuPaiType.HaiDiLaoYue, 1);
        }

        void initMahjongList()
        {
            if (m_mahjongList.Count == 0)
            {
                // 万
                for (int i = 1; i <= 9; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo((int)MahjongWeight.Wan_1 + (i - 1)));
                    }
                }

                // 条
                for (int i = 1; i <= 9; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo((int)MahjongWeight.Tiao_1 + (i - 1)));
                    }
                }

                // 筒
                for (int i = 1; i <= 9; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo((int)MahjongWeight.Tong_1 + (i - 1)));
                    }
                }

                // 风
                {
                    // 东风
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Feng_Dong));
                    }

                    // 西风
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Feng_Xi));
                    }

                    // 南风
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Feng_Nan));
                    }

                    // 北风
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Feng_Bei));
                    }
                }

                // 花
                {
                    // 发财
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_FaCai));
                    }

                    // 红中
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_HongZhong));
                    }

                    // 白板
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_BaiBan));
                    }

                    // 春夏秋冬
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_Spring));
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_Summer));
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_Autumn));
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_Winter));

                    // 梅兰竹菊
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_MeiHua));
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_LanHua));
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_ZhuZi));
                    m_mahjongList.Add(new MahjongInfo(MahjongWeight.Hua_JuHua));
                }
            }

            {
                // 万
                for (int i = 1; i <= 9; i++)
                {
                    m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)i));
                }

                // 条
                for (int i = 11; i <= 19; i++)
                {
                    m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)i));
                }

                // 筒
                for (int i = 21; i <= 29; i++)
                {
                    m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)i));
                }

                // 东南西北
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)31));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)33));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)35));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)37));

                // 中发白
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)41));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)43));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)45));

                // 春夏秋冬
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)51));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)53));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)55));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)57));

                // 梅兰竹菊
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)61));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)63));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)65));
                m_differenceMahjongList.Add(new MahjongInfo((MahjongWeight)67));
            }
        }

        // 根据牌的权重获取它的类型
        public MahjongType getMahjongType(MahjongInfo mahjongInfo)
        {
            if (mahjongInfo.m_weight <= MahjongWeight.Wan_9)
            {
                return MahjongType.Wan;
            }
            else if (mahjongInfo.m_weight <= MahjongWeight.Tiao_9)
            {
                return MahjongType.Tiao;
            }
            else if (mahjongInfo.m_weight <= MahjongWeight.Tong_9)
            {
                return MahjongType.Tong;
            }
            else if (mahjongInfo.m_weight <= MahjongWeight.Feng_Bei)
            {
                return MahjongType.Feng;
            }
            else
            {
                return MahjongType.Hua;
            }
        }

        // 供外部调用：发牌
        public void FaMahjong(List<MahjongInfo> zhuangjia, List<MahjongInfo> other1, List<MahjongInfo> other2, List<MahjongInfo> other3, List<MahjongInfo> rest)
        {
            List<MahjongInfo> mahjongList = new List<MahjongInfo>();
            for (int i = 0; i < m_mahjongList.Count; i++)
            {
                mahjongList.Add(m_mahjongList[i]);
            }

            // 先给庄家发牌:14张
            {
                for (int i = 0; i < 13; i++)
                {
                    int r = Common_Random.getRandom(0, mahjongList.Count - 1);
                    zhuangjia.Add(mahjongList[r]);

                    mahjongList.RemoveAt(r);
                }
            }

            // 给其他玩家1发牌:13张
            {
                for (int i = 0; i < 13; i++)
                {
                    int r = Common_Random.getRandom(0, mahjongList.Count - 1);
                    other1.Add(mahjongList[r]);

                    mahjongList.RemoveAt(r);
                }
            }

            // 给其他玩家2发牌:13张
            {
                for (int i = 0; i < 13; i++)
                {
                    int r = Common_Random.getRandom(0, mahjongList.Count - 1);
                    other2.Add(mahjongList[r]);

                    mahjongList.RemoveAt(r);
                }
            }

            // 给其他玩家3发牌:13张
            {
                for (int i = 0; i < 13; i++)
                {
                    int r = Common_Random.getRandom(0, mahjongList.Count - 1);
                    other3.Add(mahjongList[r]);

                    mahjongList.RemoveAt(r);
                }
            }

            // 剩余的牌
            {
                for (int i = 0; i < mahjongList.Count; i++)
                {
                    rest.Add(mahjongList[i]);
                }

                mahjongList.Clear();
            }
        }

        // 供外部调用：排序：从左到右显示
        public void SortMahjong(List<MahjongInfo> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = list.Count - 1; j >= 1; j--)
                {
                    if (list[j].m_weight < list[j - 1].m_weight)
                    {
                        MahjongInfo temp = list[j];
                        list[j] = list[j - 1];
                        list[j - 1] = temp;
                    }
                }
            }
        }

        // 供外部调用：检查是否胡牌
        public bool isHuPai(List<MahjongInfo> list)
        {
            try
            {
                int mahjongCount = list.Count;

                if ((mahjongCount % 3) != 2)
                {
                    return false;
                }

                List<MahjongInfo> list_copy = new List<MahjongInfo>();
                List<MahjongInfo> list_double = new List<MahjongInfo>();
                List<MahjongInfo> list_single = new List<MahjongInfo>();

                for (int i = 0; i < list.Count; i++)
                {
                    list_copy.Add(list[i]);
                }

                SortMahjong(list_copy);

                // 筛选对子
                list_double = selectDouble(list_copy);

                // 7个对子
                if (list_double.Count == 7)
                {
                    return true;
                }
                else
                {
                    // 尝试移除一个对子
                    // 对剩下的牌进行检测，删掉顺子和刻子
                    // 如果最后剩余牌数为0，则胡牌

                    for (int i = 0; i < list_double.Count; i++)
                    {
                        List<MahjongInfo> list_temp = new List<MahjongInfo>();
                        for (int j = 0; j < list_copy.Count; j++)
                        {
                            list_temp.Add(list_copy[j]);
                        }

                        int findCount = 0;
                        for (int k = list_temp.Count - 1; k >= 0; k--)
                        {
                            if (list_temp[k].m_weight == list_double[i].m_weight)
                            {
                                ++findCount;
                                list_temp.RemoveAt(k);
                                if (findCount == 2)
                                {
                                    if (list_temp.Count == 0)
                                    {
                                        return true;
                                    }
                                    else if (isAllShunZiOrKeZi(list_temp))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("异常：" + ex);
                Log.Error("传来的牌：" + JsonHelper.ToJson(list));
            }

            return false;
        }

        // 筛选对子
        List<MahjongInfo> selectDouble(List<MahjongInfo> list)
        {
            List<MahjongInfo> list_double = new List<MahjongInfo>();
            
            for (int i = 0; i < list.Count; i++)
            {
                int findCount = 1;
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i != j) && (list[i].m_weight == list[j].m_weight))
                    {
                        ++findCount;
                    }
                }

                if (findCount == 2)
                {
                    bool hasAdd = false;
                    for (int k = 0; k < list_double.Count; k++)
                    {
                        if (list_double[k].m_weight == list[i].m_weight)
                        {
                            hasAdd = true;
                            break;
                        }
                    }

                    if (!hasAdd)
                    {
//                        Console.WriteLine("找到对子：" + list[i].m_weight);
                        list_double.Add(list[i]);
                    }
                }
                else if (findCount == 4)
                {
                    bool hasAdd = false;
                    for (int k = 0; k < list_double.Count; k++)
                    {
                        if (list_double[k].m_weight == list[i].m_weight)
                        {
                            hasAdd = true;
                            break;
                        }
                    }

                    if (!hasAdd)
                    {
//                        Console.WriteLine("找到对子：" + list[i].m_weight);
//                        Console.WriteLine("找到对子：" + list[i].m_weight);
                        list_double.Add(list[i]);
                        list_double.Add(list[i]);
                    }
                }
            }

            return list_double;
        }

        // 筛选刻子
        List<MahjongInfo> selectThree(List<MahjongInfo> list)
        {
            List<MahjongInfo> list_three = new List<MahjongInfo>();

            for (int i = 0; i < list.Count; i++)
            {
                int findCount = 1;
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i != j) && (list[i].m_weight == list[j].m_weight))
                    {
                        ++findCount;
                    }
                }

                if (findCount == 3)
                {
                    bool hasAdd = false;
                    for (int k = 0; k < list_three.Count; k++)
                    {
                        if (list_three[k].m_weight == list[i].m_weight)
                        {
                            hasAdd = true;
                            break;
                        }
                    }

                    if (!hasAdd)
                    {
//                        Console.WriteLine("找到刻子：" + list[i].m_weight);
                        list_three.Add(list[i]);
                    }
                }
            }

            return list_three;
        }

        bool isAllShunZiOrKeZi(List<MahjongInfo> list)
        {
            string str = "传过来的牌:";
            for (int i = 0; i < list.Count; i++)
            {
                str += (list[i].m_weight + "、");
            }
//            Console.WriteLine(str);

            if (list.Count % 3 != 0)
            {
                return false;
            }

            while (list.Count != 0)
            {
                MahjongInfo first = list[0];
                MahjongInfo second = null;
                MahjongInfo third = null;

                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].m_weight == (first.m_weight + 1))
                    {
                        if (second == null)
                        {
                            second = list[i];
                        }
                    }
                    else if (list[i].m_weight == (first.m_weight + 2))
                    {
                        if (third == null)
                        {
                            third = list[i];
                        }
                    }
                }

                // 顺子找到了
                if ((second != null) && (third != null))
                {
//                    Console.WriteLine("找到顺子：" + first.m_weight + "  " + second.m_weight + "  " + third.m_weight);
                    list.Remove(first);
                    list.Remove(second);
                    list.Remove(third);
                }
                // 找不到顺子尝试去找刻子
                else
                {
                    second = null;
                    third = null;

                    for (int i = 1; i < list.Count; i++)
                    {
                        if (list[i].m_weight == first.m_weight)
                        {
                            if (second == null)
                            {
                                second = list[i];
                            }
                            else if (third == null)
                            {
                                third = list[i];
                            }
                        }
                    }

                    // 刻子找到了
                    if ((second != null) && (third != null))
                    {
//                        Console.WriteLine("找到刻子：" + first.m_weight + "  " + second.m_weight + "  " + third.m_weight);
                        list.Remove(first);
                        list.Remove(second);
                        list.Remove(third);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // 供外部调用：获取胡牌类型
        public List<HuPaiType> getHuPaiType(List<MahjongInfo> list, HuPaiNeedData huPaiNeedData)
        {
            List<MahjongInfo> list_copy = new List<MahjongInfo>();

            for (int i = 0; i < list.Count; i++)
            {
                list_copy.Add(list[i]);
            }

            SortMahjong(list_copy);

            List<HuPaiType> type_list = new List<HuPaiType>();
            
            // 门清
            {
                if (huPaiNeedData.my_pengList.Count == 0)
                {
                    type_list.Add(HuPaiType.MenQing);
                }
            }

            // 混一色
            {
                List<MahjongType> tempList = new List<MahjongType>();

                // 手牌
                for (int i = 0; i < list_copy.Count; i++)
                {
                    MahjongType mahjongType = getMahjongType(list_copy[i]);

                    bool isFind = false;
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j] == mahjongType)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        tempList.Add(mahjongType);
                    }
                }

                // 碰掉的牌
                for (int i = 0; i < huPaiNeedData.my_pengList.Count; i++)
                {
                    MahjongType mahjongType = getMahjongType(huPaiNeedData.my_pengList[i]);

                    bool isFind = false;
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j] == mahjongType)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        tempList.Add(mahjongType);
                    }
                }

                // 杠掉的牌
                for (int i = 0; i < huPaiNeedData.my_pengList.Count; i++)
                {
                    MahjongType mahjongType = getMahjongType(huPaiNeedData.my_pengList[i]);

                    bool isFind = false;
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j] == mahjongType)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        tempList.Add(mahjongType);
                    }
                }

                if (tempList.Count == 2)
                {
                    if (((tempList[0] == MahjongType.Feng) && (tempList[1] != MahjongType.Feng)) ||
                        ((tempList[1] == MahjongType.Feng) && (tempList[0] != MahjongType.Feng)))
                    {
                        type_list.Add(HuPaiType.HunYiSe);
                    }
                }
            }

            // 清一色
            {
                List<MahjongType> tempList = new List<MahjongType>();

                // 手牌
                for (int i = 0; i < list_copy.Count; i++)
                {
                    MahjongType mahjongType = getMahjongType(list_copy[i]);

                    bool isFind = false;
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j] == mahjongType)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        tempList.Add(mahjongType);
                    }
                }

                // 碰掉的牌
                for (int i = 0; i < huPaiNeedData.my_pengList.Count; i++)
                {
                    MahjongType mahjongType = getMahjongType(huPaiNeedData.my_pengList[i]);

                    bool isFind = false;
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j] == mahjongType)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        tempList.Add(mahjongType);
                    }
                }

                // 杠掉的牌
                for (int i = 0; i < huPaiNeedData.my_pengList.Count; i++)
                {
                    MahjongType mahjongType = getMahjongType(huPaiNeedData.my_pengList[i]);

                    bool isFind = false;
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j] == mahjongType)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        tempList.Add(mahjongType);
                    }
                }

                if (tempList.Count == 1)
                {
                    if (tempList[0] != MahjongType.Feng)
                    {
                        type_list.Add(HuPaiType.QingYiSe);
                    }
                }
            }

            // 对对胡
            {
                List<MahjongInfo> list_double = selectDouble(list_copy);
                List<MahjongInfo> list_three = selectThree(list_copy);

                if ((list_double.Count == 1) && ((list_three.Count + huPaiNeedData.my_pengList.Count) == 4))
                {
                    type_list.Add(HuPaiType.DuiDuiHu);
                }
            }

            // 全球独钓
            {
                if (list_copy.Count == 2)
                {
                    type_list.Add(HuPaiType.QuanQiuDuDiao);
                }
            }

            // 七对
            {
                List<MahjongInfo> list_double = selectDouble(list_copy);
                if (list_double.Count == 7)
                {
                    List<MahjongInfo> list_four = new List<MahjongInfo>();
                    for (int i = 0; i < list_double.Count; i++)
                    {
                        for (int j = 0; j < list_double.Count; j++)
                        {
                            if ((i != j) && (list_double[i] == list_double[j]))
                            {
                                bool isFind = false;
                                for (int k = 0; k < list_four.Count; k++)
                                {
                                    if (list_four[k].m_weight == list_double[i].m_weight)
                                    {
                                        isFind = true;
                                        break;
                                    }
                                }

                                if (!isFind)
                                {
                                    list_four.Add(list_double[i]);
                                }
                            }
                        }
                    }

                    // 超超豪华
                    if (list_four.Count >= 3)
                    {
                        type_list.Add(HuPaiType.QiDui_ChaoChaoHaoHua);
                    }
                    // 超豪华
                    else if (list_four.Count >= 2)
                    {
                        type_list.Add(HuPaiType.QiDui_ChaoHaoHua);
                    }
                    // 豪华
                    else if (list_four.Count >= 1)
                    {
                        type_list.Add(HuPaiType.QiDui_HaoHua);
                    }
                    // 普通
                    else
                    {
                        type_list.Add(HuPaiType.QiDui_Normal);
                    }
                }
            }

            // 压绝
            {
                bool isYaJue = false;
                for (int i = 0; i < huPaiNeedData.my_pengList.Count; i++)
                {
                    if (huPaiNeedData.my_pengList[i].m_weight == huPaiNeedData.my_lastMahjong.m_weight)
                    {
                        isYaJue = true;
                        break;
                    }
                }

                if (!isYaJue)
                {
                    for (int i = 0; i < huPaiNeedData.other1_pengList.Count; i++)
                    {
                        if (huPaiNeedData.other1_pengList[i].m_weight == huPaiNeedData.my_lastMahjong.m_weight)
                        {
                            isYaJue = true;
                            break;
                        }
                    }
                }

                if (!isYaJue)
                {
                    for (int i = 0; i < huPaiNeedData.other2_pengList.Count; i++)
                    {
                        if (huPaiNeedData.other2_pengList[i].m_weight == huPaiNeedData.my_lastMahjong.m_weight)
                        {
                            isYaJue = true;
                            break;
                        }
                    }
                }

                if (!isYaJue)
                {
                    for (int i = 0; i < huPaiNeedData.other3_pengList.Count; i++)
                    {
                        if (huPaiNeedData.other3_pengList[i].m_weight == huPaiNeedData.my_lastMahjong.m_weight)
                        {
                            isYaJue = true;
                            break;
                        }
                    }
                }

                if (isYaJue)
                {
                    type_list.Add(HuPaiType.YaJue);
                }
            }

            // 无花果
            {
                if (huPaiNeedData.my_yingHuaList.Count == 0)
                {
                    type_list.Add(HuPaiType.WuHuaGuo);
                }
            }

            // 杠后开花
            {
                // 小:抓到硬花补牌后胡牌
                if (huPaiNeedData.isGetYingHuaBuPai)
                {
                    type_list.Add(HuPaiType.GangHouKaiHua_Small);
                }

                // 大:杠完补牌后胡牌
                if (huPaiNeedData.isGangEndBuPai)
                {
                    type_list.Add(HuPaiType.GangHouKaiHua_Big);
                }
            }

            // 天胡
            {
                if (huPaiNeedData.isZhuangJia && huPaiNeedData.isGangFaWanPai && (list_copy.Count == 14))
                {
                    type_list.Add(HuPaiType.TianHu);
                }
            }

            // 地胡
            {
                if (huPaiNeedData.isFaWanPaiTingPai)
                {
                    if ((huPaiNeedData.my_pengList.Count == 0) && (huPaiNeedData.my_gangList.Count == 0))
                    {
                        type_list.Add(HuPaiType.DiHu);
                    }
                }
            }

            // 海底捞月
            {
                if ((huPaiNeedData.restMahjongCount <= (8 * 4)) && huPaiNeedData.isSelfZhuaPai)
                {
                    type_list.Add(HuPaiType.HaiDiLaoYue);
                }
            }

            // 小胡：最普通的胡牌
            if(type_list.Count == 0)
            {
                if (huPaiNeedData.my_yingHuaList.Count >= 4)
                {
                    type_list.Add(HuPaiType.Normal);
                }
            }

            return type_list;
        }

        // 供外部调用：是否可以碰
        // mahjongInfo：别人出的牌
        // list：我的手牌
        public bool isCanPeng(MahjongInfo mahjongInfo , List<MahjongInfo> list)
        {
            int findCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].m_weight == mahjongInfo.m_weight)
                {
                    ++findCount;
                }
            }

            if (findCount >= 2)
            {
                return true;
            }

            return false;
        }

        // 供外部调用：是否可以杠
        // mahjongInfo：如果是别人出的牌，则为明杠，如果是自己新抓的牌，则为暗杠
        // list：我的手牌
        public bool isCanGang(MahjongInfo mahjongInfo, List<MahjongInfo> list)
        {
            int findCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].m_weight == mahjongInfo.m_weight)
                {
                    ++findCount;
                }
            }

            if (findCount >= 3)
            {
                return true;
            }

            return false;
        }

        // 供外部调用：检查听的牌，为空则没听牌
        public List<MahjongInfo> checkTingPaiList(List<MahjongInfo> list)
        {
            List<MahjongInfo> tingpaiList = new List<MahjongInfo>();
            try
            {
                for (int i = 0; i < m_differenceMahjongList.Count; i++)
                {
                    List<MahjongInfo> temp = new List<MahjongInfo>();
                    for (int j = 0; j < list.Count; j++)
                    {
                        temp.Add(list[j]);
                    }

                    MahjongInfo mahjongInfoTemp = m_differenceMahjongList[i];
                    temp.Add(mahjongInfoTemp);

                    if (isHuPai(temp))
                    {
                        tingpaiList.Add(mahjongInfoTemp);
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error("听牌：" + e);
            }
           
            return tingpaiList;
        }

        public int GetIndex(List<MahjongInfo> mahjongInfos, MahjongInfo mahjongInfo)
        {

            int index = -1;
            for (int i = 0; i < mahjongInfos.Count; i++)
            {
                if (mahjongInfos[i].m_weight == mahjongInfo.m_weight)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public void RemoveCard(List<MahjongInfo> mahjongInfos, MahjongInfo mahjongInfo)
        {
            int index = GetIndex(mahjongInfos, mahjongInfo);
            if (index >= 0)
            {
                mahjongInfos.RemoveAt(index);
            }
        }
    }
}
