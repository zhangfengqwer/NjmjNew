using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIGMLoginSystem : StartSystem<UIGMLoginComponent>
    {
        public override void Start(UIGMLoginComponent self)
        {
            self.Start();
        }
    }

    public class UIGMLoginComponent : Component
    {
        private Button LoginBtn;
        private InputField UserInputField;
        private InputField PwdInputField;
        private Button SelToggle;
        private bool isOn = false;

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            LoginBtn = rc.Get<GameObject>("LoginBtn").GetComponent<Button>();
            UserInputField = rc.Get<GameObject>("UserInputField").GetComponent<InputField>();
            PwdInputField = rc.Get<GameObject>("PwdInputField").GetComponent<InputField>();
            SelToggle = rc.Get<GameObject>("SelToggle").GetComponent<Button>();

            LoginBtn.onClick.Add(() =>
            {
                LoginClick();
            });

            SelToggle.onClick.Add(() =>
            {
                isOn = !isOn;
                if (isOn)
                {
                    //连接线上服务器
                    NetConfig.getInstance().isFormal = true;
                    SelToggle.transform.Find("Background/Checkmark").gameObject.SetActive(true);
                }
                else
                {
                    //连接测试服务器
                    NetConfig.getInstance().isFormal = false;
                    SelToggle.transform.Find("Background/Checkmark").gameObject.SetActive(false);
                }

                PlatformHelper.SetIsFormal(NetConfig.getInstance().isFormal ? "0" : "1");
            });
        }

        private async void LoginClick()
        {
            if (string.IsNullOrEmpty(UserInputField.text) || string.IsNullOrEmpty(PwdInputField.text))
            {
                ToastScript.createToast("用户名或密码不能为空");
                return;
            }

            string Third_Id = CommonUtil.getCurTime();
            await OnLogin(PlatformHelper.GetMacId(), "", "");
        }

        public async Task OnLogin(string third_id, string name, string head)
        {
            SessionWrap sessionWrap = null;
            try
            {
                UINetLoadingComponent.showNetLoading();


                //IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
                IPEndPoint connetEndPoint = NetConfig.getInstance().ToIPEndPointWithYuMing();

                Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = new SessionWrap(session);
                R2C_ThirdLogin r2CLogin = (R2C_ThirdLogin)await sessionWrap.Call(new C2R_ThirdLogin() { Third_Id = third_id, MachineId = PlatformHelper.GetMacId(), ChannelName = PlatformHelper.GetChannelName(), ClientVersion = PlatformHelper.GetVersionName(), Name = name, Head = head });
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
                connetEndPoint = this.ToIPEndPointWithYuMing(Convert.ToInt32(temp[1]));
                Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                Game.Scene.GetComponent<SessionWrapComponent>().Session = new SessionWrap(gateSession);
                ETModel.Game.Scene.GetComponent<SessionComponent>().Session = gateSession;

                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionWrapComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

                ToastScript.createToast("登录成功");
                //挂心跳包
                Game.Scene.GetComponent<SessionWrapComponent>().Session.AddComponent<HeartBeatComponent>();
                UINetLoadingComponent.closeNetLoading();

                Game.Scene.GetComponent<PlayerInfoComponent>().uid = g2CLoginGate.Uid;

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIGM);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGMLogin);
            }
            catch (Exception e)
            {
                sessionWrap?.Dispose();
                Log.Error("OnThirdLogin:" + e);
            }
        }

        public IPEndPoint ToIPEndPointWithYuMing(int port)
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
