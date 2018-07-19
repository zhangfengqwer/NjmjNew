using DG.Tweening;
using Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class ShowRewardUtil
    {
        public static List<string> s_rewardList = new List<string>();

        public static GameObject s_showObj = null;

        public static void Show(string reward)
        {
            try
            {
                if (s_rewardList.Count == 0)
                {
                    s_rewardList.Add(reward);
                    setData(reward);
                }
                else
                {
                    s_rewardList.Add(reward);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        static void setData(string reward)
        {
            GameObject obj = CommonUtil.getGameObjByBundle("uishowreward");
            s_showObj = GameObject.Instantiate(obj, OtherData.s_commonCanvas.transform);
            s_showObj.transform.Find("Button_close").GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickClose();
            });

            // 出生动画
            {
                s_showObj.transform.Find("content").localScale = new Vector3(0.7f, 0.7f, 0.7f);
                s_showObj.transform.Find("content").DOScale(1.0f, 0.2f);
            }

            List<string> list1 = new List<string>();
            CommonUtil.splitStr(reward, list1, ';');

            for (int i = 0; i < list1.Count; i++)
            {
                List<string> list2 = new List<string>();
                CommonUtil.splitStr(list1[i], list2, ':');

                int id = int.Parse(list2[0]);
                int num = int.Parse(list2[1]);

                PropInfo propInfo = PropConfig.getInstance().getPropInfoById(id);

                GameObject item = new GameObject();
                item.transform.SetParent(s_showObj.transform.Find("content/Image_bg"));
                item.transform.localScale = new Vector3(1,1,1);

                // 图标
                {
                    Image icon = item.AddComponent<Image>();

                    // 话费
                    if (id == 3)
                    {
                        icon.sprite = CommonUtil.getSpriteByBundle("image_zhuanpan", "icon_huafei");
                        icon.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(120,80);
                    }
                    else
                    {
                        icon.sprite = CommonUtil.getSpriteByBundle("image_shop", propInfo.prop_id.ToString());
                    }
                }

                // 数量
                {
                    GameObject text_obj = new GameObject();

                    Text text = text_obj.AddComponent<Text>();

                    // 话费
                    if (id == 3)
                    {
                        text.text = "X" + (float)num / 100.0f;
                    }
                    else
                    {
                        text.text = "X" + num;
                    }
                        
                    text.fontSize = 22;
                    text.alignment = TextAnchor.MiddleCenter;

                    text_obj.transform.SetParent(item.transform);
                    text_obj.transform.localScale = new Vector3(1,1,1);
                    text_obj.transform.localPosition = new Vector3(0,-65,0);
                    CommonUtil.SetTextFont(text_obj);
                }

                float x = CommonUtil.getPosX(list1.Count, 130, i, 0);
                item.transform.localPosition = new Vector3(x, 0, 0);
            }
        }

        public static void onClickClose()
        {
            s_rewardList.RemoveAt(0);
            GameObject.Destroy(s_showObj);
            s_showObj = null;

            if (s_rewardList.Count > 0)
            {
                setData(s_rewardList[0]);
            }
        }
    }
}
