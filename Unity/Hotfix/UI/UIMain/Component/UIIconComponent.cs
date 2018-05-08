using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIIconComponentSystem : AwakeSystem<UIIconComponent>
    {
        public override void Awake(UIIconComponent self)
        {
            self.Awake();
        }
    }

    public class UIIconComponent :Component
    {
        private Image icon1;
        private Image icon2;
        private Dictionary<string, Sprite> iconDic = new Dictionary<string, Sprite>();

        public void Awake()
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"{UIType.UIIcon}.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{UIType.UIIcon}.unity3d", $"{UIType.UIIcon}");

            ReferenceCollector rc = bundleGameObject.GetComponent<ReferenceCollector>();
            icon1 = rc.Get<GameObject>("Icon1").GetComponent<Image>();
            icon2 = rc.Get<GameObject>("Icon2").GetComponent<Image>();
            AddSprite("Icon1",icon1.sprite);
            AddSprite("Icon2", icon2.sprite);
        }

        private void AddSprite(string key,Sprite value)
        {
            if (!iconDic.ContainsKey(key))
                iconDic.Add(key, value);
        }

        public Sprite GetSprite(string key)
        {
            if (iconDic.ContainsKey(key))
                return iconDic[key];
            return null;
        }
    }
}
