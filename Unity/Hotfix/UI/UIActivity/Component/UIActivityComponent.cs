using ETModel;
using Hotfix;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIActivitySystem : AwakeSystem<UIActivityComponent>
    {
        public override void Awake(UIActivityComponent self)
        {
            self.Awake();
        }
    }

    public class UIActivityComponent : Component
    {
        private Button returnBtn;
        private GameObject grid;

        private List<GameObject> objList = new List<GameObject>();

        public async void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            GameObject obj = null;
            GameObject item = CommonUtil.getGameObjByBundle(UIType.UINoticeItem);
            //await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);

            for (int i = 0; i < 6; ++i)
            {
                obj = GameObject.Instantiate(item, grid.transform);
                obj.transform.localPosition = new Vector3(0, - 100 * i, 0);

                //obj.transform.localScale = new Vector3(2, 2, 2);
                #region 
                //                UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                //                ui.AddComponent<UINoticeItemComponent>();
                //                objList.Add(obj);
                //                if (i == 0)
                //                    ui.GetComponent<UINoticeItemComponent>().SetText("领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领");
                //                if (i == 1)
                //                    ui.GetComponent<UINoticeItemComponent>().SetText("领取领取领取领取领取领取领取领取领取领取领取" +
                //                        "领取领取领取领取领取领取领取领取领领取领取领取领取领取领取领" +
                //                        "取领取领取领取领取领取领取领取领取领取领取领取领取领");
                //                else
                //                    ui.GetComponent<UINoticeItemComponent>().SetText("领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领取领");
                //                int space = ui.GetComponent<UINoticeItemComponent>().GetTextRow() - 1 * 34;
                //                float y = (float)(-158) * objList.Count -
                //                    (ui.GetComponent<UINoticeItemComponent>().GetTextRow() - 1) * 34;
                //                Debug.Log(space);
                //                //obj.transform.localPosition = new Vector3(594.0f, y, 0.0f);
                //                Debug.Log((-158) * objList.Count -
                //                    (ui.GetComponent<UINoticeItemComponent>().GetTextRow() - 1) * 34);
                #endregion
            }

            //            for(int i = 0;i< grid.transform.childCount; ++i)
            //            {
            //                grid.transform.localPosition = Vector3.zero;
            //                grid.transform.GetChild(i).localPosition = Vector3.zero;
            //                grid.transform.GetChild(i).localPosition = new Vector3(1000,1000,0);
            //                Debug.Log(grid.transform.GetChild(i).transform.localPosition);
            //            }
        }
    }
}
