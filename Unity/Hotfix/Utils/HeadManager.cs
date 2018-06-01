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
        static List<WaitSetData> m_waitSetDataList = new List<WaitSetData>();
        
        public static void setHeadSprite(Image img,string head)
        {
            try
            {
                m_waitSetDataList.Add(new WaitSetData(img,head));

                if (m_waitSetDataList.Count == 1)
                {
                    setImage(m_waitSetDataList[0]);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("HeadManager.setHeadSprite异常：" +ex);
            }
        }

        static async Task setImage(WaitSetData waitSetData)
        {
            try
            {
                Sprite sprite = null;

                if (waitSetData.m_head.Length < 10)
                {
                    sprite = CommonUtil.getSpriteByBundle("playericon", waitSetData.m_head);
                }
                else
                {
                    for (int i = 0; i < m_netHeadList.Count; i++)
                    {
                        if (m_netHeadList[i].m_url.CompareTo(waitSetData.m_head) == 0)
                        {
                            Log.Debug("使用缓存头像");
                            sprite = m_netHeadList[i].m_sprite;
                            break;
                        }
                    }

                    if (sprite == null)
                    {
                        Log.Debug("下载头像");
                        sprite = await CommonUtil.GetTextureFromUrl(waitSetData.m_head);
                        m_netHeadList.Add(new NetHeadData(waitSetData.m_head, sprite));
                    }
                }

                if (waitSetData.m_img != null)
                {
                    waitSetData.m_img.sprite = sprite;
                }
                else
                {
                    Log.Debug("头像Image为空");
                }

                {
                    m_waitSetDataList.Remove(waitSetData);
                    if (m_waitSetDataList.Count > 0)
                    {
                        await setImage(m_waitSetDataList[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("HeadManager.setImage异常：" + ex);

                {
                    m_waitSetDataList.Remove(waitSetData);
                    if (m_waitSetDataList.Count > 0)
                    {
                        await setImage(m_waitSetDataList[0]);
                    }
                }
            }
        }
    }

    public class WaitSetData
    {
        public Image m_img;
        public string m_head;

        public WaitSetData(Image img, string head)
        {
            m_img = img;
            m_head = head;
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
