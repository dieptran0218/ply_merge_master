using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class CanvasCombat2 : MonoBehaviour
{
    public Image fillImage;
    [HideInInspector] public bool isStop;
    [HideInInspector] public bool isDone;
    [HideInInspector] public float tapScore;

    private int _numDeg = 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStop || isDone) return;
        fillImage.fillAmount -= 0.3f * Time.deltaTime;
        if (fillImage.fillAmount <= 0)
        {
            HandleLose();
        }
    }

    public void HandleWin()
    {
        isDone = true;
    }

    public void HandleLose()
    {
        isDone = true;
    }

    public void HandleTouch()
    {
        if (isDone) return;
        SoundController.PlaySoundOneShot(SoundController.ins.tap);
        VibrationsManager.instance.TriggerLightImpact();
        isStop = false;
        fillImage.fillAmount += 0.125f;
        if (fillImage.fillAmount >= 1)
        {
            HandleWin();
        }

        if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Chest)
        {
            
        }
    }
    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    private float force = 0f;
    public void Setup()
    {
        fillImage.fillAmount = 0f;

        if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Combat_Normal)
        {
            force = 0.125f;
        }
        else
        {
            var t = PlayerController.ins.listPokemon[0].info.lv;
            if (t == 1)
            {
                force = 0.075f;
                _numDeg = 2;
            }
            else if (t == 2)
            {
                force = 0.1f;
                _numDeg = 1;
            }
            else if (t == 3)
            {
                force = 0.125f;
                _numDeg = 0;
            }
        }

        isStop = true;
        isDone = false;
    }

    public void OnOpen()
    {
        gameObject.SetActive(true);
        Setup();
    }
}
