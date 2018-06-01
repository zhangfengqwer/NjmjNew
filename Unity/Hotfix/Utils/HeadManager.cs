using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class HeadManager
    {
        static List<NetHeadData> m_netHeadList = new List<NetHeadData>();

        public static async Task setHeadSprite(Image img,string head)
        {
            try
            {
                Sprite sprite = null;

                if (PlayerInfoComponent.Instance.GetPlayerInfo().Icon.Length < 10)
                {
                    sprite = CommonUtil.getSpriteByBundle("playericon", PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
                }
                else
                {
                    for (int i = 0; i < m_netHeadList.Count; i++)
                    {
                        if (m_netHeadList[i].m_url.CompareTo(head) == 0)
                        {
                            Log.Debug("使用缓存头像");
                            sprite = m_netHeadList[i].m_sprite;
                            break;
                        }
                    }

                    if (sprite == null)
                    {
                        Log.Debug("下载头像");
                        sprite = await CommonUtil.GetTextureFromUrl(PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
                        m_netHeadList.Add(new NetHeadData(head, sprite));
                    }
                }

                if (img != null)
                {
                    img.sprite = sprite;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("HeadManager.setHeadSprite异常：" +ex);
            }
        }
    }

    public class NetHeadData
    {
        public string m_url = "";
        public Sprite m_sprite = null;

        public NetHeadData(string url, Sprite sprite)
        {
            m_url = url;
            m_sprite = sprite;
        }
    }
}
