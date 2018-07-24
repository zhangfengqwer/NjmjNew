using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;
using Unity_Utils;
using LitJson;

namespace ETHotfix
{
    [ObjectSystem]
    public class UiLoginComponentSystem : AwakeSystem<UILoginComponent>
    {
        public override void Awake(UILoginComponent self)
        {
            self.Awake();
        }
    }

    public class ThirdLoginData
    {
        public string channel_name = "";
        public string nick_name = "";
        public string response = "";
        public string third_id = "";
        public string otherStr1 = "";
        public string otherStr2 = "";
        public int otherInt1 = 0;
        public int otherInt2 = 0;
    }

    public class UILoginComponent : Component
    {
        private GameObject panel_start;
        private GameObject panel_phoneLogin;

        private InputField inputField_Phone;
        private InputField inputField_YanZhengMa;

        private Button btn_phone;
        private Button btn_wechat;
        private Button btn_guest;
        private Button btn_login;
        private Button btn_yanzhengma;
        private Button btn_backToStart;

        private Text text_yanzhengmadaojishi;

        bool isLoginSuccess = false;
        private Button btn_third;
        public static UILoginComponent Instance { get; set; }
        private bool isLogining;

        public async void Awake()
        {
            // 获取配置文件
            {
                string fileName = "otherConfig-" + PlatformHelper.GetVersionName() + ".json";
                await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/" + fileName, OtherConfig.getInstance().init);
            }

            ToastScript.clear();
            Instance = this;
            initData();
            CommonUtil.SetTextFont(panel_start.transform.parent.gameObject);
        }
     
        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            panel_start = rc.Get<GameObject>("Start");
            panel_phoneLogin = rc.Get<GameObject>("PhoneLogin");

            btn_phone = rc.Get<GameObject>("Button_phone").GetComponent<Button>();
            btn_wechat = rc.Get<GameObject>("Button_wechat").GetComponent<Button>();
            btn_guest = rc.Get<GameObject>("Button_guest").GetComponent<Button>();
            btn_login = rc.Get<GameObject>("Button_Login").GetComponent<Button>();
            btn_third = rc.Get<GameObject>("Button_Third").GetComponent<Button>();
            btn_yanzhengma = rc.Get<GameObject>("Button_YanZhengMa").GetComponent<Button>();
            btn_backToStart = rc.Get<GameObject>("Button_back").GetComponent<Button>();

            inputField_Phone = rc.Get<GameObject>("InputField_Phone").GetComponent<InputField>();
            inputField_YanZhengMa = rc.Get<GameObject>("InputField_YanZhengMa").GetComponent<InputField>();

            text_yanzhengmadaojishi = rc.Get<GameObject>("Text_yanzhengmadaojishi").GetComponent<Text>();

            btn_phone.onClick.Add(onClickOpenPhoneLogin);
            btn_wechat.onClick.Add(onClickWechatLogin);
            btn_third.onClick.Add(onClickWechatLogin);
            btn_guest.onClick.Add(onClickGuestLogin);
            btn_login.onClick.Add(onClickPhoneCodeLogin);
            btn_yanzhengma.onClick.Add(onClickGetPhoneCode);
            btn_backToStart.onClick.Add(onClickBackStart);

            // 四个UI层级画布
            {
                OtherData.s_loginCanvas = panel_start.transform.parent.parent.parent.Find("LoginCanvas").gameObject;
                OtherData.s_mainCanvas = panel_start.transform.parent.parent.parent.Find("MainCanvas").gameObject;
                OtherData.s_roomCanvas = panel_start.transform.parent.parent.parent.Find("RoomCanvas").gameObject;
                OtherData.s_commonCanvas = panel_start.transform.parent.parent.parent.Find("CommonCanvas").gameObject;
            }

            #region 登录按钮设置
            {
                // 测试服开启游客登录按钮
                if (!NetConfig.getInstance().isFormal)
                {
                    btn_guest.transform.localScale = new Vector3(1, 1, 1);
                }

                if (ChannelHelper.IsThirdChannel() && PlatformHelper.IsThirdLogin())
                {
                    btn_third.transform.localScale = new Vector3(1, 1, 1);
                    btn_third.GetComponentInChildren<Text>().text = ChannelHelper.GetChannelAllName() + "登录";
                }
                else
                {
                    btn_phone.transform.localScale = new Vector3(1, 1, 1);
                    btn_wechat.transform.localScale = new Vector3(1, 1, 1);

                    if (OtherData.getIsShiedPhoneLogin())
                    {
                        btn_phone.transform.localScale = Vector3.zero;
                    }
                    else if (OtherData.getIsShiedWeChatLogin())
                    {
                        btn_wechat.transform.localScale = Vector3.zero;
                    }
                }
            }
            #endregion
        }

        public void onClickDebugAccount1()
        {
            OnLoginPhone("1", "", "1");
        }

        public void onClickDebugAccount2()
        {
            OnLoginPhone("2", "", "2");
        }
        
        public void onClickDebugAccount3()
        {
            OnLoginPhone("3", "", "3");
        }
        
        public void onClickDebugAccount4()
        {
            OnLoginPhone("4", "", "4");
        }

        public async void onClickOpenPhoneLogin()
        {
            string phone = PlayerPrefs.GetString("Phone", "");
            string token = PlayerPrefs.GetString("Token", "");

            if (false)
            {
                phone = "";
                token = "";
            }

            if ((phone.CompareTo("") != 0) && (token.CompareTo("") != 0))
            {
                await OnLoginPhone(phone, "", token);
            }
            else
            {
                panel_phoneLogin.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public async void onClickPhoneCodeLogin()
        {
            if (inputField_Phone.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入手机号");
                return;
            }

            if (inputField_YanZhengMa.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入验证码");
                return;
            }

            if (!VerifyRuleUtil.CheckPhone(inputField_Phone.text))
            {
                ToastScript.createToast("请输入正确的手机号");
                return;
            }

            await OnLoginPhone(inputField_Phone.text, inputField_YanZhengMa.text, "");
        }

        public async void onClickWechatLogin()
        {
            if (false)
            {
                string Third_Id = CommonUtil.getCurTime();
            }
            else
            {
                PlatformHelper.Login("AndroidCallBack", "GetLoginResult", "weixin");
            }
        }

        public async void onClickGuestLogin()
        {
            string Third_Id = CommonUtil.getCurTime();
            await OnThirdLogin(PlatformHelper.GetMacId(), "", "");
        }

        public async void onThirdLoginCallback(ThirdLoginData thirdLoginData)
        {
            try
            {
                // 官方包
                if (!ChannelHelper.IsThirdChannel() || "vivo".Equals(PlatformHelper.GetChannelName()))
                {
                    Log.Info("vivo包");
                    JsonData jd = JsonMapper.ToObject(thirdLoginData.response);
                    string name = (string)jd["nick"];
                    string head = (string)jd["url"];

                    await OnThirdLogin(thirdLoginData.third_id, name, head);
                }
                else
                {
                    await OnThirdLogin(thirdLoginData.third_id, thirdLoginData.nick_name, "");
                    //
                    //                switch (thirdLoginData.channel_name)
                    //                {
                    //                    // 官方包
                    //                    case "":
                    //                        {
                    //
                    //                        }
                    //                        break;
                    //                }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void onClickBackStart()
        {
            panel_phoneLogin.transform.localScale = Vector3.zero;
        }

        public async void onClickGetPhoneCode()
        {
            if (inputField_Phone.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入手机号");
                return;
            }

            if (!VerifyRuleUtil.CheckPhone(inputField_Phone.text))
            {
                ToastScript.createToast("请输入正确的手机号");
                return;
            }

            btn_yanzhengma.transform.localScale = Vector3.zero;
            text_yanzhengmadaojishi.transform.localScale = new Vector3(1, 1, 1);

            UINetLoadingComponent.showNetLoading();

            Session sessionWrap = null;
            try
            {

                //IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
                IPEndPoint connetEndPoint = ToIPEndPointWithYuMing(NetConfig.getInstance().getServerPort());
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);

                sessionWrap = ComponentFactory.Create<Session, ETModel.Session>(session);

                R2C_SendSms r2CData = (R2C_SendSms)await sessionWrap.Call(new C2R_SendSms() { Phone = inputField_Phone.text });

                UINetLoadingComponent.closeNetLoading();

                if (r2CData.Error != ErrorCode.ERR_Success)
                {
                    ToastScript.createToast(r2CData.Message);
                }

                sessionWrap.Dispose();

                startPhoneCodeTimer();
            }
            catch (Exception e)
            {
                sessionWrap?.Dispose();
                Log.Error(e);
            }
        }

        public async void startPhoneCodeTimer()
        {
            int time = 60;

            text_yanzhengmadaojishi.text = time.ToString();

            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                --time;

                if (isLoginSuccess)
                {
                    return;
                }

                text_yanzhengmadaojishi.text = time.ToString();
            }

            text_yanzhengmadaojishi.transform.localScale = Vector3.zero;
            btn_yanzhengma.transform.localScale = new Vector3(1, 1, 1);
        }

        public async Task OnLoginPhone(string phone, string code, string token)
        {

            Session sessionWrap = null;
         
            try
            {
                UINetLoadingComponent.showNetLoading();

                //IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

                IPEndPoint connetEndPoint = ToIPEndPointWithYuMing(NetConfig.getInstance().getServerPort());
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_PhoneLogin r2CLogin = (R2C_PhoneLogin)await sessionWrap.Call(new C2R_PhoneLogin() { Phone = phone, Code = code, Token = token, MachineId = PlatformHelper.GetMacId(), ChannelName = PlatformHelper.GetChannelName(), ClientVersion = PlatformHelper.GetVersionName() });
                //R2C_PhoneLogin r2CLogin = (R2C_PhoneLogin)await sessionWrap.Call(new C2R_PhoneLogin() { Phone = phone, Code = code, Token = token, MachineId = "1234", ChannelName = "jav", ClientVersion = "1.1.0" });
                sessionWrap.Dispose();

                UINetLoadingComponent.closeNetLoading();

                if (r2CLogin.Error != ErrorCode.ERR_Success)
                {
                    ToastScript.createToast(r2CLogin.Message);

                    if (r2CLogin.Error == ErrorCode.ERR_TokenError)
                    {
                        PlayerPrefs.SetString("Phone", "");
                        PlayerPrefs.SetString("Token", "");

                        panel_phoneLogin.transform.localScale = new Vector3(1, 1, 1);
                    }

                    if (r2CLogin.Message.CompareTo("用户不存在") == 0)
                    {
                        panel_phoneLogin.transform.localScale = new Vector3(1, 1, 1);
                    }

                    return;
                }

                UINetLoadingComponent.showNetLoading();


                //connetEndPoint = NetworkHelper.ToIPEndPoint(r2CLogin.Address);
                //connetEndPoint = NetConfig.getInstance().ToIPEndPointWithYuMing();
                string[] temp = r2CLogin.Address.Split(':');
                connetEndPoint = ToIPEndPointWithYuMing(Convert.ToInt32(temp[1]));
                ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                Game.Scene.GetComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(session);
                ETModel.Game.Scene.GetComponent<ETModel.SessionComponent>().Session = gateSession;

                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });
                UINetLoadingComponent.closeNetLoading();

                ToastScript.createToast("登录成功");
                //挂心跳包
                Game.Scene.GetComponent<SessionComponent>().Session.AddComponent<HeartBeatComponent>();
                UINetLoadingComponent.closeNetLoading();
                await getAllData();

                isLoginSuccess = true;

                {
                    // 保存Phone
                    PlayerPrefs.SetString("Phone", phone);

                    // 保存Token
                    PlayerPrefs.SetString("Token", r2CLogin.Token);
                }

                Game.Scene.GetComponent<PlayerInfoComponent>().uid = g2CLoginGate.Uid;

                PlayerInfoComponent.Instance.SetShopInfoList(g2CLoginGate.ShopInfoList);
                PlayerInfoComponent.Instance.SetBagInfoList(g2CLoginGate.BagList);
                PlayerInfoComponent.Instance.ownIcon = g2CLoginGate.ownIcon;

                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                await resourcesComponent.LoadBundleAsync($"UIMain.unity3d");
                await resourcesComponent.LoadBundleAsync($"Image_Main.unity3d");

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIMain);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
                
            }
            catch (Exception e)
            {
                sessionWrap?.Dispose();
                Log.Error(e);
            }
        }

        public async Task OnThirdLogin(string third_id,string name,string head)
        {
            //加锁
            if (isLogining)
            {
                return;
            }
            isLogining = true;

            Session sessionWrap = null;
            try
            {
                UINetLoadingComponent.showNetLoading();

                // IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
                IPEndPoint connetEndPoint = NetConfig.getInstance().ToIPEndPointWithYuMing();
#if Localhost
                connetEndPoint = NetworkHelper.ToIPEndPoint("10.224.5.74:10002");
#endif
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = ComponentFactory.Create<Session, ETModel.Session>(session);

                name = name.Replace("'","*");
                R2C_ThirdLogin r2CLogin = (R2C_ThirdLogin)await sessionWrap.Call(new C2R_ThirdLogin() { Third_Id = third_id, MachineId = PlatformHelper.GetMacId(), ChannelName = PlatformHelper.GetChannelName(), ClientVersion = PlatformHelper.GetVersionName(),Name = name,Head = head});

                sessionWrap.Dispose();

                UINetLoadingComponent.closeNetLoading();

                if (r2CLogin.Error != ErrorCode.ERR_Success)
                {
                    ToastScript.createToast(r2CLogin.Message);
                    return;
                }
               
                UINetLoadingComponent.showNetLoading();

               // connetEndPoint = NetworkHelper.ToIPEndPoint(r2CLogin.Address);
//                connetEndPoint = NetConfig.getInstance().ToIPEndPointWithYuMing();
                string[] temp = r2CLogin.Address.Split(':');
                connetEndPoint = ToIPEndPointWithYuMing(Convert.ToInt32(temp[1]));

                // connetEndPoint = NetworkHelper.ToIPEndPoint(r2CLogin.Address);

#if Localhost
                connetEndPoint = NetworkHelper.ToIPEndPoint("10.224.5.74:10002");
#endif

                ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                Game.Scene.GetComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(session);
                ETModel.Game.Scene.GetComponent<ETModel.SessionComponent>().Session = gateSession;

                // Log.Info("gateSession:"+ connetEndPoint.ToString());
                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

                ToastScript.createToast("登录成功");
                //挂心跳包
                Game.Scene.GetComponent<SessionComponent>().Session.AddComponent<HeartBeatComponent>();
                UINetLoadingComponent.closeNetLoading();

                await getAllData();

                isLoginSuccess = true;

                Game.Scene.GetComponent<PlayerInfoComponent>().uid = g2CLoginGate.Uid;

                PlayerInfoComponent.Instance.SetShopInfoList(g2CLoginGate.ShopInfoList);
                PlayerInfoComponent.Instance.SetBagInfoList(g2CLoginGate.BagList);
                PlayerInfoComponent.Instance.ownIcon = g2CLoginGate.ownIcon;

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIMain);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
                isLogining = false;
            }
            catch (Exception e)
            {
                sessionWrap?.Dispose();
                Log.Error("OnThirdLogin:" + e);
                isLogining = false;
            }
        }

        public async Task getAllData()
        {
            UINetLoadingComponent.showNetLoading();

            try
            {
                await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/tips.json", TipsConfig.getInstance().init);
                await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/prop.json", PropConfig.getInstance().init);
                await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/zhuanpan.json", ZhuanPanConfig.getInstance().init);
                await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/notice.json", NoticeConfig.getInstance().init);
                //await SensitiveWordUtil.Req("http://fwdown.hy51v.com/online/file/stopwords.txt");

                string data = CommonUtil.getTextFileByBundle("config", "stopwords");
                SensitiveWordUtil.WordsDatas = data.Split(',');
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            UINetLoadingComponent.closeNetLoading();
        }

        public static IPEndPoint ToIPEndPointWithYuMing(int port)
        {
            string serverUrl = NetConfig.getInstance().getServerUrl();
          
            IPAddress ip;
            IPHostEntry IPinfo = Dns.GetHostEntry(serverUrl);
            if (IPinfo.AddressList.Length <= 0)
            {
                ToastScript.createToast("域名解析出错");
                return null;
            }
            ip = IPinfo.AddressList[0];
            return new IPEndPoint(ip, port);
        }
    }
}
