using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class Chest : MonoBehaviour
{
    public RectTransform gemReward;
    public RectTransform bestReward;
    GameObject chosenReward;
    public int id;
    public int gemHold;
    private Image img;
    private bool isClicked;
    // Start is called before the first frame update
    void Awake()
    {
        img = gameObject.GetComponent<Image>();
    }

    public void OnEnable()
    {
        int index;
        //int[] gemArr = { 1000, 3600, 5000, 10000 };
        if (GameManager.ins.data.level == 3)
        {
            index = Random.Range(5, 15);
        }
        else if (GameManager.ins.data.level == 6)
        {
            index = Random.Range(8, 23);
        }
        else if (GameManager.ins.data.level == 9)
        {
            index = Random.Range(10, 25);
        }
        else
        {
            index = Random.Range(10, 30);
        }

        gemHold = index * 10;
        //gemHold = gemArr[index];
        GameObject.Destroy(chosenReward);
        img.enabled = true;
        isClicked = false;
    }


    public void OnItemClick()
    {
        if (CanvasTreasure.ins.keyAmount <= 0 || img.enabled == false || isClicked == true || CanvasTreasure.ins.isClickable == false) return;
        SoundController.ins.UI_Click();
        SoundController.PlaySoundOneShot(SoundController.ins.open_chest);
        VibrationsManager.instance.TriggerLightImpact();
        isClicked = true;
        CanvasTreasure.ins.keyAmount--;
        if (CanvasTreasure.ins.keyAmount >= 0)
            CanvasTreasure.ins.keyList[CanvasTreasure.ins.keyAmount].SetActive(false);
        if (CanvasTreasure.ins.keyAmount == 0)
        {
            CanvasTreasure.ins.HandleNoKey();
        }
        RectTransform reward;

        //Xử lý trường hợp mở lần đầu
        if (CanvasTreasure.ins.openCount == CanvasTreasure.ins.reachOpenCount)
        {
            CanvasTreasure.ins.chosenId = id;
            CanvasTreasure.ins.openCount++;
        }
        else
        {
            CanvasTreasure.ins.openCount++;
        }

        img.transform.DOScale(new Vector2(1.2f, 1.2f), 0.2f).OnComplete(() =>
        {
            img.transform.DOScale(new Vector2(0, 0), 0.4f).OnComplete(() =>
            {
                img.gameObject.SetActive(false);
                img.transform.DOScale(new Vector2(1, 1), 0.1f);

                //Handle gen reward
                if (id == CanvasTreasure.ins.chosenId)
                {
                    reward = Instantiate(bestReward);
                    var img = reward.GetComponent<Image>();
                    img.sprite = CanvasTreasure.ins.bestPrizeImg.sprite;
                    img.SetNativeSize();
                    reward.transform.parent = gameObject.transform.parent;
                    reward.anchoredPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
                    reward.localScale = new Vector3(1, 1, 1);
                    CanvasTreasure.ins.isBestPick = true;
                    CanvasTreasure.ins.leftPar.Play();
                    //CanvasTreasure.ins.rightPar.Play();
                    SoundController.PlaySoundOneShot(SoundController.ins.firework);
                }
                else
                {
                    reward = Instantiate(gemReward);
                    Text gemTxt = reward.GetComponentInChildren<Text>();
                    gemTxt.text = gemHold.ToString();
                    reward.transform.parent = gameObject.transform.parent;
                    reward.anchoredPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
                    reward.localScale = Vector3.one;
                    CanvasTreasure.ins.gemEarn += gemHold;
                    CanvasTreasure.ins.txt1.text = CanvasTreasure.ins.gemEarn.ToString();
                    CanvasTreasure.ins.txt2.text = (CanvasTreasure.ins.gemEarn * 2).ToString();
                }
                chosenReward = reward.gameObject;
                reward.anchoredPosition = new Vector2(0, 0);
            });
        });
    }
}
