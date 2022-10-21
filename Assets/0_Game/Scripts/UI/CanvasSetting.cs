using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class CanvasSetting : SingletonMonoBehaviour<CanvasSetting>
{

    public Transform btnMusic;
    public Transform btnSound;
    public Transform btnVibra;

    public bool isMusicOn = true;
    public bool isSoundOn = true;
    public bool isHapticOn = true ;

    public GameObject objCheat;

    #region Setup
    public void Show()
    {
        ReloadUISettingMusic(SoundController.ins.OnMusic);
        ReloadUISettingSound(SoundController.ins.OnSound);
        ReloadUISettingVibra(SoundController.ins.OnVibration);
        gameObject.SetActive(true);
        //if (AdsManager.Ins.isMkt)
        //{
        //    objCheat.SetActive(true);
        //}
    }

    void ReloadUISettingSound(bool isOn)
    {
        btnSound.GetChild(0).gameObject.SetActive(isOn);
        btnSound.GetChild(1).gameObject.SetActive(!isOn);
    }

    void ReloadUISettingMusic(bool isOn)
    {
        btnMusic.GetChild(0).gameObject.SetActive(isOn);
        btnMusic.GetChild(1).gameObject.SetActive(!isOn);
    }

    void ReloadUISettingVibra(bool isOn)
    {
        btnVibra.GetChild(0).gameObject.SetActive(isOn);
        btnVibra.GetChild(1).gameObject.SetActive(!isOn);
    }

    #endregion

    public void BtnMusic()
    {
        SoundController.ins.UI_Click();
        SoundController.ins.ChangeMusic();
        ReloadUISettingMusic(SoundController.ins.OnMusic);
    }

    public void BtnSound()
    {
        SoundController.ins.UI_Click();
        SoundController.ins.ChangeSound();
        ReloadUISettingSound(SoundController.ins.OnSound);
    }

    public void BtnVibration()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        SoundController.ins.ChangeVibration();
        ReloadUISettingVibra(SoundController.ins.OnVibration);
    }

    public void Close()
    {
        VibrationsManager.instance.TriggerLightImpact();
        GetComponent<Animator>().Play("Popup_Hide");
        Timer.Schedule(this, 0.5f, () =>
        {
            gameObject.SetActive(false);
        });
        
    }
}
