﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChengjiuItemSystem : AwakeSystem<UIChengjiuItemComponent>
    {
        public override void Awake(UIChengjiuItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIChengjiuItemComponent : Component
    {
        private GameObject ChengjiuItemBtn;
        private TaskInfo info;
        
        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            ChengjiuItemBtn = rc.Get<GameObject>("ChengjiuItemBtn");

            ChengjiuItemBtn.GetComponent<Button>().onClick.Add(() =>
            {
                if (info.IsComplete)
                {
                    if (!info.IsGet)
                    {
                        //未领取
                        info.IsGet = true;
                        GetReward();
                    }
                    else
                    {
                        //显示提示框
                        Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChengjiu)
                        .GetComponent<UIChengjiuComponent>().SetDetail(info, true);
                    }
                }
                else
                {
                    //显示提示框
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChengjiu)
                    .GetComponent<UIChengjiuComponent>().SetDetail(info, false);
                }
            });
        }

        private async void GetReward()
        {
            G2C_UpdateChengjiu g2cTask = (G2C_UpdateChengjiu)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdateChengjiu { UId = PlayerInfoComponent.Instance.uid, TaskPrg = info });
            PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum += info.Reward;
            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain)
                .GetComponent<UIMainComponent>().refreshUI();
            //显示提示框
            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChengjiu)
            .GetComponent<UIChengjiuComponent>().SetDetail(info, true);
        }

        public void SetInfo(TaskInfo info)
        {
            this.info = info;
            string icon = new StringBuilder().Append("chengjiu1_")
                                             .Append(info.Id).ToString();
            if(info.IsComplete)
            {
                icon = new StringBuilder().Append("chengjiu_")
                                             .Append(info.Id).ToString();
            }
            ChengjiuItemBtn.GetComponent<Image>().sprite =
                CommonUtil.getSpriteByBundle("uichengjiuicon", icon);
        }
    }
}
