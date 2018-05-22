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
            GameObject playerIconBundle = CommonUtil.getGameObjByBundle(UIType.UIPlayerIcon);
            GameObject shopBundle = CommonUtil.getGameObjByBundle(UIType.UIShopIcon);
            GameObject taskBundle = CommonUtil.getGameObjByBundle(UIType.UITaskIcon);
            GameObject rankBundle = CommonUtil.getGameObjByBundle(UIType.UIRankIcon);
            AddSprite(playerIconBundle, "f_icon1");
            AddSprite(playerIconBundle, "f_icon2");
            AddSprite(playerIconBundle, "f_icon3");
            AddSprite(playerIconBundle, "f_icon4");
            AddSprite(playerIconBundle, "f_icon5");
            AddSprite(playerIconBundle, "m_icon1");
            AddSprite(playerIconBundle, "m_icon2");
            AddSprite(playerIconBundle, "m_icon3");
            AddSprite(playerIconBundle, "m_icon4");
            AddSprite(playerIconBundle, "m_icon5");
            AddSprite(shopBundle, "2");
            AddSprite(shopBundle, "item2_2");
            AddSprite(shopBundle, "item3_2");
            AddSprite(shopBundle, "item4_2");
            AddSprite(shopBundle, "1");
            AddSprite(shopBundle, "item2_1");
            AddSprite(shopBundle, "item3_1");
            AddSprite(shopBundle, "item4_1");
            AddSprite(shopBundle, "101");
            AddSprite(shopBundle, "102");
            AddSprite(shopBundle, "103");
            AddSprite(shopBundle, "104");
            AddSprite(shopBundle, "105");
            AddSprite(shopBundle, "106");
            AddSprite(shopBundle, "107");
            AddSprite(shopBundle, "108");
            AddSprite(shopBundle, "109");
            AddSprite(taskBundle, "Task_101");
            AddSprite(taskBundle, "Task_102");
            AddSprite(taskBundle, "Task_103");
            AddSprite(taskBundle, "Task_104");
            AddSprite(taskBundle, "Task_105");
            AddSprite(taskBundle, "Task_106");
            AddSprite(rankBundle, "Rank_0");
            AddSprite(rankBundle, "Rank_1");
            AddSprite(rankBundle, "Rank_2");
        }

        private void AddSprite(GameObject bundle,string iconName)
        {
            icon = bundle.Get<Sprite>(iconName);
            //texture = bundle.Get<Texture2D>(iconName);
            //icon = CreateSprite(texture);
            AddSprite(iconName, icon);
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
