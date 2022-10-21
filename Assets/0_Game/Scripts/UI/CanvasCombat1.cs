using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class CanvasCombat1 : MonoBehaviour
{
    public GameObject panel;
    public GameObject hand;
    public GameObject adsIcon;
    public RectTransform freeTxt;
    public GameObject btnAds;

    public void OnFightClick()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        PlayerController.ins._isCanEvo = false;
    }

    public void OnAdsClick()
    {
        //SoundController.ins.UI_Click();
        //if (GameManager.ins.data.level == 0) { 
        //    OnAdsDone(); 
        //}
        //else AdsManager.Ins.ShowRewardedAd("endgame_evolution_offer", OnAdsDone);
    }

    IEnumerator ie_LevelUp()
    {
        PlayerController.ins.LevelUp(3);
        foreach (var t in PlayerController.ins.listPokemon)
        {
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_CollectPokemon, t.transform.position);
        }
        yield return Yielders.Get(1f);
        PlayerController.ins._isCanEvo = false;
    }

    public void OnAdsDone()
    {
        SoundController.PlaySoundOneShot(SoundController.ins.update_wall);
        VibrationsManager.instance.TriggerLightImpact();
        if (GameManager.ins.data.level == 0)
        {
            hand.SetActive(false);
            panel.SetActive(false);
        }
        StartCoroutine(ie_LevelUp());
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void OnOpen()
    {
        gameObject.SetActive(true);
        Setup();
    }

    private void Setup()
    {
        if (GameManager.ins.data.level != 0)
        {
            hand.SetActive(false);
            panel.SetActive(false);
            adsIcon.SetActive(true);
            freeTxt.gameObject.SetActive(true);
            freeTxt.anchoredPosition = new Vector2(33, -115);
        }
        else
        {
            //var c = btnAds.GetComponent<CheckAdsReward>();
            //if (c != null) c.Remove();
            //hand.SetActive(true);
            //panel.SetActive(true);
            //adsIcon.SetActive(false);
            //freeTxt.gameObject.SetActive(true);
            //freeTxt.anchoredPosition = new Vector2(0, -115);
        }
    }
}
