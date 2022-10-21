using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(Image))]
//[RequireComponent(typeof(Mask))]
//[RequireComponent(typeof(ScrollRect))]
public class PageSwiper : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public RectTransform transContent;
    public ScrollRect scroll;
    private float pageSize = 964f;
    private int pageCount;
    private bool isLerping;
    private bool isDragging;
    private float transContentPos;
    private Vector2 _lerpTo;
    private Vector2 startMovePos;
    private float sign;
    private List<Vector2> positionList = new List<Vector2>();

    public void Init()
    {
        isLerping = false;
        if (CanvasSkinShop.ins.curTab == CanvasSkinShop.ShopTab.Character)
        {
            pageCount = Mathf.CeilToInt(GameConfig.ins.listCharacter.skinCharData.Count / 8f);
        }
        else if (CanvasSkinShop.ins.curTab == CanvasSkinShop.ShopTab.Skin)
        {
            pageCount = Mathf.CeilToInt(GameConfig.ins.listAllSkin.skinData.Count / 8f);
        }
        else if (CanvasSkinShop.ins.curTab == CanvasSkinShop.ShopTab.Dance)
        {
            pageCount = Mathf.CeilToInt(GameConfig.ins.listDance.danceData.Count / 8f);
        }
        //transContentPos = transContent.anchoredPosition.x;
        //transContentPos = (transContent.rect.width) / 2 - pageSize / 2 ;
        transContentPos = 0;
        transContent.anchoredPosition = new Vector2(transContentPos, 0);
        InitPositionList();
    }

    private void Update()
    {
        if (isLerping)
        {
            transContent.anchoredPosition = new Vector2(transContent.anchoredPosition.x + 2000 * Time.deltaTime * sign, 0);
            // time to stop lerping?
            if (Vector3.Distance(transContent.anchoredPosition, _lerpTo) < 50)
            {
                // snap to target and stop lerping
                transContent.anchoredPosition = _lerpTo;
                //Debug.Log(_lerpTo + "---:" + transContent.anchoredPosition);
                isLerping = false;
            }

            //if (Vector2.SqrMagnitude(transContent.anchoredPosition - _lerpTo) > 300) {
            //    return;
            //}
            //if ((sign < 0 && transContent.anchoredPosition.x > _lerpTo.x) || (sign > 0 && transContent.anchoredPosition.x < _lerpTo.x)) {
            //    transContent.anchoredPosition = _lerpTo;
            //    isLerping = false;
            //}
        }
    }

    private void InitPositionList()
    {
        positionList.Clear();
        for (int i = 0; i < pageCount; i++)
        {
            Vector2 pos = new Vector2(transContentPos - pageSize * i, 0);
            positionList.Add(pos);
        }
    }

    private void FindLerpToPos()
    {
        if (positionList.Count == 1)
        {
            _lerpTo = positionList[0];
            return;
        }

        float smallestDistance = 1000000;

        float dir = transContent.anchoredPosition.x - startMovePos.x;

        for (int i = 0; i < positionList.Count; i++)
        {
            float a = Mathf.Abs(transContent.anchoredPosition.x - positionList[i].x);
            float dividant = 1;
            if ((transContent.anchoredPosition.x - positionList[i].x) * dir < 0) {
                dividant = 3;
            }
            a = a / dividant;
            if (a < smallestDistance)
            {
                smallestDistance = a;
                _lerpTo = positionList[i];
            }
        }

        int exception = 1;
        if (transContent.anchoredPosition.x > positionList[0].x || transContent.anchoredPosition.x < positionList[pageCount - 1].x)
        {
            exception = 0;
        }
        sign = transContent.anchoredPosition.x > _lerpTo.x ? -1 * exception : 1 * exception;

    }

    public void OnBeginDrag(PointerEventData aEventData)
    {
        //Debug.Log("begin");
        startMovePos = transContent.anchoredPosition;
        isLerping = false;
        isDragging = false;
    }

    //------------------------------------------------------------------------
    public void OnEndDrag(PointerEventData aEventData)
    {
        //Debug.Log("end");
        isDragging = false;
        scroll.StopMovement();
        FindLerpToPos();
        isLerping = true;
    }

    //------------------------------------------------------------------------
    public void OnDrag(PointerEventData aEventData)
    {
        //Debug.Log("draggging");
        if (!isDragging)
        {
            isDragging = !isDragging;
        }
    }
}
