using System;
using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCardScipt: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform _rectTransform;
    [HideInInspector]
    public int weight;
    [HideInInspector]
    public int index;

    public int clickedCount = 2;
    public float clickedInterval = 0.5f;

    private float lastClickedTime = 0;
    private float count = 0;

    private Vector3 beginPosition;

    // Use this for initialization
    void Start()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        _rectTransform.GetComponent<Button>().onClick.Add(this.OnClicked);
    }

   
    public void OnClicked()
    {
        float interval = Time.realtimeSinceStartup - lastClickedTime;
        if (interval <= clickedInterval)
        {
            count++;
            if (count == clickedCount - 1)
            {
                //TODO：
                Game.EventSystem.Run(EventIdType.GamerPlayCard, weight, index);
            }
        }
        else
        {
            count = 0;
        }
        lastClickedTime = Time.realtimeSinceStartup;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        this.beginPosition = this.SetDraggedPosition(eventData);

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

        if (endPosion.y - beginPosition.y > 1 && sqrt > 1.5)
        {
            Game.EventSystem.Run(EventIdType.GamerPlayCard, weight,index);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            //Game.EventSystem.Run(EventIdType.GamerPlayCard, weight, index);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}