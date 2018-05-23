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
        private List<UI> uiList = new List<UI>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            GameObject obj = null;
            GameObject item = CommonUtil.getGameObjByBundle(UIType.UINoticeItem);
            //await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIActivity);
            });
            for (int i = 0; i < PlayerInfoComponent.Instance.GetNoticeInfoList().Count; ++i)
            {
                NoticeInfo info = PlayerInfoComponent.Instance.GetNoticeInfoList()[i];
                obj = GameObject.Instantiate(item);
                obj.transform.SetParent(grid.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                #region 
                UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                ui.AddComponent<UINoticeItemComponent>();
                objList.Add(obj);
                uiList.Add(ui);
                ui.GetComponent<UINoticeItemComponent>().SetText(info);
                ui.GetComponent<UINoticeItemComponent>().SetLine();
                #endregion
            }
            //float height = grid.transform.childCount * 158f;
            //grid.GetComponent<RectTransform>().rect.Set(-631, 318.3497f, 100, 1500);
            //grid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            //for(int i = 0;i< grid.transform.childCount; ++i)
            //{
            //    float y = (float)(-158) * (i)-
            //        (uiList[i].GetComponent<UINoticeItemComponent>().GetTextRow() - 1) * 34;
            //    grid.transform.GetChild(i).transform.localPosition = new Vector3(594.0f, y, 0.0f);
            //}
        }
    }
}
