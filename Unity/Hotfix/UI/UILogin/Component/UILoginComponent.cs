using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ETModel;
using Hotfix;
using Unity_Utils;
using UnityEngine;
using UnityEngine.UI;

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

    public class UILoginComponent : Component
    {
        private GameObject panel_start;
        private GameObject panel_phoneLogin;
        private GameObject DebugAccount;

        private InputField inputField_Phone;
        private InputField inputField_YanZhengMa;

        private Button btn_phone;
        private Button btn_wechat;
        private Button btn_login;
        private Button btn_yanzhengma;
        private Button btn_backToStart;

        private Text text_yanzhengmadaojishi;

        bool isLoginSuccess = false;

        public void Awake()
        {
            ToastScript.clear();

            initData();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            panel_start = rc.Get<GameObject>("Start");
            panel_phoneLogin = rc.Get<GameObject>("PhoneLogin");

            btn_phone = rc.Get<GameObject>("Button_phone").GetComponent<Button>();
            btn_wechat = rc.Get<GameObject>("Button_wechat").GetComponent<Button>();
            btn_login = rc.Get<GameObject>("Button_Login").GetComponent<Button>();
            btn_yanzhengma = rc.Get<GameObject>("Button_YanZhengMa").GetComponent<Button>();
            btn_backToStart = rc.Get<GameObject>("Button_back").GetComponent<Button>();

            inputField_Phone = rc.Get<GameObject>("InputField_Phone").GetComponent<InputField>();
            inputField_YanZhengMa = rc.Get<GameObject>("InputField_YanZhengMa").GetComponent<InputField>();

            text_yanzhengmadaojishi = rc.Get<GameObject>("Text_yanzhengmadaojishi").GetComponent<Text>();

            btn_phone.onClick.Add(onClickOpenPhoneLogin);
            btn_wechat.onClick.Add(onClickWechatLogin);
            btn_login.onClick.Add(onClickPhoneCodeLogin);
            btn_yanzhengma.onClick.Add(onClickGetPhoneCode);
            btn_backToStart.onClick.Add(onClickBackStart);

            {
                // 隐藏手机登录界面
                panel_phoneLogin.transform.localScale = Vector3.zero;
            }

            {
                DebugAccount = rc.Get<GameObject>("DebugAccount");
                DebugAccount.transform.Find("Button_player1").GetComponent<Button>().onClick.Add(onClickDebugAccount1);
                DebugAccount.transform.Find("Button_player2").GetComponent<Button>().onClick.Add(onClickDebugAccount2);
                DebugAccount.transform.Find("Button_player3").GetComponent<Button>().onClick.Add(onClickDebugAccount3);
                DebugAccount.transform.Find("Button_player4").GetComponent<Button>().onClick.Add(onClickDebugAccount4);
            }
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

            if (true)
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
            string Third_Id = CommonUtil.getCurTime();
            await OnThirdLogin("hp");
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

            SessionWrap sessionWrap = null;
            try
            {
                IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

                Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = new SessionWrap(session);
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
            SessionWrap sessionWrap = null;
            try
            {
                UINetLoadingComponent.showNetLoading();

                IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

                Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = new SessionWrap(session);
                R2C_PhoneLogin r2CLogin = (R2C_PhoneLogin)await sessionWrap.Call(new C2R_PhoneLogin() { Phone = phone, Code = code, Token = token, MachineId = PlatformHelper.GetMacId(), ChannelName = PlatformHelper.GetChannelName(), ClientVersion = PlatformHelper.GetVersionName() });
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
                    return;
                }

                UINetLoadingComponent.showNetLoading();

                connetEndPoint = NetworkHelper.ToIPEndPoint(r2CLogin.Address);
                Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                Game.Scene.GetComponent<SessionWrapComponent>().Session = new SessionWrap(gateSession);
                ETModel.Game.Scene.GetComponent<SessionComponent>().Session = gateSession;

                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionWrapComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });
                UINetLoadingComponent.closeNetLoading();

                ToastScript.createToast("登录成功");
                UINetLoadingComponent.closeNetLoading();
                getAllData();

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
                PlayerInfoComponent.Instance.SetChatList(g2CLoginGate.ChatList);

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIMain);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
            }
            catch (Exception e)
            {
                sessionWrap?.Dispose();
                Log.Error(e);
            }
        }

        public async Task OnThirdLogin(string third_id)
        {
            SessionWrap sessionWrap = null;
            try
            {
                UINetLoadingComponent.showNetLoading();

                IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

                Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = new SessionWrap(session);
                R2C_ThirdLogin r2CLogin = (R2C_ThirdLogin)await sessionWrap.Call(new C2R_ThirdLogin() { Third_Id = third_id, MachineId = PlatformHelper.GetMacId(), ChannelName = PlatformHelper.GetChannelName(), ClientVersion = PlatformHelper.GetVersionName() });
                sessionWrap.Dispose();

                UINetLoadingComponent.closeNetLoading();

                if (r2CLogin.Error != ErrorCode.ERR_Success)
                {
                    ToastScript.createToast(r2CLogin.Message);

                    return;
                }

                UINetLoadingComponent.showNetLoading();

                connetEndPoint = NetworkHelper.ToIPEndPoint(r2CLogin.Address);
                Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                Game.Scene.GetComponent<SessionWrapComponent>().Session = new SessionWrap(gateSession);
                ETModel.Game.Scene.GetComponent<SessionComponent>().Session = gateSession;

                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionWrapComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

                ToastScript.createToast("登录成功");

                UINetLoadingComponent.closeNetLoading();

                getAllData();

                isLoginSuccess = true;

                Game.Scene.GetComponent<PlayerInfoComponent>().uid = g2CLoginGate.Uid;

                PlayerInfoComponent.Instance.SetShopInfoList(g2CLoginGate.ShopInfoList);
                PlayerInfoComponent.Instance.SetBagInfoList(g2CLoginGate.BagList);
                PlayerInfoComponent.Instance.SetChatList(g2CLoginGate.ChatList);

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIMain);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
            }
            catch (Exception e)
            {
                sessionWrap?.Dispose();
                Log.Error("OnThirdLogin:" + e);
            }
        }

        public void getAllData()
        {
            UINetLoadingComponent.showNetLoading();

            HttpReqUtil.Req("http://fwdown.hy51v.com/njmj/online/files/prop.json", PropConfig.getInstance().init);
            HttpReqUtil.Req("http://fwdown.hy51v.com/njmj/online/files/zhuanpan.json", ZhuanPanConfig.getInstance().init);
            HttpReqUtil.Req("http://fwdown.hy51v.com/njmj/online/files/activity.json", ActivityConfig.getInstance().init);
            HttpReqUtil.Req("http://fwdown.hy51v.com/njmj/online/files/notice.json", NoticeConfig.getInstance().init);
            //HttpReqUtil.Req("http://fwdown.hy51v.com/online/file/stopwords.txt", SensitiveWordUtil.InitWords);

            UINetLoadingComponent.closeNetLoading();
        }
    }
}
