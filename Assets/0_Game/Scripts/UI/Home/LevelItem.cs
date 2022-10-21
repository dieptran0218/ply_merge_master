using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelItem : MonoBehaviour
{
    public GameObject norLv;
    public GameObject disLv;
    public GameObject curLv;
    public Text lvTxt;
    public GameObject monsIcon;
    public GameObject disMonsIcon;
    public GameObject chestIcon;
    public GameObject disChestIcon;
    public Text bossTxt;

    public void Setup(int level, bool isMons, bool isChest, bool isMiddle, int currentLv)
    {
        if (false)
        {
            norLv.SetActive(false);
            disLv.SetActive(false);
            curLv.SetActive(false);
            lvTxt.text = "";
            bossTxt.gameObject.SetActive(true);
            chestIcon.SetActive(false);
            disChestIcon.SetActive(false);
            monsIcon.SetActive(true);
            disMonsIcon.SetActive(false);
            if (level > currentLv) disMonsIcon.SetActive(true);
        }
        else if (isChest)
        {
            norLv.SetActive(false);
            disLv.SetActive(false);
            curLv.SetActive(false);
            lvTxt.text = "";
            bossTxt.gameObject.SetActive(false);
            monsIcon.SetActive(false);
            disMonsIcon.SetActive(false);
            chestIcon.SetActive(true);
            disChestIcon.SetActive(false);
            if (level > currentLv) disChestIcon.SetActive(true);
        }
        else
        {
            chestIcon.SetActive(false);
            disChestIcon.SetActive(false);
            monsIcon.SetActive(false);
            disMonsIcon.SetActive(false);
            lvTxt.text = level.ToString();
            bossTxt.gameObject.SetActive(false);
            norLv.SetActive(true);
            disLv.SetActive(false);
            if (level > currentLv) disLv.SetActive(true);
            if (isMiddle) curLv.SetActive(true);
        }
        if (isMiddle)
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }

    }
}
