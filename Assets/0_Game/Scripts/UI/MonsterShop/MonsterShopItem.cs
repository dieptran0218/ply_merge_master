using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class MonsterShopItem : MonoBehaviour
{
    public GameObject iconLock;
    public GameObject iconOffer;
    public GameObject iconOpen;

    public GameObject fontLock;
    public GameObject fontOffer;
    public GameObject fontOpen;

    public Image monsterLock;
    public Image monsterOpen;

    private ShopItemState currentState;
    public PokemonType type;

    //private CheckAdsReward _checkAdsReward;
    public void Setup(PokemonType type)
    {
        PokemonImgData pid;
        this.type = type;
        pid = GameConfig.ins.PokemonList.Find(i => i.type == type);
        monsterLock.sprite = pid.blurSingleImg;
        monsterLock.SetNativeSize();
        monsterOpen.sprite = pid.singleImg;
        monsterOpen.SetNativeSize();
    }

    public void SetupState(PokemonType type)
    {
        //if (_checkAdsReward != null) _checkAdsReward.Remove();

        bool isCollected = false;
        foreach (PokemonType po in GameManager.ins.data.pokemonCollected)
        {
            if (po == type)
            {
                isCollected = true;
                break;
            }
        }
        if (isCollected)
        {
            currentState = ShopItemState.Open;
            iconLock.SetActive(false);
            iconOffer.SetActive(false);
            iconOpen.SetActive(true);

            fontLock.SetActive(false);
            fontOffer.SetActive(false);
            fontOpen.SetActive(true);

            monsterLock.gameObject.SetActive(false);
            monsterOpen.gameObject.SetActive(true);
        }
        else
        {
            bool isAds = false;
            foreach (PokemonType pok in GameManager.ins.data.pokemonAds)
            {
                if (pok == type)
                {
                    isAds = true;
                    break;
                }
            }
            if (isAds)
            {
                currentState = ShopItemState.Offer;
                iconLock.SetActive(false);
                iconOffer.SetActive(true);
                iconOpen.SetActive(false);

                fontLock.SetActive(false);
                fontOffer.SetActive(true);
                fontOpen.SetActive(false);

                monsterLock.gameObject.SetActive(false);
                monsterOpen.gameObject.SetActive(true);

                //if (_checkAdsReward == null)
                //{
                //    _checkAdsReward = gameObject.AddComponent<CheckAdsReward>();
                //    _checkAdsReward.alphaDisable = 1f;
                //    _checkAdsReward.imgIconAds = iconOffer.GetComponent<Image>();
                //}
            }
            else
            {
                currentState = ShopItemState.Lock;
                iconLock.SetActive(true);
                iconOffer.SetActive(false);
                iconOpen.SetActive(false);

                fontLock.SetActive(true);
                fontOffer.SetActive(false);
                fontOpen.SetActive(false);

                monsterLock.gameObject.SetActive(true);
                monsterOpen.gameObject.SetActive(false);
            }
        }
    }

    public void OnItemClick()
    {
        
        if (currentState == ShopItemState.Lock)
        {
            //GameManager.ins.data.pokemonAds.Add(type);
            //currentState = ShopItemState.Offer;
        }
        else if (currentState == ShopItemState.Offer)
        {
            SoundController.ins.UI_Click();
            VibrationsManager.instance.TriggerLightImpact();
            ////AdsManager.Ins.ShowRewardedAd("shop_monster_offer", AdsDone);
        }
        else if (currentState == ShopItemState.Open)
        {
            SoundController.ins.UI_Click();
            VibrationsManager.instance.TriggerLightImpact();
        }
        SetupState(this.type);
    }

    public void AdsDone()
    {
        //Xem ads
        currentState = ShopItemState.Open;
        GameManager.ins.data.pokemonCollected.Add(type);
        GameManager.ins.data.pokemonAds.Remove(type);
        GameManager.ins.SaveData();
        SetupState(this.type);
    }
}
