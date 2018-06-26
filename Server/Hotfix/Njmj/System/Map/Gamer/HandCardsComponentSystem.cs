using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    public static class HandCardsComponentSystem
    {
        /// <summary>
        /// 获取所有手牌
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<MahjongInfo> GetAll(this HandCardsComponent self)
        {
            return self.library;
        }

        /// <summary>
        /// 向牌库中添加牌
        /// </summary>
        /// <param name="card"></param>
        public static void AddCard(this HandCardsComponent self, MahjongInfo card)
        {
            self.library.Add(card);
        }


        /// <summary>
        /// 手牌排序
        /// </summary>
        /// <param name="self"></param>
        public static void Sort(this HandCardsComponent self)
        {
            Logic_NJMJ.getInstance().SortMahjong(self.library);
        }

        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="self"></param>
        public static async Task<MahjongInfo> PopCard(this HandCardsComponent self)
        {
            if (self == null) return null;
            Gamer gamer = self.GetParent<Gamer>();

            HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();

            List<MahjongInfo> mahjongInfos = self.GetAll();
            MahjongInfo mahjongInfo = handCardsComponent.GrabCard;
            int index = -1;

            for (int i = 0; i < handCardsComponent.GetAll().Count; i++)
            {
                MahjongInfo info = handCardsComponent.GetAll()[i];
                if (info.m_weight == mahjongInfo.m_weight)
                {
                    index = i;
                    break;
                }
            }

            //最右边的一张
            if (index < 0)
            {
                mahjongInfo = handCardsComponent.GetAll()[handCardsComponent.GetAll().Count - 1];
                index = handCardsComponent.GetAll().Count - 1;
            }
//            int randomNumber = RandomHelper.RandomNumber(0, mahjongInfos.Count);
//
//            MahjongInfo mahjongInfo = mahjongInfos[randomNumber];
            await Actor_GamerPlayCardHandler.PlayCard(gamer, new Actor_GamerPlayCard()
            {
                Uid = gamer.UserID,
                weight = (int)mahjongInfo.m_weight,
                index = index
            });

            return mahjongInfo;
        }
    }
}
