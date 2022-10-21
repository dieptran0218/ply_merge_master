using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class CanvasTreasure : SingletonMonoBehaviour<CanvasTreasure>
{
    public Animator anim;
    public GameObject keyZoneImg;
    public GameObject claimX2Btn;
    public GameObject normalClaimBtn;
    public GameObject moreKeyBtn;
    public GameObject noTksBtn;
    public Image bestPrizeImg;
    public Text txt1;
    public Text txt2;
    public List<GameObject> keyList;

    public int chosenId;
    public bool isBestPick;
    public int keyAmount;
    [HideInInspector]
    public int gemEarn;
    [HideInInspector]
    public int cheatKeyAmount;
    [HideInInspector]
    public bool isClickable;

    public Text gemTxt;
    public GameObject gem;

    //Xử lý trường hợp mở lần đầu
    public int openCount;
    public int reachOpenCount;

    public ParticleSystem leftPar;
    public ParticleSystem rightPar;

    private Coroutine co_showKeyList;
    private bool isAddedKey;
    private bool isClaimed;

    public void HandleNoKey()
    {
        if (keyAmount == 0 && cheatKeyAmount != 6)
        {
            anim.enabled = true;
            anim.Play("HideKey");
            isAddedKey = false;
            isClickable = false;
            StartCoroutine(ie_PlayAnimAfter("ShowMoreKeys", 0.5f));
            if (co_showKeyList != null)
            {
                StopCoroutine(co_showKeyList);
            }
            foreach (GameObject key in keyList)
            {
                key.SetActive(false);
            }
        }
        else if (keyAmount == 0 && cheatKeyAmount == 6)
        {
            anim.enabled = true;
            anim.Play("HideKey");
            StartCoroutine(ie_PlayAnimAfter("ShowClaim", 0.5f));
            if (co_showKeyList != null)
            {
                StopCoroutine(co_showKeyList);
            }
            foreach (GameObject key in keyList)
            {
                key.SetActive(false);
            }
        }
    }

    private void Setup()
    {
        if (GameManager.ins.data.level == 3)
        {
            reachOpenCount = Random.Range(0, 3);
            openCount = 0;
        }
        else
        {
            reachOpenCount = Random.Range(3, 9);
            openCount = 0;
        }
        if (GameManager.ins.data.charCollected.Count == GameConfig.ins.listCharacter.skinCharData.Count)
        {
            reachOpenCount = 100;
        }
        isBestPick = false;
        //chosenId = Random.Range(0, 9);
        chosenId = 1000;
        keyAmount = 3;
        cheatKeyAmount = 0;
        gemEarn = 0;
        isClickable = true;
        isClaimed = false;
        keyZoneImg.SetActive(true);
        normalClaimBtn.SetActive(false);
        claimX2Btn.SetActive(false);
        moreKeyBtn.SetActive(false);
        noTksBtn.SetActive(false);
        gem.SetActive(false);
        co_showKeyList = StartCoroutine(ie_showKeyList(1.5f));
        ChooseBestPrize();
    }

    private void ChooseBestPrize()
    {
        List<CharacterType> tempList = new List<CharacterType>();
        //if (GameManager.ins.data.level == 3)
        //{
        //    GetBestPrizeData(0);
        //    return;
        //}
        //else if (GameManager.ins.data.level == 6)
        //{
        //    GetBestPrizeData(5);
        //    return;
        //}
        //else if (GameManager.ins.data.level == 9)
        //{
        //    GetBestPrizeData(7);
        //    return;
        //}
        //else if (GameManager.ins.data.level == 12)
        //{
        //    GetBestPrizeData(11);
        //    return;
        //}
        //else if (GameManager.ins.data.level == 15)
        //{
        //    GetBestPrizeData(14);
        //    return;
        //}
        //else if (GameManager.ins.data.level == 18)
        //{
        //    GetBestPrizeData(16);
        //    return;
        //}
        //else if (GameManager.ins.data.level == 21)
        //{
        //    GetBestPrizeData(18);
        //    return;
        //}

        foreach (SkinCharData skin in GameConfig.ins.listCharacter.skinCharData)
        {
            tempList.Add(skin.skinCharID);
        }
        foreach (CharacterType skin in GameManager.ins.data.charCollected)
        {
            tempList.Remove(skin);
        }
        if (tempList.Count > 0)
        {
            int rnd = Random.Range(0, tempList.Count);
            GameManager.ins.data.charAdsId = tempList[rnd];
            SkinCharData chosenChar = GameConfig.ins.listCharacter.skinCharData.Find(i => i.skinCharID == GameManager.ins.data.charAdsId);
            bestPrizeImg.sprite = chosenChar.charIcon;
            bestPrizeImg.SetNativeSize();
            //CanvasSkinShop.ins.ReloadUI();
        }
    }

    private void GetBestPrizeData(int index)
    {
        GameManager.ins.data.charAdsId = GameConfig.ins.listCharacter.skinCharData[index].skinCharID;
        SkinCharData chosenChar = GameConfig.ins.listCharacter.skinCharData[index];
        bestPrizeImg.sprite = chosenChar.charIcon;
        bestPrizeImg.SetNativeSize();
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    [ContextMenu("test")]
    public void OnOpen()
    {
        gameObject.SetActive(true);
        Setup();
    }

    public void OnClaimX2Click()
    {
        if (isClaimed) return;
        isClaimed = true;

        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();


        //AdsManager.Ins.ShowRewardedAd("chestroom_x2_reward", AdsDone_OnClaimX2Click);
    }

    public void AdsDone_OnClaimX2Click()
    {
        //Watch ads

        gem.SetActive(true);
        gemTxt.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
        StartCoroutine(ie_GetCoin(2));
        EfxManager.ins.GetGemFx(claimX2Btn.transform.position, gem.transform.position);
        if (isBestPick)
        {
            GameManager.ins.data.charCollected.Add(GameManager.ins.data.charAdsId);
            GameManager.ins.data.charUsed = GameManager.ins.data.charAdsId;
            //PlayerController.ins.WearSkin(GameManager.ins.data.skinUsed);
            GameManager.ins.data.charAdsId = CharacterType.None;
            //CanvasSkinShop.ins.ReloadUI();
        }
    }

    public void OnNormalClaimClick()
    {
        if (isClaimed) return;
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        isClaimed = true;
        gem.SetActive(true);
        gemTxt.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
        StartCoroutine(ie_GetCoin(1));
        //AdsManager.Ins.ShowInterstitial();
        EfxManager.ins.GetGemFx(claimX2Btn.transform.position, gem.transform.position);
        if (isBestPick)
        {
            GameManager.ins.data.charCollected.Add(GameManager.ins.data.charAdsId);
            GameManager.ins.data.charUsed = GameManager.ins.data.charAdsId;
            //PlayerController.ins.WearSkin(GameManager.ins.data.skinUsed);
            GameManager.ins.data.charAdsId = CharacterType.None;
            //CanvasSkinShop.ins.ReloadUI();
        }
    }

    public void OnMoreKeysClick()
    {
        if (isAddedKey) return;
        isAddedKey = true;
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        //AdsManager.Ins.ShowRewardedAd("get_more_key", AdsDone_OnMoreKeysClick);
    }

    public void AdsDone_OnMoreKeysClick()
    {
        keyAmount += 3;
        cheatKeyAmount += 3;
        //moreKeyBtn.SetActive(false);
        //noTksBtn.SetActive(false);
        //keyZoneImg.SetActive(true);
        anim.enabled = true;
        anim.Play("HideMoreKeys");
        StartCoroutine(ie_PlayAnimAfter("ShowKey", 0.5f));
        StartCoroutine(ie_turnClickable());
        co_showKeyList = StartCoroutine(ie_showKeyList(1.5f));
    }

    private IEnumerator ie_turnClickable()
    {
        yield return Yielders.Get(1.5f);
        isClickable = true;
    }

    private IEnumerator ie_GetCoin(int multi)
    {
        yield return Yielders.Get(1f);
        int tempCoin = GameManager.ins.data.gemCollected;
        for (int i = 0; i < 50; i++)
        {
            gemTxt.text = GameHelper.ConvertNumber((tempCoin + Random.Range(2, 5) * i));
            yield return Yielders.Get(0.01f);
        }
        GameManager.ins.data.AddGem(gemEarn * multi);
        //FirebaseManager.Ins.earn_virtual_currency("Gem", gemEarn * multi, "open_treasure");
        gemTxt.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
        yield return Yielders.Get(1f);
        GameManager.ins.ReLoadGame();
    }

    public void OnNoTksClick()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        claimX2Btn.SetActive(true);
        normalClaimBtn.SetActive(true);
        anim.enabled = true;
        anim.Play("HideMoreKeys");
        StartCoroutine(ie_PlayAnimAfter("ShowClaim", 0.5f));
    }

    private IEnumerator ie_PlayAnimAfter(string animName, float time)
    {
        yield return Yielders.Get(time);
        anim.enabled = true;
        anim.Play(animName);
    }

    private IEnumerator ie_showKeyList(float time)
    {
        yield return Yielders.Get(time);
        foreach (GameObject key in keyList)
        {
            key.SetActive(true);
        }
    }

}
