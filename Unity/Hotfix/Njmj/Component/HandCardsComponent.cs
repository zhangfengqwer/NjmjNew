using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class HandCardsComponentAwakeSystem: AwakeSystem<HandCardsComponent, GameObject>
    {
        public override void Awake(HandCardsComponent self, GameObject panel)
        {
            self.Awake(panel);
        }
    }

    public class HandCardsComponent: Component
    {
        public const string HANDCARD_NAME = "HandCard";
        public const string PLAYCARD_NAME = "PlayCard";

        private List<MahjongInfo> handCards = new List<MahjongInfo>();
        private List<GameObject> ItemCards = new List<GameObject>();

        private GameObject cardRight;
        private GameObject cardLeft;

        private int width = 66;

        public GameObject Panel { get; private set; }

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
        /// <returns></returns>
        public GameObject GetSprite(int index)
        {
            return ItemCards[index];
        }

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

        /// <summary>
        /// 卡牌精灵更新
        /// </summary>
        public void CardsSpriteUpdate(List<MahjongInfo> mahjongInfos, float interval)
        {
            if (mahjongInfos.Count == 0)
            {
                return;
            }

            Logic_NJMJ.getInstance().SortMahjong(mahjongInfos);
        }

        /// <summary>
        /// 玩家出牌
        /// </summary>
        /// <param name="mahjong"></param>
        /// <param name="messageIndex"></param>
        /// <param name="messageWeight"></param>
        public void PlayCard(MahjongInfo mahjong, int index)
        {
            MahjongInfo info = handCards[index];
            if (info.weight == mahjong.weight)
            {
                GameObject gameObject = this.GetSprite(index);
                GameObject.Destroy(gameObject);
                handCards.RemoveAt(index);
                ItemCards.RemoveAt(index);
            }

            UpdateCards();
        }

        /// <summary>
        /// 抓牌
        /// </summary>
        /// <param name="mahjong"></param>
        public void GrabCard(MahjongInfo mahjong)
        {

            GameObject cardSprite = this.CreateCardSprite("card_" + mahjong.weight, cardLeft.transform, 0);
            SetPosition(cardSprite, 792 + width * 2);

            //查询插入的index
            int index = -1;
            for (int i = 0; i < handCards.Count - 1; i++)
            {
                if (mahjong.m_weight < handCards[0].m_weight)
                {
                    index = -1;
                    break;
                }

                if (mahjong.m_weight >= handCards[handCards.Count - 1].m_weight)
                {
                    index = handCards.Count - 1;
                    break;
                }

                MahjongInfo mahjongInfo1 = this.handCards[i];
                MahjongInfo mahjongInfo2 = this.handCards[i + 1];
                if (mahjongInfo1.m_weight <= mahjong.m_weight && mahjongInfo2.m_weight > mahjong.m_weight)
                {
                    index = i;
                    break;
                }
            }

            index++;
            handCards.Insert(index, mahjong);
            ItemCards.Insert(index, cardSprite);

            //大于index的item本身的index需要加1
            for (int i = 0; i < ItemCards.Count; i++)
            {
                if (i > index)
                {
                    ItemCards[i].GetComponent<ItemCardScipt>().index = i;
                }
            }

            //设置item
            cardSprite.GetComponent<ItemCardScipt>().weight = mahjong.weight;
            cardSprite.GetComponent<ItemCardScipt>().index = index;
        }

        /// <summary>
        /// 更新ui
        /// </summary>
        private void UpdateCards()
        {
            for (int i = 0; i < handCards.Count; i++)
            {
                GameObject itemCard = this.GetSprite(i);
                SetPosition(itemCard, (i) * width);
                itemCard.GetComponent<ItemCardScipt>().index = i;
            }
        }

        public void AddCards(List<MahjongInfo> mahjongs)
        {
            Logic_NJMJ.getInstance().SortMahjong(mahjongs);
            handCards.Clear();
            ItemCards.Clear();

            for (int i = 0; i < mahjongs.Count; i++)
            {
                if (i > 12)
                {
                    AddCard(mahjongs[i], cardLeft.transform, width * (i + 1), i);
                }
                else
                {
                    AddCard(mahjongs[i], cardLeft.transform, width * i, i);
                }
            }

            handCards = mahjongs;
        }

        private void AddCard(MahjongInfo mahjong, Transform parent, int postionX, int index)
        {
            GameObject cardSprite = this.CreateCardSprite("card_" + mahjong.weight, parent, postionX);

            ItemCards.Add(cardSprite);

            //设置item
            cardSprite.GetComponent<ItemCardScipt>().weight = mahjong.weight;
            cardSprite.GetComponent<ItemCardScipt>().index = index;
        }

        private GameObject CreateCardSprite(string cardName, Transform parent, int postionX)
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"Image_Game.unity3d");
            resourcesComponent.LoadBundle($"ItemCard.unity3d");

            GameObject obj = (GameObject) resourcesComponent.GetAsset("Image_Game.unity3d", "Image_Game");
            GameObject itemObj = (GameObject) resourcesComponent.GetAsset("ItemCard.unity3d", "ItemCard");

            Sprite sprite = obj.Get<Sprite>(cardName);
            GameObject ItemCard = GameObject.Instantiate(itemObj, parent);
            ItemCard.GetComponent<Image>().sprite = sprite;
            ItemCard.name = cardName;
            ItemCard.layer = LayerMask.NameToLayer("UI");

            SetPosition(ItemCard, postionX);

            return ItemCard;
        }

        private void SetPosition(GameObject obj, int postionX)
        {
            obj.transform.localPosition = new Vector3(postionX, obj.transform.localPosition.y, obj.transform.localPosition.z);
        }

        /// <summary>
        /// 其他人的牌
        /// </summary>
        /// <param name="messageMahjongs"></param>
        public void AddOtherCards(List<MahjongInfo> messageMahjongs)
        {
        }
    }
}