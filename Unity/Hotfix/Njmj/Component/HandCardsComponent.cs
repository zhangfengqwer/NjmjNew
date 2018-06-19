using System;
using System.Collections.Generic;
using System.Threading;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class HandCardsComponentAwakeSystem: AwakeSystem<HandCardsComponent, GameObject, int, int>
    {
        public override void Awake(HandCardsComponent self, GameObject panel, int index, int seatIndex)
        {
            self.Awake(panel, index, seatIndex);
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
        public List<GameObject> cardDisplayObjs = new List<GameObject>();
        private List<MahjongInfo> faceCards = new List<MahjongInfo>();

        //碰对应的obj
        private Dictionary<int, GameObject> pengDic = new Dictionary<int, GameObject>();
        public GameObject CardBottom;

        private int width = 66;
        private ResourcesComponent resourcesComponent;
        private GameObject prefabObj;
        private GameObject itemObj;
        private GameObject grabObj;
        private GameObject showCard;
        private GameObject cardDisplay;
        private GameObject directionObj;
        private GameObject bg;
        private GameObject pengObj;
        private Image faceImage;
        private Image faceImageGe;

        public GameObject currentPlayCardObj;
        private Vector3 cardBottonPosition;
        private GameObject changeMoney;
        //当前抓牌的索引
        private int grabIndex;
        private int num = 0;
        private GameObject faceCard;


        public int Index { get; set; }
        public GameObject Panel { get; private set; }

        public void Awake(GameObject panel, int index, int seatIndex)
        {
            this.Panel = panel;
            this.Index = index;
            this.CardBottom = panel.Get<GameObject>("CardBottom");
            this.cardBottonPosition = this.CardBottom.transform.localPosition;
            this.showCard = panel.Get<GameObject>("ShowCard");
            this.cardDisplay = panel.Get<GameObject>("CardDisplay");
            this.directionObj = panel.Get<GameObject>("Direction");
            this.pengObj = panel.Get<GameObject>("Peng");
            this.bg = panel.Get<GameObject>("Bg");
            this.changeMoney = panel.Get<GameObject>("ChangeMoney");
            this.faceImage = panel.Get<GameObject>("FaceImage").GetComponent<Image>();
            this.faceImageGe = panel.Get<GameObject>("FaceImageGe").GetComponent<Image>();
            Image image = this.directionObj.GetComponent<Image>();

            //花牌显示
            this.faceCard = panel.Get<GameObject>("FaceCard");

            faceCard.GetComponent<Button>().onClick.Add(() =>
            {
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                uiRoomComponent.faceCardObj.SetActive(true);
            });
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

            //设置东南西北
            switch (seatIndex)
            {
                case 0:
                    image.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "desk_dong");
                    break;
                case 1:
                    image.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "desk_nan");
                    break;
                case 2:
                    image.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "desk_xi");
                    break;
                case 3:
                    image.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "desk_bei");
                    break;
            }

            //Load AB
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
            Reset();
            foreach (var obj in ItemCards)
            {
                GameObject.Destroy(obj.gameObject);
            }

            ItemCards.Clear();
            faceCards.Clear();
            cardDisplayObjs.Clear();

            DeleteAllItem(CardBottom);
            DeleteAllItem(cardDisplay);
            DeleteAllItem(pengObj);
            pengDic.Clear();
            prefabObj = null;
            itemObj = null;
            grabObj = null;
            faceImage = null;
            faceImageGe = null;
            tokenSource.Cancel();
            handCards.Clear();
           
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
        /// 得到牌
        /// </summary>
        public List<MahjongInfo> GetAllCards()
        {
            return handCards;
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
        public void PlayCard(MahjongInfo mahjong, int index, GameObject currentItem)
        {
            try
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
                this.currentPlayCardObj = GameObject.Instantiate(obj, this.cardDisplay.transform);

                currentPlayCardObj.GetComponent<Image>().sprite = obj2.Get<Sprite>("card_" + mahjong.weight);
                currentPlayCardObj.layer = LayerMask.NameToLayer("UI");

                currentItem = currentPlayCardObj;

                ShowCard(mahjong.weight);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
          
        }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        public async void ShowCard(byte mahjongWeight)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }

            tokenSource = new CancellationTokenSource();
            this.resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            GameObject obj = (GameObject) this.resourcesComponent.GetAsset("Image_Top_Card.unity3d", "Image_Top_Card");
            showCard.GetComponent<Image>().sprite = obj.Get<Sprite>("card_" + mahjongWeight);

            showCard.SetActive(true);
            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(3000, tokenSource.Token);
            showCard.SetActive(false);
        }

        /// <summary>
        /// 抓牌
        /// </summary>
        /// <param name="mahjong"></param>
        public void GrabCard(MahjongInfo mahjong)
        {
            GameObject cardSprite = this.CreateCardSprite($"{Direction}_" + mahjong.weight, 0, 0);
            SetPosition(cardSprite, (handCards.Count) * postionX + 30, 0);

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

            grabIndex = index;
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
                    AddCard(mahjongs[i], (mahjongs.Count - 1) * postionX + 30, 0, i);
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
        /// 获取精灵
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Sprite GetSprite(string prefabName, string name)
        {
            GameObject asset = (GameObject) this.resourcesComponent.GetAsset($"{prefabName}.unity3d", prefabName);
            return asset.Get<Sprite>(name);
        }

        /// <summary>
        /// 其他人的牌
        /// </summary>
        /// <param name="messageMahjongs"></param>
        public void AddOtherCards(bool isBanker)
        {
            this.grabObj = this.CreateCardSprite($"{this.Direction}_back1", this.grabPostionX, this.grabPostionY);

            if (!isBanker)
            {
                //grabObj.SetActive(false);
                grabObj.transform.localScale = Vector3.zero;
            }

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
            grabObj.transform.localScale = Vector3.one;
        }

        public void PlayOtherCard(MahjongInfo mahjongInfo, GameObject currentItem)
        {
            try
            {
                //grabObj.SetActive(false);
                grabObj.transform.localScale = Vector3.zero;
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
                this.currentPlayCardObj = GameObject.Instantiate(obj, this.cardDisplay.transform);
                this.currentPlayCardObj.GetComponent<Image>().sprite = obj2.Get<Sprite>(item3);
                this.currentPlayCardObj.layer = LayerMask.NameToLayer("UI");
                currentPlayCardObj.name = item3;
                cardDisplayObjs.Add(this.currentPlayCardObj);
                if (Index == 1)
                {
                    this.currentPlayCardObj.transform.SetAsFirstSibling();
                    int count = this.cardDisplayObjs.Count;
//                    Log.Info("cardDisplayObjs:" + count);
                    int x = -107;
                    int y = -192;
                    int i = count / 10;
                    int i1 = count % 10;

                    if (i1 == 0)
                    {
                        i1 = 10;
                        i--;
                    }

                    this.currentPlayCardObj.transform.localPosition = new Vector3(-107 + (i * 53), 34 * (i1 - 1) - 192, 0);
                }

                currentItem = currentPlayCardObj;
                //            Log.Info("别人出的牌：" + currentItem.name);

                ShowCard(mahjongInfo.weight);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void ShowBg()
        {
            bg.SetActive(true);
        }

        public void CloseBg()
        {
            bg.SetActive(false);
        }

        public void SetPeng(int type, MahjongInfo mahjong)
        {
            GameObject obj = null;
            if (type == 0)
            {
                obj = CommonUtil.getGameObjByBundle("Item_Peng_Card");
                //更新手牌
                for (int i = 0; i < 2; i++)
                {
                    int index = Logic_NJMJ.getInstance().GetIndex(this.handCards, mahjong);
                    this.RemoveCard(index);
                }
            }
            //明杠
            else if (type == 1)
            {
                obj = CommonUtil.getGameObjByBundle("Item_Gang_Card");
                //更新手牌
                for (int i = 0; i < 3; i++)
                {
                    int index = Logic_NJMJ.getInstance().GetIndex(this.handCards, mahjong);
                    this.RemoveCard(index);
                }
            }
            //暗杆
            else if (type == 4)
            {
                obj = CommonUtil.getGameObjByBundle("Item_Gang_Card");
                for (int i = 0; i < 4; i++)
                {
                    int index = Logic_NJMJ.getInstance().GetIndex(this.handCards, mahjong);
                    this.RemoveCard(index);
                }
            }
            //碰刚
            else if (type == 5)
            {
                obj = CommonUtil.getGameObjByBundle("Item_Gang_Card");
                for (int i = 0; i < 1; i++)
                {
                    int index = Logic_NJMJ.getInstance().GetIndex(this.handCards, mahjong);
                    this.RemoveCard(index);
                }
            }

            UpdateCards();
            //显示碰
            GameObject gameObject = GameObject.Instantiate(obj, this.pengObj.transform);
            for (int i = 0; i < 2; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<Image>().sprite =
                        CommonUtil.getSpriteByBundle("Image_Top_Card", "card_" + mahjong.weight);
            }

            if (type == 0)
            {
                pengDic.Add(mahjong.weight, gameObject);
            }

            Vector3 localPosition = this.CardBottom.transform.localPosition;
            this.CardBottom.transform.localPosition =
                    new Vector3(localPosition.x + (postionX) * 2f, localPosition.y + (postionY) * 2f, localPosition.z);
        }

        /// <summary>
        /// 碰刚
        /// </summary>
        /// <param name="messageOperationType"></param>
        /// <param name="mahjong"></param>
        public void SetPengGang(int messageOperationType, MahjongInfo mahjong)
        {
            Log.Debug("显示碰后杠");
            GameObject obj = CommonUtil.getGameObjByBundle("Item_Gang_Card");
            int index = Logic_NJMJ.getInstance().GetIndex(this.handCards, mahjong);
            this.RemoveCard(index);
            UpdateCards();

            //显示碰
            GameObject gameObject = GameObject.Instantiate(obj, this.pengObj.transform);
            for (int i = 0; i < 2; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<Image>().sprite =
                        CommonUtil.getSpriteByBundle("Image_Top_Card", "card_" + mahjong.weight);
            }

            GameObject lastPengObj = null;
            if (pengDic.TryGetValue(mahjong.weight, out lastPengObj))
            {
                gameObject.transform.SetSiblingIndex(lastPengObj.transform.GetSiblingIndex());
                gameObject.transform.localPosition = lastPengObj.transform.localPosition;
                GameObject.Destroy(lastPengObj);
                pengDic.Remove(mahjong.weight);
            }
        }

        public void SetOtherPengGang(int messageOperationType, MahjongInfo mahjong)
        {
            GameObject obj = null;
            if (Index == 2)
            {
                obj = CommonUtil.getGameObjByBundle("Item_Gang_Card");
            }
            else
            {
                obj = CommonUtil.getGameObjByBundle("Item_Right_Gang_Card");
            }


            //显示碰
            GameObject gameObject = GameObject.Instantiate(obj, this.pengObj.transform);
            GameObject lastPengObj = null;
            if (pengDic.TryGetValue(mahjong.weight, out lastPengObj))
            {
                gameObject.transform.SetSiblingIndex(lastPengObj.transform.GetSiblingIndex());
                gameObject.transform.localPosition = lastPengObj.transform.localPosition;
                GameObject.Destroy(lastPengObj);
                pengDic.Remove(mahjong.weight);
            }

            //显示出牌
            string item1 = null;
            string item2 = null;
            string item3 = null;
            if (Index == 1)
            {
                item1 = "Item_Horizontal_Card";
                item2 = "Image_Right_Card";
                item3 = "right_" + mahjong.weight;
            }
            else if (Index == 2)
            {
                item1 = "Item_Vertical_Card";
                item2 = "Image_Top_Card";
                item3 = "card_" + mahjong.weight;
            }
            else if (Index == 3)
            {
                item1 = "Item_Horizontal_Card";
                item2 = "Image_Left_Card";
                item3 = "left_" + mahjong.weight;
            }

            for (int i = 1; i < 3; i++)
            {
                gameObject.transform.Find("Item_" + i).GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle(item2, item3);
            }

            if (Index == 3)
            {
//                for (int i = 0; i < 1; i++)
//                {
//                    GameObject.Destroy(CardBottom.transform.GetChild(i).gameObject);
//                }
            }
            else
            {
//                for (int i = 0; i < 1; i++)
//                {
//                    int childCount = this.CardBottom.transform.childCount;
//                    GameObject.Destroy(CardBottom.transform.GetChild(childCount - 1 - i).gameObject);
//                }
//
//                Vector3 localPosition = this.CardBottom.transform.localPosition;
//                float x = localPosition.x - postionX * 1f;
//                float y = localPosition.y - postionY * 1f;
//
//                this.CardBottom.transform.localPosition = new Vector3(x, y, localPosition.z);
            }
        }

     
        /// <summary>
        /// 其他人碰刚
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mahjongInfo"></param>
        public void SetOtherPeng(int type, MahjongInfo mahjongInfo)
        {
          
            GameObject obj = null;
            int temp = 0;
            if (type == 0)
            {
                if (Index == 2)
                {
                    obj = CommonUtil.getGameObjByBundle("Item_Peng_Card");
                }
                else
                {
                    obj = CommonUtil.getGameObjByBundle("Item_Right_Peng_Card");
                }

                temp = 3;
            }
            else if (type == 1)
            {
                if (Index == 2)
                {
                    obj = CommonUtil.getGameObjByBundle("Item_Gang_Card");
                }
                else
                {
                    obj = CommonUtil.getGameObjByBundle("Item_Right_Gang_Card");
                }

                temp = 3;
            }
            //暗杆
            //            else if (type == 4)
            else
            {
                if (Index == 2)
                {
                    obj = CommonUtil.getGameObjByBundle("Item_Gang_Card");
                }
                else
                {
                    obj = CommonUtil.getGameObjByBundle("Item_Right_Gang_Card");
                }

                temp = 4;
            }

            //设置碰杠位置
            GameObject gameObject = GameObject.Instantiate(obj, this.pengObj.transform);
            int count = this.pengObj.transform.childCount;
            if (Index == 1)
            {
                gameObject.transform.localPosition = new Vector3(0, (count - 1) * 150, 0);
            }
            else if (Index == 2)
            {
                gameObject.transform.localPosition = new Vector3((count - 1) * (-160.93f), 0, 0);
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(count * (postionX) * 3.5f, count * (postionY) * 3.5f, 0);
            }

            if (type == 0)
            {
                pengDic.Add(mahjongInfo.weight, gameObject);
            }

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

            for (int i = 1; i < 3; i++)
            {
                gameObject.transform.Find("Item_" + i).GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle(item2, item3);
            }

            num++;

            if (Index == 3)
            {
                for (int i = 0; i < temp; i++)
                {
                    GameObject.Destroy(CardBottom.transform.GetChild(i).gameObject);
                }
            }
            else
            {
              
                for (int i = 0; i < temp; i++)
                {
                    int childCount = this.CardBottom.transform.childCount;
                    Log.Debug("销毁childCount：" + (childCount));
                    GameObject.DestroyImmediate(CardBottom.transform.GetChild(childCount - 1).gameObject);
                }

                Vector3 localPosition = this.CardBottom.transform.localPosition;
                float x = localPosition.x - postionX * 0.2f;
                float y = localPosition.y - postionY * 0.2f;

                this.CardBottom.transform.localPosition = new Vector3(x, y, localPosition.z);
            }
        }

        public void DeleteAllItem(GameObject gameObject)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 去除手牌
        /// </summary>
        /// <param name="index"></param>
        private void RemoveCard(int index)
        {
            GameObject gameObject = this.GetSprite(index);
            GameObject.Destroy(gameObject);
            this.handCards.RemoveAt(index);
            this.ItemCards.RemoveAt(index);
        }

        /// <summary>
        /// 设置花牌
        /// </summary>
        /// <param name="gameDataFaceCards"></param>
        public void SetFaceCards(List<MahjongInfo> gameDataFaceCards)
        {
            faceCards = gameDataFaceCards;

            int geWei = gameDataFaceCards.Count % 10;
            int shiWei = gameDataFaceCards.Count / 10;
            if (shiWei > 0)
            {
                faceImageGe.gameObject.SetActive(true);
                faceImage.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "time_" + shiWei);
                faceImageGe.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "time_" + geWei);
            }
            else
            {
                faceImageGe.gameObject.SetActive(false);
                faceImage.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "time_" + gameDataFaceCards.Count);
            }
        }

        /// <summary>
        /// 收到补花
        /// </summary>
        /// <param name="mahjongInfo"></param>
        /// <param name="b"></param>
        public async void BuHua(MahjongInfo mahjongInfo, bool isSelf)
        {
            //补花数量
            try
            {
                faceCards.Add(mahjongInfo);
                SetFaceCards(faceCards);
                //            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(100);

                //删除花牌
                if (isSelf)
                {
//                    GameObject gameObject = this.GetSprite(grabIndex);
//                    GameObject.Destroy(gameObject);
//                    handCards.RemoveAt(grabIndex);
//                    ItemCards.RemoveAt(grabIndex);
//                    UpdateCards();
                }
                else
                {
//                    grabObj.SetActive(false);
                    grabObj.transform.localScale = Vector3.zero;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void ClearAll()
        {
            handCards.Clear();
            foreach (var obj in ItemCards)
            {
                GameObject.Destroy(obj.gameObject);
            }

            ItemCards.Clear();
            faceCards.Clear();
            cardDisplayObjs.Clear();

            DeleteAllItem(CardBottom);
            DeleteAllItem(cardDisplay);
            DeleteAllItem(pengObj);
            pengDic.Clear();
        }

        public void Reset()
        {
            Log.Debug("重设bottom");
            CardBottom.transform.localPosition = cardBottonPosition;
        }

        public void ChangeGold(int amount)
        {
            Log.Debug("改变财富");
            GameHelp.GoldChange(changeMoney.gameObject, amount);
        }
    }
}