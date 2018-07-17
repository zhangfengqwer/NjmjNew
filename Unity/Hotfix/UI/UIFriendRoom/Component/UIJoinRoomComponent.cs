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

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            JoinRoom = rc.Get<GameObject>("JoinRoom");
            CloseJoinRoom = rc.Get<GameObject>("CloseJoinRoom").GetComponent<Button>();
            ANewBtn = rc.Get<GameObject>("ANewBtn").GetComponent<Button>();
            DeleteBtn = rc.Get<GameObject>("DeleteBtn").GetComponent<Button>();
            InputCountGrid = rc.Get<GameObject>("InputCountGrid");
            EnterTxt = rc.Get<GameObject>("EnterTxt").GetComponent<Text>();

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
                    builder.Remove(curEnterValue.Length - 5, 5);
                    curEnterValue = builder.ToString();
                    EnterTxt.text = curEnterValue;
                }
            });

            ANewBtn.onClick.Add(() =>
            {
                ClearText();
            });
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
            if (curEnterValue.Replace(" ", "").Length >= 6)
            {
                //向服务器发送消息
                ToastScript.createToast("房间号已经输入完成");
                //return;
            }
            #endregion

            curEnterValue = builder.Append(value).Append("    ").ToString();
            EnterTxt.text = curEnterValue;
            curEnterValue = curEnterValue.Replace(" ", "");
            await EnterFriendRoom(curEnterValue);
        }

        public static async Task EnterFriendRoom(string curEnterValue)
        {
            if (curEnterValue.Length >= 6)
            {
                //向服务器发送消息
                ToastScript.createToast(curEnterValue);

                G2C_EnterRoom g2CEnterRoom =
                        (G2C_EnterRoom) await SessionComponent.Instance.Session.Call(new C2G_EnterRoom()
                        {
                                RoomType = 3,
                                RoomId = Convert.ToInt32(curEnterValue)
                        });
                Log.Info(g2CEnterRoom.Message);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            clickBtns.Clear();
        }
    }
}
