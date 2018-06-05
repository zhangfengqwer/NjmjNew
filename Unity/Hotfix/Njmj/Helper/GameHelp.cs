using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class GameHelp
    {
        private static List<int> numList = new List<int>();

        public static void ShowPlusGoldChange(GameObject gameObject, int num)
        {
            numList.Clear();
//            int unitPlace = num / 1 % 10;
//            int tenPlace = num / 10 % 10;
//            int hundredPlace = num / 100 % 10;
//            int thousandPlace = num / 1000 % 10;
//            int wPlace = num / 10000 % 10;

            //第一位是否是0
            bool isStart = true;
            for (int i = 5; i >= 0; i--)
            {
                int pow = (int) Math.Pow(10, i);
                Log.Debug("pw0:" + pow);
                int temp = num / pow % 10;
                if (isStart)
                {
                    if (temp > 0)
                    {
                        numList.Add(temp);
                        isStart = false;
                    }
                }
                else
                {
                    numList.Add(temp);
                }
            }

            Log.Debug("多少位：" + JsonHelper.ToJson(numList));
            GameObject obj = new GameObject();
            Image image = obj.AddComponent<Image>();
            image.sprite = CommonUtil.getSpriteByBundle("Image_Plus_Gold", "plus");
            GameObject.Instantiate(obj, gameObject.transform);

            foreach (var item in numList)
            {
                obj = new GameObject();
                image = obj.AddComponent<Image>();
                image.sprite = CommonUtil.getSpriteByBundle("Image_Plus_Gold", item+"");
                GameObject.Instantiate(obj, gameObject.transform);
            }
        }
    }
}