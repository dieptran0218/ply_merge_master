using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class CanvasFail : MonoBehaviour
{
    public Transform justRotate;
    public Text reviveTime;
    public GameObject canvasLose;
    public GameObject noTksBtn;
    public GameObject blockClickBtn;
    public Text lvTxt;
    public Text lvLoseTxt;
    private int leftTime;
    private float saveTime = 0;
    private float timeKeep;

    // Update is called once per frame
    void Update()
    {
        //timeKeep += Time.deltaTime;
        if (leftTime < 0) return;
        justRotate.Rotate(0f, 0f, -200f * Time.deltaTime, Space.Self);
        saveTime += Time.deltaTime;
        if (saveTime > 1)
        {
            leftTime--;
            if (leftTime < 0)
            {
                SoundController.PlaySoundOneShot(SoundController.ins.gameLose);
                blockClickBtn.SetActive(true);
                noTksBtn.gameObject.SetActive(false);
                canvasLose.SetActive(true);

                GameManager.ins.data.failCount++;
                //FirebaseManager.Ins.check_point_end(GameManager.ins.data.level, GameManager.ins.data.level.ToString(), false);
                //FirebaseManager.Ins.level_fail(GameManager.ins.data.level.ToString(), GameManager.ins.data.failCount);
            }
            else
            {
                reviveTime.text = leftTime.ToString();
            }
            saveTime = 0;
        }
    }

    private void Setup(int time)
    {
        leftTime = time;
        reviveTime.text = leftTime.ToString();
        timeKeep = 0;
        lvTxt.text = "LEVEL " + (GameManager.ins.data.level + 1).ToString();
        lvLoseTxt.text = "LEVEL " + (GameManager.ins.data.level + 1).ToString();
        canvasLose.SetActive(false);
        blockClickBtn.SetActive(false);
        StartCoroutine(ie_ShowNoTks());
    }

    public void OnClickRevive()
    {
        SoundController.ins.UI_Click();
        //AdsManager.Ins.ShowRewardedAd("revive", ReviveDone);
    }

    public void ReviveDone()
    {
        PlayerController.ins.trans.localPosition = Vector3.zero;
        //GameManager.ins.ReLoadGame();
        VibrationsManager.instance.TriggerLightImpact();
        PlayerController.ins.Revive();
        OnClose();
    }

    public void OnClickNoTks()
    {
        SoundController.ins.UI_Click();
        //AdsManager.Ins.ShowInterstitial();
        VibrationsManager.instance.TriggerLightImpact();
        GameManager.ins.ReLoadGame();
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        noTksBtn.SetActive(false);
    }

    public void OnOpen(int time = 5)
    {
        gameObject.SetActive(true);
        Setup(time);
    }

    private IEnumerator ie_ShowNoTks()
    {
        yield return new WaitForSeconds(3f);
        noTksBtn.SetActive(true);
    }
}
