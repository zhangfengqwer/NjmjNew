using ETModel;
using Hotfix;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIGMSystem : StartSystem<UIGMComponent>
    {
        public override void Start(UIGMComponent self)
        {
            self.Start();
        }
    }

    public class UIGMComponent : Component
    {
        private Button OperateBlackListBtn;
        private Button DissRoomBtn;
        private Button SendMailBtn;
        private Button RefreshExelBtn;
        private Button GenerateDataBtn;
        private GameObject Mail;
        private GameObject BlackList;
        private GameObject ExelData;
        private Button ShowUserInfoBtn;
        private Button MailBtn;
        private Button BlackListBtn;
        private Button ExelDataBtn;

        private Button GerateBtn;
        private InputField CreateTimeInputField;
        private InputField EndTimeInputField;
        private InputField IPInputField;
        private InputField BUIdInputField;
        private InputField RewardInputField;
        private InputField ContentInputField;
        private InputField TitleInputField;
        private InputField UIdInputField;
        private InputField ReasonInputField;

        #region User
        private Text UserIdTxt;
        private Text MaxhuaTxt;
        private Text ExTxt;
        private Text VipTimeTxt;
        private Text WinGameTxt;
        private Text TatalGameTxt;
        private Text IsRealNameTxt;
        private Text UserNameTxt;
        private Text YuanbaoTxt;
        private Text GoldNumTxt;
        private Text HuafeiTxt;
        private GameObject UserData;
        private Button UserDataBtn;
        private InputField UserIdInputField;
        private InputField NameInputField;
        private Button SureBtn;
        private Text LastOnLineTxt;
        private Text PhoneTxt;
        private Text RegisterTimeTxt;
        private Text IpTxt;
        private Text ChannelTxt;
        #endregion

        #region 强制离线
        private Button ForceOfflineBtn;
        private Button ForceUserBtn;
        private GameObject ForceOffline;
        private InputField FReasonInputField;
        private InputField ForceUIdInputField;
        private Button ForceOfflineCloseBtn;
        #endregion

        #region 修改用户信息
        private Button ModifyBtn;
        private GameObject Modify;
        private Button ModifyCloseBtn;
        private InputField RestChangeNameInputField;
        private Button ModifySureBtn;
        private InputField ModifyPropInputField;
        private InputField ModifyIconInputField;
        private InputField MoUIdInputField;
        #endregion

        #region 房间信息
        private Button RoomBtn;
        private Button CloseRoomBtn;
        private GameObject RoomInfo;
        private Button RequestBtn;
        private Text NewRoomTxt;
        private Text NewInGameTxt;
        private Text JingRoomTxt;
        private Text JingInGameTxt;
        #endregion

        private bool isOn = false;

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            OperateBlackListBtn = rc.Get<GameObject>("OperateBlackListBtn").GetComponent<Button>();
            DissRoomBtn = rc.Get<GameObject>("DissRoomBtn").GetComponent<Button>();
            SendMailBtn = rc.Get<GameObject>("SendMailBtn").GetComponent<Button>();
            RefreshExelBtn = rc.Get<GameObject>("RefreshExelBtn").GetComponent<Button>();
            GenerateDataBtn = rc.Get<GameObject>("GenerateDataBtn").GetComponent<Button>();
            ShowUserInfoBtn = rc.Get<GameObject>("ShowUserInfoBtn").GetComponent<Button>();
            GerateBtn = rc.Get<GameObject>("GerateBtn").GetComponent<Button>();
            UIdInputField = rc.Get<GameObject>("UIdInputField").GetComponent<InputField>();
            ContentInputField = rc.Get<GameObject>("ContentInputField").GetComponent<InputField>();
            TitleInputField = rc.Get<GameObject>("TitleInputField").GetComponent<InputField>();
            RewardInputField = rc.Get<GameObject>("RewardInputField").GetComponent<InputField>();
            BUIdInputField = rc.Get<GameObject>("BUIdInputField").GetComponent<InputField>();
            IPInputField = rc.Get<GameObject>("IPInputField").GetComponent<InputField>();
            EndTimeInputField = rc.Get<GameObject>("EndTimeInputField").GetComponent<InputField>();
            ReasonInputField = rc.Get<GameObject>("ReasonInputField").GetComponent<InputField>();
            CreateTimeInputField = rc.Get<GameObject>("CreateTimeInputField").GetComponent<InputField>();

            MailBtn = rc.Get<GameObject>("MailBtn").GetComponent<Button>();
            ExelDataBtn = rc.Get<GameObject>("ExelDataBtn").GetComponent<Button>();
            BlackListBtn = rc.Get<GameObject>("BlackListBtn").GetComponent<Button>();

            #region User
            MaxhuaTxt = rc.Get<GameObject>("MaxhuaTxt").GetComponent<Text>();
            UserIdTxt = rc.Get<GameObject>("UserIdTxt").GetComponent<Text>();
            ExTxt = rc.Get<GameObject>("ExTxt").GetComponent<Text>();
            VipTimeTxt = rc.Get<GameObject>("VipTimeTxt").GetComponent<Text>();
            WinGameTxt = rc.Get<GameObject>("WinGameTxt").GetComponent<Text>();
            TatalGameTxt = rc.Get<GameObject>("TatalGameTxt").GetComponent<Text>();
            IsRealNameTxt = rc.Get<GameObject>("IsRealNameTxt").GetComponent<Text>();
            UserNameTxt = rc.Get<GameObject>("UserNameTxt").GetComponent<Text>();
            YuanbaoTxt = rc.Get<GameObject>("YuanbaoTxt").GetComponent<Text>();
            GoldNumTxt = rc.Get<GameObject>("GoldNumTxt").GetComponent<Text>();
            HuafeiTxt = rc.Get<GameObject>("HuafeiTxt").GetComponent<Text>();
            UserData = rc.Get<GameObject>("UserData");
            UserDataBtn = rc.Get<GameObject>("UserDataBtn").GetComponent<Button>();
            UserIdInputField = rc.Get<GameObject>("UserIdInputField").GetComponent<InputField>();
            SureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();
            LastOnLineTxt = rc.Get<GameObject>("LastOnLineTxt").GetComponent<Text>();
            PhoneTxt  = rc.Get<GameObject>("PhoneTxt").GetComponent<Text>();
            IpTxt = rc.Get<GameObject>("IpTxt").GetComponent<Text>();
            RegisterTimeTxt = rc.Get<GameObject>("RegisterTimeTxt").GetComponent<Text>();
            NameInputField  = rc.Get<GameObject>("NameInputField").GetComponent<InputField>();
            ChannelTxt = rc.Get<GameObject>("ChannelTxt").GetComponent<Text>();
            #endregion

            #region ForceOffline
            ForceOfflineBtn = rc.Get<GameObject>("ForceOfflineBtn").GetComponent<Button>();
            ForceUserBtn = rc.Get<GameObject>("ForceUserBtn").GetComponent<Button>();
            FReasonInputField = rc.Get<GameObject>("FReasonInputField").GetComponent<InputField>();
            ForceUIdInputField = rc.Get<GameObject>("ForceUIdInputField").GetComponent<InputField>();
            ForceOfflineCloseBtn = rc.Get<GameObject>("ForceOfflineCloseBtn").GetComponent<Button>();
            ForceOffline = rc.Get<GameObject>("ForceOffline");
            #endregion

            #region 修改用户信息
            ModifyBtn = rc.Get<GameObject>("ModifyBtn").GetComponent<Button>();
            Modify = rc.Get<GameObject>("Modify");
            ModifyCloseBtn = rc.Get<GameObject>("ModifyCloseBtn").GetComponent<Button>();
            ModifySureBtn = rc.Get<GameObject>("ModifySureBtn").GetComponent<Button>();
            RestChangeNameInputField = rc.Get<GameObject>("RestChangeNameInputField").GetComponent<InputField>();
            ModifyPropInputField = rc.Get<GameObject>("ModifyPropInputField").GetComponent<InputField>();
            ModifyIconInputField = rc.Get<GameObject>("ModifyIconInputField").GetComponent<InputField>();
            MoUIdInputField = rc.Get<GameObject>("MoUIdInputField").GetComponent<InputField>();
            #endregion

            #region Room
            RoomBtn = rc.Get<GameObject>("RoomBtn").GetComponent<Button>();
            CloseRoomBtn = rc.Get<GameObject>("CloseRoomBtn").GetComponent<Button>();
            RoomInfo = rc.Get<GameObject>("RoomInfo");
            RequestBtn = rc.Get<GameObject>("RequestBtn").GetComponent<Button>();
            NewRoomTxt = rc.Get<GameObject>("NewRoomTxt").GetComponent<Text>();
            NewInGameTxt = rc.Get<GameObject>("NewInGameTxt").GetComponent<Text>();
            JingRoomTxt = rc.Get<GameObject>("JingRoomTxt").GetComponent<Text>();
            JingInGameTxt = rc.Get<GameObject>("JingInGameTxt").GetComponent<Text>();
            #endregion

            Mail = rc.Get<GameObject>("Mail");
            BlackList = rc.Get<GameObject>("BlackList");
            ExelData = rc.Get<GameObject>("ExelData");

            //刷新所有配置表
            RefreshExelBtn.onClick.Add(() =>
            {
                RefreshExelOnClick();
            });

            //解散房间
            DissRoomBtn.onClick.Add(() =>
            {

            });

            //发送邮件
            SendMailBtn.onClick.Add(() =>
            {
                Mail.SetActive(true);
            });

            //增减黑名单
            OperateBlackListBtn.onClick.Add(() =>
            {
                BlackList.SetActive(true);
            });

            //生成报表
            GenerateDataBtn.onClick.Add(() =>
            {
                ExelData.SetActive(true);
            });

            
            Mail.transform.Find("SelToggle").GetComponent<Button>().onClick.Add(() =>
            {
                isOn = !isOn;
                if (isOn)
                {
                    //发送邮件给所有玩家
                    Mail.transform.Find("SelToggle/Background/Checkmark").gameObject.SetActive(true);
                }
                else
                {
                    //发送给单个玩家
                    Mail.transform.Find("SelToggle/Background/Checkmark").gameObject.SetActive(false);
                }
            });

            //发送邮件
            Mail.transform.Find("SendBtn").GetComponent<Button>().onClick.Add(() =>
            {
                SendMailOnClick();
            });

            //关闭邮件编辑界面
            MailBtn.GetComponent<Button>().onClick.Add(() =>
            {
                Mail.SetActive(false);
            });

            //关闭编辑增减黑名单界面
            BlackListBtn.GetComponent<Button>().onClick.Add(() =>
            {
                BlackList.SetActive(false);
            });

            //关闭编辑生成报表界面
            ExelDataBtn.GetComponent<Button>().onClick.Add(() =>
            {
                ExelData.SetActive(false);
            });

            //增加黑名单
            BlackList.transform.Find("AddBlackListBtn").GetComponent<Button>().onClick.Add(() =>
            {
                AddBlackListOnClick();
            });

            //删除黑名单
            BlackList.transform.Find("DelBlackListBtn").GetComponent<Button>().onClick.Add(() =>
            {
                DeleteBlackListOnClick();
            });

            //生成报表
            ExelData.transform.Find("GerateBtn").GetComponent<Button>().onClick.Add(() =>
            {
                CreateExelData();
            });

            //强制玩家离线
            ForceOfflineBtn.onClick.Add(() =>
            {
                ForceOffline.SetActive(true);
            });

            //发送消息强制玩家离线
            ForceUserBtn.onClick.Add(() =>
            {
                ForceOfflineOnClick();
            });

            //关闭强制玩家离线界面
            ForceOfflineCloseBtn.onClick.Add(() =>
            {
                ForceOffline.SetActive(false);
            });

            //显示用户信息
            ShowUserInfoBtn.onClick.Add(() =>
            {
                UserData.SetActive(true);
            });

            //关闭用户信息界面
            UserDataBtn.onClick.Add(() =>
            {
                UserData.SetActive(false);
            });

            //请求用户信息
            SureBtn.onClick.Add(() =>
            {
                GetUserInfo();
            });

            RoomBtn.onClick.Add(() =>
            {
                RoomInfo.SetActive(true);
            });

            CloseRoomBtn.onClick.Add(() =>
            {
                RoomInfo.SetActive(false);
            });

            RequestBtn.onClick.Add(() =>
            {
                //RequestRoomInfo();
            });

            ModifyBtn.onClick.Add(() =>
            {
                Modify.SetActive(true);
            });

            ModifyCloseBtn.onClick.Add(() =>
            {
                Modify.SetActive(false);
            });

            ModifySureBtn.onClick.Add(() =>
            {
                ModifyUserInfo();
            });

            HeartBeat.getInstance().startHeartBeat();

        }

        private async void ModifyUserInfo()
        {
            if (string.IsNullOrEmpty(MoUIdInputField.text))
            {
                ToastScript.createToast("UID不能为空");
                return;
            }
            if(string.IsNullOrEmpty(RestChangeNameInputField.text) || RestChangeNameInputField.text.Equals("0"))
            {
                RestChangeNameInputField.text = "0";
            }
            if (string.IsNullOrEmpty(ModifyIconInputField.text) || ModifyIconInputField.text.Equals("0"))
            {
                ModifyIconInputField.text = "0";
            }
            if (string.IsNullOrEmpty(ModifyPropInputField.text) || ModifyPropInputField.text.Equals("0"))
            {
                ModifyPropInputField.text = "0";
            }

            G2C_GM gm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
            {
                UId = long.Parse(MoUIdInputField.text),
                RestChangeNameCount = int.Parse(RestChangeNameInputField.text),
                Icon = ModifyIconInputField.text,
                Prop = ModifyPropInputField.text,
                Type = 8
            });

            if(gm.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(gm.Message);
                return;
            }
            ToastScript.createToast("修改成功");
        }

        private async void RequestRoomInfo()
        {
            G2C_GM gm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
            {
                Type = 9
            });

            if(gm.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(gm.Message);
                return;
            }

            SetRoomInfo(gm.Room);
        }

        private void SetRoomInfo(RoomInfo info)
        {
            NewRoomTxt.text = info.NewRoomCount.ToString();
            NewInGameTxt.text = info.NewTotalPlayerInGameCount.ToString();
            JingRoomTxt.text = info.JingRoomCount.ToString();
            JingInGameTxt.text = info.JingTotalPlayerInGameCount.ToString();
        }

        private async void ForceOfflineOnClick()
        {
            G2C_GM gm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
            {
                UId = long.Parse(ForceUIdInputField.text),
                Reason = FReasonInputField.text,
                Type = 7
            });
        }

        private async void GetUserInfo()
        {
            if (string.IsNullOrEmpty(UserIdInputField.text) && string.IsNullOrEmpty(NameInputField.text))
            {
                ToastScript.createToast("必须填写ID或者名称");
                return;
            }
            else if(!string.IsNullOrEmpty(UserIdInputField.text) && !string.IsNullOrEmpty(NameInputField.text))
            {
                ToastScript.createToast("请确定是根据ID还是名称来查询");
                return;
            }
            G2C_GM gm = null;
            if (!string.IsNullOrEmpty(UserIdInputField.text))
            {
                gm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
                {
                    UId = long.Parse(UserIdInputField.text),
                    Type = 6
                });
            }
            else if (!string.IsNullOrEmpty(NameInputField.text))
            {
                gm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
                {
                    Name = NameInputField.text,
                    Type = 6
                });
            }

            if(gm.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(gm.Message);
                return;
            }
            SetUserInfo(gm);
        }

        private void SetUserInfo(G2C_GM gm)
        {
            UserIdTxt.text = gm.UId.ToString();
            UserNameTxt.text = gm.Info.Name;
            GoldNumTxt.text = gm.Info.GoldNum.ToString();
            YuanbaoTxt.text = gm.Info.WingNum.ToString();
            HuafeiTxt.text = gm.Info.HuaFeiNum.ToString();
            IsRealNameTxt.text = gm.Info.IsRealName ? "是" : "否";
            TatalGameTxt.text = gm.Info.TotalGameCount.ToString();
            WinGameTxt.text = gm.Info.WinGameCount.ToString();
            VipTimeTxt.text = gm.Info.VipTime.ToString();
            ExTxt.text = gm.Info.EmogiTime.ToString();
            MaxhuaTxt.text = gm.Info.MaxHua.ToString();
            if (gm.LastOnlineTime != null)
            {
                LastOnLineTxt.text = gm.LastOnlineTime.ToString();
            }
            if (gm.RegisterTime != null)
            {
                RegisterTimeTxt.text = gm.RegisterTime.ToString();
            }
            IpTxt.text = gm.Ip.ToString();
            if(gm.Info.Phone != null)
            {
                PhoneTxt.text = gm.Info.Phone.ToString();
            }
        }

        //刷新所有配置表
        private async void RefreshExelOnClick()
        {
            //其他在服务器刷新
            G2C_GM g2cGm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
            {
                Type = 1
            });
        }

        //发送邮件
        private async void SendMailOnClick()
        {
            if (isOn)
            {
                if (string.IsNullOrEmpty(RewardInputField.text))
                {
                    ToastScript.createToast("奖励不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(UIdInputField.text))
                {
                    UIdInputField.text = "0";
                }
            }

            G2C_GM g2cGm = (G2C_GM)await SessionWrapComponent.Instance.Session.Call(SendGM(long.Parse(UIdInputField.text)));
            if (isOn)
            {
                ToastScript.createToast("群发邮件");
            }
            else
            {
                ToastScript.createToast("发送邮件");
            }
        }

        private C2G_GM SendGM(long uid)
        {
            return new C2G_GM
            {
                UId = uid,
                Type = 2,
                Title = TitleInputField.text,
                Content = ContentInputField.text,
                Reward = RewardInputField.text
            };
        }

        //增加黑名单
        private async void AddBlackListOnClick()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GM g2cGm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
            {
                UId = long.Parse(BUIdInputField.text),
                Type = 4,
                IP = IPInputField.text,
                EndTime = EndTimeInputField.text,
                Reason = ReasonInputField.text
            });
            UINetLoadingComponent.closeNetLoading();
            ToastScript.createToast("增加黑名单");
        }

        //删除黑名单
        private async void DeleteBlackListOnClick()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GM g2cGm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
            {
                UId = long.Parse(BUIdInputField.text),
                Type = 4,
                IP = IPInputField.text,
                EndTime = ""
            });
            UINetLoadingComponent.closeNetLoading();
            ToastScript.createToast("删除黑名单");
        }

        //解散房间
        private void DissRoomOnClick()
        {

        }

        //生成报表
        private async void CreateExelData()
        {
            G2C_GM g2cGm = (G2C_GM)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GM
            {
                CreateBaobiaoTime = CreateTimeInputField.text,
                Type = 5
            });
        }
    }
}
