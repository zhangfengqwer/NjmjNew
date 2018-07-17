using ETModel;

namespace ETHotfix
{
    public static class PlayerInfoFactory
    {
        /// <summary>
        /// 创建玩家对象
        /// </summary>
        /// <param name="playerBaseInfo"></param>
        /// <returns></returns>
        public static PlayerInfo Create(PlayerBaseInfo playerBaseInfo)
        {
            PlayerInfo playerInfo = new PlayerInfo
            {
                Icon = playerBaseInfo.Icon,
                Name = playerBaseInfo.Name,
                GoldNum = playerBaseInfo.GoldNum,
                WinGameCount = playerBaseInfo.WinGameCount,
                TotalGameCount = playerBaseInfo.TotalGameCount,
                VipTime = playerBaseInfo.VipTime,
                PlayerSound = playerBaseInfo.PlayerSound,
                RestChangeNameCount = playerBaseInfo.RestChangeNameCount,
                EmogiTime = playerBaseInfo.EmogiTime,
                MaxHua = playerBaseInfo.MaxHua
            };
            return playerInfo;
        }
    }
}
