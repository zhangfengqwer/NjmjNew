using UnityEngine;
using System.Collections;

public class UnityGameInfo : MonoBehaviour
{

    public float fpsMeasuringDelta = 2.0f;

    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;

    void OnGUI()
    {

        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
        bb.fontSize = 20;       //当然，这是字体大小
        GUILayout.Space(50);
//        GUILayout.Label("Total DrawCall: " + UnityEditor.UnityStats.drawCalls, bb, GUILayout.Width(500));
//        GUILayout.Label("Batch: " + UnityEditor.UnityStats.batches, bb, GUILayout.Width(500));
//        GUILayout.Label("Static Batch DC: " + UnityEditor.UnityStats.staticBatchedDrawCalls, bb, GUILayout.Width(500));
//        GUILayout.Label("Static Batch: " + UnityEditor.UnityStats.staticBatches, bb, GUILayout.Width(500));
//        GUILayout.Label("DynamicBatch DC: " + UnityEditor.UnityStats.dynamicBatchedDrawCalls, bb, GUILayout.Width(500));
//        GUILayout.Label("DynamicBatch: " + UnityEditor.UnityStats.dynamicBatches, bb, GUILayout.Width(500));
//
//        GUILayout.Label("Tri: " + UnityEditor.UnityStats.triangles, GUILayout.Width(500));
//        GUILayout.Label("Ver: " + UnityEditor.UnityStats.vertices, GUILayout.Width(500));

        //居中显示FPS
        GUILayout.Label( "FPS: " + m_FPS, bb);
    }

    private void Start()
    {
        timePassed = 0.0f;
    }

    private void Update()
    {
        m_FrameCount = m_FrameCount + 1;
        timePassed = timePassed + Time.deltaTime;

        if (timePassed > fpsMeasuringDelta)
        {
            m_FPS = m_FrameCount / timePassed;

            timePassed = 0.0f;
            m_FrameCount = 0;
        }
    }
}