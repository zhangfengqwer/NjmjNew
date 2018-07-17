using DG.Tweening;
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
    public class FingerAnimation
    {
        static GameObject s_finger = null;

        public static void Show(GameObject parent)
        {
            // 同一时间只能显示一个手指
            if (s_finger != null)
            {
                GameObject.Destroy(s_finger);
                s_finger = null;
            }
            
            // 显示手指
            {
                s_finger = new GameObject();
                Image img = s_finger.AddComponent<Image>();
                img.sprite = CommonUtil.getSpriteByBundle("image_main", "finger");

                s_finger.transform.SetParent(parent.transform);
                s_finger.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                Vector3 pos = new Vector3(30, -30, 0);
                s_finger.transform.localPosition = pos;

                {
                    Sequence seq = DOTween.Sequence();
                    seq.Append(s_finger.GetComponent<RectTransform>().DOAnchorPos(new Vector2(pos.x - 15, pos.y + 15), 0.5f, false))
                       .Append(s_finger.GetComponent<RectTransform>().DOAnchorPos(new Vector2(pos.x, pos.y), 0.5f, false)).SetLoops(-1).Play();
                }
            }
        }

        public static void Hide()
        {
            if (s_finger != null)
            {
                GameObject.Destroy(s_finger);
            }
        }
    }
}
