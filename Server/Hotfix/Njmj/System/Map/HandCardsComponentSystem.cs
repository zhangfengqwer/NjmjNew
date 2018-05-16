﻿using System.Collections.Generic;
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
        /// 出牌
        /// </summary>
        /// <param name="self"></param>
        /// <param name="card"></param>
        public static void PopCard(this HandCardsComponent self, MahjongInfo card)
        {
            self.library.Remove(card);
        }

        /// <summary>
        /// 手牌排序
        /// </summary>
        /// <param name="self"></param>
        public static void Sort(this HandCardsComponent self)
        {
            Logic_NJMJ.getInstance().SortMahjong(self.library);
        }
    }
}