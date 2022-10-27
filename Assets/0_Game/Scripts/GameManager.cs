using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool isTest;
    public int lvTest = -1;
    public PokemonType pokemonTest;

    public MapManager mapCurrent;
    public bool isGamePlaying;
    public DataManager data;
    [HideInInspector] public bool _isSetupDone;
    public CanvasSelectChar canvasSelectChar;

    [HideInInspector] public int timePlayed;

    private void Start()
    {
        StartCoroutine(ie_Reload());
    }

    IEnumerator ie_Reload()
    {
        isGamePlaying = false;
        _isSetupDone = false;
        yield return new WaitUntil(() =>
            GameConfig.ins != null
            && LoadingScene.ins != null
            && SoundController.ins != null
        );

        SoundController.ins.PlayBgSound();

        LoadingScene.ins.SetPercent(0.35f, 1f);

        LoadData();

        yield return Yielders.Get(1f);

        LoadingScene.ins.SetPercent(0.45f, 1f);

        yield return Yielders.Get(1f);

        LoadingScene.ins.SetPercent(0.7f, 1f);
        yield return Yielders.Get(1f);

        if (GameManager.ins.data.charUsed == CharacterType.None)
        {
            var checkReward = false;
            Timer.Schedule(this, 3f, () => checkReward = true);

            //yield return new WaitUntil(() => checkReward || MaxManager.Ins.isVideoLoaded);

            LoadingScene.ins.Close();
            OpenSelectChar();
        }

        yield return new WaitUntil(() => GameManager.ins.data.charUsed != CharacterType.None);
        if(canvasSelectChar != null) Destroy(canvasSelectChar.gameObject);

        AsyncOperation sync = null;

        ScreenSwitch.ins.ShowOn(() =>
        {
            sync = SceneManager.LoadSceneAsync("Game");
        });

        yield return new WaitUntil(() => sync != null && sync.isDone);

        LoadingScene.ins.SetPercent(1f, 0.5f);

        yield return new WaitUntil(() =>
            PlayerController.ins != null
            && LevelManager.ins != null
            && MapParent.ins != null
        );

        SaveData();

        //yield return new WaitUntil

        ScreenSwitch.ins.Show(() =>
        {
            //Vào đến game rồi thì không hiện AOA nữa
            //AdsManager.Ins.DropAOA();

            PlayerController.ins.LoadCharacter();
            if(canvasSelectChar != null) canvasSelectChar.OnClose();
            CameraController.ins.SetCameraHome();
            LoadingScene.ins.Close();
            if (data.skinUsed != SkinId.None)
                PlayerController.ins.WearSkin(data.skinUsed);
            if (mapCurrent != null) Destroy(mapCurrent.gameObject);
            var p = LevelManager.ins.GetMap(lvTest >= 0 ? lvTest : data.level);

            MapParent.ins.transform.RemoveAllChild();
            var o = Instantiate(p, Vector3.zero, Quaternion.identity).transform;
            o.gameObject.SetActive(true);
            o.SetParent(MapParent.ins.transform);
            o.localPosition = Vector3.zero;
            o.localScale = Vector3.one;
            mapCurrent = o.GetComponent<MapManager>();

            PreloadInStartGame();

            //AnimationController.ins.Setup();

            CanvasManager.ins.OpenHome();
            _isSetupDone = true;

            //FirebaseManager.Ins.check_point_start(data.level, data.level.ToString());
            //FirebaseManager.Ins.level_start(GameManager.ins.data.level.ToString());
        }, 1f);
        StartCoroutine(ie_Clear());
    }

    IEnumerator ie_Clear()
    {
        while(true)
        {
            yield return Yielders.Get(10f);
            System.GC.Collect();
        }
    }

    IEnumerator ie_ResetGame()
    {
        isGamePlaying = false;
        _isSetupDone = false;
        yield return new WaitUntil(() =>
            GameConfig.ins != null
            && LoadingScene.ins != null
            && PlayerController.ins != null
            && LevelManager.ins != null
            && MapParent.ins != null
        );

        CameraController.ins.SetCameraHome();

        if (mapCurrent != null) Destroy(mapCurrent.gameObject);

        var p = LevelManager.ins.GetMap(lvTest >= 0 ? lvTest : data.level);

        PlayerController.ins.LoadCharacter();
        if (data.skinUsed != SkinId.None)
            PlayerController.ins.WearSkin(data.skinUsed);


        MapParent.ins.transform.RemoveAllChild();
        var o = Instantiate(p, Vector3.zero, Quaternion.identity).transform;
        o.gameObject.SetActive(true);
        o.SetParent(MapParent.ins.transform);
        o.localPosition = Vector3.zero;
        o.localScale = Vector3.one;
        mapCurrent = o.GetComponent<MapManager>();

        PreloadInStartGame();

        //AnimationController.ins.Setup();

        CanvasManager.ins.OpenHome();
        _isSetupDone = true;

        //FirebaseManager.Ins.OnSetUserProperty();
        //FirebaseManager.Ins.check_point_start(data.level, data.level.ToString());
        //FirebaseManager.Ins.level_start(GameManager.ins.data.level.ToString());
    }

    [ContextMenu("play")]
    public void StartPlaying()
    {
        PlayerController.ins.Setup();
        timePlayed = 0;
        isGamePlaying = true;
        PlayerController.ins.StartPlaying();
        CanvasManager.ins.CloseHome();
        if (mapCurrent.typeEndGame == EndGameType.BigMonster)
        {
            //CanvasManager.ins.canvasEvolution.OnOpen();
        }
        CanvasManager.ins.OpenIngame();
    }

    public void SaveData()
    {
        var str = "PLAYER_DATA_POKEMON";
        //var json = JsonUtility.ToJson(data);
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(str, json);
    }

    public void LoadData()
    {
        var d = PlayerPrefs.GetString("PLAYER_DATA_POKEMON", "");
        if (d != "")
        {
            data = JsonUtility.FromJson<DataManager>(d);
        }
        else
        {
            data = new DataManager();
        }

        data.currentPokemon = Mathf.Max(data.currentPokemon, 3);
        if (data.pokemonCollected.Count < 3)
        {
            data.pokemonCollected.Clear();
            for (int i = 0; i < data.currentPokemon; i++)
            {
                data.pokemonCollected.Add(GameConfig.ins.PokemonList[i].type);
            }
        }

        if (data.charCollected.Count == 0)
        {
            data.charCollected.Add(CharacterType.Stickman);
        }

        //Load / Create ceil endgame
        if (data.endgame_CeilInfo == null || data.endgame_CeilInfo.Count == 0)
        {
            if (data.endgame_CeilInfo == null) data.endgame_CeilInfo = new List<CeilInfo>();

            for (var i = 0; i < data.totalCeilOpened; i++)
            {
                var tmp = new CeilInfo(i);

                //Test MODE
                if (isTest)
                {
                    tmp.levelUpdate = 1;
                    if (i < 3) tmp.type = pokemonTest;
                    else tmp.type = PokemonType.Bee;
                }

                data.endgame_CeilInfo.Add(tmp);
            }
        }
        else
        {
            foreach (var t in data.endgame_CeilInfo) t.preLevelUpdate = 0;
        }

        //FirebaseManager.Ins.OnSetUserProperty();
    }

    public Mode GetMode()
    {
        if (mapCurrent.typeEndGame == EndGameType.Combat_Normal) return Mode.Normal;
        else if (mapCurrent.typeEndGame == EndGameType.BigMonster) return Mode.BigBoss;
        else return Mode.Chest;
    }

    public void ReLoadGame()
    {
        SaveData();
        ScreenSwitch.ins.Show(() =>
        {
            SceneManager.LoadScene("Game");
            StartCoroutine(ie_ResetGame());
        });

    }

    public void OpenSelectChar()
    {
        if (canvasSelectChar != null)
            canvasSelectChar.OnOpen();
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_EDITOR
        return;
#else
        if(pause) SaveData();
#endif
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void PreLoadPokemon(PokemonType type)
    {
        for (var i = 1; i <= 3; i++)
        {
            var info = GameConfig.ins.GetPokemon(type, i);
            SimplePool.Preload(info.objPrefab);
        }
    }

    public void PreloadGameObject(GameObject obj, int num)
    {
        SimplePool.Preload(obj, num);
    }

    public void PreloadInStartGame()
    {
        PreloadGameObject(GameConfig.ins.fx_CollectPokemon, 1);
        PreloadGameObject(GameConfig.ins.fx_FireWork_End, 3);
        PreloadGameObject(GameConfig.ins.fx_FireWork_OpenChest, 3);
        PreloadGameObject(GameConfig.ins.fx_Get_Ball, 10);
        PreloadGameObject(GameConfig.ins.fx_Get_Boom, 5);
        PreloadGameObject(GameConfig.ins.fx_Get_Gem, 10);
        PreloadGameObject(GameConfig.ins.fx_Get_Key, 10);
    }

    public void PreloadBullet(Projectile type, int num)
    {

    }

    #region Price + Reward
    public int GetSlotPrize()
    {
        if (data.level == 0 && PlayerController.ins._countPokemon >= 3 && Tutorial.ins != null) return 0;
        var res = (160 + (data.countBuySlot * 128)) * (Mathf.Pow(data.countInsPrize, 3) * 0.0625f);
        return (int) res;
    }

    public int GetUnitPrize()
    {
        if (data.level == 0 && Tutorial.ins != null) return 0;
        var res = (160 + (data.countBuyUnit * 128)) * (Mathf.Pow(data.countInsPrize, 2) * 0.0625f);
        return (int)res;
    }

    public int GetRewardEndGame()
    {
        var res = 40  * Mathf.Pow(data.countInsReward * 2, 2) * 0.0625f;
        return (int)res;
    }
    #endregion
}
