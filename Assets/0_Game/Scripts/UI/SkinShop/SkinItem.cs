using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class SkinItem : MonoBehaviour
{

    public Image icon;
    public GameObject openFont;
    public GameObject offerFont;
    public GameObject lockFont;

    public GameObject openTick;
    public GameObject offerTick;
    public GameObject gem;
    public GameObject lockTick;
    public Text priceTxt;

    public GameObject chooseBg;

    private bool isOpenAd;
    private int price;
    private int unlockLv;
    private ShopItemState currentState;

    private SkinId skinID;
    private CharacterType charID;
    private DanceId danceID;

    //private CheckAdsReward _checkAdsReward;

    public void Setup(SkinId skinID)
    {
        this.skinID = skinID;
        SkinData dt = GameManager.ins.data.GetSkin(skinID);
        icon.sprite = dt.skinIcon;
        unlockLv = dt.unlockLevel;
        isOpenAd = dt.isOpenAd;
        price = dt.price;
        SetupSkinUI();
    }

    public void Setup(CharacterType charID) {
        this.charID = charID;
        SkinCharData dt = GameManager.ins.data.GetChar(charID);
        icon.sprite = dt.charIcon;
        unlockLv = dt.unlockLevel;
        price = dt.price;
        SetupCharUI();
    }

    public void Setup(DanceId danceID)
    {
        this.danceID = danceID;
        DanceData dt = GameManager.ins.data.GetDance(danceID);
        icon.sprite = dt.danceIcon;
        unlockLv = dt.unlockLevel;
        price = dt.price;
        SetupDanceUI();
    }

    public void SetupSkinUI()
    {
        //if (_checkAdsReward != null) _checkAdsReward.Remove();

        icon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        if (skinID == GameManager.ins.data.skinUsed)
        {
            PlayerController.ins.WearSkin(skinID);
            lockFont.SetActive(false);
            openTick.SetActive(true);
            chooseBg.SetActive(true);
        }
        else {
            openTick.SetActive(false);
        }
        if (GameManager.ins.data.skinCollected.Contains(skinID))
        {
            openFont.SetActive(true);
            lockFont.SetActive(false);
            //openTick.SetActive(true);
            currentState = ShopItemState.Open;
            return;
        }
        if (skinID == GameManager.ins.data.skinAdsId)
        {
            offerFont.SetActive(true);
            offerTick.SetActive(true);
            lockFont.SetActive(false);
            priceTxt.text = "FREE";
            priceTxt.gameObject.SetActive(true);
            isOpenAd = true;
            currentState = ShopItemState.Offer;
            //if(_checkAdsReward == null)
            //{
            //    _checkAdsReward = gameObject.AddComponent<CheckAdsReward>();
            //    _checkAdsReward.alphaDisable = 1f;
            //    _checkAdsReward.imgIconAds = offerTick.GetComponent<Image>();
            //}
            return;
        }
        if (unlockLv <= GameManager.ins.data.level)
        {
            offerFont.SetActive(true);
            lockFont.SetActive(false);
            gem.SetActive(true);
            priceTxt.text = price.ToString();
            priceTxt.gameObject.SetActive(true);
            currentState = ShopItemState.Offer;
            if (GameManager.ins.data.gemCollected < price)
            {
                offerFont.SetActive(false);
                lockFont.SetActive(true);
            }
            return;
        }
        lockFont.SetActive(true);
        lockTick.SetActive(true);
        icon.GetComponent<Image>().color = new Color32(0, 0, 0, 105);
        currentState = ShopItemState.Lock;
    }

    public void SetupCharUI()
    {
        //if (_checkAdsReward != null) _checkAdsReward.Remove();

        icon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        if (charID == GameManager.ins.data.charUsed)
        {
            //PlayerController.ins.WearSkin(skinID);
            openTick.SetActive(true);
            chooseBg.SetActive(true);
        }
        else
        {
            openTick.SetActive(false);
        }
        if (GameManager.ins.data.charCollected.Contains(charID))
        {
            openFont.SetActive(true);
            //openTick.SetActive(true);
            currentState = ShopItemState.Open;
            return;
        }
        if (charID == GameManager.ins.data.charAdsId)
        {
            offerFont.SetActive(true);
            offerTick.SetActive(true);
            priceTxt.text = "FREE";
            priceTxt.gameObject.SetActive(true);
            isOpenAd = true;
            currentState = ShopItemState.Offer;
            //if (_checkAdsReward == null)
            //{
            //    _checkAdsReward = gameObject.AddComponent<CheckAdsReward>();
            //    _checkAdsReward.alphaDisable = 1f;
            //    _checkAdsReward.imgIconAds = offerTick.GetComponent<Image>();
            //}
            return;
        }
        if (unlockLv <= GameManager.ins.data.level)
        {
            offerFont.SetActive(true);
            gem.SetActive(true);
            priceTxt.text = price.ToString();
            priceTxt.gameObject.SetActive(true);
            currentState = ShopItemState.Offer;
            if (GameManager.ins.data.gemCollected < price)
            {
                offerFont.SetActive(false);
                lockFont.SetActive(true);
            }
            return;
        }
        lockFont.SetActive(true);
        lockTick.SetActive(true);
        icon.GetComponent<Image>().color = new Color32(0, 0, 0, 105);
        currentState = ShopItemState.Lock;
    }

    public void SetupDanceUI()
    {
        //if (_checkAdsReward != null) _checkAdsReward.Remove();

        icon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        if (danceID == GameManager.ins.data.danceUsed)
        {
            //PlayerController.ins.WearSkin(skinID);
            openTick.SetActive(true);
            chooseBg.SetActive(true);
        }
        else
        {
            openTick.SetActive(false);
        }
        if (GameManager.ins.data.danceCollected.Contains(danceID))
        {
            openFont.SetActive(true);
            //openTick.SetActive(true);
            currentState = ShopItemState.Open;
            return;
        }
        if (danceID == GameManager.ins.data.danceAdsId)
        {
            offerFont.SetActive(true);
            offerTick.SetActive(true);
            priceTxt.text = "FREE";
            priceTxt.gameObject.SetActive(true);
            isOpenAd = true;
            currentState = ShopItemState.Offer;
            //if (_checkAdsReward == null)
            //{
            //    _checkAdsReward = gameObject.AddComponent<CheckAdsReward>();
            //    _checkAdsReward.alphaDisable = 1f;
            //    _checkAdsReward.imgIconAds = offerTick.GetComponent<Image>();
            //}
            return;
        }
        if (unlockLv <= GameManager.ins.data.level)
        {
            offerFont.SetActive(true);
            gem.SetActive(true);
            priceTxt.text = price.ToString();
            priceTxt.gameObject.SetActive(true);
            currentState = ShopItemState.Offer;
            if (GameManager.ins.data.gemCollected < price)
            {
                offerFont.SetActive(false);
                lockFont.SetActive(true);
            }
            return;
        }
        lockFont.SetActive(true);
        lockTick.SetActive(true);
        icon.GetComponent<Image>().color = new Color32(0, 0, 0, 105);
        currentState = ShopItemState.Lock;
    }

    public void OnItemClick()
    {
        if (currentState == ShopItemState.Lock)
        {
        }
        else if (currentState == ShopItemState.Offer)
        {
            SoundController.ins.UI_Click();
            VibrationsManager.instance.TriggerLightImpact();
            if (isOpenAd)
            {
                OnAdsClick();
            }
            else
            {
                OnBuyClick();
            }
        }
        else if (currentState == ShopItemState.Open)
        {
            SoundController.ins.UI_Click();
            VibrationsManager.instance.TriggerLightImpact();

            if (skinID != SkinId.None && charID == CharacterType.None && danceID == DanceId.None)
            {
                GameManager.ins.data.skinUsed = skinID;
                GameManager.ins.SaveData();
                PlayerController.ins.WearSkin(skinID);
            }
            else if (skinID == SkinId.None && charID != CharacterType.None && danceID == DanceId.None)
            {
                GameManager.ins.data.charUsed = charID;
                GameManager.ins.SaveData();
                //Change char model
                CanvasSkinShop.ins.ChangePlayerModel(charID);
            }
            else if (skinID == SkinId.None && charID == CharacterType.None && danceID != DanceId.None)
            {
                GameManager.ins.data.danceUsed = danceID;
                GameManager.ins.SaveData();
            }

            foreach (var go in CanvasSkinShop.ins.itemList) {
                go.openTick.SetActive(false);
            }
            openTick.SetActive(true);
            CanvasSkinShop.ins.TurnOffAllBg();
            chooseBg.SetActive(true);
        }
    }

    public void OnAdsClick()
    {
        ////AdsManager.Ins.ShowRewardedAd("shop_claim_skin_offer", AdsDone);
    }

    public void AdsDone()
    {
        //watch ads
        if (skinID != SkinId.None && charID == CharacterType.None && danceID == DanceId.None)
        {
            GameManager.ins.data.skinUsed = skinID;
            bool isExist = GameManager.ins.data.skinCollected.Contains(skinID);
            if (!isExist)
            {
                GameManager.ins.data.skinCollected.Add(skinID);
                GameManager.ins.data.skinAdsId = SkinId.None;
            }
            GameManager.ins.SaveData();
            PlayerController.ins.WearSkin(skinID);
        }
        else if (skinID == SkinId.None && charID != CharacterType.None && danceID == DanceId.None)
        {
            GameManager.ins.data.charUsed = charID;
            bool isExist = GameManager.ins.data.charCollected.Contains(charID);
            if (!isExist)
            {
                GameManager.ins.data.charCollected.Add(charID);
                GameManager.ins.data.charAdsId = CharacterType.None;
            }
            GameManager.ins.SaveData();
            //change model
            CanvasSkinShop.ins.ChangePlayerModel(charID);
        }
        else if (skinID == SkinId.None && charID == CharacterType.None && danceID != DanceId.None)
        {
            GameManager.ins.data.danceUsed = danceID;
            bool isExist = GameManager.ins.data.danceCollected.Contains(danceID);
            if (!isExist)
            {
                GameManager.ins.data.danceCollected.Add(danceID);
                GameManager.ins.data.danceAdsId = DanceId.None;
            }
            GameManager.ins.SaveData();
        }
        
        CanvasSkinShop.ins.TurnOffAllBg();
        chooseBg.SetActive(true);

        CanvasSkinShop.ins.ReloadUI();
        priceTxt.gameObject.SetActive(false);
        offerTick.SetActive(false);
        offerFont.SetActive(false);
        openFont.SetActive(true);
        foreach (var go in CanvasSkinShop.ins.itemList)
        {
            go.openTick.SetActive(false);
        }
        openTick.SetActive(true);
        currentState = ShopItemState.Open;
    }

    public void OnBuyClick()
    {
        if (GameManager.ins.data.gemCollected < price) return;
        GameManager.ins.data.AddGem(-price);

        if (skinID != SkinId.None && charID == CharacterType.None && danceID == DanceId.None)
        {
            //FirebaseManager.Ins.spend_virtual_currency("Gem", price, "shop_buy_skin_" + skinID);
            GameManager.ins.data.skinUsed = skinID;
            bool isExist = GameManager.ins.data.skinCollected.Contains(skinID);
            if (!isExist) GameManager.ins.data.skinCollected.Add(skinID);
            GameManager.ins.SaveData();
            PlayerController.ins.WearSkin(skinID);
        }
        else if (skinID == SkinId.None && charID != CharacterType.None && danceID == DanceId.None)
        {
            GameManager.ins.data.charUsed = charID;
            bool isExist = GameManager.ins.data.charCollected.Contains(charID);
            if (!isExist) GameManager.ins.data.charCollected.Add(charID);
            GameManager.ins.SaveData();
            //Change model
            CanvasSkinShop.ins.ChangePlayerModel(charID);
        }
        else if (skinID == SkinId.None && charID == CharacterType.None && danceID != DanceId.None)
        {
            GameManager.ins.data.danceUsed = danceID;
            bool isExist = GameManager.ins.data.danceCollected.Contains(danceID);
            if (!isExist) GameManager.ins.data.danceCollected.Add(danceID);
            GameManager.ins.SaveData();
        }

        CanvasSkinShop.ins.gemCollected.text = GameManager.ins.data.gemCollected.ToString();
        CanvasSkinShop.ins.TurnOffAllBg();
        chooseBg.SetActive(true);

        CanvasSkinShop.ins.ReloadUI();
        priceTxt.gameObject.SetActive(false);
        gem.SetActive(false);
        offerFont.SetActive(false);
        lockFont.SetActive(false);
        openFont.SetActive(true);
        openTick.SetActive(true);
        currentState = ShopItemState.Open;
    }
}
