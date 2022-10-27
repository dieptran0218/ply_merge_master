using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
using DG.Tweening;

public class CanvasFail : MonoBehaviour
{
    public Transform justRotate;
    public Text reviveTime;
    public GameObject canvasLose;
    //public GameObject noTksBtn;
    //public GameObject blockClickBtn;
    public Text lvTxt;
    public Text lvLoseTxt;
    public Image[] monster_icons;
    private int leftTime;
    private float saveTime = 0;
    private float timeKeep;

    // Update is called once per frame
    //void Update()
    //{
    //    //timeKeep += Time.deltaTime;
    //    if (leftTime < 0) return;
    //    justRotate.Rotate(0f, 0f, -200f * Time.deltaTime, Space.Self);
    //    saveTime += Time.deltaTime;
    //    if (saveTime > 1)
    //    {
    //        leftTime--;
    //        if (leftTime < 0)
    //        {
    //            if (GameManager_PLY_V2.Instance.m_GoToStore)
    //            {
    //                GameManager_PLY_V2.Instance.m_GoToStore = false;
    //                GameManager_PLY_V2.Instance.ClickStore();
    //                SoundController.PlaySoundOneShot(SoundController.ins.gameLose);

    //            }

    //            blockClickBtn.SetActive(true);
    //            //noTksBtn.gameObject.SetActive(false);
    //            canvasLose.SetActive(true);

    //            //GameManager.ins.data.failCount++;
    //            //FirebaseManager.Ins.check_point_end(GameManager.ins.data.level, GameManager.ins.data.level.ToString(), false);
    //            //FirebaseManager.Ins.level_fail(GameManager.ins.data.level.ToString(), GameManager.ins.data.failCount);
    //        }
    //        else
    //        {
    //            reviveTime.text = leftTime.ToString();
    //        }
    //        saveTime = 0;
    //    }
    //}

    private void Setup(int time)
    {
        leftTime = time;
        reviveTime.text = leftTime.ToString();
        timeKeep = 0;
        lvTxt.text = "LEVEL BOSS";
        //StartCoroutine(ie_ShowNoTks());
    }

    private void Start()
    {
        Invoke("ShowEndCard", 0.7f);
    }

    public void ShowEndCard()
    {
        //blockClickBtn.SetActive(true);
        //noTksBtn.gameObject.SetActive(false);
        canvasLose.SetActive(true);
        DOTween.Sequence()
            .Append(lvLoseTxt.DOColor(new Color(0, 0, 0, 1), 0.5f))
            .AppendInterval(1.5f)
            .Append(lvLoseTxt.DOColor(new Color(0, 0, 0, 0), 1f)
                .OnComplete(
                    () =>
                    {
                        DOTween.Sequence()
                        .Append(monster_icons[0].DOColor(new Color(1, 1, 1, 1), 0.5f))
                        .Join(monster_icons[1].DOColor(new Color(1, 1, 1, 1), 0.5f))
                        .Join(monster_icons[2].DOColor(new Color(1, 1, 1, 1), 0.5f));
                    }
                )
           );

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
        //VibrationsManager.instance.TriggerLightImpact();
        GameManager.ins.ReLoadGame();
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        //noTksBtn.SetActive(false);
    }

    public void OnOpen(int time = 5)
    {
        gameObject.SetActive(true);
    }

    private IEnumerator ie_ShowNoTks()
    {
        yield return new WaitForSeconds(3f);
        //noTksBtn.SetActive(true);
    }
}
