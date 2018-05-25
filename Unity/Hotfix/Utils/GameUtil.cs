using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    class GameUtil
    {
        /*
         * 101:1000
         */
        static public void changeDataWithStr(string reward)
        {
            List<string> list1 = new List<string>();
            CommonUtil.splitStr(reward, list1, ';');

            for (int i = 0; i < list1.Count; i++)
            {
                List<string> list2 = new List<string>();
                CommonUtil.splitStr(list1[i], list2, ':');

                int id = int.Parse(list2[0]);
                float num = float.Parse(list2[1]);

                changeData(id, num);
            }
        }

        /*
         * 用于玩家 <金币、元宝、道具> 的获得和消耗
         * 如果当前在主界面，会刷新主界面的金币和元宝数值
         */
        static public void changeData(int id, float num)
        {
            if (id == 1)
            {
                PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum += (int)num;

                if (PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum < 0)
                {
                    PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum = 0;
                }
            }
            else if (id == 2)
            {
                PlayerInfoComponent.Instance.GetPlayerInfo().WingNum += (int)num;

                if (PlayerInfoComponent.Instance.GetPlayerInfo().WingNum < 0)
                {
                    PlayerInfoComponent.Instance.GetPlayerInfo().WingNum = 0;
                }
            }
            else if (id == 3)
            {
                PlayerInfoComponent.Instance.GetPlayerInfo().HuaFeiNum += num;

                if (PlayerInfoComponent.Instance.GetPlayerInfo().HuaFeiNum < 0)
                {
                    PlayerInfoComponent.Instance.GetPlayerInfo().HuaFeiNum = 0;
                }
            }
            else
            {
                bool isFind = false;
                for (int i = 0; i < PlayerInfoComponent.Instance.GetBagInfoList().Count; i++)
                {
                    if (PlayerInfoComponent.Instance.GetBagInfoList()[i].ItemId == id)
                    {
                        isFind = true;

                        PlayerInfoComponent.Instance.GetBagInfoList()[i].Count += (int)num;

                        if (PlayerInfoComponent.Instance.GetBagInfoList()[i].Count <= 0)
                        {
                            PlayerInfoComponent.Instance.GetBagInfoList().RemoveAt(i);
                        }

                        break;
                    }
                }

                if (!isFind)
                {
                    Bag bag = new Bag();
                    bag.ItemId = id;
                    bag.Count = (int)num;
                    PlayerInfoComponent.Instance.GetBagInfoList().Add(bag);
                }
            }
            
            // 刷新主界面
            if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain) != null)
            {
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>() != null)
                {
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().refreshUI();
                }
            }
        }

        static public int GetDataCount(int id)
        {
            if (id == 1)
            {
                return (int)PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum;
            }
            else if (id == 2)
            {
                return (int)PlayerInfoComponent.Instance.GetPlayerInfo().WingNum;
            }
            else
            {
                for (int i = 0; i < PlayerInfoComponent.Instance.GetBagInfoList().Count; i++)
                {
                    if (PlayerInfoComponent.Instance.GetBagInfoList()[i].ItemId == id)
                    {
                        return PlayerInfoComponent.Instance.GetBagInfoList()[i].Count;
                    }
                }
            }

            return 0;
        }

        static public bool isVIP()
        {
            if (PlayerInfoComponent.Instance.GetPlayerInfo().VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
            {
                return true;
            }

            return false;
        }

        static public bool isCanUseEmoji()
        {
            if (PlayerInfoComponent.Instance.GetPlayerInfo().EmogiTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
            {
                return true;
            }

            return false;
        }

        public static async void StartTimer(float time, UnityEngine.GameObject obj, bool isDestroy = false)
        {
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(300);
                --time;
            }
            if (isDestroy)
                UnityEngine.GameObject.DestroyObject(obj);
            obj.SetActive(false);
        }
    }
}
