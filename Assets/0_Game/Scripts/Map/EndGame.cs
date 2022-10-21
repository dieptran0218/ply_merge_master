using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public Transform posEnd;
    public Transform posCenter;

    [Header("=== MODE: Combat Normal ===")]
    public Endgame3_CeilManager ceilManager;
    public List<Transform> transEnemys;
    public Transform transBoss;
    public Transform transHumanBoss;
    public Transform transPlayer;
    public List<Transform> listPosPokemonPlayer;
    public Transform Parent;
    public GameObject ObjTower;
    public List<Transform> posMoveTower;
    public List<Pokemon> _lstPokemonBoss = new List<Pokemon>();
    public Firework firework;
    public EndgameBoss boss;

    [Header("=== MODE: Chest ===")]
    private GameObject objChest;
    [HideInInspector] public CrystalManager scrChest;

    [Header("=== MODE: Boss ===")]
    public Transform transPlayerInBoss;
    public Transform transPokemonInBoss;

    public GameObject fx_firework_1;
    public GameObject fx_firework_2;
    public static EndGame Instance;

    public int totalHp;
    //Setup in start game
    IEnumerator Start()
    {
        Instance = this;
        yield return new WaitUntil(() =>
            GameConfig.ins != null
            && GameManager.ins != null
            && GameManager.ins._isSetupDone
            && PlayerController.ins != null
        );

        ceilManager.Setup(GameManager.ins.data.totalCeilOpened);

        if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Combat_Normal)
        {
            var lv = PlayerPrefs.GetInt("LevelEndgame", 0);
            LoadMap(lv);
        }
        else if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Chest)
        {
            //spawn chest
            objChest = SimplePool.Spawn(GameConfig.ins.prefab_Crystal, transBoss.position, Quaternion.identity);
            objChest.transform.SetParent(transform);
            objChest.transform.localScale = Vector3.one * 0.5f;
            objChest.transform.localRotation = Quaternion.Euler(180, -90, -90);
            scrChest = objChest.GetComponent<CrystalManager>();
            ObjTower.SetActive(false);
            _lstPokemonBoss.Add(objChest.GetComponent<Pokemon>());
        }
        InputManager.ins.canTouch = true;
    }

    public void BossCallMonster()
    {
        boss.CallMonster(_lstPokemonBoss);
    }

    public void LoadMap(int lv)
    {
        if (endgameRegister.info.Count <= lv)
        {
            Debug.LogError("Chưa có map này !!!!! - id: " + lv);
            return;
        }
        else
        {
            _lstPokemonBoss.Clear();
            ClearMap();

            //Respawn all monster
            var lst = GameConfig.ins.endgameRegister.info[lv].listMonster;
            for (var i = 0; i < lst.Count; i++)
            {
                var o = SimplePool.Spawn(
                    GameConfig.ins.objPrefabPokemon, transHumanBoss.position, Quaternion.identity)
                    .GetComponent<Pokemon>();
                
                o.transform.SetParent(transParent);

                o.Setup(
                    GameConfig.ins.GetPokemon(lst[i].type, lst[i].lv)
                    , false
                    , Mode.Normal
                );

                totalHp += o.info.hp;

                _lstPokemonBoss.Add(o);

                lst[i].objTmp = o.gameObject;

                o.transform.localPosition = lst[i].localPosition;
                o.transform.localScale = lst[i].localScale;
                o.transform.localRotation = Quaternion.Euler(0, 90, 90);
                o.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
                o.gameObject.SetActive(false);
            }
        }
    }

    #region Mode Chest

    IEnumerator ie_ShowFireWork()
    {
        yield return Yielders.Get(0.5f);

        for (var i = 0; i < 6; i++)
        {
            var pos = PlayerController.ins.objFocus.position
                + PlayerController.ins.trans.forward * 8f
                + Vector3.up * 7
                + Random.insideUnitSphere * 5f;

            //GameConfig.ins.SpawnFx(GameConfig.ins.fx_FireWork_OpenChest
            //, pos);
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_FireWork_OpenChest, pos, 1.5f);


            if (i < 5) SoundController.PlaySoundOneShot(SoundController.ins.firework);
            //EfxManager.ins.GetGemFx_2(Camera.main.WorldToScreenPoint(pos), CanvasInGame.ins.icoGem.position);

            yield return Yielders.Get(0.3f);
        }
    }
    #endregion

    #region Move Normal

    public void OnBtnFight()
    {
        Time.timeScale = 1.2f;

        InputManager.ins.canTouch = false;
        CanvasManager.ins.canvasFight.OnHide();
        Endgame3_CeilManager.ins.HideAllMerge();

        //Ẩn tất cả các ô vuông trên bàn cờ
        foreach(var t in Endgame3_CeilManager.ins.sprCeils)
        {
            t.enabled = false;
            t.transform.GetChild(0).gameObject.SetActive(false);
        }

        //tất cả các pokemon trên ô đều là pokemon của player
        PlayerController.ins.listPokemon.Clear();
        var ceilParent = Endgame3_CeilManager.ins.listCeilManager;
        foreach(var t in ceilParent)
        {
            foreach(var t1 in t._mainPokemon)
            {
                PlayerController.ins.listPokemon.Add(t1);
            }
        }

        //Set trạng thái fighting cho tất cả pokemon trên sàn
        foreach (var t in PlayerController.ins.listPokemon)
        {
            t.transform.SetParent(Parent);
            t.transform.DOScale(Vector3.one * 1.25f, 1f);
            t.stage = PokemonStage.Attack_Normal;
            t.SetTriggerStatus(false);
            //t.pokemonEvent.ShowHpBar();
            if(t.info.type == PokemonType.Bomb)
            {
                Timer.Schedule(this, t.pokemonEvent.timeCooldown, (() =>
                {
                    t.pokemonEvent.Explosive();
                }));
            }
        }

        foreach(var t in _lstPokemonBoss)
        {
            if (t.info.type == PokemonType.Crystal) continue;

            t.transform.SetParent(Parent);
            t.stage = PokemonStage.Attack_Normal;
            t.SetTriggerStatus(false);
            //t.pokemonEvent.ShowHpBar();

            if (t.info.type == PokemonType.Bomb)
            {
                Timer.Schedule(this, t.pokemonEvent.timeCooldown, (() =>
                {
                    t.pokemonEvent.Explosive();
                }));
            }
        }

        CameraController.ins.SetCameraEndGame_3_Step_2();

        if(CrystalManager.ins != null)
        {
            CanvasInGame.ins.txtLevel.gameObject.SetActive(false);
            CrystalManager.ins.OnStart(10);
        }
    }
    #endregion

    #region Create Map
    [Header("====== XẾP MAP =======")]
    public GameConfig gameconfig;
    public int endgameId;
    public List<EndgameMonsterInfo> listMonster = new List<EndgameMonsterInfo>();
    public Transform transParent;
    public EndgameRegister endgameRegister;

    [Header("----------- SETUP MONSTER --------------")]
    public PokemonType monsterAdd;
    public int lv;
    public float scale = 1.5f;
    public float scalePower = 100;

#if UNITY_EDITOR

    [ContextMenu("Add Monster")]
    public void AddMonster()
    {
        var o = Instantiate(
                    gameconfig.objPrefabPokemon, transHumanBoss.position, Quaternion.identity)
                    .GetComponent<Pokemon>();
        o.transform.SetParent(transParent);
        o.SetupTmp(
            gameconfig.GetPokemon(monsterAdd, lv)
            , false
            , Mode.Normal
        );

        var mon = new EndgameMonsterInfo();
        mon.type = monsterAdd;
        mon.lv = lv;
        mon.scalePower = scalePower;
        mon.objTmp = o.gameObject;
        listMonster.Add(mon);

        o.transform.localPosition = Vector3.zero;
        o.transform.localScale = Vector3.one * scale;
        o.transform.localRotation = Quaternion.Euler(0, 90, 90);
        o.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);

        //Selection.activeGameObject = o.gameObject;
    }

    [ContextMenu("Load Map")]
    public void LoadMapTest()
    {
        if (endgameRegister.info.Count <= endgameId)
        {
            EditorUtility.DisplayDialog("Warning", "Map not register!", "OK");
        }
        else
        {
            ClearMap();

            //Respawn all monster
            var lst = endgameRegister.info[endgameId].listMonster;
            for (var i = 0; i < lst.Count; i++)
            {
                var o = Instantiate(
                    gameconfig.objPrefabPokemon, transHumanBoss.position, Quaternion.identity)
                    .GetComponent<Pokemon>();
                o.transform.SetParent(transParent);
                o.SetupTmp(
                    gameconfig.GetPokemon(lst[i].type, lst[i].lv)
                    , false
                    , Mode.Normal
                );
                lst[i].objTmp = o.gameObject;
                
                o.transform.localPosition = lst[i].localPosition;
                o.transform.localScale = lst[i].localScale;
                o.transform.localRotation = Quaternion.Euler(0, 90, 90);
                o.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }
            DeepClone(lst);
        }
    }

    public void DeepClone(List<EndgameMonsterInfo> clone)
    {
        listMonster.Clear();
        foreach(var t in clone)
        {
            var tmp = new EndgameMonsterInfo();
            tmp.type = t.type;
            tmp.lv = t.lv;
            tmp.scalePower = t.scalePower;
            tmp.localPosition = t.localPosition;
            tmp.localScale = t.localScale;
            tmp.typeAttack = t.typeAttack;
            tmp.objTmp = t.objTmp;
            listMonster.Add(tmp);
        }
    }

    [ContextMenu("Save Map")]
    public void SaveMap()
    {
        CreateMonsterList();
        if(endgameRegister.info.Count > 0 
            && endgameRegister.info.Count > endgameId)
        {
            if(EditorUtility.DisplayDialog("Info", "Map Override!", "OK", "Cancel"))
            {
                endgameRegister.info[endgameId].listMonster.Clear();
                foreach (var t1 in listMonster)
                {
                    endgameRegister.info[endgameId].listMonster.Add(t1);
                }
            }
        }
        else
        {
            if (EditorUtility.DisplayDialog("Info", "Map Create New!", "OK", "Cancel"))
            {
                endgameId = endgameRegister.info.Count;
                endgameRegister.info.Add(new EndgameRegisterInfo());
                var t = endgameRegister.info[endgameRegister.info.Count - 1];
                t.listMonster.Clear();
                foreach(var t1 in listMonster)
                {
                    t.listMonster.Add(t1);
                }
            }
        }
        EditorUtility.SetDirty(endgameRegister);
        AssetDatabase.SaveAssets();
    }

    void CreateMonsterList()
    {
        for(var i =0; i < listMonster.Count; i++)
        {
            var t = listMonster[i];
            if(t != null && t.objTmp != null)
            {
                t.localPosition = t.objTmp.transform.localPosition;
                t.localScale = t.objTmp.transform.localScale;
                t.objTmp = null;
            }
            else
            {
                listMonster.RemoveAt(i);
                i--;
            }
        }
    }

    [ContextMenu("Re:size")]
    public void ResizeAll()
    {
        foreach(var t in endgameRegister.info)
        {
            foreach(var t1 in t.listMonster)
            {
                if(t1.lv < 3)
                {
                    t1.localScale = Vector3.one * 1.5f;
                }
            }
        }
        EditorUtility.SetDirty(endgameRegister);
        AssetDatabase.SaveAssets();
    }
#endif

    [ContextMenu("Clear Map")]
    public void ClearMap()
    {
        transParent.RemoveAllChildOnEditor();
        listMonster.Clear();
    }
    #endregion
}