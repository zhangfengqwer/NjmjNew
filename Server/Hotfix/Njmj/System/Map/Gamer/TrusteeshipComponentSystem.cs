using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class TrusteeshipComponentStartSystem : StartSystem<TrusteeshipComponent>
    {
        public override void Start(TrusteeshipComponent self)
        {
            self.Start();
        }
    }

    public static class TrusteeshipComponentSystem
    {
        public static async void Start(this TrusteeshipComponent self)
        {
            //玩家所在房间
            Room room = Game.Scene.GetComponent<RoomComponent>().Get(self.GetParent<Gamer>().RoomID);
            OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
            Gamer gamer = self.GetParent<Gamer>();
            bool isStartPlayCard = false;

            while (true)
            {
                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);

                if (self.IsDisposed)
                {
                    return;
                }

                if (gamer.UserID != orderController?.CurrentAuthority)
                {
                    continue;
                }

                //自动出牌开关,用于托管延迟出牌
                isStartPlayCard = !isStartPlayCard;
                if (isStartPlayCard)
                {
                    continue;
                }

                //出牌
                await gamer.GetComponent<HandCardsComponent>().PopCard();
            }
        }
    }
}
