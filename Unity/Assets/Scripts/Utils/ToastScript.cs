using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ToastScript : MonoBehaviour
{

    public Canvas m_canvas;
    public static Text m_text;

    public static List<GameObject> s_toactObj = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        Invoke("onInvoke", 2);
    }

    void onInvoke()
    {
        s_toactObj.Remove(gameObject);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static GameObject createToast(string text)
    {
        GameObject prefab = Resources.Load("UI/Toast") as GameObject;
        GameObject obj = MonoBehaviour.Instantiate(prefab);
        m_text = obj.transform.Find("Text").GetComponent<Text>();

        obj.GetComponent<ToastScript>().setData(obj, text);
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(text.Length * 35, 60);

        return obj;
    }

    public static void clear()
    {
        s_toactObj.Clear();
    }

    public void setData(GameObject obj, string text)
    {
        m_text.text = text;

        m_canvas = GameObject.Find("CommonCanvas").GetComponent<Canvas>();
        obj.transform.SetParent(m_canvas.transform);
        obj.transform.localScale = new Vector3(1, 1, 1);

        for (int i = s_toactObj.Count - 1; i >= 0; i--)
        {
            Destroy(s_toactObj[i]);
        }
        s_toactObj.Clear();

        s_toactObj.Add(obj);
        s_toactObj[0].transform.localPosition = new Vector3(0, 0, 0);

        //if (s_toactObj.Count < 4)
        //{
        //    s_toactObj.Add(obj);
        //}
        //else
        //{
        //    Destroy(s_toactObj[0]);
        //    s_toactObj.RemoveAt(0);

        //    s_toactObj.Add(obj);
        //}

        //for (int i = s_toactObj.Count - 1; i >= 0; i--)
        //{
        //    s_toactObj[i].transform.localPosition = new Vector3(0, -230 + (s_toactObj.Count - i) * 60, 0);
        //}
    }
}