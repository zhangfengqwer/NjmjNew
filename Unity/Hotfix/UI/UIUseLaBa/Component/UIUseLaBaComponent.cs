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
	public class UiUseLaBaComponentSystem : AwakeSystem<UIUseLaBaComponent>
	{
		public override void Awake(UIUseLaBaComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIUseLaBaComponent : Component
	{
        private InputField InputField_content;

        private Button Button_OK;
        private Button Button_close;

        public void Awake()
		{
            ToastScript.clear();

            initData();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            InputField_content = rc.Get<GameObject>("InputField_content").GetComponent<InputField>();

            Button_OK.onClick.Add(onClickOK);
            Button_close.onClick.Add(onClickClose);
        }
        
        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseLaBa);
        }

        public async void onClickOK()
        {
            if (InputField_content.text.CompareTo("javdebug") == 0)
            {
                await RequestUpdateServer();
                return;
            }

            if (GameUtil.GetDataCount(105) > 0)
            {
                if (InputField_content.text.CompareTo("") == 0)
                {
                    ToastScript.createToast("请输入内容");
                    return;
                }

                if (InputField_content.text.Length > 20)
                {
                    ToastScript.createToast("您发送的内容大于20个字");
                    return;
                }

                UINetLoadingComponent.showNetLoading();
                if (SensitiveWordUtil.IsSensitiveWord(InputField_content.text))
                {
                    ToastScript.createToast("您输入的内容包含敏感词");
                    UINetLoadingComponent.closeNetLoading();
                    return;
                }
                UINetLoadingComponent.closeNetLoading();

                RequestUseLaBa();
            }
            else
            {
                ToastScript.createToast("您没有喇叭可以使用");
                return;
            }
        }

        private async void RequestUseLaBa()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_UseLaBa g2cUseLaBa = (G2C_UseLaBa)await SessionWrapComponent.Instance.Session.Call(new C2G_UseLaBa { Uid = PlayerInfoComponent.Instance.uid , Content = InputField_content.text});
            UINetLoadingComponent.closeNetLoading();

            if (g2cUseLaBa.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUseLaBa.Message);

                return;
            }

            ToastScript.createToast("发送成功");

            GameUtil.changeData(105,-1);

            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseLaBa);

            if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIBag) != null)
            {
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIBag).GetComponent<UIBagComponent>() != null)
                {
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIBag).GetComponent<UIBagComponent>().RefreshUI();
                }
            }
        }

        private async Task RequestUpdateServer()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_UpdateServer g2cUpdateServer = (G2C_UpdateServer)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdateServer {});
            UINetLoadingComponent.closeNetLoading();

            if (g2cUpdateServer.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUpdateServer.Message);

                return;
            }

            ToastScript.createToast("更新成功");
        }
    }
}
