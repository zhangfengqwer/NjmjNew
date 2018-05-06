using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ETHotfix.Consts;

namespace ETHotfix
{
    public class MahjongInfo
    {
        public MahjongWeight m_weight;

        public MahjongInfo(MahjongWeight weight)
        {
            m_weight = weight;
        }
    }

    public class Logic_NJMJ
    {
        static Logic_NJMJ s_instance = null;

        List<MahjongInfo> m_mahjongList = new List<MahjongInfo>();

        public static Logic_NJMJ getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new Logic_NJMJ();
                s_instance.initMahjongList();
            }

            return s_instance;
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
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Wan_1 + 0));
                    }
                }

                // 条
                for (int i = 1; i <= 9; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Tiao_1 + 10));
                    }
                }

                // 筒
                for (int i = 1; i <= 9; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        m_mahjongList.Add(new MahjongInfo(MahjongWeight.Tong_1 + 20));
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
        }

        // 发牌
        public void FaMahjong(List<MahjongInfo> zhuangjia, List<MahjongInfo> other1, List<MahjongInfo> other2, List<MahjongInfo> other3, List<MahjongInfo> rest)
        {
            List<MahjongInfo> mahjongList = new List<MahjongInfo>();
            for (int i = 0; i < m_mahjongList.Count; i++)
            {
                mahjongList.Add(m_mahjongList[i]);
            }

            // 先给庄家发牌:14张
            {
                for (int i = 0; i < 14; i++)
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

        // 排序：从左到右显示
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

        // 检查是否胡牌
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
                {
                    for (int i = 0; i < list_copy.Count; i++)
                    {
                        int findCount = 1;
                        for (int j = 0; j < list_copy.Count; j++)
                        {
                            if ((i != j) && (list_copy[i].m_weight == list_copy[j].m_weight))
                            {
                                ++findCount;
                            }
                        }

                        if (findCount == 2)
                        {
                            bool hasAdd = false;
                            for (int k = 0; k < list_double.Count; k++)
                            {
                                if (list_double[k].m_weight == list_copy[i].m_weight)
                                {
                                    hasAdd = true;
                                    break;
                                }
                            }

                            if (!hasAdd)
                            {
                                Console.WriteLine("找到对子：" + list_copy[i].m_weight);
                                list_double.Add(list_copy[i]);
                            }
                        }
                        else if (findCount == 4)
                        {
                            bool hasAdd = false;
                            for (int k = 0; k < list_double.Count; k++)
                            {
                                if (list_double[k].m_weight == list_copy[i].m_weight)
                                {
                                    hasAdd = true;
                                    break;
                                }
                            }

                            if (!hasAdd)
                            {
                                Console.WriteLine("找到对子：" + list_copy[i].m_weight);
                                Console.WriteLine("找到对子：" + list_copy[i].m_weight);
                                list_double.Add(list_copy[i]);
                                list_double.Add(list_copy[i]);
                            }
                        }
                    }
                }

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
                Console.WriteLine("异常：" + ex);
            }

            return false;
        }

        public bool isAllShunZiOrKeZi(List<MahjongInfo> list)
        {
            string str = "传过来的牌:";
            for (int i = 0; i < list.Count; i++)
            {
                str += (list[i].m_weight + "、");
            }
            Console.WriteLine(str);

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
                    Console.WriteLine("找到顺子：" + first.m_weight + "  " + second.m_weight + "  " + third.m_weight);
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
                        Console.WriteLine("找到刻子：" + first.m_weight + "  " + second.m_weight + "  " + third.m_weight);
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
    }
}
