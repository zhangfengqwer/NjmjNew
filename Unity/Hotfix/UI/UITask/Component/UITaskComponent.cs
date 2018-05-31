using ETModel;
using System.Collections.Generic;
using System.Text;
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
        private List<TaskInfo> taskProgressList = new List<TaskInfo>();

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

            taskItem = CommonUtil.getGameObjByBundle(UIType.UITaskItem);
            GetTaskInfoList();
        }

        private async void GetTaskInfoList()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_Task g2cTask = (G2C_Task)await SessionWrapComponent.Instance.Session.Call(new C2G_Task { uid = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();
            taskProgressList = g2cTask.TaskProgressList;
            progressTxt.text = new StringBuilder().Append("<color=#E8DBAAFF>完成数量:</color>")
                                                  .Append(GetProgress())
                                                  .Append("/")
                                                  .Append(taskProgressList.Count)
                                                  .ToString();
            CreateTaskItem(g2cTask.TaskProgressList);
        }

        private int GetProgress()
        {
            int count = 0;
            for (int i = 0; i < taskProgressList.Count; ++i)
            {
                if (taskProgressList[i].IsComplete)
                    count++;
            }
            return count;
        }

        private void CreateTaskItem(List<TaskInfo> taskInfoList)
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
                uiList[i].GetComponent<UITaskItemComponent>().SetTaskItemInfo(taskInfoList[i]);
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
