using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class HttpReqUtil
    {
        public delegate void CallBack(string data);

        public static async void Req(string url, CallBack callback)
        {
            using (UnityWebRequestAsync webRequestAsync = ETModel.ComponentFactory.Create<UnityWebRequestAsync>())
            {
                await webRequestAsync.DownloadAsync(url);
                callback(webRequestAsync.Request.downloadHandler.text);
            }
        }
    }
}
