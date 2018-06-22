using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class FrameAnimation
    {
        public delegate void CallBack();

        public static async void Start(Image image,string bundleName ,string animationName,long durtime,CallBack callBack)
        {
            int i = 0;
            while (true)
            {
                Sprite sprite = CommonUtil.getSpriteByBundle(bundleName, animationName + (++i));
                if (sprite != null)
                {
                    image.sprite = sprite;
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(durtime);
                }
                else
                {
                    break;
                }
            }

            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
