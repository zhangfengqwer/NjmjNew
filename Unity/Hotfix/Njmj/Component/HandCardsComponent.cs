using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class HandCardsComponentAwakeSystem : AwakeSystem<HandCardsComponent, GameObject>
    {
        public override void Awake(HandCardsComponent self, GameObject panel)
        {
            self.Awake(panel);
        }
    }

    public class HandCardsComponent : Component
    {
        public const string HANDCARD_NAME = "HandCard";
        public const string PLAYCARD_NAME = "PlayCard";

        private readonly Dictionary<string, GameObject> cardsSprite = new Dictionary<string, GameObject>();
        private readonly List<MahjongInfo> handCards = new List<MahjongInfo>();
        private readonly List<MahjongInfo> playCards = new List<MahjongInfo>();
        private GameObject cardRight;
        private GameObject cardLeft;

        public GameObject Panel { get; private set; }
//        public Identity AccessIdentity { get; set; }

        public void Awake(GameObject panel)
        {
            this.Panel = panel;
            this.cardRight = panel.Get<GameObject>("CardRight");
            this.cardLeft = panel.Get<GameObject>("CardLeft");
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            Reset();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            ClearHandCards();
            ClearPlayCards();
        }

        /// <summary>
        /// 显示玩家游戏UI
        /// </summary>
        public void Appear()
        {
//            _poker?.SetActive(true);
//            _handCards?.SetActive(true);
        }

        /// <summary>
        /// 隐藏玩家游戏UI
        /// </summary>
        public void Hide()
        {

        }

        /// <summary>
        /// 获取卡牌精灵
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
//        public GameObject GetSprite(MahjongInfo card)
//        {
////            GameObject cardSprite;
//////            cardsSprite.TryGetValue(card.GetName(), out cardSprite);
////            return cardSprite;
//        }

        /// <summary>
        /// 设置手牌数量
        /// </summary>
        /// <param name="num"></param>
        public void SetHandCardsNum(int num)
        {
//            _pokerNum.text = num.ToString();
        }

        /// <summary>
        /// 添加多张牌
        /// </summary>
        /// <param name="cards"></param>
//        public void AddCards(Card[] cards)
//        {
//            for (int i = 0; i < cards.Length; i++)
//            {
//                AddCard(cards[i]);
//            }
//            CardsSpriteUpdate(handCards, 50.0f);
//        }

        /// <summary>
        /// 出多张牌
        /// </summary>
        /// <param name="cards"></param>
//        public void PopCards(Card[] cards)
//        {
//            ClearPlayCards();
//
//            for (int i = 0; i < cards.Length; i++)
//            {
//                PopCard(cards[i]);
//            }
//            CardsSpriteUpdate(playCards, 25.0f);
//            CardsSpriteUpdate(handCards, 50.0f);
//
//            //同步剩余牌数
//            GameObject poker = this.Panel.Get<GameObject>("Poker");
//            if (poker != null)
//            {
//                Text pokerNum = poker.GetComponentInChildren<Text>();
//                pokerNum.text = (int.Parse(pokerNum.text) - cards.Length).ToString();
//            }
//        }

        /// <summary>
        /// 清空手牌
        /// </summary>
        public void ClearHandCards()
        {
//            ClearCards(handCards);
        }

        /// <summary>
        /// 清空出牌
        /// </summary>
        public void ClearPlayCards()
        {
//            ClearCards(playCards);
        }

//        /// <summary>
//        /// 卡牌精灵更新
//        /// </summary>
//        public void CardsSpriteUpdate(List<Card> cards, float interval)
//        {
//            if (cards.Count == 0)
//            {
//                return;
//            }
//
//            Sort(cards);
//
//            float width = GetSprite(cards[0]).GetComponent<RectTransform>().sizeDelta.x;
//            float startX = -((cards.Count - 1) * interval) / 2;
//            for (int i = 0; i < cards.Count; i++)
//            {
//                RectTransform rect = GetSprite(cards[i]).GetComponent<RectTransform>();
//                rect.anchoredPosition = new Vector2(startX + (i * interval), rect.anchoredPosition.y);
//            }
//        }

        /// <summary>
        /// 清空卡牌
        /// </summary>
        /// <param name="cards"></param>
//        private void ClearCards(List<Card> cards)
//        {
//            for (int i = cards.Count - 1; i >= 0; i--)
//            {
//                Card card = cards[i];
//                GameObject cardSprite = cardsSprite[card.GetName()];
//                cardsSprite.Remove(card.GetName());
//                cards.Remove(card);
//                UnityEngine.Object.Destroy(cardSprite);
//            }
//        }

        /// <summary>
        /// 卡牌排序
        /// </summary>
        /// <param name="cards"></param>
//        private void Sort(List<Card> cards)
//        {
//            CardHelper.Sort(cards);
//
//            //卡牌精灵层级排序
//            for (int i = 0; i < cards.Count; i++)
//            {
//                GetSprite(cards[i]).transform.SetSiblingIndex(i);
//            }
//        }

        /// <summary>
        /// 添加卡牌
        /// </summary>
        /// <param name="card"></param>
//        private void AddCard(Card card)
//        {
//            GameObject handCardSprite = CreateCardSprite(HANDCARD_NAME, card.GetName(), this.Panel.Get<GameObject>("HandCards").transform);
//            handCardSprite.GetComponent<HandCardSprite>().Poker = card;
//
//            cardsSprite.Add(card.GetName(), handCardSprite);
//            handCards.Add(card);
//        }

        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="card"></param>
//        private void PopCard(Card card)
//        {
//            //移除手牌
//            if (handCards.Contains(card))
//            {
//                GameObject handCardSprite = GetSprite(card);
//                cardsSprite.Remove(card.GetName());
//                handCards.Remove(card);
//                UnityEngine.Object.Destroy(handCardSprite);
//            }
//
//            GameObject playCardSprite = CreateCardSprite(PLAYCARD_NAME, card.GetName(), this.Panel.Get<GameObject>("PlayCards").transform);
//
//            cardsSprite.Add(card.GetName(), playCardSprite);
//            playCards.Add(card);
//        }

        /// <summary>
        /// 创建卡牌精灵
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="cardName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
//        private GameObject CreateCardSprite(string prefabName, string cardName, Transform parent)
//        {
////            GameObject cardSpritePrefab = Game.Scene.GetComponent<ResourcesComponent>().GetAsset($"{prefabName}.unity3d", prefabName);
////            GameObject cardSprite = UnityEngine.Object.Instantiate(cardSpritePrefab);
////
////            cardSprite.name = cardName;
////            cardSprite.layer = LayerMask.NameToLayer("UI");
////            cardSprite.transform.SetParent(parent.transform, false);
////
////            Sprite sprite = CardHelper.GetCardSprite(cardName);
////            cardSprite.GetComponent<Image>().sprite = sprite;
////
////            return cardSprite;
//        }
        public void AddCards(List<MahjongInfo> mahjongs)
        {
            for (int i = 0; i < mahjongs.Count; i++)
            {
                if (i > 13)
                {
                    AddCard(mahjongs[i], cardRight.transform);
                }
                else
                {
                    AddCard(mahjongs[i],cardLeft.transform);
                }
            }
//            CreateCardSprite(mahjongs.);
        }

        private void AddCard(MahjongInfo mahjong, Transform parent)
        {
            GameObject cardSprite = this.CreateCardSprite("card_" + mahjong.weight, parent);
        }

        private GameObject CreateCardSprite(string cardName,Transform parent)
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"Image_Game.unity3d");
            resourcesComponent.LoadBundle($"ItemCard.unity3d");

            GameObject obj = (GameObject)resourcesComponent.GetAsset("Image_Game.unity3d", "Image_Game");
            GameObject itemObj = (GameObject)resourcesComponent.GetAsset("ItemCard.unity3d", "ItemCard");

            Sprite sprite = obj.Get<Sprite>(cardName);
            GameObject ItemCard = GameObject.Instantiate(itemObj,parent);
            ItemCard.GetComponent<Image>().sprite = sprite;
            ItemCard.name = cardName;
            ItemCard.layer = LayerMask.NameToLayer("UI");

            return ItemCard;
        }
    }
}
