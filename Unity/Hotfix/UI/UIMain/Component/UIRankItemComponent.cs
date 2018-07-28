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
        private Image Img;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            GoldTxt = rc.Get<GameObject>("GoldTxt").GetComponent<Text>();
            NameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            RankImg = rc.Get<GameObject>("RankImg").GetComponent<Image>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            RankTxt = rc.Get<GameObject>("RankTxt").GetComponent<Text>();
            Img = rc.Get<GameObject>("Img").GetComponent<Image>();
        }

        public void SetGoldItem(WealthRank wealth, int index)
        {
            RankImg.gameObject.SetActive(index < 3);
            RankTxt.gameObject.SetActive(index >= 3);
            if (RankTxt.gameObject.activeInHierarchy)
                RankTxt.text = (index + 1).ToString();
            NameTxt.text = wealth.PlayerName;
            GoldTxt.text = new StringBuilder().Append(wealth.GoldNum)
                                              .ToString();
            Img.sprite = CommonUtil.getSpriteByBundle("image_main","icon_jinbi");
            HeadManager.setHeadSprite(Icon, wealth.Icon);
            string rIcon = new StringBuilder().Append("Rank_")
                                              .Append(index + 1)
                                              .ToString() ;
            if (RankImg.gameObject.activeInHierarchy)
                RankImg.sprite = CommonUtil.getSpriteByBundle("image_main", rIcon);
        }

        public void SetGameItem(GameRank gameRank,int index)
        {
            RankImg.gameObject.SetActive(index < 3);
            RankTxt.gameObject.SetActive(index >= 3);
            NameTxt.text = gameRank.PlayerName;
            Img.sprite = CommonUtil.getSpriteByBundle("image_main", "win");
            GoldTxt.text = new StringBuilder().Append(gameRank.WinCount)
                                              .ToString();
            if (RankTxt.gameObject.activeInHierarchy)
                RankTxt.text = (index + 1).ToString();
            HeadManager.setHeadSprite(Icon, gameRank.Icon);
            string rIcon = new StringBuilder().Append("Rank_")
                                           .Append(index +1)
                                           .ToString();
            if (RankImg.gameObject.activeInHierarchy)
                RankImg.sprite = CommonUtil.getSpriteByBundle("image_main", rIcon);
        }
    }
}
