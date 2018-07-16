using ETModel;
using Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    class UICreateFriendRoomSystem : StartSystem<UICreateFriendRoomComponent>
    {
        public override void Start(UICreateFriendRoomComponent self)
        {
            self.Start();
        }
    }

    public class UICreateFriendRoomComponent : Component
    {
        private Button SureBtn;
        private Button Type1Btn;
        private Button JuType3Btn;
        private Button JuType2Btn;
        private Button JuType1Btn;
        private Button ModeTypeBtn;
        private Button HuaType3Btn;
        private Button HuaType2Btn;
        private Button HuaType1Btn;
        private Button Type2Btn;
        private Text OwnKeyTxt;

        #region toggle选择
        private bool isRoomPublic;
        #endregion
        private Button CloseCreateRoom;

        private const int huaTypeCount = 3;
        private const int juTypeCount = 3;
        private const int typeCount = 2;

        private int curHuaType = 1;
        private int curJuType = 1;

        private List<ToggleTypeStruct> huaTypes = new List<ToggleTypeStruct>();
        private List<ToggleTypeStruct> juTypes = new List<ToggleTypeStruct>();
        private List<ToggleTypeStruct> typeBtns = new List<ToggleTypeStruct>();

        private string curHuaValue;
        private string curJuValue;
        

        public async void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/friendRoomConfig.json", FriendRoomConfig.getInstance().init);


            #region CreateRoom
            SureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();
            JuType3Btn = rc.Get<GameObject>("JuType3Btn").GetComponent<Button>();
            JuType2Btn = rc.Get<GameObject>("JuType2Btn").GetComponent<Button>();
            JuType1Btn = rc.Get<GameObject>("JuType1Btn").GetComponent<Button>();
            ModeTypeBtn = rc.Get<GameObject>("ModeTypeBtn").GetComponent<Button>();
            HuaType3Btn = rc.Get<GameObject>("HuaType3Btn").GetComponent<Button>();
            HuaType2Btn = rc.Get<GameObject>("HuaType2Btn").GetComponent<Button>();
            HuaType1Btn = rc.Get<GameObject>("HuaType1Btn").GetComponent<Button>();
            Type1Btn  = rc.Get<GameObject>("Type1Btn").GetComponent<Button>();
            Type2Btn = rc.Get<GameObject>("Type2Btn").GetComponent<Button>();
            #endregion
            OwnKeyTxt = rc.Get<GameObject>("OwnKeyTxt").GetComponent<Text>();

            CloseCreateRoom = rc.Get<GameObject>("CloseCreateRoom").GetComponent<Button>();

            AddDiffToggleStruct(huaTypeCount, rc, "HuaType", huaTypes);
            AddDiffToggleStruct(juTypeCount, rc, "JuType", juTypes);
            AddDiffToggleStruct(typeCount, rc, "Type", typeBtns);

            #region 选择局数
            JuType1Btn.onClick.Add(() =>
            {
                SetCheckMarKSate(JuType1Btn.gameObject, juTypes, 2);
            });

            JuType2Btn.onClick.Add(() =>
            {
                SetCheckMarKSate( JuType2Btn.gameObject, juTypes, 2);
            });

            JuType3Btn.onClick.Add(() =>
            {
                SetCheckMarKSate(JuType3Btn.gameObject, juTypes, 2);
            });
            #endregion

            #region 选择每花
            HuaType1Btn.onClick.Add(() =>
            {
                SetCheckMarKSate( HuaType1Btn.gameObject, huaTypes, 1);
            });

            HuaType2Btn.onClick.Add(() =>
            {
                SetCheckMarKSate( HuaType2Btn.gameObject, huaTypes, 1);
            });

            HuaType3Btn.onClick.Add(() =>
            {
                SetCheckMarKSate(HuaType3Btn.gameObject, huaTypes, 1);
            });
            #endregion

            #region 选择类型
            //设置房间类型（开放 or 不开放）
            Type1Btn.onClick.Add(() =>
            {
                isRoomPublic = true;
                SetCheckMarKSate(Type1Btn.gameObject);
            });

            //设置房间类型（开放 or 不开放）
            Type2Btn.onClick.Add(() =>
            {
                isRoomPublic = false;
                SetCheckMarKSate(Type2Btn.gameObject);
            });
            #endregion

            if (huaTypes.Count > 0)
            {
                SetCheckMarKSate(huaTypes[0].go, huaTypes, 1);
            }
            if (juTypes.Count > 0)
            {
                SetCheckMarKSate(juTypes[0].go, juTypes, 2);
            }

            //选择房主开房还是AA制
            ModeTypeBtn.onClick.Add(() =>
            {
                //暂时只有一个房主开放，之后会开放AA制
                //isFangMain = !isFangMain;
                //SetCheckMarKSate(ModeTypeBtn.gameObject);
            });

            //确定创建房间
            SureBtn.onClick.Add(() =>
            {
                int curmeihua = 100;
                int curjushu = 4;
                int curKeyCost = 3;
                
                {
                    string[] sps = curJuValue.Split('/');
                    int index = sps[0].IndexOf('局');
                    curjushu = int.Parse(sps[0].Substring(0, index));
                    curKeyCost = int.Parse(sps[1].TrimStart());
                    curmeihua = int.Parse(curHuaValue);
                }

                int myFriendKey = PlayerInfoComponent.Instance.GetBagById(112).Count;
                if(myFriendKey < curKeyCost)
                {
                    ToastScript.createToast("钥匙不够！");
                    return;
                }

                {
                    //确定创建房间：向服务器发送消息
                }

            });

            //关闭创建房间UI
            CloseCreateRoom.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICreateFriendRoom);
            });

            Init();
        }

        private void Init()
        {
            for(int i = 0;i< FriendRoomConfig.getInstance().beilvList.Count; ++i)
            {
                SetLabel(huaTypes[i].go, FriendRoomConfig.getInstance().beilvList[i].ToString());
            }

            for(int i = 0;i< FriendRoomConfig.getInstance().juShuList.Count; ++i)
            {
                string txt = $"{FriendRoomConfig.getInstance().juShuList[i].m_jushu}局/     {FriendRoomConfig.getInstance().juShuList[i].m_yaoshi}";
                SetLabel(juTypes[i].go, txt);
            }

            curHuaValue = FriendRoomConfig.getInstance().beilvList[0].ToString();

            curJuValue = $"{FriendRoomConfig.getInstance().juShuList[0].m_jushu}局/     {FriendRoomConfig.getInstance().juShuList[0].m_yaoshi}";

            curHuaType = 1;
            curJuType = 1;

            if (PlayerInfoComponent.Instance.GetBagById(112) != null)
            {
                OwnKeyTxt.text = PlayerInfoComponent.Instance.GetBagById(112).Count.ToString();
            }
        }

        private void SetActive(int index)
        {
            for(int i = index; i< FriendRoomConfig.getInstance().typeList.Count; ++i)
            {
                //完善
            }
        }

        private GameObject GetHuaObj(int i)
        {
            switch (i)
            {
                case 1:
                    return HuaType1Btn.gameObject;
                case 2:
                    return HuaType2Btn.gameObject;
                case 3:
                    return HuaType3Btn.gameObject;
            }
            return null;
        }

        private GameObject GetJuObj (int i)
        {
            switch (i)
            {
                case 1:
                    return JuType1Btn.gameObject;
                case 2:
                    return JuType2Btn.gameObject;
                case 3:
                    return JuType3Btn.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 添加不同类型的toggle
        /// </summary>
        /// <param name="typeCount"></param>
        /// <param name="rc"></param>
        /// <param name="Objname"></param>
        /// <param name="list"></param>
        private void AddDiffToggleStruct(int typeCount,ReferenceCollector rc,string Objname,List<ToggleTypeStruct> list)
        {
            for (int i = 0; i < typeCount; ++i)
            {
                GameObject go = rc.Get<GameObject>($"{Objname}{i + 1}Btn");
                ToggleTypeStruct tog = new ToggleTypeStruct();
                tog.go = go;
                tog.isSelect = i == 0 ? true : false;
                list.Add(tog);
            }

            list.Distinct();
        }

        private void SetLabel(GameObject go,string value)
        {
            go.transform.Find("Label").GetComponent<Text>().text = value;
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        /// <param name="go"></param>
        /// <param name="list"></param>
        private void SetCheckMarKSate(GameObject go, List<ToggleTypeStruct> list, int type)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (!go.Equals(list[i].go))
                {
                    list[i].go.transform.Find("Checkmark").gameObject.SetActive(false);
                    list[i].isSelect = false;
                }
                else
                {
                    if (type == 1)
                    {
                        curHuaValue = list[i].go.transform.Find("Label").GetComponent<Text>().text;
                        curHuaType = i + 1;
                        /*Log.Debug("花 类型：" + curHuaType);*/
                    }
                    else
                    {
                        curJuValue = list[i].go.transform.Find("Label").GetComponent<Text>().text;
                        curJuType = i + 1;
                        /*Log.Debug("局 类型：" + curJuType);*/
                    }
                    list[i].go.transform.Find("Checkmark").gameObject.SetActive(true);
                    list[i].isSelect = true;
                }
            }
        }

        private void SetCheckMarKSate(GameObject go)
        {
            for(int i = 0;i< typeBtns.Count; ++i)
            {
                if(typeBtns[i].go.Equals(go))
                {
                    typeBtns[i].go.transform.Find("Checkmark").gameObject.SetActive(true);
                    typeBtns[i].isSelect = true;
                }
                else
                {
                    typeBtns[i].go.transform.Find("Checkmark").gameObject.SetActive(false);
                    typeBtns[i].isSelect = false;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            juTypes.Clear();
            huaTypes.Clear();
            typeBtns.Clear();
        }
    }

    /// <summary>
    /// toggle选择类型
    /// </summary>
    public class ToggleTypeStruct
    {
        public bool isSelect;
        public GameObject go;
    }

    public class TestRoomInfo
    {
        public int roomId;
        public List<string> icons;
        public string ju;
        public string hua;
        public bool isPublic;
    }
}
