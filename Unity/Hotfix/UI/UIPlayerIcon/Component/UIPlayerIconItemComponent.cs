using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPlayerIconItemSystem:AwakeSystem<UIPlayerIconItemComponent>
    {
        public override void Awake(UIPlayerIconItemComponent self)
        {
            self.Awake();
        }
    }
    public class UIPlayerIconItemComponent : Component
    {
        private GameObject select;
        private GameObject uiIcon;
        private int index;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            select = rc.Get<GameObject>("Select");
            uiIcon = rc.Get<GameObject>("UIIcon");

            uiIcon.GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIIcon).GetComponent<UIPlayerIconComponent>().SetSelectState(index);
            });
        }
        
        public void SetSelect(bool isActive)
        {
            select.gameObject.SetActive(isActive);
        }

        public Sprite GetIcon()
        {
            return uiIcon.GetComponent<Image>().sprite;
        }

        public void SetIcon(Sprite sprite,int index)
        {
            this.index = index;
            uiIcon.GetComponent<Image>().sprite = sprite;
        }
    }
}
