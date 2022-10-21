using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
public class CanvasHome : MonoBehaviour
{
    public GameObject skinBtn;
    public GameObject monsterBtn;
    public Animator anim;
    public ShopItemState currentState;

    //Level
    public GameObject levelPrefab;
    public Transform levelHolder;
    public Text lvTxt;
    public Text gemTxt;
    public Text timeLeft;
    public int currentLevel;

    private void Update()
    {
        //update time count
        if (GameHelper.CurrentTimeInSecond - GameManager.ins.data.lastSpinTime < 300)
        {
            timeLeft.text = GameHelper.FormatTimeMMSS(300 - (GameHelper.CurrentTimeInSecond - GameManager.ins.data.lastSpinTime));
        }
        else
        {
            timeLeft.text = "";
        }
    }
    public void OnSettingClick()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        CanvasManager.ins.OpenSetting();
    }

    public void OnClose()
    {
        anim.Play("Hide");
        VibrationsManager.instance.TriggerLightImpact();
        StartCoroutine(ie_Close());
    }

    private IEnumerator ie_Close()
    {
        yield return Yielders.Get(0.5f);
        gameObject.SetActive(false);
        GameHelper.RemoveChildren(levelHolder);
    }

    public void OnPlay()
    {
        VibrationsManager.instance.TriggerLightImpact();
        CanvasManager.ins.canvasIngame.SetActive(true);
        GameManager.ins.StartPlaying();
        OnClose();
    }

    public void OnOpen()
    {
        gameObject.SetActive(true);

        SetupLevel();
    }

    public void OnSkinShopClick()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        CanvasManager.ins.OpenSkinShop();
    }

    public void OnMonsterShopClick()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        CanvasManager.ins.OpenMonstershop();
    }

    public void OnDaylySpinClick()
    {
        OnClose();
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        CanvasManager.ins.OpenWheel_Daily();
    }

    public void SetupLevel()
    {
        currentLevel = GameManager.ins.data.level + 1;
        lvTxt.text = "LEVEL " + currentLevel.ToString();
        gemTxt.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);

        int price = 0;
        if (GameManager.ins.data.wallLevel > GameConfig.ins.wall.wallUpgradeData.Count - 1)
        {
            price = GameConfig.ins.wall.wallUpgradeData[GameConfig.ins.wall.wallUpgradeData.Count - 1].price + 50 * (GameManager.ins.data.wallLevel - GameConfig.ins.wall.wallUpgradeData.Count);
        }
        else
        {
            price = GameConfig.ins.wall.wallUpgradeData[GameManager.ins.data.wallLevel].price;
        }

        //chỉnh button ẩn hiện
        skinBtn.SetActive(true);
        monsterBtn.SetActive(true);
        if (currentLevel == 1)
        {
            skinBtn.SetActive(false);
            monsterBtn.SetActive(false);
        }
        else if (currentLevel == 2)
        {
            skinBtn.SetActive(false);
            monsterBtn.SetActive(false);
        }
        else if (currentLevel == 3)
        {
            skinBtn.SetActive(false);
        }
        Debug.Log("levelevel: "+currentLevel);

        //Chỉnh thanh level trên đầu
        if (currentLevel == 1 || currentLevel == 2 || currentLevel == 3)
        {
            for (int i = 0; i < 5; i++)
            {
                bool isMons = false;
                bool isChest = false;
                bool isMiddle = false;
                GameObject levelPre = Instantiate(levelPrefab);
                Debug.Log("instan lv " + i);
                levelPre.transform.parent = levelHolder;
                levelPre.transform.localScale = Vector3.one;

                int insertLevel = 1 + i;
                if (insertLevel == 5 || insertLevel == 11 || insertLevel == 17) isMons = true;
                if (insertLevel == 6 || insertLevel == 12 || insertLevel == 18) isChest = true;
                if (currentLevel == insertLevel) isMiddle = true;

                levelPre.GetComponent<LevelItem>().Setup(insertLevel, isMons, isChest, isMiddle, currentLevel);
            }
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            bool isMons = false;
            bool isChest = false;
            bool isMiddle = false;
            GameObject levelPre = Instantiate(levelPrefab);
            levelPre.transform.parent = levelHolder;
            levelPre.transform.localScale = Vector3.one;

            int insertLevel = currentLevel + i - 2;
            if (insertLevel == 5 || insertLevel == 11 || insertLevel == 17) isMons = true;
            if (insertLevel == 6 || insertLevel == 12 || insertLevel == 18) isChest = true;
            if (i == 2 && currentLevel != 1 && currentLevel != 2) isMiddle = true;

            levelPre.GetComponent<LevelItem>().Setup(insertLevel, isMons, isChest, isMiddle, currentLevel);
        }
    }

    private int CountUpTime()
    {
        int count = 0;
        int currentGold = GameManager.ins.data.gemCollected;
        int currentWallLevel = GameManager.ins.data.wallLevel;
        while (currentGold > 0)
        {
            int price = 0;
            if (currentWallLevel > GameConfig.ins.wall.wallUpgradeData.Count - 1)
            {
                price = GameConfig.ins.wall.wallUpgradeData[GameConfig.ins.wall.wallUpgradeData.Count - 1].price + 50 * (currentWallLevel - GameConfig.ins.wall.wallUpgradeData.Count);
            }
            else
            {
                price = GameConfig.ins.wall.wallUpgradeData[currentWallLevel].price;
            }
            currentGold -= price;
            currentWallLevel++;
            count++;
        }
        return count;
    }
}
