using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotfix
{
    class OtherData
    {
        public static bool isShiedShare = false;
        public static bool isShiedRealName = false;
        public static bool isShiedBindPhone = false;
        public static bool isShiedPhoneLogin = false;
        public static bool isShiedWeChatLogin = false;

        public static GameObject s_loginCanvas = null;
        public static GameObject s_mainCanvas = null;
        public static GameObject s_roomCanvas = null;
        public static GameObject s_commonCanvas = null;

        public static string ShareUrl = "";

        public static bool getIsShiedShare()
        {
            return isShiedShare;
        }

        public static bool getIsShiedRealName()
        {
            return isShiedRealName;
        }

        public static bool getIsShiedBindPhone()
        {
            return isShiedBindPhone;
        }

        public static bool getIsShiedPhoneLogin()
        {
            return isShiedPhoneLogin;
        }

        public static bool getIsShiedWeChatLogin()
        {
            return isShiedWeChatLogin;
        }
    }
}
