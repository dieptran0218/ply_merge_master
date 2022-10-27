using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : SingletonMonoBehaviour<Tutorial>
{

    public Transform transTut;
    public Transform transHand;
    public Text txtTut;
    public Button btnSkip;
    public Text txtBtnSkip;

    public string tipUnit;
    public string tipSlot;
    public string tipMerge;
    public string tipMove;
    public string tipFight;

    public int step = 0;

    IEnumerator ie_Tutorial()
    {
        foreach (var t in Endgame3_CeilManager.ins.listCeilManager)
        {
            t.BlockInput(true);
        }
        btnSkip.interactable = false;
        txtBtnSkip.text = "---";
        
        ShowTutUnit();
        yield return new WaitUntil(() => step == 1);
        ShowTutSlot();
        yield return new WaitUntil(() => step == 2);
        ShowTutMerge();
        yield return new WaitUntil(() => step == 3);
        ShowTutMove();
        yield return new WaitUntil(() => step == 4);
        ShowTutFight();

    }

    void ShowTutUnit()
    {
        StartCoroutine(ie_ShowTutUnit());
    }

    IEnumerator ie_ShowTutUnit()
    {
        var btnUnit = CanvasFight.ins.btnMonster_Gem;
        txtTut.text = tipUnit;
        var canvas = btnUnit.GetComponent<Canvas>();
        canvas.sortingOrder = 5;

        transTut.position = CanvasFight.ins.transTut_Unit.position;
        transHand.position = CanvasFight.ins.transHand_Unit.position;

        yield return new WaitUntil(() => step == 1);
        btnUnit.GetComponent<Button>().interactable = false;
        CanvasFight.ins.txtMonsterCost.text = "---";
        canvas.sortingOrder = 0;
    }

    void ShowTutSlot()
    {
        StartCoroutine(ie_ShowTutSlot());
    }

    IEnumerator ie_ShowTutSlot()
    {
        var btnSlot = CanvasFight.ins.btnSlot_Gem;
        txtTut.text = tipSlot;
        var canvas = btnSlot.GetComponent<Canvas>();
        canvas.sortingOrder = 5;

        transTut.position = CanvasFight.ins.transTut_Slot.position;
        transHand.position = CanvasFight.ins.transHand_Slot.position;

        yield return new WaitUntil(() => step == 2);
        btnSlot.GetComponent<Button>().interactable = false;
        CanvasFight.ins.txtSlotCost.text = "---";
        CanvasFight.ins.txtMonsterCost.text = "---";
        canvas.sortingOrder = 0;
    }

    void ShowTutMerge()
    {
        btnSkip.interactable = true;
        txtBtnSkip.text = "Skip";
        CanvasFight.ins._isBlock = true;
        Endgame3_CeilManager.ins.listCeilManager[0].BlockInput(false);
        Endgame3_CeilManager.ins.listCeilManager[1].BlockInput(false);
        StartCoroutine(ie_ShowTutMerge());
    }

    IEnumerator ie_ShowTutMerge()
    {
        txtTut.text = tipMerge;
        var p1 = Endgame3_CeilManager.ins.listCeilManager[0];
        var p2 = Endgame3_CeilManager.ins.listCeilManager[1];
        var pos1 = Camera.main.WorldToScreenPoint(p1.transform.position);
        var pos2 = Camera.main.WorldToScreenPoint(p2.transform.position);

        var ptut = Camera.main.WorldToScreenPoint(p2.transform.position + p2.transform.up * 2.5f - p2.transform.right * 2.5f);
        var phand_1 = Camera.main.WorldToScreenPoint(p1.transform.position + p1.transform.right);
        var phand_2 = Camera.main.WorldToScreenPoint(p2.transform.position + p2.transform.right);

        transTut.position = ptut;
        transHand.position = phand_1;
        var t = StartCoroutine(ie_HandMerge(phand_1, phand_2));
        yield return new WaitUntil(() => step == 3);
        transHand.DOKill();
        StopCoroutine(t);
    }

    IEnumerator ie_HandMerge(Vector3 pos1, Vector3 pos2)
    {
        while (true)
        {
            transHand.position = pos1;
            HandTutorial.ins.OnPause(false);
            yield return Yielders.Get(0.5f);
            HandTutorial.ins.OnPause(true);
            transHand.DOMove(pos2, 1f)
                .SetEase(Ease.Linear);
            yield return Yielders.Get(1.5f);
        }
    }

    void ShowTutMove()
    {
        Endgame3_CeilManager.ins.listCeilManager[3].BlockInput(false);
        StartCoroutine(ie_ShowTutMove());
    }

    IEnumerator ie_ShowTutMove()
    {
        txtTut.text = tipMove;
        Endgame_3_Ceil p1 = null;
        if (Endgame3_CeilManager.ins.listCeilManager[0].info.levelUpdate > 0)
        {
            p1 = Endgame3_CeilManager.ins.listCeilManager[0];
        }
        else if (Endgame3_CeilManager.ins.listCeilManager[1].info.levelUpdate > 0)
        {
            p1 = Endgame3_CeilManager.ins.listCeilManager[1];
        }

        var p2 = Endgame3_CeilManager.ins.listCeilManager[3];
        var pos1 = Camera.main.WorldToScreenPoint(p1.transform.position);
        var pos2 = Camera.main.WorldToScreenPoint(p2.transform.position);

        var ptut = Camera.main.WorldToScreenPoint(p2.transform.position + p2.transform.up * 2.5f - p2.transform.right * 1.5f);
        var phand_1 = Camera.main.WorldToScreenPoint(p1.transform.position + p1.transform.right * 0.7f);
        var phand_2 = Camera.main.WorldToScreenPoint(p2.transform.position + p2.transform.right * 0.7f);

        transTut.position = ptut;
        transHand.position = phand_1;

        var t = StartCoroutine(ie_HandMove(phand_1, phand_2));
        yield return new WaitUntil(() => step == 4);
        transHand.DOKill();
        StopCoroutine(t);
    }

    IEnumerator ie_HandMove(Vector3 pos1, Vector3 pos2)
    {
        while (true)
        {
            transHand.position = pos1 + new Vector3(0, -30, 0);
            HandTutorial.ins.OnPause(false);
            yield return Yielders.Get(0.5f);
            HandTutorial.ins.OnPause(true);
            transHand.DOMove(pos2 + new Vector3(0, -30, 0), 1f)
                .SetEase(Ease.Linear);
            yield return Yielders.Get(1.5f);
        }
    }

    void ShowTutFight()
    {
        HandTutorial.ins.OnPause(false);
        StartCoroutine(ie_ShowTutFight());
    }

    IEnumerator ie_ShowTutFight()
    {
        var btnUnit = CanvasFight.ins.objBtnFight;
        txtTut.text = tipFight;
        var canvas = btnUnit.AddComponent<Canvas>();
        btnUnit.AddComponent<GraphicRaycaster>();

        yield return Yielders.FixedUpdate;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 5;

        transTut.position = CanvasFight.ins.transTut_Fight.position;
        transHand.position = CanvasFight.ins.transHand_Fight.position;

        yield return new WaitUntil(() => step == 5);
        Destroy(canvas.GetComponent<GraphicRaycaster>());
        Destroy(canvas.GetComponent<Canvas>());
        Destroy(gameObject);
    }

    public void BtnSkip()
    {
        SoundController.ins.UI_Click();
        Luna.Unity.Analytics.LogEvent("Tutorial Skipped", 0);
        Destroy(gameObject);
    }


}
