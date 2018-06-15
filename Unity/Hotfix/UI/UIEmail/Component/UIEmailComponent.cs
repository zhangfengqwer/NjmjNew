using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIEmailSystem: StartSystem<UIEmailComponent>
    {
        public override void Start(UIEmailComponent self)
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
        private Text NoEmailTip;
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
            NoEmailTip = rc.Get<GameObject>("NoEmailTip").GetComponent<Text>();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIEmail);
            });

            emailItem = CommonUtil.getGameObjByBundle(UIType.UIEmailItem);

            GetEmail();
        }

        /// <summary>
        /// 获得邮件信息
        /// </summary>
        public async void GetEmail()
        {

            long uid = PlayerInfoComponent.Instance.uid;
            UINetLoadingComponent.showNetLoading();
            G2C_Email g2cEmail = (G2C_Email)await SessionWrapComponent.Instance.Session.Call(new C2G_Email() { Uid = uid });
            UINetLoadingComponent.closeNetLoading();

            emailList = g2cEmail.EmailInfoList;
            if(emailList.Count <= 0)
            {
                NoEmailTip.gameObject.SetActive(true);
                NoEmailTip.text = "暂无邮件!";
            }
            else
            {
                NoEmailTip.gameObject.SetActive(false);   
            }
            CreateEmailItemList();
            GetNoGetCount();
        }

        //删除邮件后刷新界面显示
        public void RefreshMailUI()
        {
            GetEmail();
            //删除之后remove相应的ui组件
        }

        int notGetcount = 0;
        private int GetNoGetCount()
        {
            for (int i = 0; i < emailList.Count; ++i)
            {
                if (emailList[i].State == 0)
                {
                    notGetcount++;
                }
            }
            return notGetcount;
        }

        public void DeCount()
        {
            --notGetcount;
            if (notGetcount <= 0)
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRedTip(5, false);
            }
        }

        /// <summary>
        /// 创建邮件列表
        /// </summary>
        public void CreateEmailItemList()
        {
            if (emailList.Count <= 0)
            {
                SetMoreMailHide(0);
            }

            GameObject obj = null;
            for (int i = 0;i< emailList.Count; ++i)
            {
                if(i < emailItemList.Count)
                {
                    emailItemList[i].SetActive(true);
                    obj = emailItemList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(emailItem);
                    obj.transform.SetParent(grid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.name = emailList[i].EId.ToString();
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIEmailItemComponent>();
                    uiList.Add(ui);
                    emailItemList.Add(obj);
                }
                if (emailList[i].State == 0)
                {
                    obj.transform.SetAsFirstSibling();
                }
                try
                {
                    uiList[i].GetComponent<UIEmailItemComponent>().SetEmailData(emailList[i]);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            emailCountTxt.text = new StringBuilder()
                                .Append(emailList.Count)
                                .Append("/")
                                .Append(50).ToString();
            SetMoreMailHide(emailList.Count);
        }

        /// <summary>
        /// 设置多余的item被隐藏掉
        /// </summary>
        private void SetMoreMailHide(int index)
        {
            for(int i = index;i< emailItemList.Count;++i)
            {
                emailItemList[i].SetActive(false);
            }
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

        public override async void Dispose()
        {
            base.Dispose();
            for (int i = 0; i < emailList.Count; ++i)
            {
                //设置当前未读文本邮件为已读
                if (string.IsNullOrEmpty(emailList[i].RewardItem))
                {
                    G2C_EmailOperate g2cGetItem = (G2C_EmailOperate)await SessionWrapComponent.Instance
                .Session.Call(new C2G_EmailOperate
                {
                    Uid = PlayerInfoComponent.Instance.uid,
                    InfoList = new List<GetItemInfo>(),
                    EmailId = emailList[i].EId,
                    state = 1
                });
                }
            }

            emailItemList.Clear();
            uiList.Clear();
            emailList.Clear();
            Instance = null;

            using (UnityWebRequestAsync webRequestAsync = ETModel.ComponentFactory.Create<UnityWebRequestAsync>())
            {
                string versionUrl = GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + "Version.txt";
                //Log.Debug(versionUrl);
                await webRequestAsync.DownloadAsync(versionUrl);

                string test = webRequestAsync.Request.downloadHandler.text;
                //Log.Debug(JsonHelper.ToJson(this.VersionConfig));
            }
        }
    }
}
