using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class HandCardsComponentAwakeSystem: AwakeSystem<HandCardsComponent, GameObject, int>
    {
        public override void Awake(HandCardsComponent self, GameObject panel, int index)
        {
            self.Awake(panel, index);
        }
    }

    public class HandCardsComponent: Component
    {
        public const string HANDCARD_NAME = "HandCard";
        public const string PLAYCARD_NAME = "PlayCard";

        public string PrefabName = "Image_Bottom_Card";
        public string ItemName = "Item_Bottom_Card";
        public string Direction = "bottom";
        public int postionX = 0;
        public int postionY = 0;

        //抓牌的位置
        public int grabPostionX = 0;
        public int grabPostionY = 0;

        private List<MahjongInfo> handCards = new List<MahjongInfo>();
        private List<GameObject> ItemCards = new List<GameObject>();

        private GameObject CardBottom;

        private int width = 66;
        private ResourcesComponent resourcesComponent;
        private GameObject prefabObj;
        private GameObject itemObj;
        private GameObject grabObj;
        private GameObject showCard;
        private GameObject cardDisplay;

        public int Index { get; set; }
        public GameObject Panel { get; private set; }

        public void Awake(GameObject panel, int index)
        {
            this.Panel = panel;
            this.Index = index;
            this.CardBottom = panel.Get<GameObject>("CardBottom");
            this.showCard = panel.Get<GameObject>("ShowCard");
            this.cardDisplay = panel.Get<GameObject>("CardDisplay");

            switch (index)
            {
                case 0:
                    PrefabName = "Image_Bottom_Card";
                    ItemName = "Item_Bottom_Card";
                    postionX = 78;
                    postionY = 0;
                    Direction = "bottom";
                    break;
                case 1:
                    PrefabName = "Image_Right_Card";
                    ItemName = "Item_Right_Card";
                    postionX = 0;
                    postionY = -34;

                    grabPostionX = 0;
                    grabPostionY = 51;
                    Direction = "right";
                    break;
                case 2:
                    PrefabName = "Image_Top_Card";
                    ItemName = "Item_Top_Card";
                    postionX = 47;
                    postionY = 0;
                    grabPostionX = -70;
                    grabPostionY = 0;
                    Direction = "card";
                    break;
                case 3:
                    PrefabName = "Image_Left_Card";
                    ItemName = "Item_Left_Card";
                    postionX = 0;
                    postionY = -34;
                    grabPostionX = 0;
                    grabPostionY = -457;
                    Direction = "left";

                    break;
            }

            this.resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            this.resourcesComponent.LoadBundle($"{PrefabName}.unity3d");
            this.resourcesComponent.LoadBundle($"{ItemName}.unity3d");
            this.resourcesComponent.LoadBundle($"Item_Horizontal_Card.unity3d");
            this.resourcesComponent.LoadBundle($"Item_Vertical_Card.unity3d");

            this.prefabObj = (GameObject) this.resourcesComponent.GetAsset($"{this.PrefabName}.unity3d", this.PrefabName);
            this.itemObj = (GameObject) this.resourcesComponent.GetAsset($"{this.ItemName}.unity3d", this.ItemName);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
            resourcesComponent.UnloadBundle($"{PrefabName}.unity3d");
            resourcesComponent.UnloadBundle($"{ItemName}.unity3d");
            prefabObj = null;
            itemObj = null;
            grabObj = null;
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

            //显示出牌
            GameObject obj = (GameObject)this.resourcesComponent.GetAsset("Item_Vertical_Card.unity3d", "Item_Vertical_Card");
            GameObject obj2 = (GameObject)this.resourcesComponent.GetAsset("Image_Top_Card.unity3d", "Image_Top_Card");
            GameObject instantiate = GameObject.Instantiate(obj, this.cardDisplay.transform);

            instantiate.GetComponent<Image>().sprite = obj2.Get<Sprite>("card_" + mahjong.weight);
            instantiate.layer = LayerMask.NameToLayer("UI");

            ShowCard(mahjong.weight);
        }

        public async void ShowCard(byte mahjongWeight)
        {
            this.resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            GameObject obj = (GameObject)this.resourcesComponent.GetAsset("Image_Top_Card.unity3d", "Image_Top_Card");
            showCard.GetComponent<Image>().sprite = obj.Get<Sprite>("card_" + mahjongWeight);
            showCard.SetActive(true);

            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(3000);

            showCard.SetActive(false);

        }

        /// <summary>
        /// 抓牌
        /// </summary>
        /// <param name="mahjong"></param>
        public void GrabCard(MahjongInfo mahjong)
        {
            GameObject cardSprite = this.CreateCardSprite($"{Direction}_" + mahjong.weight, 0, 0);
            SetPosition(cardSprite, 1041, 0);

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
                SetPosition(itemCard, (i) * postionX, i * postionY);
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
                    AddCard(mahjongs[i], 1041, 0, i);
                }
                else
                {
                    AddCard(mahjongs[i], postionX * i, postionY * i, i);
                }
            }

            handCards = mahjongs;
        }

        private void AddCard(MahjongInfo mahjong, int postionX, int postionY, int index)
        {
            GameObject cardSprite = this.CreateCardSprite($"{Direction}_" + mahjong.weight, postionX, postionY);

            ItemCards.Add(cardSprite);

            //设置item
            cardSprite.GetComponent<ItemCardScipt>().weight = mahjong.weight;
            cardSprite.GetComponent<ItemCardScipt>().index = index;
        }

        private GameObject CreateCardSprite(string cardName, int postionX, int postionY)
        {
            Sprite sprite = prefabObj.Get<Sprite>(cardName);
            GameObject ItemCard = GameObject.Instantiate(itemObj, CardBottom.transform);
            ItemCard.GetComponent<Image>().sprite = sprite;
            ItemCard.name = cardName;
            ItemCard.layer = LayerMask.NameToLayer("UI");

            SetPosition(ItemCard, postionX, postionY);

            return ItemCard;
        }

        private void SetPosition(GameObject obj, int postionX, int postionY)
        {
            obj.transform.localPosition = new Vector3(postionX, postionY, obj.transform.localPosition.z);
        }

        /// <summary>
        /// 其他人的牌
        /// </summary>
        /// <param name="messageMahjongs"></param>
        public void AddOtherCards(bool isBanker)
        {
            this.grabObj = this.CreateCardSprite($"{this.Direction}_back1", this.grabPostionX, this.grabPostionY);

            if(!isBanker) grabObj.SetActive(false);

            for (int i = 0; i < 13; i++)
            {
                GameObject cardSprite = this.CreateCardSprite($"{Direction}_back1", i * this.postionX, i * this.postionY);
            }

            if (Direction.Equals("left"))
            {
                grabObj.transform.SetAsLastSibling();
            }
        }

        //其他人抓牌
        public void GrabOtherCard()
        {
            grabObj.SetActive(true);
        }

        public void PlayOtherCard(MahjongInfo mahjongInfo)
        {
            grabObj.SetActive(false);

            //显示出牌
            //显示出牌
            string item1 = null;
            string item2 = null;
            string item3 = null;
            if (Index == 1)
            {
                item1 = "Item_Horizontal_Card";
                item2 = "Image_Right_Card";
                item3 = "right_" + mahjongInfo.weight;
            }
            else if (Index == 2)
            {
                item1 = "Item_Vertical_Card";
                item2 = "Image_Top_Card";
                item3 = "card_" + mahjongInfo.weight;
            }
            else if (Index == 3)
            {
                item1 = "Item_Horizontal_Card";
                item2 = "Image_Left_Card";
                item3 = "left_" + mahjongInfo.weight;
            }
            GameObject obj = (GameObject)this.resourcesComponent.GetAsset($"{item1}.unity3d", item1);
            GameObject obj2 = (GameObject)this.resourcesComponent.GetAsset($"{item2}.unity3d", item2);
            GameObject instantiate = GameObject.Instantiate(obj, this.cardDisplay.transform);

            instantiate.GetComponent<Image>().sprite = obj2.Get<Sprite>(item3);
            instantiate.layer = LayerMask.NameToLayer("UI");

            ShowCard(mahjongInfo.weight);
        }
    }
}