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
        private Sprite icon1;
        private Sprite icon2;
        private Dictionary<string, Sprite> iconDic = new Dictionary<string, Sprite>();

        public void Awake()
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"{UIType.UIIcon}.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{UIType.UIIcon}.unity3d", $"{UIType.UIIcon}");
            Texture2D texture1 = bundleGameObject.Get<Texture2D>("Icon1");
            icon1 = CreateSprite(texture1);
            Texture2D texture2 = bundleGameObject.Get<Texture2D>("Icon1");
            icon2 = CreateSprite(texture2);
            AddSprite("Icon1",icon1);
            AddSprite("Icon2", icon2);
        }

        private Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
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
