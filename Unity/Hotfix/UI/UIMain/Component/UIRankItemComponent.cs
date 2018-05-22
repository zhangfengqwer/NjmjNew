using ETModel;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIRankItemSystem: AwakeSystem<UIRankItemComponent>
    {
        public override void Awake(UIRankItemComponent self)
        {
            self.Awake();
        }
    }
    public class UIRankItemComponent : Component
    {
        private Text GoldTxt;
        private Text NameTxt;
        private Image RankImg;
        private Text RankTxt;
        private Image Icon;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            GoldTxt = rc.Get<GameObject>("GoldTxt").GetComponent<Text>();
            NameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            RankImg = rc.Get<GameObject>("RankImg").GetComponent<Image>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            RankTxt = rc.Get<GameObject>("RankTxt").GetComponent<Text>();
        }

        public void SetGoldItem(WealthRank wealth,int index)
        {
            RankImg.gameObject.SetActive(index < 3);
            RankTxt.gameObject.SetActive(index >= 3);
            NameTxt.text = wealth.PlayerName;
            GoldTxt.text = new StringBuilder().Append("金币:")
                                              .Append(wealth.GoldNum)
                                              .ToString();
            RankTxt.text = index.ToString();
            Icon.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(wealth.Icon);
            int rIcon = index;
            if (index == 31)
                RankTxt.text = "未上榜";
            if (RankImg.gameObject.activeInHierarchy)
                RankImg.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite("Rank_" + rIcon);
        }

        public void SetGameItem(GameRank gameRank,int index)
        {
            RankImg.gameObject.SetActive(index < 3);
            RankTxt.gameObject.SetActive(index >= 3);
            NameTxt.text = gameRank.PlayerName;
            GoldTxt.text = new StringBuilder().Append("总局数:")
                                              .Append(gameRank.TotalCount)
                                              .ToString();
            RankTxt.text = index.ToString();
            Icon.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(gameRank.Icon);
            int rIcon = index + 1;
            if (RankImg.gameObject.activeInHierarchy)
                RankImg.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite("Rank_" + rIcon);
        }
    }
}
