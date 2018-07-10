using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiNetLoadingComponentSystem : AwakeSystem<UINetLoadingComponent>
	{
		public override void Awake(UINetLoadingComponent self)
		{
			self.Awake();
		}
	}
	
	public class UINetLoadingComponent : Component
	{
        bool isDispose = false;

        public void Awake()
		{
            startTimer();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            isDispose = true;
        }

        public static void showNetLoading()
        {
            if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UINetLoading) != null)
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UINetLoading);
            }

            Game.Scene.GetComponent<UIComponent>().Create(UIType.UINetLoading);
        }

        public static void closeNetLoading()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UINetLoading);
        }

        public async void startTimer()
        {
            int time = 10;
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                --time;

                //Log.Debug(time.ToString());

                if (isDispose)
                {
                    isDispose = false;
                    return;
                }
            }

            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UINetLoading);
            isDispose = false;
        }
    }
}
