using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
{
    public CanvasHome canvasHome;
    public GameObject canvasIngame;
    public CanvasCombat2 canvasCombat2;
    public CanvasCombat3 canvasCombat3;
    public CanvasEvolution canvasEvolution;
    public CanvasWin canvasWin;
    public CanvasFail canvasFail;
    public CanvasSetting canvasSetting;
    public CanvasMonsterShop canvasShopMonster;
    public CanvasSkinShop canvasShopSkin;
    public CanvasTreasure canvasTreasure;
    public CanvasFight canvasFight;
    public CanvasWheelDaily canvasWheel_Daily;
    public CanvasWheel canvasWheel_EndGame;
    public Tutorial tut;

    public void OpenHome()
    {
        canvasHome.OnOpen();
    }

    public void CloseHome()
    {
        canvasHome.OnClose();
    }
    public void OpenIngame()
    {
        canvasIngame.SetActive(true);
    }

    public void CloseIngame()
    {
        canvasIngame.SetActive(false);
    }
    public void OpenCombat2()
    {
        canvasCombat2.OnOpen();
    }

    public void CloseCombat2()
    {
        canvasCombat2.OnClose();
    }

    public void CloseWin()
    {
        canvasWin.OnClose();
    }

    public void OpenFail()
    {
        canvasFail.OnOpen();
    }

    public void CloseFail()
    {
        canvasFail.OnClose();
    }

    public void OpenSetting()
    {
        canvasSetting.Show();
    }

    public void CloseSetting()
    {
        canvasSetting.Close();
    }

    public void OpenMonstershop()
    {
        canvasShopMonster.OnOpen();
    }

    public void CloseMonsterShop()
    {
        canvasShopMonster.OnClose();
    }

    public void OpenSkinShop()
    {
        canvasShopSkin.OnOpen();
    }

    public void CloseSkinShop()
    {
        canvasShopSkin.OnClose();
    }
    public void OpenTreasure()
    {
        canvasTreasure.OnOpen();
    }

    public void CloseTreasure()
    {
        canvasTreasure.OnClose();
    }

    public void OpenWheel_Daily()
    {
        canvasWheel_Daily.OnOpen(0, true);
    }

    public void OpenWheel_EndGame(int gemReward, bool isWin)
    {
        canvasIngame.SetActive(false);
        canvasWheel_EndGame.OnOpen(gemReward, isWin);
    }

    public void CloseWheel()
    {
        canvasWheel_Daily.OnClose();
    }
}
