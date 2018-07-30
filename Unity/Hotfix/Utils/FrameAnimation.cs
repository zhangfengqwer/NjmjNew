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

        public static async void Start(Image image,string bundleName ,string animationName,long durtime,CallBack callBack,bool isLoop = false,bool indexIsGuDingLength = false)
        {
            int i = 0;
            while (true)
            {
                Sprite sprite;

                if (!indexIsGuDingLength)
                {
                    sprite = CommonUtil.getSpriteByBundle(bundleName, animationName + (++i));
                }
                else
                {
                    string name = animationName;;
                    if (++i > 9)
                    {
                        name += i;
                    }
                    else
                    {
                        name += ("0" + i);
                    }

                    sprite = CommonUtil.getSpriteByBundle(bundleName, name);
                }

                if (sprite != null)
                {
                    image.sprite = sprite;
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(durtime);
                }
                else
                {
                    if (isLoop)
                    {
                        i = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
