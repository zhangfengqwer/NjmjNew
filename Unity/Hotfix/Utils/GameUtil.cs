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
        static public void changeDataWithStr(string reward,char splitType)
        {
            List<string> list1 = new List<string>();
            CommonUtil.splitStr(reward, list1, ';');

            for (int i = 0; i < list1.Count; i++)
            {
                List<string> list2 = new List<string>();
                CommonUtil.splitStr(list1[i], list2, splitType);

                int id = int.Parse(list2[0]);
                int num = int.Parse(list2[1]);

                changeData(id, num);
            }
        }

        /*
         * 用于玩家 <金币、元宝、道具> 的获得和消耗
         * 如果当前在主界面，会刷新主界面的金币和元宝数值
         */
        static public void changeData(int id, int num)
        {
            if (id == 1)
            {
                PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum += num;

                if (PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum < 0)
                {
                    PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum = 0;
                }

                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain) != null)
                {
                    if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>() != null)
                    {
                        Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().refreshUI();
                    }
                }
            }
            else if (id == 2)
            {
                PlayerInfoComponent.Instance.GetPlayerInfo().WingNum += num;

                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain) != null)
                {
                    if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>() != null)
                    {
                        Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().refreshUI();
                    }
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

                        PlayerInfoComponent.Instance.GetBagInfoList()[i].Count += num;

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
                    bag.Count = num;
                    PlayerInfoComponent.Instance.GetBagInfoList().Add(bag);
                }
            }
        }
    }
}
