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
        private List<TaskProgress> taskProgressList = new List<TaskProgress>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            progressTxt = rc.Get<GameObject>("ProgressTxt").GetComponent<Text>();

            returnBtn.onClick.Add(() =>
            {
                TaskTest();
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UITask);
            });
            taskInfoList = Game.Scene.GetComponent<PlayerInfoComponent>().GetTaskInfoList();
            Debug.Log(JsonHelper.ToJson(taskInfoList));

            taskItem = CommonUtil.getGameObjByBundle(UIType.UITaskItem);
            RequestTaskInfo();
        }

        private async void TaskTest()
        {
            long uid = PlayerInfoComponent.Instance.uid;
            TaskProgress taskProgress = new TaskProgress();
            taskProgress.TaskId = 101;
            taskProgress.Progress = 30;
            taskProgress.Target = 30;
            G2C_UpdateTaskProgress g2cTask = (G2C_UpdateTaskProgress)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdateTaskProgress { UId = uid,TaskPrg = taskProgress });
        }

        private async void RequestTaskInfo()
        {
            long uid = PlayerInfoComponent.Instance.uid;
            G2C_Task g2cTask = (G2C_Task)await SessionWrapComponent.Instance.Session.Call(new C2G_Task { uid = uid });
            taskProgressList = g2cTask.TaskProgressList;
            CreateTaskItem();
        }

        private void CreateTaskItem()
        {
            GameObject obj = null;
            TaskProgress progress = null;
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
                progress = GetProgressByTaskID(taskInfoList[i].Id);
                uiList[i].GetComponent<UITaskItemComponent>().SetTaskItemInfo(taskInfoList[i], progress);
            }
        }

        private TaskProgress GetProgressByTaskID(int TaskId)
        {
            for (int i = 0;i< taskProgressList.Count; ++i)
            {
                if (taskProgressList[i].TaskId == TaskId)
                    return taskProgressList[i];
            }
            return null;
        }
        public override void Dispose()
        {
            //taskInfoList.Clear();
            taskItemList.Clear();
            uiList.Clear();
        }
    }
}
