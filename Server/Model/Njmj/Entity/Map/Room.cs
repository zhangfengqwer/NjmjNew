﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ETHotfix;

namespace ETModel
{
    /// <summary>
    /// 房间状态
    /// </summary>
    public enum RoomState : byte
    {
        Idle,
        Ready,
        Game
    }

    /// <summary>
    /// 房间对象
    /// </summary>
    public sealed class Room : Entity
    {
        public readonly Dictionary<long, int> seats = new Dictionary<long, int>();
        public readonly Gamer[] gamers = new Gamer[4];
        public readonly List<IActorMessage> reconnectList = new List<IActorMessage>();

        //是否超时
        public bool IsTimeOut = false;

        public bool IsGameOver = false;

        //10秒超时
        public int TimeOut = 10;

        //5秒碰刚超时
        public int OperationTimeOut = 6;

        //
        public bool IsOperate = false;

        public CancellationTokenSource tokenSource;

        //房间状态
        public RoomState State { get; set; } = RoomState.Idle;

        //房间玩家数量
        public int Count
        {
            get { return seats.Values.Count; }
        }

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

        public long ziMoUid;

        public long fangPaoUid;

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="gamer"></param>
        public void Add(Gamer gamer)
        {
            int seatIndex = GetEmptySeat();
            //玩家需要获取一个座位坐下
            if (seatIndex >= 0)
            {
                gamers[seatIndex] = gamer;
                seats[gamer.UserID] = seatIndex;

                gamer.RoomID = this.Id;
            }
        }

        /// <summary>
        /// 根据userid获取玩家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Gamer Get(long userId)
        {
            int seatIndex = GetGamerSeat(userId);
            if (seatIndex >= 0)
            {
                return gamers[seatIndex];
            }

            return null;
        }

        /// <summary>
        /// 获取所有玩家
        /// </summary>
        /// <returns></returns>
        public Gamer[] GetAll()
        {
            return gamers;
        }

        /// <summary>
        /// 获取玩家座位索引
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetGamerSeat(long userId)
        {
            if (seats.TryGetValue(userId, out int seatIndex))
            {
                return seatIndex;
            }

            return -1;
        }

        /// <summary>
        /// 移除玩家并返回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Gamer Remove(long userId)
        {
            int seatIndex = GetGamerSeat(userId);
            if (seatIndex >= 0)
            {
                Gamer gamer = gamers[seatIndex];
                gamers[seatIndex] = null;
                seats.Remove(userId);

                gamer.RoomID = 0;
                return gamer;
            }

            return null;
        }

        /// <summary>
        /// 获取空座位
        /// </summary>
        /// <returns>返回座位索引，没有空座位时返回-1</returns>
        public int GetEmptySeat()
        {
            for (int i = 0; i < gamers.Length; i++)
            {
                if (gamers[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="message"></param>
//        public void Broadcast(IMessage message)
//        {
//            foreach (Gamer gamer in gamers)
//            {
//                if (gamer == null || gamer.isOffline)
//                {
//                    continue;
//                }
//                ActorProxy actorProxy = gamer.GetComponent<UnitGateComponent>().GetActorProxy();
//                actorProxy.Send(message);
//            }
//        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            seats.Clear();

            for (int i = 0; i < gamers.Length; i++)
            {
                if (gamers[i] != null)
                {
                    gamers[i].Dispose();
                    gamers[i] = null;
                }
            }

            State = RoomState.Idle;
            reconnectList.Clear();

            IsGameOver = false;
            isSelfZhuaPai = false;
            // 自己是否是庄家
            isZhuangJia = false;
            // 是否是刚发完牌
            isGangFaWanPai = false;
            // 是否是刚发完牌就听牌
            isFaWanPaiTingPai = false;
            // 是否是杠完补牌
            isGangEndBuPai = false;
            // 是否是抓到硬花补牌
            isGetYingHuaBuPai = false;
        }
    }
}