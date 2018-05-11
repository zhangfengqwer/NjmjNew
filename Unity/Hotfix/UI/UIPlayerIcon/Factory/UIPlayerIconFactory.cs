using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIIcon)]
    class UIPlayerIconFactory : IUIFactory
    {
        public UI Create(Scene scene, string type, GameObject gameobject)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject uiplayicon = UnityEngine.Object.Instantiate(bundleGameObject);
                uiplayicon.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(uiplayicon);
                ui.AddComponent<UIPlayerIconComponent>();
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
