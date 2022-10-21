using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page_Dot_UI : MonoBehaviour
{
    public RectTransform onDotPrefab;
    public RectTransform offDotPrefab;
    public RectTransform transContent;

    private RectTransform onDot;

    private float contentSize;
    private float pageSize = 964;
    private float dotDistance = 60f;
    private float firstDotPos;
    private bool firstInit = true;
    private int pageCount;
    private List<Vector2> positionList = new List<Vector2>();
    private List<GameObject> dotObs = new List<GameObject>();

    public void Init()
    {
        //if (!firstInit) return;

        //firstInit = false;
        ResetAll();
        contentSize = transContent.rect.width;
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
        GetFirstDotPos();
        InitListPos();



        for (int i = 0; i < pageCount; i++)
        {
            RectTransform offDot = Instantiate(offDotPrefab);
            dotObs.Add(offDot.gameObject);
            offDot.parent = gameObject.transform;
            offDot.transform.localScale = Vector3.one;
            offDot.anchoredPosition = positionList[i];
        }

        onDot = Instantiate(onDotPrefab);
        dotObs.Add(onDot.gameObject);
        onDot.parent = gameObject.transform;
        onDot.transform.localScale = Vector3.one;
        onDot.anchoredPosition = positionList[0];
    }


    // Update is called once per frame
    void Update()
    {
        contentSize = transContent.rect.width;
        int page = DetectCurrentPage();
        onDot.anchoredPosition = positionList[page];
    }

    private void GetFirstDotPos()
    {
        if (pageCount % 2 == 0)
        {
            firstDotPos = -(dotDistance * (pageCount - 1) / 2);
        }
        else
        {
            firstDotPos = -dotDistance / 2 * pageCount / 2;
        }
    }

    private void InitListPos()
    {
        for (int i = 0; i < pageCount; i++)
        {
            Vector2 pos = new Vector2(firstDotPos, 0);
            firstDotPos += dotDistance;
            positionList.Add(pos);
        }
    }

    private int DetectCurrentPage()
    {
        //int page = 1;
        //if (pageCount == 1) { return 1; }
        //for (int i = 0; i < pageCount; i++)
        //{
        //    if (transContent.anchoredPosition.x < -transContentPos + pageSize * (i + 1))
        //    {
        //        return pageCount - i;
        //    }
        //}
        //return page;
        //if (transContent.anchoredPosition.x > -482)
        //{
        //    return 1;
        //}
        //else if (transContent.anchoredPosition.x > -1446)
        //{
        //    return 2;
        //}
        //else {
        //    return 3;
        //}

        for (int i = 0; i < pageCount; i++) {
            if (transContent.anchoredPosition.x > -pageSize / 2 - i * pageSize) {
                return i;
            }
        }
        return pageCount-1;

    }

    public void ResetAll() {
        positionList.Clear();
        foreach (GameObject t in dotObs)
        {
            GameObject.Destroy(t);
        }
    }
    public void ResetPage()
    {
        transContent.anchoredPosition = new Vector2(2000, 0);
    }

}
