using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIEmailSystem: AwakeSystem<UIEmailComponent>
    {
        public override void Awake(UIEmailComponent self)
        {
            self.Awake();
        }
    }

    public class UIEmailComponent : Component
    {
        private Button returnBtn;
        private GameObject grid;
        private Text emailCountTxt;
        private List<Email> emailList = new List<Email>();
        private GameObject emailItem = null;
        private List<GameObject> emailItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        public static UIEmailComponent Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            emailCountTxt = rc.Get<GameObject>("EmailCountTxt").GetComponent<Text>();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIEmail);
            });

            emailItem = CommonUtil.getGameObjByBundle(UIType.UIEmailItem);

            GetEmail();
        }

        public async void GetEmail()
        {
            try
            {
                long uid = PlayerInfoComponent.Instance.uid;
                G2C_Email g2cEmail = (G2C_Email)await SessionWrapComponent.Instance.Session.Call(new C2G_Email() { Uid = uid });
                emailList = g2cEmail.EmailInfoList;
                Debug.Log(JsonHelper.ToJson(emailList));
                if (emailList != null && g2cEmail.EmailInfoList.Count > 0)
                {
                    CreateEmailItemList();
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
            
        }

        public void CreateEmailItemList()
        {
            GameObject obj = null;
            for(int i = 0;i< emailList.Count; ++i)
            {
                if(i < emailItemList.Count)
                {
                    obj = emailItemList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(emailItem);
                    obj.transform.SetParent(grid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIEmailItemComponent>();
                    if (emailList[i].IsRead)
                        obj.transform.SetAsFirstSibling();
                    uiList.Add(ui);
                    emailItemList.Add(obj);
                }
                uiList[i].GetComponent<UIEmailItemComponent>().SetEmailData(emailList[i]);
            }
            emailCountTxt.text = new StringBuilder()
                                .Append(emailList.Count)
                                .Append("/")
                                .Append(50).ToString();
        }

        /// <summary>
        /// 排序 暂时不用
        /// </summary>
        public void SortList()
        {
            for(int i = 0; i < grid.transform.childCount; ++i)
            {
                Transform tr = grid.transform.GetChild(i);
                if (tr.Find("Flag").gameObject.activeInHierarchy)
                    tr.SetAsFirstSibling();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            for(int i = 0;i< grid.transform.childCount; ++i)
            {
                //设置当前未读文本邮件为已读
            }
            emailItemList.Clear();
            uiList.Clear();
            emailList.Clear();
            Instance = null;
        }
    }
}
