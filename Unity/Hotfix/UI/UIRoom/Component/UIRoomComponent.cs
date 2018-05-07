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
	    private GameObject bottom;
	    private GameObject left;
	    private GameObject top;
	    private GameObject right;

	    public void Awake()
	    {
	        ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

	        this.bottom = rc.Get<GameObject>("Bottom");
	        this.right = rc.Get<GameObject>("Right");
	        this.top = rc.Get<GameObject>("Top");
	        this.left = rc.Get<GameObject>("Left");

	        this.changeTableBtn = rc.Get<GameObject>("ChangeTableBtn").GetComponent<Button>();
	        this.changeTableBtn.onClick.Add(OnChangeTable);
	    }

	    private async void OnChangeTable()
	    {

	    }
	}
}
