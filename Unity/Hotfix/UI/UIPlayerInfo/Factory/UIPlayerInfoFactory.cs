using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIPlayerInfo)]
    public class UIPlayerInfoFactory : IUIFactory
    {
        public UI Create(Scene scene,string type,GameObject gameobject)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject plInfo = UnityEngine.Object.Instantiate(bundleGameObject);
                plInfo.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(plInfo);
                ui.AddComponent<UIPlayerInfoComponent>();
                return ui;
            }
            catch(Exception e)
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
