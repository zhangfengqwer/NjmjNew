using System;
using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCardScipt: MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform _rectTransform;
    [HideInInspector]
    public int weight;
    [HideInInspector]
    public int index;

    private Vector3 beginPosition;

    // Use this for initialization
    void Start()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        Log.Info("开始");

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Log.Info("开始tuozhai");
        this.beginPosition = this.SetDraggedPosition(eventData);
        Log.Debug(beginPosition.x + "");
        Log.Debug(beginPosition.y + "");
    }

    private Vector3 SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera,
                                                                    out globalMousePos))
        {
        }

        return globalMousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 endPosion = this.SetDraggedPosition(eventData);

        double sqrt = Math.Sqrt((endPosion.x - this.beginPosition.x) * (endPosion.x - this.beginPosition.x) 
                                + (endPosion.y - this.beginPosition.y) * (endPosion.y - this.beginPosition.y));

        Log.Debug(endPosion.y+"");
        Log.Debug(beginPosition.y+"");
        Log.Debug(sqrt+"");

        if (endPosion.y - beginPosition.y > 1 && sqrt > 1.5)
        {
            Log.Debug("拓展成功");
            Game.EventSystem.Run(EventIdType.GamerPlayCard, weight,index);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Log.Debug("双击");
            Game.EventSystem.Run(EventIdType.GamerPlayCard, weight, index);
        }
    }
}