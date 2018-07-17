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
        private Text OwnKeyTxt;
        private ToggleGroup HuaGrid;
        private ToggleGroup JuGrid;
        private ToggleGroup RoomTypeGrid;

        #region toggle选择
        private bool isRoomPublic;
        #endregion

        private Button CloseCreateRoom;

        #region 倍率
        private List<GameObject> huaTypeToggles = new List<GameObject>();
        private List<UI> huaUIList = new List<UI>();
        private GameObject huaItem = null;
        #endregion

        #region 局数
        private List<GameObject> juTypeToggles = new List<GameObject>();
        private List<UI> juUIList = new List<UI>();
        private GameObject juItem = null;
        #endregion

        #region 房间类型
        private List<GameObject> typeToggles = new List<GameObject>();
        private List<UI> typeUIList = new List<UI>();
        private GameObject typeItem = null;
        #endregion

        public async void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/friendRoomConfig.json", FriendRoomConfig.getInstance().init);

            #region CreateRoom
            SureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();
            RoomTypeGrid = rc.Get<GameObject>("RoomTypeGrid").GetComponent<ToggleGroup>();
            JuGrid = rc.Get<GameObject>("JuGrid").GetComponent<ToggleGroup>();
            HuaGrid = rc.Get<GameObject>("HuaGrid").GetComponent<ToggleGroup>();
            #endregion
            OwnKeyTxt = rc.Get<GameObject>("OwnKeyTxt").GetComponent<Text>();

            huaItem = CommonUtil.getGameObjByBundle(UIType.UIHuaTypeToggle);
            juItem = CommonUtil.getGameObjByBundle(UIType.UIJuTypeToggle);
            typeItem = CommonUtil.getGameObjByBundle(UIType.UITypeToggle);

            CloseCreateRoom = rc.Get<GameObject>("CloseCreateRoom").GetComponent<Button>();

            //选择房主开房还是AA制

            //确定创建房间
            SureBtn.onClick.Add(() =>
            { 
                {
                    //确定创建房间：向服务器发送消息
                    CreateRoom();
                }

            });

            //关闭创建房间UI
            CloseCreateRoom.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICreateFriendRoom);
            });

            Init();
        }

        int curHua = 100;
        int curJu = 4;
        int curType = 1;
        public void SetCurHua(int curHua)
        {
            this.curHua = curHua;
        }

        public void SetCurJu(int curJu)
        {
            this.curJu = curJu;
        }

        public void SetCurType(int curType)
        {
            this.curType = curType;
        }

        private async void CreateRoom()
        {
            FriendRoomInfo info = new FriendRoomInfo();
            info.Hua = curHua;
            info.Ju = curJu;
            info.IsPublic = curType;
            UINetLoadingComponent.showNetLoading();
            G2C_CreateFriendRoom c2gCreate = (G2C_CreateFriendRoom) await SessionComponent.Instance.Session.Call(new C2G_CreateFriendRoom
            {
                  FriendRoomInfo = info,
                  UserId = PlayerInfoComponent.Instance.uid
            });
            Log.Debug("===" + c2gCreate.RoomId + "===");
            UINetLoadingComponent.closeNetLoading();
            await UIJoinRoomComponent.EnterFriendRoom(c2gCreate.RoomId.ToString());

        }

        private void Init()
        {
            GameObject obj = null;
            for(int i = 0;i< FriendRoomConfig.getInstance().beilvList.Count; ++i)
            {
                if(i < huaTypeToggles.Count)
                {
                    obj = huaTypeToggles[i];
                }
                else
                {
                    obj = GameObject.Instantiate(huaItem, HuaGrid.transform);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIHuaTypeToggleComponent>();
                    huaUIList.Add(ui);
                    huaTypeToggles.Add(obj);
                }
                huaUIList[i].GetComponent<UIHuaTypeToggleComponent>().SetToggleInfo(FriendRoomConfig.getInstance().beilvList[i],HuaGrid,i);
            }

            for(int i = 0;i< FriendRoomConfig.getInstance().juShuList.Count; ++i)
            {
                if(i < juTypeToggles.Count)
                {
                    obj = juTypeToggles[i];
                }
                else
                {
                    obj = GameObject.Instantiate(juItem, JuGrid.transform);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIJuTypeToggleComponent>();
                    juUIList.Add(ui);
                    juTypeToggles.Add(obj);
                }
                juUIList[i].GetComponent<UIJuTypeToggleComponent>().SetToggleInfo(FriendRoomConfig.getInstance().juShuList[i], JuGrid,i);
            }

            for (int i = 0; i < FriendRoomConfig.getInstance().typeList.Count; ++i)
            {
                if (i < typeToggles.Count)
                {
                    obj = typeToggles[i];
                }
                else
                {
                    obj = GameObject.Instantiate(typeItem, RoomTypeGrid.transform);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UITypeToggleComponent>();
                    typeUIList.Add(ui);
                    typeToggles.Add(obj);
                }

                typeUIList[i].GetComponent<UITypeToggleComponent>().SetToggleInfo(FriendRoomConfig.getInstance().typeList[i], RoomTypeGrid, i);
            }

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

        public override void Dispose()
        {
            base.Dispose();
            juTypeToggles.Clear();
            juUIList.Clear();
            huaTypeToggles.Clear();
            huaUIList.Clear();
            typeToggles.Clear();
            typeUIList.Clear();
        }
    }

    public class TestRoomInfo
    {
        public int roomId;
        public List<string> icons;
        public string ju;
        public string hua;
        public int isPublic;
    }
}
