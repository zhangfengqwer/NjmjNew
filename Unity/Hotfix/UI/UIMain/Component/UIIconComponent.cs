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
        private Sprite icon;
        private Texture2D texture;
        private Dictionary<string, Sprite> iconDic = new Dictionary<string, Sprite>();

        public void Awake()
        {
            GameObject playerIconBundle = GetBundleByUIType(UIType.UIPlayerIcon);
            GameObject shopBundle = GetBundleByUIType(UIType.UIShopIcon);
            AddSprite(playerIconBundle, "Icon1");
            AddSprite(playerIconBundle, "Icon2");
            AddSprite(shopBundle, "1001");
            AddSprite(shopBundle, "1002");
            AddSprite(shopBundle, "1003");
            AddSprite(shopBundle, "1004");
            AddSprite(shopBundle, "1005");
            AddSprite(shopBundle, "1006");
            AddSprite(shopBundle, "1007");
            AddSprite(shopBundle, "1008");
            AddSprite(shopBundle, "1009");
            AddSprite(shopBundle, "1010");
            AddSprite(shopBundle, "1011");
            AddSprite(shopBundle, "1012");
            AddSprite(shopBundle, "1013");
            AddSprite(shopBundle, "1014");
            AddSprite(shopBundle, "1015");
            AddSprite(shopBundle, "1016");
            AddSprite(shopBundle, "1017");
        }

        private void AddSprite(GameObject bundle,string iconName)
        {
            texture = bundle.Get<Texture2D>(iconName);
            Debug.Log(texture);
            icon = CreateSprite(texture);
            AddSprite(iconName, icon);
        }

        private GameObject GetBundleByUIType(string type)
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"{type}.unity3d");
             return (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
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
