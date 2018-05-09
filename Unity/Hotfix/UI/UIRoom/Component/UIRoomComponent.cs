using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UIRoomComponentSystem : AwakeSystem<UIRoomComponent>
	{
		public override void Awake(UIRoomComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIRoomComponent: Component
	{
	    private Button changeTableBtn;
	    private Button exitBtn;

        public readonly GameObject[] GamersPanel = new GameObject[4];

	    public void Awake()
	    {
	        ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

	        GamersPanel[0] = rc.Get<GameObject>("Bottom");
	        GamersPanel[1] = rc.Get<GameObject>("Right");
	        GamersPanel[2] = rc.Get<GameObject>("Top");
	        GamersPanel[3] = rc.Get<GameObject>("Left");

	        this.changeTableBtn = rc.Get<GameObject>("ChangeTableBtn").GetComponent<Button>();
	        this.exitBtn = rc.Get<GameObject>("ExitBtn").GetComponent<Button>();

	        this.changeTableBtn.onClick.Add(OnChangeTable);
	        this.exitBtn.onClick.Add(OnExit);
	    }

	    private async void OnExit()
	    {
            M2C_ActorGamerExitRoom exitRoom = (M2C_ActorGamerExitRoom)await SessionWrapComponent.Instance.Session.Call(
                new C2M_ActorGamerExitRoom());
	     
	    }

	    private async void OnChangeTable()
	    {

	    }

	    /// <summary>
	    /// 添加玩家
	    /// </summary>
	    /// <param name="gamer"></param>
	    /// <param name="index"></param>
	    public void AddGamer(Gamer gamer, int index)
	    {
	        GetParent<UI>().GetComponent<GamerComponent>().Add(gamer, index);
	        gamer.GetComponent<GamerUIComponent>().SetPanel(this.GamersPanel[index]);
	    }

	    /// <summary>
	    /// 移除玩家
	    /// </summary>
	    /// <param name="id"></param>
	    public void RemoveGamer(long id)
	    {
	        Gamer gamer = GetParent<UI>().GetComponent<GamerComponent>().Remove(id);
	        gamer.GetComponent<GamerUIComponent>().Panel.SetActive(false);
            gamer.Dispose();
	    }
    }
}
