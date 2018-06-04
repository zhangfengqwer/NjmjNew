using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownApkScript : MonoBehaviour {

    public Text m_text_title;
    public Text m_text_content;

    public Button m_btn_OK;

    public static GameObject create()
    {
        GameObject prefab = Resources.Load("UI/Panel_downApk") as GameObject;
        GameObject obj = GameObject.Instantiate(prefab, GameObject.Find("Global/UI/CommonCanvas").transform);

        return obj;
    }

    // Use this for initialization
    void Start ()
    {
        if (PlatformHelper.GetChannelName().CompareTo("ios") == 0)
        {
            m_text_content.text = "检测到新版本，请到应用市场获取最新版本。";
            m_btn_OK.transform.localScale = Vector3.zero;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onClickOK()
    {
        PlatformHelper.DownApk();
    }
}
