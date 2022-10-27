using System;
using UnityEngine;

public partial class SoundController : MonoBehaviour
{
    public static SoundController ins;
    [Header("[SOUND]")]
    public bool OnSound = true;
    [Header("[MUSIC]")]
    public bool OnMusic = true;
    [Header("[VIBRATION]")]
    public bool OnVibration = true;

    //Nhạc nền
    public AudioSource BackgroundSound;

    //Sound phát 1 lần, thường là sound effect
    public AudioSource SoundOne;

    //Sound phát lặp lại, thường là sound effect
    [HideInInspector] public AudioSource SoundLoop;

    void Awake()
    {
        if (ins == null)
        {
            ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SoundLoop = gameObject.AddComponent<AudioSource>();
    }

    #region Setting
    public void Start()
    {
        OnSound = PlayerPrefs.GetInt("SOUND", 1) == 1;
        OnMusic = PlayerPrefs.GetInt("MUSIC", 1) == 1;
        OnVibration = PlayerPrefs.GetInt("VIBRATION", 1) == 1;
    }

    public void ReloadSound(float volume) {
        if (volume == 0)
        {
            OnSound = false;
        }
        else
        {
            OnSound = true;
        }
        PlayerPrefs.SetInt("SOUND", OnSound ? 1 : 0);
    }

    public void ReloadMusic(float volume) {
        if (volume == 0)
        {
            OnMusic = false;
            StopBackgroundSound();
        }
        else
        {
            OnMusic = true;
        }
        PlayerPrefs.SetInt("MUSIC", OnMusic ? 1 : 0);
    }

    public void ChangeSound()
    {
        OnSound = !OnSound;
        PlayerPrefs.SetInt("SOUND", OnSound ? 1 : 0);
    }

    public void ChangeMusic()
    {
        OnMusic = !OnMusic;
        if (OnMusic) ResumeBackgroundSound();
        else StopBackgroundSound();
        PlayerPrefs.SetInt("MUSIC", OnMusic ? 1 : 0);
    }

    public void ChangeVibration() {
        OnVibration = !OnVibration;
        PlayerPrefs.SetInt("VIBRATION", OnVibration ? 1 : 0);
    }
    #endregion

    #region Background Sound
    public void PlayBgSound()
    {
        PlayBackgroundSound(ins.HomeBg);
        //Timer.Schedule(this, ins.InGameBg.Clip.length, () =>
        //{
        //    PlayBackgroundSound(ins.InGameBg);
        //});
    }

    public void PlayBg(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (OnMusic)
        {
            BackgroundSound.Stop();
            BackgroundSound.clip = CurrentBgClip;
            BackgroundSound.volume = volume;
            BackgroundSound.loop = loop;
            BackgroundSound.Play();
        }
    }

    public static AudioClip CurrentBgClip;
    public static void PlayBackgroundSound(AudioClip clip, float volume = 1, bool loop = true)
    {
        CurrentBgClip = clip;
        ins.PlayBg(CurrentBgClip, volume, loop);
    }

    public static void PlayBackgroundSound(SoundInfor infor = null, bool loop = true)
    {
        if (infor == null)
            ins.BackgroundSound.volume = 1;
        else
        {
            CurrentBgClip = infor.Clip;
            ins.PlayBg(CurrentBgClip, ins.OnMusic ? infor.Volume : 0, loop);
        }

    }

    public static void StopBackgroundSound()
    {
        ins.BackgroundSound.volume = 0;
        ins.BackgroundSound.Pause();
    }

    public static void ResumeBackgroundSound()
    {
        ins.BackgroundSound.volume = 1f;
        ins.BackgroundSound.Play();
        if(ins.BackgroundSound.clip == null)
        {
            ins.PlayBgSound();
        }
    }
    #endregion

    #region Effect Sound
    public void PlaySoundOneShot(AudioClip clip, float volume = 1)
    {
        if (!OnSound)
            return;

        SoundOne.clip = clip;
        SoundOne.volume = volume;
        SoundOne.PlayOneShot(clip);
    }

    public static void PlaySoundOneShot(SoundInfor infor)
    {
        if (infor.Clip == null)
            return;
        ins.PlaySoundOneShot(infor.Clip, infor.Volume);
    }
    #endregion

    #region Loop Sound
    public void PlaySoundLoop(AudioClip clip, float volume = 1)
    {
        if (!OnSound)
            return;

        SoundLoop.clip = clip;
        SoundLoop.volume = volume;
        SoundLoop.loop = true;
        SoundLoop.Play();
    }

    public static AudioSource PlaySoundLoop(SoundInfor infor)
    {
        if (infor.Clip == null)
            return null;
        ins.PlaySoundLoop(infor.Clip, infor.Volume);
        return ins.SoundLoop;
    }
    #endregion

    public static void StopSound()
    {
        ins.SoundOne.Stop();
    }

    public static void StopAll()
    {
        StopBackgroundSound();
        StopSound();
    }

    #region play sound

    public void UI_Click()
    {
        PlaySoundOneShot(ins.UIClick);
    }

    public void TriggerItem()
    {
        PlaySoundOneShot(ins.get_item);
    }

    public void PokemonAttack()
    {
        PlaySoundOneShot(ins.lstPokemonAttack[UnityEngine.Random.Range(0, ins.lstPokemonAttack.Count)]);
    }

    #endregion
}

[Serializable]
public class SoundInfor
{
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume = 1;
}
