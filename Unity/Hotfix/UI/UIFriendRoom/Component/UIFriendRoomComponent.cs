using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    class UIFriendRoomSystem : StartSystem<UIFriendRoomComponent>
    {
        public override void Start(UIFriendRoomComponent self)
        {
            self.Start();
        }
    }

    public class UIFriendRoomComponent : Component
    {
        private Button SureBtn;
        private Button SetRoomTypeBtn;
        private Button JuType3Btn;
        private Button JuType2Btn;
        private Button JuType1Btn;
        private Button ModeTypeBtn;
        private Button HuaType3Btn;
        private Button HuaType2Btn;
        private Button HuaType1Btn;
        private GameObject Grid;
        private GameObject NoRoomTipTxt;
        private GameObject CreateRoom;
        private Button JoinRoomBtn;
        private Button CreateRoomBtn;
        private Button CloseCreateRoom;
        private Button CloseFrRoomBtn;

        #region toggle选择
        private bool isSelectHuaType1;
        private bool isSelectHuaType2;
        private bool isSelectHuaType3;
        private bool isFangMain;
        private bool isJuType1;
        private bool isJuType2;
        private bool isJuType3;
        private bool isRoomPublic;
        #endregion

        #region 加入房间
        private GameObject JoinRoom;
        private Button CloseJoinRoom;
        private Button ANewBtn;
        private Button DeleteBtn;
        private GameObject InputCountGrid;
        private Text EnterTxt;
        #endregion

        private const int huaTypeCount = 3;
        private const int juTypeCount = 3;

        private List<ToggleTypeStruct> huaTypes = new List<ToggleTypeStruct>();
        private List<ToggleTypeStruct> juTypes = new List<ToggleTypeStruct>();

        private string curHuaValue;
        private string curJuValue;
        private string curEnterValue = "";
        StringBuilder builder = new StringBuilder();
        private List<Button> clickBtns = new List<Button>();

        private List<TestRoomInfo> roomInfos = new List<TestRoomInfo>();
        private List<GameObject> roomItems = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private GameObject roomItem = null;

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            #region CreateRoom
            SureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();
            SetRoomTypeBtn = rc.Get<GameObject>("SetRoomTypeBtn").GetComponent<Button>();
            JuType3Btn = rc.Get<GameObject>("JuType3Btn").GetComponent<Button>();
            JuType2Btn = rc.Get<GameObject>("JuType2Btn").GetComponent<Button>();
            JuType1Btn = rc.Get<GameObject>("JuType1Btn").GetComponent<Button>();
            ModeTypeBtn = rc.Get<GameObject>("ModeTypeBtn").GetComponent<Button>();
            HuaType3Btn = rc.Get<GameObject>("HuaType3Btn").GetComponent<Button>();
            HuaType2Btn = rc.Get<GameObject>("HuaType2Btn").GetComponent<Button>();
            HuaType1Btn = rc.Get<GameObject>("HuaType1Btn").GetComponent<Button>();
            CloseCreateRoom = rc.Get<GameObject>("CloseCreateRoom").GetComponent<Button>();
            #endregion

            Grid = rc.Get<GameObject>("Grid");
            NoRoomTipTxt = rc.Get<GameObject>("NoRoomTipTxt");
            CreateRoom = rc.Get<GameObject>("CreateRoom");
            JoinRoomBtn = rc.Get<GameObject>("JoinRoomBtn").GetComponent<Button>();
            CreateRoomBtn = rc.Get<GameObject>("CreateRoomBtn").GetComponent<Button>();
            CloseFrRoomBtn  = rc.Get<GameObject>("CloseFrRoomBtn").GetComponent<Button>();

            #region 加入房间
            JoinRoom = rc.Get<GameObject>("JoinRoom");
            CloseJoinRoom = rc.Get<GameObject>("CloseJoinRoom").GetComponent<Button>();
            ANewBtn = rc.Get<GameObject>("ANewBtn").GetComponent<Button>();
            DeleteBtn = rc.Get<GameObject>("DeleteBtn").GetComponent<Button>();
            InputCountGrid = rc.Get<GameObject>("InputCountGrid");
            EnterTxt = rc.Get<GameObject>("EnterTxt").GetComponent<Text>();
            #endregion

            #region Btn
            for(int i = 0;i< 10; ++i)
            {
                string name = "inputCount" + i;
                Button go = rc.Get<GameObject>(name).GetComponent<Button>();
                clickBtns.Add(go);
            }
            clickBtns.Distinct();
            #endregion

            AddDiffToggleStruct(huaTypeCount, rc, "HuaType", huaTypes);
            AddDiffToggleStruct(juTypeCount, rc, "JuType", juTypes);

            roomItem = CommonUtil.getGameObjByBundle(UIType.UIFriendRoomItem);

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

            //关闭当前界面
            CloseFrRoomBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIFriendRoom);
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRank(true);
            });

            //选择房主开房还是AA制
            ModeTypeBtn.onClick.Add(() =>
            {
                //暂时只有一个房主开放，之后会开放AA制
                //isFangMain = !isFangMain;
                SetCheckMarKSate(ModeTypeBtn.gameObject, true);
            });

            //设置房间类型（开放 or 不开放）
            SetRoomTypeBtn.onClick.Add(() =>
            {
                isRoomPublic = !isRoomPublic;
                SetCheckMarKSate(SetRoomTypeBtn.gameObject, isRoomPublic);
            });

            //确定创建房间
            SureBtn.onClick.Add(() =>
            {
                //确定创建房间：向服务器发送消息
            });

            //打开创建房间UI
            CreateRoomBtn.onClick.Add(() =>
            {
                CreateRoom.SetActive(true);
                if(huaTypes.Count > 0)
                {
                    SetCheckMarKSate(huaTypes[0].go, huaTypes, 1);
                }
                if(juTypes.Count > 0)
                {
                    SetCheckMarKSate(juTypes[0].go, juTypes, 2);
                }
                
            });

            //关闭创建房间UI
            CloseCreateRoom.onClick.Add(() =>
            {
                CreateRoom.SetActive(false);
            });

            //打开加入房间
            JoinRoomBtn.onClick.Add(() =>
            {
                JoinRoom.SetActive(true);
            });

            //关闭
            CloseJoinRoom.onClick.Add(() =>
            {
                ClearText();
                JoinRoom.SetActive(false);
            });

            AddClick();

            DeleteBtn.onClick.Add(() =>
            {
                if (curEnterValue.Length > 0)
                {
                    builder.Remove(curEnterValue.Length - 6, 6);
                    curEnterValue = builder.ToString();
                    EnterTxt.text = curEnterValue;
                }
            });

            ANewBtn.onClick.Add(() =>
            {
                ClearText();
            });

            GetRoomInfoReq();

            //判断今日有没有赠送房间钥匙
            /**/

            Init();
        }

        private /*async*/ void GetRoomInfoReq()
        {
            TestRoomInfo info = new TestRoomInfo();
            info.roomId = 123560;
            info.hua = "1000";
            info.ju = "8";
            info.icons = new List<string>() { "f_icon1","f_icon2"};
            roomInfos.Add(info);
            info = new TestRoomInfo();
            info.roomId = 987345;
            info.hua = "100";
            info.ju = "4";
            info.icons = new List<string>() { "m_icon1" };
            roomInfos.Add(info);
            info = new TestRoomInfo();
            info.roomId = 435465;
            info.hua = "1000";
            info.ju = "8";
            info.icons = new List<string>() { "m_icon3" };
            roomInfos.Add(info);
            if(roomInfos.Count < 0)
            {
                NoRoomTipTxt.SetActive(true);
            }
            else
            {
                NoRoomTipTxt.SetActive(false);
                CreateRoomItemss();
            }
        }

        /// <summary>
        /// 创建房间Item
        /// </summary>
        private void CreateRoomItemss()
        {
            GameObject obj = null;
            for(int i = 0;i< roomInfos.Count; ++i)
            {
                if(i < roomItems.Count)
                {
                    obj = roomItems[i];
                }
                else
                {
                    obj = GameObject.Instantiate(roomItem,Grid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    roomItems.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIFriendRoomItemComponent>();
                    uiList.Add(ui);
                }
                uiList[i].GetComponent<UIFriendRoomItemComponent>().SetItemInfo(roomInfos[i]);
            }
        }

        private void ClearText()
        {
            if (curEnterValue.Length > 0)
            {
                builder.Clear();
                curEnterValue = builder.ToString();
                EnterTxt.text = curEnterValue;
            }
        }

        private void AddClick()
        {
            for(int i = 0;i< clickBtns.Count; ++i)
            {
                int index = i;
                clickBtns[index].onClick.Add(() => 
                {
                    BtnClick(clickBtns[index].transform.Find("count").GetComponent<Text>().text);
                });
            }
        }

        private void BtnClick(string value)
        {
            #region test
            if (curEnterValue.Replace(" ", "").Length >= 6)
            {
                //向服务器发送消息
                ToastScript.createToast("房间号已经输入完成");
                return;
            }
            #endregion

            curEnterValue = builder.Append(value).Append("     ").ToString();
            EnterTxt.text = curEnterValue;
            if (curEnterValue.Replace(" ", "").Length >= 6)
            {
                //向服务器发送消息
                ToastScript.createToast("房间号已经输入完成");
            }
        }

        private void Init()
        {
            SetLabel(HuaType1Btn.gameObject, "100");
            SetLabel(HuaType2Btn.gameObject, "1000");
            SetLabel(HuaType3Btn.gameObject, "10000");
            SetLabel(JuType1Btn.gameObject, "4局/钥匙3");
            SetLabel(JuType2Btn.gameObject, "8局/钥匙4");
            SetLabel(JuType3Btn.gameObject, "16局/钥匙6");
            curHuaValue = "100";
            curJuValue = "4局/钥匙3";
            isRoomPublic = true;
            isFangMain = true;
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
                    }
                    else
                    {
                        curJuValue = list[i].go.transform.Find("Label").GetComponent<Text>().text;
                    }
                    list[i].go.transform.Find("Checkmark").gameObject.SetActive(true);
                    list[i].isSelect = true;
                }
            }
        }

        private void SetCheckMarKSate(GameObject go,bool isSelect)
        {
            go.transform.Find("Checkmark").gameObject.SetActive(isSelect);
        }

        public override void Dispose()
        {
            base.Dispose();
            clickBtns.Clear();
            uiList.Clear();
            roomItems.Clear();
            roomInfos.Clear();
            juTypes.Clear();
            huaTypes.Clear();
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
    }
}
