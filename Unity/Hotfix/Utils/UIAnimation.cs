using DG.Tweening;
using ETModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ETHotfix
{
    public class UIAnimation
    {
        public static void ShowLayer(GameObject obj)
        {
            obj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            obj.transform.DOScale(1.0f, 0.2f);

            {
                Sequence seq = DOTween.Sequence();
                seq.Append(obj.transform.DOScale(1.1f, 0.2f))
                   .Append(obj.transform.DOScale(1.0f, 0.1f)).Play();
            }
        }
    }
}