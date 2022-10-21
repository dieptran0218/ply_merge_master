using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFight : SingletonMonoBehaviour<CanvasFight>
{
    public GameObject objBtnFight;
    public Animator anim;
    public int stage = 0;

    //Buy Slot
    public GameObject btnSlot_Gem;
    public GameObject btnSlot_Ads;
    public CanvasGroup cSlotGem;
    public CanvasGroup cSlotAds;
    public Text txtSlotCost;

    //Buy Monster
    public GameObject btnMonster_Gem;
    public GameObject btnMonster_Ads;
    public CanvasGroup cUnitGem;
    public CanvasGroup cUnitAds;
    public Text txtMonsterCost;

    [Header("Neo")]
    public Transform transHand_Fight;
    public Transform transHand_Unit;
    public Transform transHand_Slot;

    public Transform transTut_Fight;
    public Transform transTut_Unit;
    public Transform transTut_Slot;

    private int _costMonster;
    private int _costSlot;
    [HideInInspector] public bool _isBlock;
    public void Refresh()
    {
        if (_isBlock) return;
        CheckStatusBtnSlot();
        CheckStatusBtnUnit();
    }

    public void FixedUpdate()
    {
        if (_isBlock) return;
        ReloadUI();
        Refresh();
    }

    public void ReloadUI()
    {
        if (_isBlock) return;
        _costSlot = GameManager_PLY_V2.Instance.m_SlotPrice;
        _costMonster = GameManager_PLY_V2.Instance.m_MonsterPrice;
        txtSlotCost.text = GameHelper.ConvertNumber(_costSlot);
        txtMonsterCost.text = GameHelper.ConvertNumber(_costMonster);

        if (GameManager_PLY_V2.Instance.gemCollected > _costMonster)
        {
            btnMonster_Gem.SetActive(true);
            btnMonster_Ads.SetActive(false);
        }
        else
        {
            btnMonster_Gem.SetActive(false);
            btnMonster_Ads.SetActive(true);
        }

        if (GameManager_PLY_V2.Instance.gemCollected > _costSlot)
        {
            btnSlot_Gem.SetActive(true);
            btnSlot_Ads.SetActive(false);
        }
        else
        {
            btnSlot_Gem.SetActive(false);
            btnSlot_Ads.SetActive(true);
        }
    }

    public void OnShow_1()
    {
        gameObject.SetActive(true);
        objBtnFight.SetActive(true);
        stage = 1;
        anim.Play("Show_1");
    }

    public void OnShow_2()
    {
        PlayerController.ins._endgame.BossCallMonster();
        gameObject.SetActive(true);
        objBtnFight.SetActive(true);
        stage = 2;
        anim.Play("Show_2");
        Refresh();
    }

    public void ShowEnd()
    {
        CanvasManager.ins.tut.gameObject.SetActive(true);
    }

    public void OnShow_1_2()
    {
        anim.Play("Show_1_2");
        stage = 2;
    }

    public void OnHide()
    {
        objBtnFight.SetActive(false);
        gameObject.SetActive(false);
    }

    void CheckStatusBtnSlot()
    {
        if (GameManager_PLY_V2.Instance.totalCeilOpened >= 20)
        {
            cSlotAds.interactable = false;
            cSlotAds.alpha = 0.3f;

            cSlotGem.interactable = false;
            cSlotGem.alpha = 0.3f;
        }
        else
        {
            cSlotAds.interactable = true;
            cSlotAds.alpha = 1f;

            cSlotGem.interactable = true;
            cSlotGem.alpha = 1f;
        }
    }

    void CheckStatusBtnUnit()
    {
        //var check = false;
        //foreach (var t in GameManager_PLY_V2.Instance.endgame_CeilInfo)
        //{
        //    if (t.levelUpdate == 0)
        //    {
        //        check = true;
        //        break;
        //    }
        //}

        //if (!check)
        //{
        //    cUnitGem.interactable = false;
        //    cUnitGem.alpha = 0.3f;

        //    cUnitAds.interactable = false;
        //    cUnitAds.alpha = 0.3f;
        //}
        //else
        //{
        //    cUnitGem.interactable = true;
        //    cUnitGem.alpha = 1f;

        //    cUnitAds.interactable = true;
        //    cUnitAds.alpha = 1f;
        //}
    }

    #region Button
    public void OnFight()
    {
        Endgame3_CeilManager.ins.btnRemove.gameObject.SetActive(false);
        SoundController.ins.UI_Click();
        EndGame.Instance.OnBtnFight();
        if (Tutorial.ins != null && Tutorial.ins.step == 4)
        {
            Tutorial.ins.step = 5;
        }
    }

    public void BtnBuySlot()
    {
        SoundController.ins.UI_Click();
        GameManager_PLY_V2.Instance.AddGem(-_costSlot);
        BuySlotComplete();

        if (Tutorial.ins != null)
        {
            Tutorial.ins.step = 2;
        }
    }

    public void BtnBuySlotAds()
    {
        ////AdsManager.Ins.ShowRewardedAd("buy_slot_ads", () =>
        //{
        //    SoundController.ins.UI_Click();
        //    BuySlotComplete();
        //});
    }

    public void BtnBuyUnit()
    {
        SoundController.ins.UI_Click();
        GameManager_PLY_V2.Instance.AddGem(-_costMonster);
        BuyUnitComplete();
        PlayerController.ins._countPokemon += 1;
        if (Tutorial.ins != null && PlayerController.ins._countPokemon >= 3)
        {
            Tutorial.ins.step = 1;
        }
    }

    public void BtnBuyUnitAds()
    {
        ////AdsManager.Ins.ShowRewardedAd("buy_unit_ads", () =>
        //{
        //    SoundController.ins.UI_Click();
        //    BuyUnitComplete();
        //});
    }

    public void BuySlotComplete()
    {
        var pos = Endgame3_CeilManager.ins.sprCeils[GameManager_PLY_V2.Instance.totalCeilOpened];
        var t = GameConfig.ins.SpawnFx(GameConfig.ins.fx_UnlockSlot, pos.transform.position, 1.6f).transform;
        t.transform.SetParent(pos.transform);
        t.transform.localRotation = Quaternion.Euler(0, 0, 0);
        t.transform.localPosition = Vector3.zero;
        t.transform.SetParent(null);
        //GameManager_PLY_V2.Instance.AddSlot();
        //if(GameManager_PLY_V2.ins.data.level != 0) GameManager_PLY_V2.ins.data.countBuySlot++;
        Endgame3_CeilManager.ins.Refresh();
        Refresh();
    }

    public void BuyUnitComplete()
    {
        foreach (var t in Endgame3_CeilManager.ins.listCeilManager)
        {
            if (t.info == null || t.info.levelUpdate == 0)
            {
                t.SpawnMergeFx();
                break;
            }
        }
        //if (GameManager_PLY_V2.ins.data.level != 0) GameManager_PLY_V2.ins.data.countBuyUnit++;
        //GameManager_PLY_V2.ins.data.AddUnit();
        Endgame3_CeilManager.ins.Refresh();
        Refresh();
    }

    #endregion
}
