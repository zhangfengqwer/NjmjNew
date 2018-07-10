﻿using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIChangeName)]
    public class UIChangeNameFactory : IUIFactory
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
	        try
	        {
				ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
				resourcesComponent.LoadBundle($"{type}.unity3d");
				GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
				GameObject login = UnityEngine.Object.Instantiate(bundleGameObject);
				login.layer = LayerMask.NameToLayer(LayerNames.UI);
		        UI ui = ComponentFactory.Create<UI, GameObject>(login);

				ui.AddComponent<UIChangeNameComponent>();
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