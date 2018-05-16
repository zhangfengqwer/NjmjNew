using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIBagSystem : AwakeSystem<UIBagComponent>
    {
        public override void Awake(UIBagComponent self)
        {
            self.Awake();
        }
    }

    public class UIBagComponent : Component
    {
        /*UseBtn DescTxt UIItemIcon Grid BgGrid ReturnBtn
         */
        private Button useBtn;
        private Text descTxt;
        private Image uiItemIcon;
        private GameObject grid;
        private GameObject bgGrid;
        private Button returnBtn;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            useBtn = rc.Get<GameObject>("UseBtn").GetComponent<Button>();
            descTxt = rc.Get<GameObject>("DescTxt").GetComponent<Text>();
            uiItemIcon = rc.Get<GameObject>("UIItemIcon").GetComponent<Image>();
            grid = rc.Get<GameObject>("Grid");
            bgGrid = rc.Get<GameObject>("BgGrid");
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
        }
    }
}
