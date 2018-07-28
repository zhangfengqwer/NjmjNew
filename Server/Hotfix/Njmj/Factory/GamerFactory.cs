using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    public static class GamerFactory
    {
        /// <summary>
        /// 创建玩家对象
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<Gamer> Create(long playerId, long userId, long? id = null)
        {
            Gamer gamer = ComponentFactory.CreateWithId<Gamer, long>(id ?? IdGenerater.GenerateId(), userId);
            gamer.PlayerID = playerId;
            gamer.isOffline = false;
            gamer.playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(userId);
            return gamer;
        }
    }
}
