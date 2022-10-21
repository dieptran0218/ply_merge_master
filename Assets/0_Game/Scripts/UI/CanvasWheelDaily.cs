using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasWheelDaily : MonoBehaviour
{

    public GameObject wheel;
    public GameObject spinBtn;
    public GameObject adsBtn;
    public GameObject claimBtn;
    public GameObject closeBtn;
    public Text txtGem;

    //Spin Btn
    public Text txtSpin;
    public Text txtSpinAd;
    public GameObject adsImg;
    public GameObject bgAdsImg;

    public GameObject levelTxt;
    public GameObject gemAllTxt;
    public GameObject gemCollectedTxt;

    private float rotSpeed;
    public float currentRot;
    private float timeTrack;
    private bool isSpeedup = true;

    public int _rw;
    private bool _isRoll;
    private bool isRollAd;

    public void Init(int rewardCollected, bool isWin)
    {
        this._rw = rewardCollected;

        _isRoll = false;
        rotSpeed = 0;
        currentRot = 0;
        timeTrack = 0;
        isSpeedup = true;
        closeBtn.SetActive(true);
        wheel.transform.localRotation = Quaternion.identity;

        //Set up spin ads
        if (GameHelper.CurrentTimeInSecond - GameManager.ins.data.lastSpinTime < 300)
        {
            isRollAd = true;
            txtSpin.gameObject.SetActive(false);
            txtSpinAd.gameObject.SetActive(true);
            adsImg.SetActive(true);
            bgAdsImg.SetActive(true);
        }
        else
        {
            isRollAd = false;
            txtSpin.gameObject.SetActive(true);
            txtSpinAd.gameObject.SetActive(false);
            adsImg.SetActive(false);
            bgAdsImg.SetActive(false);
        }

        spinBtn.GetComponent<Button>().interactable = true;
        spinBtn.gameObject.SetActive(true);
        txtGem.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);

        adsBtn.gameObject.SetActive(false);
        claimBtn.gameObject.SetActive(false);
    }

    void RefreshReward()
    {
        float left = currentRot % 360;
        if (left >= 330 || left < 30)
        {
            _rw = 100;
        }
        else if (left >= 30 && left < 90)
        {
            _rw = 250;
        }
        else if (left >= 90 && left < 150)
        {
            _rw = 500;
        }
        else if (left >= 150 && left < 210)
        {
            _rw = 850;
        }
        else if (left >= 210 && left < 270)
        {
            _rw = 150;
        }
        else if (left >= 270 && left < 330)
        {
            _rw = 1500;
        }
    }

    void Update()
    {
        if (!_isRoll) return;

        timeTrack += Time.deltaTime;
        if (timeTrack < 0.3f) return;
        if (isSpeedup)
        {
            if (rotSpeed < 2000)
            {
                rotSpeed += 15f;
            }
            wheel.transform.Rotate(0, 0, rotSpeed * Time.deltaTime);
            currentRot += rotSpeed * Time.deltaTime;
            if (currentRot > 1500f)
            {
                isSpeedup = false;
            }
        }
        else
        {
            if (rotSpeed > 0)
            {
                rotSpeed -= Random.Range(5f, 10f);
                wheel.transform.Rotate(0, 0, rotSpeed * Time.deltaTime);
                currentRot += rotSpeed * Time.deltaTime;
            }
            else
            {
                HandleEnd();
                RefreshReward();
            }
        }
        RefreshReward();
    }

    private void HandleEnd()
    {
        _isRoll = false;
        float left = currentRot % 360;

        #region Reward
        if (left >= 330 || left < 30)
        {
            _rw = 100;
        }
        else if (left >= 30 && left < 90)
        {
            _rw = 250;
        }
        else if (left >= 90 && left < 150)
        {
            _rw = 500;
        }
        else if (left >= 150 && left < 210)
        {
            _rw = 850;
        }
        else if (left >= 210 && left < 270)
        {
            _rw = 150;
        }
        else if (left >= 270 && left < 330)
        {
            _rw = 1500;
        }
        #endregion

        spinBtn.gameObject.SetActive(false);
        if (isRollAd) isRollAd = false;
        //GetComponent<Animator>().Play("None");
        GetComponent<Animator>().enabled = false;
        adsBtn.transform.localScale = Vector3.one;
        claimBtn.transform.localScale = Vector3.one;
        adsBtn.SetActive(true);
        claimBtn.SetActive(true);
    }

    public void OnClickAds()
    {
        //AdsManager.Ins.ShowRewardedAd("x3_daily_spin", () =>
        //{
        //    AdsDone();
        //    OnClose();
        //});
    }

    public void AdsDone()
    {
        _rw *= 3;
        GameManager.ins.data.gemCollected += _rw;
    }

    public void OnClickClaim()
    {
        GameManager.ins.data.gemCollected += _rw;
        OnClose();
    }

    public void OnOpen(int rewardCollected, bool isWin)
    {
        Init(rewardCollected, isWin);
        gameObject.SetActive(true);
    }

    public void OnSpin()
    {
        if (isRollAd)
        {
            //AdsManager.Ins.ShowRewardedAd("respin_daily_spin", () =>
            //{
            //    closeBtn.SetActive(false);
            //    spinBtn.SetActive(false);
            //    _isRoll = true;
            //});
        }
        else
        {
            GameManager.ins.data.lastSpinTime = GameHelper.CurrentTimeInSecond;
            closeBtn.SetActive(false);
            spinBtn.SetActive(false);
            _isRoll = true;
        }
    }


    public void OnClose()
    {
        CanvasManager.ins.OpenHome();
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().Play("Hide");

        Timer.Schedule(this, 1f, () =>
        {
            gameObject.SetActive(false);
            adsBtn.SetActive(false);
            claimBtn.SetActive(false);
        });
    }
}
