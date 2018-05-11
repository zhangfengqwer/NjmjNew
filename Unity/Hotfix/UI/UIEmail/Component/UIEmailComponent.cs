using ETModel;
using System;
using System.Collections.Generic;
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

        public void Awake()
        {
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
                G2C_Eamil g2cEmail = (G2C_Eamil)await SessionWrapComponent.Instance.Session.Call(new C2G_Eamil() { Uid = uid });
                emailList = g2cEmail.EmailInfoList;
                Debug.Log(emailList);
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
                    uiList.Add(ui);
                    emailItemList.Add(obj);
                }
                uiList[i].GetComponent<UIEmailItemComponent>().SetEmailData(emailList[i]);
            }
        }
    }
}
