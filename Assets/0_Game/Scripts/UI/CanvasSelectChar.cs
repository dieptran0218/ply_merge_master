using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class CanvasSelectChar : MonoBehaviour
{
    public GameObject slot1SelectImg;
    public GameObject slot2SelectImg;
    public GameObject slot3SelectImg;
    public GameObject ads;
    public GameObject ads2;
    public Transform slot1Zone;
    public Transform slot2Zone;
    public Transform slot3Zone;
    public Button selectBtn;
    public Sprite sprBtnOn;
    public Sprite sprBtnOff;
    public List<Animator> anim;
    public List<CharacterType> charType;

    public GameObject btnRandomCharacter_0;
    public GameObject btnRandomCharacter_1;
    public GameObject btnRandomCharacter_2;

    private bool isWatchedAds;
    private bool isWatchedAds2;
    private CharacterType chosenChar;
    private bool isScaling;
    public void OnOpen()
    {
        isWatchedAds = false;
        isWatchedAds2 = false;
        gameObject.SetActive(true);
        selectBtn.interactable = false;
        selectBtn.GetComponent<Image>().sprite = sprBtnOff;
        OnSelectCharClick(1);

        //btnRandomCharacter_0.gameObject.SetActive(AdsManager.Ins.isMkt);
        //btnRandomCharacter_1.gameObject.SetActive(AdsManager.Ins.isMkt);
        //btnRandomCharacter_2.gameObject.SetActive(AdsManager.Ins.isMkt);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void OnSelectCharClick(int index)
    {
        if (isScaling) return;

        foreach (var t in anim)
        {
            t.SetInteger("stage", 7);
            t.transform.DOLocalRotate(Vector3.up * 180, 0.5f);
        }
        anim[index - 1].SetInteger("stage", 10);

        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        selectBtn.interactable = true;
        selectBtn.GetComponent<Image>().sprite = sprBtnOn;
        if (index == 1)
        {
            slot1SelectImg.SetActive(true);
            slot2SelectImg.SetActive(false);
            slot3SelectImg.SetActive(false);
            chosenChar = charType[0];
            isScaling = true;
            if (!GameManager.ins.data.charCollected.Contains(chosenChar))
                GameManager.ins.data.charCollected.Add(chosenChar);

            var t = slot1SelectImg.GetComponent<Image>();
            var c = t.color;
            c.a = 0;
            t.color = c;
            c.a = 1;
            t.DOColor(c, 0.2f);

            slot1Zone.DOScale(new Vector3(1, 1, 1), 0.2f).OnComplete(() =>
            {
                 isScaling = false;
            });

            slot2Zone.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
            slot3Zone.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
        }
        else if (index == 2)
        {
            if (isWatchedAds)
            {
                OnChooseOther(index);
            }
            else
            {
                //AdsManager.Ins.ShowRewardedAd("choose_character", () =>
                //{
                //    isWatchedAds = true;
                //    OnChooseOther(index);
                //});
            }
        }
        else
        {
            if (isWatchedAds2)
            {
                OnChooseOther(index);
            }
            else
            {
                //AdsManager.Ins.ShowRewardedAd("choose_character", () =>
                //{
                //    isWatchedAds2 = true;
                //    OnChooseOther(index);
                //});
            }
        }
    }

    public void ReplaceIndex(int idx)
    {
        var old = anim[idx].gameObject;
        var newId = Random.Range(1, 12);
        var newObj = GameConfig.ins.listCharacter.skinCharData.Find(x => x.skinCharID == newId.ToEnum<CharacterType>());
        if(newObj != null)
        {
            var o = Instantiate(newObj.charModel).transform;
            o.transform.SetParent(old.transform.parent);
            o.localScale = old.transform.localScale;
            o.localPosition = old.transform.localPosition;
            o.localRotation = Quaternion.Euler(o.localRotation.eulerAngles);
            var newAnim = o.gameObject.GetComponent<Animator>();
            newAnim.SetInteger("stage", 7);
            anim[idx] = newAnim;
            charType[idx] = newId.ToEnum<CharacterType>();
            Destroy(old);
        }
    }

    void OnChooseOther(int index)
    {
        if (isWatchedAds)
        {
            ads.SetActive(false);
        }
        if (isWatchedAds2)
        {
            ads2.SetActive(false);
        }
        if (index == 2)
        {
            slot1SelectImg.SetActive(false);
            slot2SelectImg.SetActive(true);
            slot3SelectImg.SetActive(false);
            chosenChar = charType[1];
            isScaling = true;
            if (!GameManager.ins.data.charCollected.Contains(chosenChar))
                GameManager.ins.data.charCollected.Add(chosenChar);

            var t = slot2SelectImg.GetComponent<Image>();
            var c = t.color;
            c.a = 0;
            t.color = c;
            c.a = 1;
            t.DOColor(c, 0.2f);

            slot2Zone.DOScale(new Vector3(1, 1, 1), 0.2f).OnComplete(() =>
             {
                 isScaling = false;
             });
            slot1Zone.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
            slot3Zone.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
        }
        else if (index == 3)
        {
            slot1SelectImg.SetActive(false);
            slot2SelectImg.SetActive(false);
            slot3SelectImg.SetActive(true);
            chosenChar = charType[2];
            isScaling = true;
            if (!GameManager.ins.data.charCollected.Contains(chosenChar))
                GameManager.ins.data.charCollected.Add(chosenChar);

            var t = slot3SelectImg.GetComponent<Image>();
            var c = t.color;
            c.a = 0;
            t.color = c;
            c.a = 1;
            t.DOColor(c, 0.2f);

            slot3Zone.DOScale(new Vector3(1, 1, 1), 0.2f).OnComplete(() =>
             {
                 isScaling = false;
             });
            slot1Zone.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
            slot2Zone.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
        }
    }

    public void OnSelectClick()
    {
        VibrationsManager.instance.TriggerLightImpact();
        GameManager.ins.data.charUsed = chosenChar;
    }
}
