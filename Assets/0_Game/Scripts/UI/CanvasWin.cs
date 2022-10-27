using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasWin : MonoBehaviour
{
    public RectTransform arrow;
    public Animator anim;
    private float range = 350f;
    private bool isStop;
    private float timeKeep;
    private int loadPercent;
    private int currentLoadPercent;

    [Header("Btn Claim Ads")]
    public GameObject claimX2Btn;
    public GameObject claimx2DisBg;
    public GameObject normalClaimBtn;
    public Text txtAdsClaim;

    [Header("Btn Claim Only Lv1")]
    public GameObject lv1ClaimBtn;
    public GameObject lv1ClaimDisBg;
    public Text txtLv1Claim;

    [Header("Btn Claim Only Lv2")]
    public GameObject getAllBtn;
    public GameObject getAllLv1Btn;

    [Header("Only Lv chest")]
    public GameObject chestImg;
    public GameObject giftChestImg;
    public Text giftChestTxt;

    public GameObject roller;
    public GameObject disroller;
    public GameObject fullRewardImg;

    public Image monsImg;
    public Image monsImgBlur;
    public GameObject blockButtonClick;
    public GameObject newMonsterBg;

    public Text claimTxt;
    public Text rewardTxt;
    public Text lvTxt;
    public Text percentTxt;
    public Text fullRewardTxt;
    public Text giftTxt;
    public Text normalRewardTxt;
    public Text claimLv1Txt;
    public Text gemTxt;

    private float wallXTime;
    private int multiX;
    private bool didClaim;

    private int _rw;

    public void OnClose()
    {
        DisableAll();
        gameObject.SetActive(false);
    }

    public void OnOpen(float ins)
    {
        gemTxt.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
        SoundController.PlaySoundOneShot(SoundController.ins.gameWin);
        gameObject.SetActive(true);
        anim.Play("Show");
        //Set up truoc 1 vai cai quan trong
        if (GameManager.ins.data.currentPokemon < GameConfig.ins.PokemonList.Count)
        {
            monsImg.sprite = GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon].img;
            monsImgBlur.sprite = GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon].blurImg;
        }
        monsImg.fillAmount = (float)GameManager.ins.data.loadPokemonPercent / 100f;
        percentTxt.text = GameManager.ins.data.loadPokemonPercent.ToString() + "%";
        //giftTxt.text = GameHelper.ConvertNumber(((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins)));
        CanvasManager.ins.canvasIngame.SetActive(false);
        lvTxt.text = "LEVEL " + (GameManager.ins.data.level + 1).ToString();
        if ((GameManager.ins.data.level + 1) % 6 == 0 || GameManager.ins.data.currentPokemon >= GameConfig.ins.PokemonList.Count)
        {
            giftTxt.transform.parent.gameObject.SetActive(false);
            monsImgBlur.gameObject.SetActive(false);
            chestImg.SetActive(true);
            giftChestImg.SetActive(true);
            //giftChestTxt.text = ((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins)).ToString();
        }

        StartCoroutine(ie_DelaySetup(ins));
        GameManager.ins.data.failCount = 0;
        //FirebaseManager.Ins.check_point_end(GameManager.ins.data.level, GameManager.ins.data.level.ToString(), true);
        //FirebaseManager.Ins.level_complete(GameManager.ins.data.level.ToString(), GameManager.ins.timePlayed);
        //AppsflyerEventRegister.af_Level_Achived(GameManager.ins.data.level, (int) (CanvasManager.ins.canvasWheel_EndGame._rw * ins));
    }

    // Update is called once per frame
    void Update()
    {
        timeKeep += Time.deltaTime;
        if (isStop || timeKeep < 0.6f) return;
        float posx = range * Mathf.Sin(Time.time * 4f);
        arrow.anchoredPosition = new Vector2(posx, arrow.anchoredPosition.y);
        multiX = GetXRewardCount();
        //rewardTxt.text = GameHelper.ConvertNumber(((int)((int)(CanvasManager.ins.canvasWheel_EndGame._rw * wallXTime) * multiX)));
    }

    private IEnumerator ie_GetCoin(int multiXTime)
    {
        yield return Yielders.Get(1f);
        int tempCoin = GameManager.ins.data.gemCollected;
        for (int i = 0; i < 50; i++)
        {
            gemTxt.text = GameHelper.ConvertNumber((tempCoin + Random.Range(1, 3) * i));
            yield return Yielders.Get(0.01f);
        }
        //GameManager.ins.data.AddGem((int)(CanvasManager.ins.canvasWheel_EndGame._rw * wallXTime) * multiXTime);
        //FirebaseManager.Ins
            //.earn_virtual_currency("Gem"
            //, (int)(CanvasManager.ins.canvasWheel_EndGame._rw * wallXTime) * multiXTime
            //, "endgame");
        gemTxt.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
    }

    public void NormalClaim()
    {
        //Button này chỉ nhận tiền và chuyển cảnh,ko xem ads
        if (didClaim) return;
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        EfxManager.ins.GetGemFx(normalClaimBtn.transform.position, gemTxt.transform.position);
        didClaim = true;
        WaitResetButton();
        //GameManager.ins.data.gemCollected += (int)(PlayerController.ins.gemCollected * wallXTime);
        //gemTxt.text = GameManager.ins.data.gemCollected.ToString();
        StartCoroutine(ie_GetCoin(1));
        StartCoroutine(ie_normalClaim());

        //AdsManager.Ins.ShowInterstitial();
    }

    public void WaitResetButton()
    {
        Timer.Schedule(this, 5f, () => { didClaim = false; });
    }

    private IEnumerator ie_normalClaim()
    {
        isStop = true;
        anim.enabled = false;
        yield return Yielders.Get(3f);
        OnClose();
        if ((GameManager.ins.data.level) % 3 == 0 && GameManager.ins.data.skinCollected.Count < GameConfig.ins.listAllSkin.skinData.Count)
        {
            ScreenSwitch.ins.Show(() =>
            {
                CanvasManager.ins.OpenTreasure();
                //GameManager.ins.ReLoadGame();
            });
        }
        else
        {
            GameManager.ins.ReLoadGame();
        }
    }

    public void ClaimX2()
    {
        if (didClaim) return;
        SoundController.ins.UI_Click();
        didClaim = true;
        isStop = true;
        //AdsManager.Ins.ShowRewardedAd("win_claim_x2", Ads_ClaimX2Done);
        WaitResetButton();
    }

    public void Ads_ClaimX2Done()
    {
        //Button này nhận tiền dựa trên xTime bên dưới và có xem ads
        VibrationsManager.instance.TriggerLightImpact();
        EfxManager.ins.GetGemFx(claimX2Btn.transform.position, gemTxt.transform.position);
        StartCoroutine(ie_GetCoin(multiX));
        StartCoroutine(ie_ClaimX2());
    }

    private IEnumerator ie_ClaimX2()
    {
        anim.enabled = false;
        yield return Yielders.Get(3f);
        int xTime = GetXRewardCount();
        
        if ((GameManager.ins.data.level) % 3 == 0 
            && GameManager.ins.data.skinCollected.Count < GameConfig.ins.listAllSkin.skinData.Count)
        {
            ScreenSwitch.ins.Show(() =>
            {
                //GameManager.ins.ReLoadGame();
                CanvasManager.ins.OpenTreasure();
            });
        }
        else
        {
            GameManager.ins.ReLoadGame();
        }

        OnClose();
    }

    public void ClaimLevel1()
    {
        //Button này nhận tiền x2 lần và ko xem ads
        if (didClaim) return;
        SoundController.ins.UI_Click();
        didClaim = true;
        VibrationsManager.instance.TriggerLightImpact();
        EfxManager.ins.GetGemFx(lv1ClaimBtn.transform.position, gemTxt.transform.position);
        //GameManager.ins.data.gemCollected += (int)(PlayerController.ins.gemCollected * wallXTime);
        //gemTxt.text = GameManager.ins.data.gemCollected.ToString();
        StartCoroutine(ie_GetCoin(2));
        StartCoroutine(ie_ClaimLv1());
    }

    private IEnumerator ie_ClaimLv1()
    {
        anim.enabled = false;
        yield return Yielders.Get(3f);
        OnClose();
        GameManager.ins.ReLoadGame();
    }

    public void GetAllBtn()
    {
        if (didClaim) return;
        SoundController.ins.UI_Click();
        didClaim = true;
        ////AdsManager.Ins.ShowRewardedAd("win_getall", Ads_GetAllBtnDone);
        WaitResetButton();
    }

    public void Ads_GetAllBtnDone()
    {
        //Button này nhận tiền x2 lần và nhận luôn monster,có xem ads

        VibrationsManager.instance.TriggerLightImpact();
        EfxManager.ins.GetGemFx(getAllBtn.transform.position, gemTxt.transform.position);
        //GameManager.ins.data.gemCollected += (int)(PlayerController.ins.gemCollected * wallXTime);
        //gemTxt.text = GameManager.ins.data.gemCollected.ToString();
        StartCoroutine(ie_GetCoin(2));
        StartCoroutine(ie_GetAllBtn());
    }

    private IEnumerator ie_GetAllBtn()
    {
        anim.enabled = false;
        yield return Yielders.Get(3f);
        OnClose();
        GameManager.ins.data.pokemonCollected.Add(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type);
        GameManager.ins.data.pokemonAds.Remove(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type);
        GameManager.ins.SaveData();
        if ((GameManager.ins.data.level) % 3 == 0 && GameManager.ins.data.skinCollected.Count < GameConfig.ins.listAllSkin.skinData.Count)
        {
            ScreenSwitch.ins.Show(() =>
            {
                //GameManager.ins.ReLoadGame();
                CanvasManager.ins.OpenTreasure();
            });
        }
        else
        {
            GameManager.ins.ReLoadGame();
        }
    }

    public void GetAllLv1Btn()
    {
        //Button này nhận tiền x2 lần và monster,không xem ads
        if (didClaim) return;
        didClaim = true;
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        EfxManager.ins.GetGemFx(getAllLv1Btn.transform.position, gemTxt.transform.position);
        //GameManager.ins.data.gemCollected += (int)(PlayerController.ins.gemCollected * wallXTime);
        //gemTxt.text = GameManager.ins.data.gemCollected.ToString();
        StartCoroutine(ie_GetCoin(2));
        StartCoroutine(ie_GetAllLv1Btn());
    }

    private IEnumerator ie_GetAllLv1Btn()
    {
        anim.enabled = false;
        yield return Yielders.Get(3f);
        OnClose();
        GameManager.ins.data.pokemonCollected.Add(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type);
        GameManager.ins.data.pokemonAds.Remove(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type);
        GameManager.ins.SaveData();
        GameManager.ins.ReLoadGame();
    }

    private IEnumerator ie_DelaySetup(float ins)
    {
        yield return Yielders.Get(1f);
        Setup(ins);
        PlayerPrefs.SetInt("LevelEndgame", PlayerPrefs.GetInt("LevelEndgame", 0) + 1);
        GameManager.ins.data.level++;
        GameManager.ins.data.countInsPrize++;
        GameManager.ins.data.countInsReward++;
    }
    public void Setup(float ins)
    {
        CanvasManager.ins.canvasIngame.SetActive(false);
        wallXTime = ins;
        anim.enabled = true;
        didClaim = false;
        if (GameManager.ins.data.level < 2)
        {
            claimX2Btn.SetActive(false);
            normalClaimBtn.SetActive(false);
            roller.SetActive(false);
            lv1ClaimBtn.SetActive(true);
            lv1ClaimDisBg.SetActive(false);
            anim.enabled = true;
            anim.Play("ClaimLv1Btn");
        }
        else
        {
            claimX2Btn.SetActive(true);
            claimx2DisBg.SetActive(false);
            roller.SetActive(true);
            lv1ClaimBtn.SetActive(false);
            anim.Play("ClaimX2Btn");
        }

        gemTxt.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
        lvTxt.text = "LEVEL " + (GameManager.ins.data.level + 1).ToString();
        isStop = false;
        timeKeep = 0;

        //txtAdsClaim.text = GameHelper.ConvertNumber(((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins) * 2));
        //txtLv1Claim.text = GameHelper.ConvertNumber(((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins) * 2));
        //giftTxt.text = GameHelper.ConvertNumber(((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins)));
        //normalRewardTxt.text = GameHelper.ConvertNumber(((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins)));
        //claimLv1Txt.text = GameHelper.ConvertNumber(((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins) * 2));
        //fullRewardTxt.text = GameHelper.ConvertNumber(((int)((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins) * 2)));

        if (GameManager.ins.data.currentPokemon < GameConfig.ins.PokemonList.Count)
        {
            monsImg.sprite = GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon].img;
            monsImgBlur.sprite = GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon].blurImg;
        }

        if ((GameManager.ins.data.level + 1) % 6 == 0 || GameManager.ins.data.currentPokemon >= GameConfig.ins.PokemonList.Count)
        {
            giftTxt.transform.parent.gameObject.SetActive(false);
            monsImgBlur.gameObject.SetActive(false);
            chestImg.SetActive(true);
            giftChestImg.SetActive(true);
            //giftChestTxt.text = GameHelper.ConvertNumber(((int)(CanvasManager.ins.canvasWheel_EndGame._rw * ins)));
        }

        loadPercent = GameManager.ins.data.loadPokemonPercent;
        currentLoadPercent = loadPercent;
        int earnedPercent = GetEarnedPercent();
        loadPercent += earnedPercent;
        GameManager.ins.data.loadPokemonPercent = loadPercent;
        GameManager.ins.SaveData();

        if (loadPercent >= 100 && GameManager.ins.data.level > 2 && GameManager.ins.data.currentPokemon < GameConfig.ins.PokemonList.Count)
        {
            normalClaimBtn.SetActive(false);
            claimx2DisBg.SetActive(true);
            //claimx2DisBg.transform.parent.GetComponent<CheckAdsReward>().isLoadNeeded = false;
            anim.enabled = false;
            lv1ClaimDisBg.SetActive(true);
            blockButtonClick.SetActive(true);
            roller.SetActive(false);
            disroller.SetActive(true);

            //Lưu pokemon
            if (GameManager.ins.data.currentPokemon < GameConfig.ins.PokemonList.Count)
            {
                GameManager.ins.data.currentPokemon++;
                GameManager.ins.data.loadPokemonPercent = 0;
                GameManager.ins.data.pokemonAds.Add(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type);
                GameManager.ins.SaveData();
            }
        }
        else if (loadPercent < 100 && GameManager.ins.data.level > 2)
        {
            StartCoroutine(ie_ShowNoTks());
        }
        if (loadPercent >= 100 && GameManager.ins.data.level < 2)
        {
            lv1ClaimDisBg.SetActive(true);
            blockButtonClick.SetActive(true);
            anim.enabled = false;
            //Lưu pokemon
            if (GameManager.ins.data.currentPokemon < GameConfig.ins.PokemonList.Count - 1)
            {
                GameManager.ins.data.currentPokemon++;
                GameManager.ins.data.loadPokemonPercent = 0;
                GameManager.ins.data.pokemonAds.Add(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type);
                GameManager.ins.SaveData();
            }
        }

        if (GameManager.ins.data.currentPokemon <= GameConfig.ins.PokemonList.Count)
        {
            StartCoroutine(ie_IncreasePercent());
        }
    }

    public int GetXRewardCount()
    {
        if (Mathf.Abs(arrow.anchoredPosition.x) < range / 5)
        {
            claimTxt.text = "CLAIM X4";
            return 4;
        }
        else if (Mathf.Abs(arrow.anchoredPosition.x) < range / 5 * 3)
        {
            claimTxt.text = "CLAIM X3";
            return 3;
        }
        else
        {
            claimTxt.text = "CLAIM X2";
            return 2;
        }
    }

    private int GetEarnedPercent()
    {
        if (GameManager.ins.data.currentPokemon >= GameConfig.ins.PokemonList.Count)
        {
            return 0;
        }
        int curLv = GameManager.ins.data.level + 1;
        int remainder = curLv % 6;
        if (remainder == 0)
        {
            return 0;
        }
        else if (remainder == 1 || remainder == 2 || remainder == 5)
        {
            return 50;
        }
        else if (remainder == 3 || remainder == 4)
        {
            return 25;
        }
        else
        {
            return 0;
        }
    }

    private IEnumerator ie_IncreasePercent()
    {
        var t = SoundController.PlaySoundLoop(SoundController.ins.unlock_pokemon_win);
        while (currentLoadPercent < loadPercent)
        {
            yield return new WaitForSeconds(0.015f);
            currentLoadPercent += 1;
            monsImg.fillAmount = (float)currentLoadPercent / 100f;
            percentTxt.text = currentLoadPercent.ToString() + "%";
            if (currentLoadPercent == loadPercent)
            {
                if (GameManager.ins.data.level < 3)
                {
                    if (loadPercent >= 100)
                    {
                        blockButtonClick.SetActive(false);
                        getAllLv1Btn.SetActive(true);
                        newMonsterBg.SetActive(true);
                        fullRewardImg.SetActive(true);
                        anim.enabled = true;
                        anim.Play("GetAllLv1Btn");
                        lv1ClaimBtn.SetActive(false);
                        try
                        {
                            //AppsflyerEventRegister.af_achievement_unlocked(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type + "", GameManager.ins.data.level);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                    else
                    {
                        lv1ClaimBtn.SetActive(true);
                    }

                }
                else
                {
                    if (loadPercent >= 100)
                    {
                        blockButtonClick.SetActive(false);
                        getAllBtn.SetActive(true);
                        anim.enabled = true;
                        anim.Play("GetAllBtn");
                        newMonsterBg.SetActive(true);
                        fullRewardImg.SetActive(true);
                        disroller.SetActive(false);
                        StartCoroutine(ie_ShowNoTks());
                        try
                        {
                            //AppsflyerEventRegister.af_achievement_unlocked(GameConfig.ins.PokemonList[GameManager.ins.data.currentPokemon - 1].type + "", GameManager.ins.data.level);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                    else
                    {
                        StartCoroutine(ie_ShowNoTks());
                        claimX2Btn.SetActive(true);
                    }
                }
            }
        }
        t.Stop();
    }

    private IEnumerator ie_ShowNoTks()
    {
        yield return new WaitForSeconds(2f);
        normalClaimBtn.SetActive(true);
    }

    private void DisableAll()
    {
        claimX2Btn.SetActive(false);
        claimx2DisBg.SetActive(false);
        normalClaimBtn.SetActive(false);
        lv1ClaimBtn.SetActive(false);
        lv1ClaimDisBg.SetActive(false);
        roller.SetActive(false);
        disroller.SetActive(false);
        fullRewardImg.SetActive(false);
        getAllBtn.SetActive(false);
        getAllLv1Btn.SetActive(false);
        blockButtonClick.SetActive(false);
        newMonsterBg.SetActive(false);

        //for lv %6=0
        chestImg.SetActive(false);
        giftChestImg.SetActive(false);
    }

    #region Button Action
    public void BtnClaimLv1()
    {

    }
    #endregion

}
