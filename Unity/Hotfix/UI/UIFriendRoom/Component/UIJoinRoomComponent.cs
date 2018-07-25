using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIJoinRoomSystem : StartSystem<UIJoinRoomComponent>
    {
        public override void Start(UIJoinRoomComponent self)
        {
            self.Start();
        }
    }

    public class UIJoinRoomComponent : Component
    {
        private GameObject JoinRoom;
        private Button CloseJoinRoom;
        private Button ANewBtn;
        private Button DeleteBtn;
        private GameObject InputCountGrid;
        private Text EnterTxt;

        StringBuilder builder = new StringBuilder();
        private List<Button> clickBtns = new List<Button>();
        private string curEnterValue = "";
        string space = "    ";

        private static long curRoomId = 0;

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            JoinRoom = rc.Get<GameObject>("JoinRoom");
            CloseJoinRoom = rc.Get<GameObject>("CloseJoinRoom").GetComponent<Button>();
            ANewBtn = rc.Get<GameObject>("ANewBtn").GetComponent<Button>();
            DeleteBtn = rc.Get<GameObject>("DeleteBtn").GetComponent<Button>();
            InputCountGrid = rc.Get<GameObject>("InputCountGrid");
            EnterTxt = rc.Get<GameObject>("EnterTxt").GetComponent<Text>();

            curRoomId = 0;
            for (int i = 0; i < 10; ++i)
            {
                string name = "inputCount" + i;
                Button go = rc.Get<GameObject>(name).GetComponent<Button>();
                clickBtns.Add(go);
            }
            clickBtns.Distinct();

            //关闭
            CloseJoinRoom.onClick.Add(() =>
            {
                ClearText();
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIJoinRoom);
            });

            AddClick();

            DeleteBtn.onClick.Add(() =>
            {
                if (curEnterValue.Length > 0)
                {
                    int index = builder.ToString().LastIndexOf(space);
                    builder.Remove(index - 1, 5);
                    curEnterValue = builder.ToString();
                    EnterTxt.text = curEnterValue;
                }
            });

            ANewBtn.onClick.Add(() =>
            {
                ClearText();
            });

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);
        }

        private void AddClick()
        {
            for (int i = 0; i < clickBtns.Count; ++i)
            {
                int index = i;
                clickBtns[index].onClick.Add(() =>
                {
                    BtnClick(clickBtns[index].transform.Find("count").GetComponent<Text>().text);
                });
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


        private async void BtnClick(string value)
        {
            #region test
            if (curEnterValue.Replace(" ", "").Length >= 5)
            {
                if(curEnterValue.Replace(" ", "").Length < 6)
                {
                    curEnterValue = builder.Append(value).Append(space).ToString();
                    EnterTxt.text = curEnterValue;
                }
                
                //向服务器发送消息
                ToastScript.createToast("房间号已经输入完成");
                curEnterValue = curEnterValue.Replace(" ", "");
                await EnterFriendRoom(curEnterValue);
                return;
            }
            else
            {
                curEnterValue = builder.Append(value).Append(space).ToString();
                EnterTxt.text = curEnterValue;
            }
            #endregion
        }

        public void SetCurRoomId(long roomId)
        {
            curRoomId = roomId;
        }

        public static async Task EnterFriendRoom(string curEnterValue)
        {
            if (curEnterValue.Length >= 6)
            {
                //向服务器发送消息
                // ToastScript.createToast(curEnterValue);
                //如果curRoomId不为0，则是点击私密房间
                if (curRoomId != 0)
                {
                    if(curRoomId != Convert.ToInt64(curEnterValue))
                    {
                        UICommonPanelComponent.showCommonPanel("提示","房间号输入错误，请重新输入！");
                        return;
                    }
                }
                UINetLoadingComponent.showNetLoading();
                G2C_EnterRoom g2CEnterRoom = (G2C_EnterRoom) await SessionComponent.Instance.Session.Call(new C2G_EnterRoom()
                {
                    RoomType = 3,
                    RoomId = Convert.ToInt32(curEnterValue)
                });
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIJoinRoom);
                UINetLoadingComponent.closeNetLoading();
                if (g2CEnterRoom.Error != ErrorCode.ERR_Success)
                {
                    UICommonPanelComponent.showCommonPanel("提示", g2CEnterRoom.Message);
                    return;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            clickBtns.Clear();
            curEnterValue = "";
            EnterTxt.text = "";
            builder.Clear();
        }
    }
}
