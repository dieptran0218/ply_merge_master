using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luna.Unity;

public class GameManager_PLY_V2 : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isFirstTouched = false;
    public static GameManager_PLY_V2 Instance;
    public int gemCollected = 2000;
    public int m_SlotPrice = 1000;
    public int m_MonsterPrice = 1000;
    public int totalCeilOpened = 4;
    public bool isGamePlaying = true;
    public Pokemon[] MonsterList;
    public Endgame_3_Ceil[] Ceil_List;
    public Endgame_3_Ceil[] Ceil_Tmp_List;

    [LunaPlaygroundField("Go to store?", 0, "Game Settings")]
    public bool m_GoToStore = true;


    //CAMERA SETTINGS
    public Camera GPCamera;
    private bool rotateScreen = false;
    public float horizontalCameraSize = 9.5f;
    public float verticalCameraSize = 15f;
    public GameObject[] verticalObjects;
    public GameObject[] horizontalObjects;
    [LunaPlaygroundFieldStep(1f)]
    [LunaPlaygroundField("Fixed Xs Camera Size", 1, "Camera Settings")]
    [Range(50, 60)]
    public float FixedXsCameraSize = 55f;
    private bool isHor;

    void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        SetUpMonsters();
        isHor = !(Screen.width >= Screen.height);
        checkCameraSettings();
        LifeCycle.OnMute += MuteAudio;
        LifeCycle.OnUnmute += UnmuteAudio;
        SoundController.ins.PlayBgSound();
    }

    void MuteAudio()
    {
        AudioListener.volume = 0;
    }

    void UnmuteAudio()
    {
        AudioListener.volume = 1;
    }

    private void SetUpMonsters()
    {
        foreach(Pokemon aPokemon in MonsterList)
        {
            aPokemon.Setup(aPokemon.info, false, Mode.Normal, PokemonStage.Endgame);
        }
    }

    public int AddGem(int value)
    {
        gemCollected += value;
        gemCollected = Mathf.Max(gemCollected, 0);
        if (CanvasInGame.ins != null
            && CanvasInGame.ins.gameObject.activeInHierarchy) CanvasInGame.ins.ReloadGem();
        return gemCollected;
    }

    public void ClickStore()
    {
        Luna.Unity.Playable.InstallFullGame();
        m_GoToStore = false;
    }

    IEnumerator DelayStore()
    {
        yield return new WaitForSeconds(0.5f);
        if (m_GoToStore)
        {
            ClickStore();
        }
    }

    public void EndGame()
    {
        Luna.Unity.LifeCycle.GameEnded();
        //AutoStore();

    }

    private void FixedUpdate()
    {
        checkCameraSettings();
    }

    public void checkCameraSettings()
    {
        if (Screen.width < Screen.height && isHor)
        {
            //Debug.Log(Math.Abs(Screen.width / (float)Screen.height - 1125f / 2436f));
            //Debug.Log(Screen.width + " - " + Screen.height);
            if (Math.Abs(Screen.width / (float)Screen.height - 1125f / 2436f) < 0.1)
            {
                GPCamera.fieldOfView = FixedXsCameraSize;
            }
            //else if (Math.Abs(Screen.width / (float)Screen.height - 320f / 427f) < 0.1)
            //{
            //    camera.orthographicSize = FixediPadCameraSize;
            //}
            else
            {
                GPCamera.fieldOfView = verticalCameraSize;
            }
            isHor = false;

            if (rotateScreen)
            {
                rotateScreen = false;
                //fighter.movement.SetupMouseLimit(Constant.HORIZONTAL_MOUSE_X_LIMIT, Constant.HORIZONTAL_MOUSE_Y_LIMIT);
            }

            foreach (GameObject gameOb in verticalObjects)
            {
                if(gameOb != null) gameOb.SetActive(!isHor);
            }

            foreach (GameObject gameOb in horizontalObjects)
            {
                if (gameOb != null) gameOb.SetActive(isHor);
            }

            //SetObjectsPosition();
        }
        else if (Screen.width > Screen.height && !isHor)
        {
            GPCamera.fieldOfView = horizontalCameraSize;
            isHor = true;
            if (!rotateScreen)
            {
                rotateScreen = true;
                //fighter.movement.SetupMouseLimit(Constant.VERTICAL_MOUSE_X_LIMIT, Constant.VERTICAL_MOUSE_Y_LIMIT);
            }

            foreach (GameObject gameOb in verticalObjects)
            {
                gameOb.SetActive(!isHor);
            }

            foreach (GameObject gameOb in horizontalObjects)
            {
                gameOb.SetActive(isHor);
            }
        }
    }

}
