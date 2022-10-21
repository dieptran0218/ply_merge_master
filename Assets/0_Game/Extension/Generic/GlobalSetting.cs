using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSetting : SingletonMonoBehaviour<GlobalSetting>
{
    #region Inspector Variables
    #endregion

    #region Member Variables
    [HideInInspector]
    public bool canShowAds = true;
    [HideInInspector]
    public bool isShowPopup = false;
    private float _fps,_deltaTime =0;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (FindObjectsOfType(typeof(GlobalSetting)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;        
        canShowAds = PlayerPrefs.GetInt(Const.SHOW_AD, 1) == 1;
        DontDestroyOnLoad(this);
        
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        //Game FPS
        //_deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        //_fps = 1.0f / _deltaTime;
    }
    #endregion

    #region Public Methods    
    #endregion

    #region Private Methods
    #endregion
}

