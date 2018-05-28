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

        public void SetGoldItem(WealthRank wealth, int index)
        {
            RankImg.gameObject.SetActive(index < 3);
            RankTxt.gameObject.SetActive(index >= 3);
            if (RankTxt.gameObject.activeInHierarchy)
                RankTxt.text = (index + 1).ToString();
            NameTxt.text = wealth.PlayerName;
            GoldTxt.text = new StringBuilder().Append("金币:")
                                              .Append(wealth.GoldNum)
                                              .ToString();
            Icon.sprite = CommonUtil.getSpriteByBundle("playericon", wealth.Icon);
            string rIcon = new StringBuilder().Append("Rank_")
                                              .Append(index + 1)
                                              .ToString() ;
            if (RankImg.gameObject.activeInHierarchy)
                RankImg.sprite = CommonUtil.getSpriteByBundle("uirankicon", rIcon);
        }

        public void SetGameItem(GameRank gameRank,int index)
        {
            RankImg.gameObject.SetActive(index < 3);
            RankTxt.gameObject.SetActive(index >= 3);
            NameTxt.text = gameRank.PlayerName;
            GoldTxt.text = new StringBuilder().Append("总局数:")
                                              .Append(gameRank.TotalCount)
                                              .ToString();
            if (RankTxt.gameObject.activeInHierarchy)
                RankTxt.text = (index + 1).ToString();
            Icon.sprite = CommonUtil.getSpriteByBundle("playericon", gameRank.Icon);
            string rIcon = new StringBuilder().Append("Rank_")
                                           .Append(index +1)
                                           .ToString();
            if (RankImg.gameObject.activeInHierarchy)
                RankImg.sprite = CommonUtil.getSpriteByBundle("uirankicon", rIcon);
        }
    }
}
