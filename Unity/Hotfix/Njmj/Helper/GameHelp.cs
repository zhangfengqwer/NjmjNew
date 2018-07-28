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

        public static void GoldChange(GameObject gameObject, int num)
        {
            if (num > 0)
            {
                ShowPlusGoldChange(gameObject, num);
            }
            else
            {
                ShowDownGoldChange(gameObject, -num);
            }
        }

        private static async void ShowDownGoldChange(GameObject gameObject, int num)
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

            GameObject obj = new GameObject();
            Image image = obj.AddComponent<Image>();
            image.sprite = CommonUtil.getSpriteByBundle("Image_Down_Gold", "minus");
            obj.transform.SetParent(gameObject.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.one;
            foreach (var item in numList)
            {
                obj = new GameObject();
                image = obj.AddComponent<Image>();
                image.sprite = CommonUtil.getSpriteByBundle("Image_Down_Gold", item + "");
                obj.transform.SetParent(gameObject.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.one;
            }

            Vector3 localPosition = gameObject.transform.localPosition;
            gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(localPosition.x, localPosition.y + 30), 1.5f, false).OnComplete(() =>
            {
                gameObject.transform.localPosition = localPosition;
                DeleteAllItem(gameObject);
            });
        }

        public static async void ShowPlusGoldChange(GameObject gameObject, int num)
        {
            numList.Clear();
            //            int unitPlace = num / 1 % 10;
            //            int tenPlace = num / 10 % 10;
            //            int hundredPlace = num / 100 % 10;
            //            int thousandPlace = num / 1000 % 10;
            //            int wPlace = num / 10000 % 10;

            //第一位是否是0
            bool isStart = true;
            for (int i = 6; i >= 0; i--)
            {
                int pow = (int)Math.Pow(10, i);
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

            GameObject obj = new GameObject();
            Image image = obj.AddComponent<Image>();
            image.sprite = CommonUtil.getSpriteByBundle("Image_Plus_Gold", "plus");
            obj.transform.SetParent(gameObject.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.one;
            foreach (var item in numList)
            {
                obj = new GameObject();
                image = obj.AddComponent<Image>();
                image.sprite = CommonUtil.getSpriteByBundle("Image_Plus_Gold", item + "");
                obj.transform.SetParent(gameObject.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.one;
            }

            Vector3 localPosition = gameObject.transform.localPosition;
            gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(localPosition.x, localPosition.y + 30), 1.5f, false).OnComplete(() =>
            {
                //Log.Debug("完成");
                gameObject.transform.localPosition = localPosition;
                DeleteAllItem(gameObject);
            });
        }

        public static void DeleteAllItem(GameObject gameObject)
        {
            if (gameObject == null) return;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }

        public static void SetRoomId(GameObject gameObject, int num)
        {
            numList.Clear();
            //            int unitPlace = num / 1 % 10;
            //            int tenPlace = num / 10 % 10;
            //            int hundredPlace = num / 100 % 10;
            //            int thousandPlace = num / 1000 % 10;
            //            int wPlace = num / 10000 % 10;

            //第一位是否是0
            bool isStart = true;
            for (int i = 6; i >= 0; i--)
            {
                int pow = (int)Math.Pow(10, i);
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

            foreach (var item in numList)
            {
                GameObject obj = new GameObject();
                Image image = obj.AddComponent<Image>();
                image.sprite = CommonUtil.getSpriteByBundle("Image_RoomId", item + "");
                obj.transform.SetParent(gameObject.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.one;
            }

        }
    }
}