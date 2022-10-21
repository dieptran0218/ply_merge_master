using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : SingletonMonoBehaviour<TestManager>
{
    public Toggle toggleEnable;
    public bool isTest;

    private void OnEnable()
    {
        isTest = toggleEnable.enabled;
    }

    public void changeEnable()
    {
        isTest = toggleEnable.enabled;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    #region Currency
    [Header("=======Currency=======")]
    public Text txtGold;
    public Text txtGem;
    public Text txtEnergy;

    public void AddGold()
    {
        try
        {
            var gold = int.Parse(txtGold.text);
            //add gold
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void AddGem()
    {
        try
        {
            var gem = int.Parse(txtGem.text);
            //add gold
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void AddEnergy()
    {
        try
        {
            var e = int.Parse(txtEnergy.text);
            //add gold
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    #endregion

    #region Marketing
    [Header("=======Marketing=======")]
    public Toggle toggleBlueBg;
    public Toggle toggleNerverDie;
    public bool isBlueBg;
    public bool isNerverDie;

    public void BlueBgChange()
    {

    }

    public void NerverDieChange()
    {

    }
    #endregion

    #region Game Setting
    [Header("=======Game Setting=======")]
    public Text txtLevelOpen;

    public void ClearGameData()
    {

    }

    public void OpenLevel()
    {
        try
        {
            var lv = int.Parse(txtLevelOpen.text);
            //open level
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    #endregion

    #region ....
    #endregion
}
