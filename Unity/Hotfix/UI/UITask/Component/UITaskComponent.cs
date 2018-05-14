using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UITaskSystem: AwakeSystem<UITaskComponent>
    {
        public override void Awake(UITaskComponent self)
        {
            self.Awake();
        }
    }
     
    public class UITaskComponent : Component
    {
        private Button returnBtn;
        private GameObject grid;
        private Text progressTxt;
        private GameObject taskItem = null;
        private List<GameObject> taskItemList = new List<GameObject>();
        private List<TaskInfo> taskInfoList = new List<TaskInfo>();
        private List<UI> uiList = new List<UI>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            progressTxt = rc.Get<GameObject>("ProgressTxt").GetComponent<Text>();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UITask);
            });
            taskInfoList = Game.Scene.GetComponent<PlayerInfoComponent>().GetTaskInfoList();
            Debug.Log(JsonHelper.ToJson(taskInfoList));

            taskItem = CommonUtil.getGameObjByBundle(UIType.UITaskItem);
            CreateTaskItem();
        }

        private void CreateTaskItem()
        {
            GameObject obj = null;
            for(int i = 0;i< taskInfoList.Count; ++i)
            {
                if (i < taskItemList.Count)
                    obj = taskItemList[i];
                else
                {
                    obj = GameObject.Instantiate(taskItem);
                    obj.transform.SetParent(grid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UITaskItemComponent>();
                    taskItemList.Add(obj);
                    uiList.Add(ui);
                }
                uiList[i].GetComponent<UITaskItemComponent>().SetTaskItemInfo(taskInfoList[i], 0);
            }
        }

        public override void Dispose()
        {
            //taskInfoList.Clear();
            taskItemList.Clear();
            uiList.Clear();
        }
    }
}
