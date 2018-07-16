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
    public class ItemCardComponentSystem : AwakeSystem<ItemCardComponent,GameObject>
    {
        public override void Awake(ItemCardComponent self, GameObject obj)
        {
            self.Awake(obj);
        }
    }

    public class ItemCardComponent : Component
    {
        private GameObject ItemCard;
        private GameObject ClickedCard;
        private bool IsSelect;

        public void Awake(GameObject obj)
        {
            this.ItemCard = this.GetParent<UI>().GameObject;
            ClickedCard = obj;
            Button button = this.ItemCard.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.Add(OnClick);
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
                if(ClickedCard != null)
                {
                    Log.Info(ClickedCard.name);

                    ClickedCard.GetComponent<Button>().onClick.Invoke();
                }
                ClickedCard = ItemCard;
            }
    
            RectTransform rectTransform = ItemCard.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += Vector2.up * move;
            IsSelect = !IsSelect;
        }
    }
}
