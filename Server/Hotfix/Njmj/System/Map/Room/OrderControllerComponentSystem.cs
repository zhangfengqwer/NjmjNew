using System;
using ETModel;

namespace ETHotfix
{
    public static class OrderControllerComponentSystem
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="self"></param>
        public static void Init(this OrderControllerComponent self, long id)
        {
            self.FirstAuthority = new System.Collections.Generic.KeyValuePair<long, bool>(id, false);
            self.CurrentAuthority = id;
            self.SelectLordIndex = 1;
            self.GamerLandlordState.Clear();
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="self"></param>
        public static void Start(this OrderControllerComponent self, long id)
        {
            self.CurrentAuthority = id;
        }

        /// <summary>
        /// 轮转
        /// </summary>
        /// <param name="self"></param>
        public static void Turn(this OrderControllerComponent self)
        {
            int index = -1;
            try
            {
                Room room = self.GetParent<Room>();
                Gamer[] gamers = room.GetAll();

                for (int i = 0; i < gamers.Length; i++)
                {
                    Gamer gamer = gamers[i];
                    if (gamer == null)
                    {
                        Log.Error("在轮转的时候玩家为null:" + room.State);
                        continue;
                    }

                    if (self.CurrentAuthority == gamer.UserID)
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0)
                {
                    Log.Error("玩家轮转的时候room的玩家都是null");
                    return;
                }

                index++;
                if (index == gamers.Length)
                {
                    index = 0;
                }

                self.CurrentAuthority = gamers[index].UserID;
            }
            catch (Exception e)
            {
                Log.Error($"角标越界{index}:"+e);
            }
        }

        /// <summary>
        /// 找到下一个玩家
        /// </summary>
        /// <param name="self"></param>
        public static Gamer FindNextGamer(this OrderControllerComponent self, long userId)
        {
            Room room = self.GetParent<Room>();
            Gamer[] gamers = room.GetAll();

            int index = room.GetGamerSeat(userId);
            index++;
            if (index == gamers.Length)
            {
                index = 0;
            }
            return gamers[index];
        }
    }
}