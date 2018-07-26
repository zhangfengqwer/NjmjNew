using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{

    [ObjectSystem]
    public class ItemCardComponentSystem : AwakeSystem<ItemCardComponent>
    {
        public override void Awake(ItemCardComponent self)
        {
            self.Awake();
        }
    }

    public class ItemCardComponent : Component
    {
        private GameObject ItemCard;
        private bool IsSelect;
        private HandCardsComponent handCardsComponent;

        public void Awake()
        {
            this.ItemCard = this.GetParent<UI>().GameObject;
            Button button = this.ItemCard.GetComponent<Button>();
            UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
            this.handCardsComponent = gamerComponent.LocalGamer.GetComponent<HandCardsComponent>();
            button.onClick.RemoveAllListeners();
            button.onClick.Add(() => { OnClick(); });

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);
        }

        public void OnClick()
        {
            float move = 40.0f;
            if (IsSelect)
            {
                move = -move;
            }
            else
            {
                if(handCardsComponent.clickedCard != null)
                {
                    Log.Info(handCardsComponent.clickedCard.name);

                    handCardsComponent.clickedCard.GetComponent<Button>().onClick.Invoke();
                }
                handCardsComponent.clickedCard = ItemCard;
            }
    
            RectTransform rectTransform = ItemCard.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += Vector2.up * move;
            IsSelect = !IsSelect;
        }
    }
}
