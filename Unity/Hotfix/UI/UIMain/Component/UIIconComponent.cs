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
        private Sprite icon3;
        private Sprite icon4;
        private Sprite icon5;
        private Sprite icon6;
        private Dictionary<string, Sprite> iconDic = new Dictionary<string, Sprite>();

        public void Awake()
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"{UIType.UIIcon}.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{UIType.UIIcon}.unity3d", $"{UIType.UIIcon}");
            Texture2D texture1 = bundleGameObject.Get<Texture2D>("Icon1");
            icon1 = CreateSprite(texture1);
            Texture2D texture2 = bundleGameObject.Get<Texture2D>("Icon2");
            icon2 = CreateSprite(texture2);
            Texture2D texture3 = bundleGameObject.Get<Texture2D>("1001");
            icon3 = CreateSprite(texture3);
            Texture2D texture4 = bundleGameObject.Get<Texture2D>("1002");
            icon4 = CreateSprite(texture4);
            Texture2D texture5 = bundleGameObject.Get<Texture2D>("1003");
            icon5 = CreateSprite(texture5);
            Texture2D texture6 = bundleGameObject.Get<Texture2D>("1004");
            icon6 = CreateSprite(texture6);
            AddSprite("Icon1",icon1);
            AddSprite("Icon2", icon2);
            AddSprite("1001", icon3);
            AddSprite("1002", icon4);
            AddSprite("1003", icon5);
            AddSprite("1004", icon6);
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
