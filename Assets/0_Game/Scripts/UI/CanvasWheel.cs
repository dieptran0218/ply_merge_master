using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasWheel : MonoBehaviour
{
    public bool isEndGame;
    public bool isDaily;
    public GameObject wheel;
    public GameObject spinBtn;
    public GameObject adsBtn;
    public GameObject claimBtn;
    public GameObject objReward;
    public Text txtReward;
    public Text txtGem;
    public Text txtTittle;

    public GameObject levelTxt;
    public GameObject gemAllTxt;
    public GameObject gemCollectedTxt;

    private float rotSpeed;
    private float currentRot;
    private float timeTrack;
    private bool isSpeedup = true;
    private bool isStop;
    private bool isWin;

    public int _rw;
    public int _ratio;
    public bool isDone;

    public void Init(int rewardCollected, bool isWin)
    {
        isStop = false;
        isDone = false;
        this.isWin = isWin;
        this._rw = rewardCollected;
        if (isDaily)
        {
            _isRoll = false;
            spinBtn.GetComponent<Button>().interactable = true;
            spinBtn.gameObject.SetActive(true);
            txtGem.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
        }
        if(isEndGame)
        {
            txtTittle.text = isWin ? "Level Complete" : "Level Fail";
            objReward.SetActive(true);
            txtReward.text = GameHelper.ConvertNumber(rewardCollected);
            _isRoll = true;
            spinBtn.gameObject.SetActive(false);
            txtGem.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
        }
        wheel.transform.localRotation = Quaternion.identity;
        currentRot = 0;

        adsBtn.gameObject.SetActive(false);
        claimBtn.gameObject.SetActive(false);
    }

    void RefreshReward()
    {
        float left = currentRot % 360;
        if (left >= 330 || left < 30)
        {
            if (isEndGame)
            {
                _ratio = 2;
                txtReward.text = GameHelper.ConvertNumber(_rw * 2);
            }
            if(isDaily)
            {
                _rw = 100;
            }
        }
        else if (left >= 30 && left < 90)
        {
            if (isEndGame)
            {
                _ratio = 3;
                txtReward.text = GameHelper.ConvertNumber(_rw * 3);
            }
            if (isDaily)
            {
                _rw = 250;
            }
        }
        else if (left >= 90 && left < 150)
        {
            if (isEndGame)
            {
                _ratio = 4;
                txtReward.text = GameHelper.ConvertNumber(_rw * 4);
            }
            if (isDaily)
            {
                _rw = 500;
            }
        }
        else if (left >= 150 && left < 210)
        {
            if (isEndGame)
            {
                _ratio = 2;
                txtReward.text = GameHelper.ConvertNumber(_rw * 2);
            }
            if (isDaily)
            {
                _rw = 850;
            }
        }
        else if (left >= 210 && left < 270)
        {
            if (isEndGame)
            {
                _ratio = 3;
                txtReward.text = GameHelper.ConvertNumber(_rw * 3);
            }
            if (isDaily)
            {
                _rw = 150;
            }
        }
        else if (left >= 270 && left < 330)
        {
            if (isEndGame)
            {
                _ratio = 2;
                txtReward.text = GameHelper.ConvertNumber(_rw * 2);
            }
            if (isDaily)
            {
                _rw = 1500;
            }
        }
    }

    private bool _isRoll;
    void Update()
    {
        if (!_isRoll) return;

        timeTrack += Time.deltaTime;
        if (timeTrack < 0.3f || isStop) return;
        if (isSpeedup)
        {
            if (rotSpeed < 1500f)
            {
                rotSpeed += 20f;
            }
            wheel.transform.Rotate(0, 0, rotSpeed * Time.deltaTime);
            currentRot += rotSpeed * Time.deltaTime;
            if (currentRot > 1000f)
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

            }
        }
        if(!isStop) RefreshReward();
    }

    private void HandleEnd()
    {
        isStop = true;
        float left = currentRot % 360;

        #region Reward
        if (left >= 330 || left < 30)
        {
            if (isEndGame)
            {
                _rw *= 2;
            }
            if (isDaily)
            {
                _rw = 100;
            }
        }
        else if (left >= 30 && left < 90)
        {
            if (isEndGame)
            {
                _rw *= 3;
            }
            if (isDaily)
            {
                _rw = 250;
            }
        }
        else if (left >= 90 && left < 150)
        {
            if (isEndGame)
            {
                _rw *= 4;
            }
            if (isDaily)
            {
                _rw = 500;
            }
        }
        else if (left >= 150 && left < 210)
        {
            if (isEndGame)
            {
                _rw *= 2;
            }
            if (isDaily)
            {
                _rw = 850;
            }
        }
        else if (left >= 210 && left < 270)
        {
            if (isEndGame)
            {
                _rw *= 3;
            }
            if (isDaily)
            {
                _rw = 150;
            }
        }
        else if (left >= 270 && left < 330)
        {
            if (isEndGame)
            {
                _rw *= 2;
            }
            if (isDaily)
            {
                _rw = 1500;
            }
        }
        #endregion

        txtReward.text = GameHelper.ConvertNumber(_rw);

        if (isEndGame)
        {
            if (!isWin)
            {
                spinBtn.gameObject.SetActive(false);
                adsBtn.SetActive(true);
                claimBtn.SetActive(true);
            }
            else
            {
                spinBtn.gameObject.SetActive(false);
                adsBtn.SetActive(false);
                claimBtn.SetActive(false);
                Timer.Schedule(this, 1f, () =>
                {
                    OnClickClaim();
                });
            }
        }

        if (isDaily)
        {
            spinBtn.gameObject.SetActive(false);
            adsBtn.SetActive(true);
            claimBtn.SetActive(true);
        }
    }

    public void OnClickAds()
    {
        //AdsManager.Ins.ShowRewardedAd("x3_wheel_lose", () =>
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
        spinBtn.GetComponent<Button>().interactable = false;
        _isRoll = true;
    }

    public void OnClose()
    {
        if(isDaily) CanvasManager.ins.OpenHome();
        GetComponent<Animator>().Play("Hide");

        Timer.Schedule(this, 1f, () =>
        {
            gameObject.SetActive(false);
            adsBtn.SetActive(false);
            claimBtn.SetActive(false);
            isDone = true;
        });
    }
}
