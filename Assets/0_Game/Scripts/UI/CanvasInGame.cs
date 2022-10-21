using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class CanvasInGame : SingletonMonoBehaviour<CanvasInGame>
{
    public Text txtLevel;

    public GameObject objGem;
    public Text txtGem;
    public Transform icoGem;
    public Animation animGem;

    public GameObject objBall;
    public Text txtBall;
    public Transform icoBall;
    public Animation animBall;

    public GameObject objEnergy;
    public Text txtEnergy;
    public Transform icoEnergy;
    public Animation animEnergy;

    public GameObject objKey;
    public Text txtKey;
    public Transform icoKey;
    public Animation animKey;

    public GameObject slider;
    public GameObject btnReviveLayout;


    private void Start()
    {
        //txtLevel.text = "LEVEL " + (GameManager_PLY_V2.Instance.level + 1);
        txtGem.text = GameHelper.ConvertNumber(GameManager_PLY_V2.Instance.gemCollected);

        objGem.SetActive(true);
        objBall.SetActive(false);
        objEnergy.SetActive(false);
        objKey.SetActive(false);
    }


    public void AddKey(int key)
    {
        animKey.Play();
    }

    public void AddBall()
    {
        animBall.Play();
    }

    public void AddEnergy()
    {
        animEnergy.Play();
    }

    Coroutine i_addGem;
    public void AddGem(int gem)
    {
        animGem.Play();
        if (i_addGem != null) StopCoroutine(i_addGem);
        i_addGem = StartCoroutine(ie_AddGem(gem));
    }

    IEnumerator ie_AddGem(int gem)
    {
        yield return Yielders.Get(1f);
        var cur = GameManager_PLY_V2.Instance.gemCollected - gem;
        var t = GameManager_PLY_V2.Instance.gemCollected - cur;
        var spd = 1f / t;
        while (cur < GameManager_PLY_V2.Instance.gemCollected)
        {
            cur++;
            txtGem.text = GameHelper.ConvertNumber(cur);
            animGem.Play();
            yield return Yielders.Get(spd);
        }
    }

    public void ReloadGem()
    {
        txtGem.text = GameHelper.ConvertNumber(GameManager_PLY_V2.Instance.gemCollected);
    }

    public void ReLoad(ItemType type, int value = 0)
    {
        switch (type)
        {
            case ItemType.Gem:
                AddGem(value);
                break;
            case ItemType.Pokeball:
                AddBall();
                break;
            case ItemType.Energy:
                AddEnergy();
                break;
            case ItemType.Key:
                AddKey(value);
                break;
        }

        txtGem.text = GameHelper.ConvertNumber(GameManager_PLY_V2.Instance.gemCollected);
        txtBall.text = PlayerController.ins.ballCollected.ToString();
        txtEnergy.text = PlayerController.ins.energyCollected.ToString();
        txtKey.text = PlayerController.ins.keyCollected.ToString();
        if (PlayerController.ins.ballCollected > 0) objBall.SetActive(true);
        if (PlayerController.ins.energyCollected > 0) objEnergy.SetActive(true);
        if (PlayerController.ins.keyCollected > 0) objKey.SetActive(true);
    }

    public void ShowContinuePanel(bool b)
    {
        slider.SetActive(b);
        btnReviveLayout.SetActive(b);
    }

    public void BtnContinue()
    {
        Time.timeScale = 0.6f;
        StartCoroutine(ie_timeScale());
        ShowContinuePanel(false);
    }

    IEnumerator ie_timeScale()
    {
        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.deltaTime * 0.3f;
            yield return Yielders.FixedUpdate;
        }
    }
}
