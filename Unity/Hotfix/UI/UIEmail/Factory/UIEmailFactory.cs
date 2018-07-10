using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIEmail)]
    public class UIEmailFactory : IUIFactory
    {
        public UI Create(Scene scene, string type, GameObject gameobject)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject email = UnityEngine.Object.Instantiate(bundleGameObject);
                email.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(email);
                ui.AddComponent<UIEmailComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public void Remove(string type)
        {
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }
    }
}
