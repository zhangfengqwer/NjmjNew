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
        private bool IsSelect;

        public void Awake(GameObject obj)
        {
            this.ItemCard = this.GetParent<UI>().GameObject;
            Button button = this.ItemCard.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.Add(() => { OnClick(obj); });
        }

        public void OnClick(GameObject obj)
        {
            float move = 40.0f;
            if (IsSelect)
            {
                move = -move;
            }
            else
            {
                if(obj != null)
                {
                    Log.Info(obj.name);

                    obj.GetComponent<Button>().onClick.Invoke();
                }
                obj = ItemCard;
            }
    
            RectTransform rectTransform = ItemCard.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += Vector2.up * move;
            IsSelect = !IsSelect;
        }
    }
}
