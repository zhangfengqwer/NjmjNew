using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerBuyYuanBaoHandler : AMHandler<Actor_GamerBuyYuanBao>
    {
        protected override async void Run(ETModel.Session session, Actor_GamerBuyYuanBao message)
        {
            try
            {
                ToastScript.createToast("支付成功");
                G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo)await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = PlayerInfoComponent.Instance.uid });

                if (g2CPlayerInfo == null)
                {
                    Log.Debug("用户信息错误");
                    return;
                }
                PlayerInfoComponent.Instance.SetPlayerInfo(g2CPlayerInfo.PlayerInfo);
                GameUtil.changeData(1, 0);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
