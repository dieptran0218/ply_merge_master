using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    public Animator anim;
    public Transform playerGroup;
    public Transform playerHolder;
    public Transform transParent;
    public Transform trans;
    public Transform transRotate;
    public Transform playerX;
    public float speed;
    public float speedX;
    public float speedXPlayer;
    public Transform objFocus;
    public List<Transform> listRoad;
    public PlayerStage stage;
    public Transform skinParent;
    private Transform oldModel;

    [Header("Gioi han di chuyen")]
    [SerializeField] private InputMouse _mouseInput;
    public float minX;
    public float maxX;

    [Header("Game Params")]
    public int ballCollected;
    public int energyCollected;
    public int evolutionCollected;
    public int keyCollected;

    [Header("Obj Throw")]
    public Transform transThrow;
    public GameObject objThrow;

    [Header("Xep doi hinh")]
    public List<Pokemon> listPokemon = new List<Pokemon>();

    public Transform group_1;
    public Transform group_2;
    public Transform group_3;
    public Transform group_4;
    public Transform group_5;
    public Transform group_6;

    [HideInInspector] public bool _isCanEvo;

    private Gate _gateTrigger;
    private Vector3[] _curPath;
    [HideInInspector] public EndGame _endgame;

    public GameObject shopLight;

    private List<PokemonType> _pokemonDie = new List<PokemonType>();

    #region Init
    public void LoadCharacter()
    {
        if (oldModel != null)
            GameObject.Destroy(oldModel.gameObject);
        //SkinCharData currentChar = GameManager.ins.data.GetChar(GameManager.ins.data.charUsed);
        //GameObject charModel = Instantiate(currentChar.charModel);
        //charModel.transform.parent = playerHolder;
        //charModel.transform.localScale = Vector3.one * 0.8f;
        //charModel.transform.localPosition = new Vector3(0, 0, 0);
        //charModel.transform.rotation = Quaternion.Euler(0, 180, 0);
        //oldModel = charModel.transform;
        //anim = charModel.GetComponent<Animator>();
        //skinParent = charModel.transform;
        //transRotate = charModel.transform;
    }

    public void Setup()
    {
        objFocus = GameManager.ins.mapCurrent.transPosFocus;
        ballCollected = 0;
        energyCollected = 0;
        evolutionCollected = 0;
        keyCollected = 0;
        _isCanEvo = true;
        stage = PlayerStage.Idle;
    }

    public void SpawnPokemon(PokemonType typePokemon, int lv = 1)
    {
        var info = GameConfig.ins.GetPokemon(typePokemon, lv);
        var o = SimplePool.Spawn(GameConfig.ins.objPrefabPokemon, transform.position, Quaternion.identity).transform;
        var scr = o.GetComponent<Pokemon>();
        var trans = AddPokemon(scr);
        o.SetParent(transform.parent);
        o.position = trans.position - Vector3.right * 0f;
        scr.Setup(info, true, GameManager.ins.GetMode());
        scr.stage = PokemonStage.Move;
        scr._isFirstInPlayer = true;
        scr._targetFocus = trans;
    }

    public void StartPlaying()
    {
        playerGroup.SetParent(GameManager.ins.mapCurrent.transPosFocus);
        _curPath = CreatePath();
        objFocus.DOPath(_curPath, speed, PathType.CatmullRom)
            .SetSpeedBased(true)
            .SetLookAt(0)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
            });
        stage = PlayerStage.Run;
        CameraController.ins.SetCameraGamePlay();
    }
    #endregion

    #region Action

    Vector3 newCamPos;
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GameManager.ins.mapCurrent.endgame.OnBtnFight();
        //    return;
        //}
        //if (!GameManager.ins.isGamePlaying) return;

        //if (stage == PlayerStage.Die)
        //{
        //    return;
        //}

        //if (stage == PlayerStage.GetHit)
        //{
        //    return;
        //}

        //if (stage == PlayerStage.EndGame
        //    || stage == PlayerStage.EndGame_Win
        //    || stage == PlayerStage.EndGame_Lose
        //    || stage == PlayerStage.Combat)
        //{
        //    return;
        //}

        //if (stage != PlayerStage.TakePokemon) stage = PlayerStage.Run;
        ////Di chuyển player (anim) theo chiều ngang
        //playerX.localPosition = new Vector3(playerX.localPosition.x + (_mouseInput.MoveFactorX * speedX * 0.001f), playerX.localPosition.y, playerX.localPosition.z);
        //trans.localPosition = Vector3.LerpUnclamped(trans.localPosition, playerX.localPosition, speedXPlayer * Time.deltaTime);
        //Rotate(_mouseInput.MoveFactorX);
        //playerX.localPosition = new Vector3(Mathf.Clamp(playerX.localPosition.x, minX, maxX), playerX.localPosition.y, playerX.localPosition.z);
        //trans.localPosition = new Vector3(Mathf.Clamp(trans.localPosition.x, minX, maxX), trans.localPosition.y, trans.localPosition.z);

        //newCamPos = new Vector3(Mathf.Clamp(playerX.localPosition.x * 0.3f, minX * 0.3f, maxX * 0.3f), CameraController.ins.transform.localPosition.y, CameraController.ins.transform.localPosition.z);

        ////Chỉnh camera
        //CameraController.ins.transform.localPosition
        //    = Vector3.Lerp
        //    (
        //        CameraController.ins.transform.localPosition
        //        , newCamPos
        //        , rCam
        //    );
    }

    Coroutine i_throwItem;
    void ThrowItem(ItemType type)
    {
        if (type == ItemType.Ads) return;
        if (i_throwItem != null) StopCoroutine(i_throwItem);
        i_throwItem = StartCoroutine(ie_throwItem(type));
    }

    private float _dir = -0.2f;
    IEnumerator ie_throwItem(ItemType type)
    {
        var gateTmp = _gateTrigger;
        var spd = 1f;

        yield return Yielders.Get(0.15f);

        if (type == ItemType.Pokeball || type == ItemType.Key)
        {
            spd = 1f / _gateTrigger.numRequire;
            spd = Mathf.Min(spd, 0.25f);
            spd = Mathf.Max(spd, 0.15f);
        }
        else
        {
            spd = 1f / _gateTrigger.numRequire;
            spd = Mathf.Min(spd, 0.18f);
            spd = Mathf.Max(spd, 0.08f);
        }

        for (var i = _gateTrigger.numRequire; i > 0; i--)
        {
            if (_gateTrigger == null || !CheckHasItem(type)) yield break;

            AddItem(type, -1);
            var pA2 = new Vector3(_dir, 0, 0);
            _dir *= -1;
            var _t = SimplePool
                .Spawn(objThrow, Vector3.zero, Quaternion.identity)
                .GetComponent<ThrowItem>();
            VibrationsManager.instance.TriggerLightImpact();
            _t.transform.SetParent(transThrow);
            _t.transform.localPosition = pA2;
            _t.transform.SetParent(null);
            _t.Setup(type, _gateTrigger);
            CanvasInGame.ins.ReLoad(type);
            yield return Yielders.Get(spd);
        }
    }

    private void GetHit(Pokemon po = null)
    {
        //Còn pokemon thì không chết được
        if (listPokemon.Count > 0)
        {
            //var r = UnityEngine.Random.Range(0, listPokemon.Count);
            //var t = listPokemon[r];
            //if (t != null && !t._isDied)
            //{
            //    t.GetHit();
            //}
            //KnockBack();
            for(var i = listPokemon.Count - 1; i >= 0; i--)
            {
                var t = listPokemon[i];
                if (t != null && !t._isDied) t.Die();
            }
            KnockBack(SetDie);
        }
        //Hết pokemon thì chúc mừng em
        else
        {
            KnockBack(SetDie);
        }

    }

    private void GetHitSpecialMode(bool isDie, bool isKnockback = true)
    {
        if (isDie)
        {
            if (isKnockback) KnockBack(SetDie);
            listPokemon[0].Die();
            return;
        }
        if (isKnockback) KnockBack();
    }

    public bool CheckHasPokemon()
    {
        foreach (var t in listPokemon) if (!(t.stage == PokemonStage.Die)) return true;
        return false;
    }

    public void SetDie()
    {
        if (stage == PlayerStage.Die) return;
        stage = PlayerStage.Die;
        ReloadAnimation();

        Timer.Schedule(this, 2f, () =>
        {
            CanvasManager.ins.canvasFail.OnOpen();
        });
    }

    public void Revive()
    {
        GameManager.ins.isGamePlaying = false;
        foreach (var t in _pokemonDie)
        {
            SpawnPokemon(t, 1);
        }

        stage = PlayerStage.Idle;
        ReloadAnimation();
        Time.timeScale = 0.6f;
        StartCoroutine(ie_timeScale());
    }

    IEnumerator ie_timeScale()
    {
        yield return Yielders.Get(0.5f);
        GameManager.ins.isGamePlaying = true;
        objFocus.DOPlayForward();
        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.deltaTime * 0.3f;
            yield return Yielders.FixedUpdate;
        }
    }

    private void stopMove()
    {
        objFocus.DOPause();
    }

    private void MoveContinue()
    {
        objFocus.DOPause();
        objFocus.DOPlayForward();
    }

    private void KnockBack(Action c = null)
    {
        stage = PlayerStage.GetHit;
        objFocus.DOPause();
        objFocus.DOPlayBackwards();
        StartCoroutine(ie_KnockBack(c));
    }

    IEnumerator ie_KnockBack(Action c)
    {
        if (c != null)
        {
            transRotate.DOLocalMoveY(transRotate.localPosition.y + 0.5f, 0.4f)
                .SetEase(Ease.OutQuint)
                .OnComplete(() =>
                {
                    transRotate.DOLocalMoveY(transRotate.localPosition.y - 0.5f, 0.4f)
                    .SetEase(Ease.InQuint)
                    .OnComplete(() =>
                    {
                        SoundController.PlaySoundOneShot(SoundController.ins.player_drop_ground);
                    });
                });
            yield return Yielders.Get(0.5f);
            c.Invoke();
            yield return Yielders.Get(0.7f);
            stopMove();
            yield return Yielders.Get(2f);
            CanvasManager.ins.canvasFail.OnOpen();
            yield break;
        }
        else
        {
            yield return Yielders.Get(1f);
            stopMove();
            yield return Yielders.Get(0.1f);
            stage = PlayerStage.Run;
            MoveContinue();
        }

    }

    public bool CheckEndGame()
    {

        if (listPokemon.Count == 0)
        {
            Time.timeScale = 1f;
            StartCoroutine(ie_LoseAction());
            stage = PlayerStage.EndGame_Lose;
            GameManager_PLY_V2.Instance.EndGame();
            return true;

        }

        return false;
    }

    private bool _isEndgame;
    IEnumerator ie_WinAction()
    {
        if (_isEndgame) yield break;

        yield return Yielders.Get(1f);

        SoundController.PlaySoundOneShot(SoundController.ins.firework);
        _endgame.fx_firework_1.SetActive(true);
        _endgame.fx_firework_2.SetActive(true);

        if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Combat_Normal)
        {
            CameraController.ins.SetCameraEndGame_3_Step_3();
            PlayerMoveToTower();
            yield return Yielders.Get(4.5f);
        }
        else yield return Yielders.Get(3f);

        //if(GameManager.ins.data.level >= 2) //AdsManager.Ins.ShowInterstitial("endgame_win");
        //else if(//AdsManager.Ins.isShowAdsInLv1_2) //AdsManager.Ins.ShowInterstitial("endgame_win");

        CanvasManager.ins.OpenWheel_EndGame((int)(GameManager.ins.GetRewardEndGame()), true);

        //yield return new WaitUntil(() => CanvasManager.ins.canvasWheel_EndGame.isDone);

        _isEndgame = true;

        //doing something...
        foreach (var t in listPokemon)
        {
            t.stage = PokemonStage.Win;
            t.SetAnimation(PokemonAnimStage.Idle);
            t.SetWin();
        }
        yield return Yielders.Get(0.2f);
        //if (stage == PlayerStage.EndGame_Win) CanvasManager.ins.canvasWin.OnOpen(1);
        //else if (stage == PlayerStage.EndGame_Lose) CanvasManager.ins.canvasFail.OnOpen(0);

    }

    IEnumerator ie_LoseAction()
    {
        yield return new WaitForSeconds(0.8f);
        CanvasManager.ins.OpenFail();
    }

    public float GetFailRatio()
    {
        var curHp = 0;
        foreach (var t in _endgame._lstPokemonBoss)
        {
            curHp += t._hp;
        }
        return 1 - (float)curHp / _endgame.totalHp;
    }

    public void PlayerMoveToTower()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        anim.SetInteger("stage", 1);
        trans.DOMove(_endgame.posMoveTower[0].position, 2.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                trans.position = _endgame.posMoveTower[1].position;
                transRotate.localRotation = Quaternion.Euler(0, 180, 0);
                trans.DOMove(_endgame.posMoveTower[2].position, 0.5f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _endgame.firework.Setup();
                        anim.SetInteger("stage", 6);
                    });
            });
    }

    #endregion

    #region Animation
    public PlayerStage ani_Apply;

    void LateUpdate()
    {
        if (stage == PlayerStage.Die
            || GameManager.ins == null
            || !GameManager.ins.isGamePlaying)
        {
            return;
        }

        ReloadAnimation();
    }

    public void ReloadAnimation()
    {
        //Nếu trạng thái animation thay đổi
        if (ani_Apply == stage)
            return;

        //Bật animation sau mỗi frame
        ani_Apply = stage;
        anim.speed = 1;
        ClearAnim();

        switch (stage)
        {
            case PlayerStage.Idle:
                anim.SetInteger("stage", 0);
                break;
            case PlayerStage.EndGame:
                anim.SetInteger("stage", 0);
                break;
            case PlayerStage.Run:
                anim.SetInteger("stage", 1);
                break;
            case PlayerStage.GetHit:
                anim.SetInteger("stage", 2);
                break;
            case PlayerStage.TakePokemon:
                anim.SetInteger("stage", 3);
                break;
            case PlayerStage.Die:
                anim.SetInteger("stage", 4);
                break;
            case PlayerStage.Victory:
                anim.SetInteger("stage", 5);
                break;
            case PlayerStage.Combat:
                anim.SetInteger("stage", 6);
                break;
            case PlayerStage.IdleShop:
                anim.SetInteger("stage", 7);
                break;
            default:
                Debug.LogError("Lỗi animationState Player" + stage.ToString());
                break;
        }
    }

    protected void ClearAnim()
    {
        anim.SetInteger("stage", -1);
    }

    public void CallTakePokemon()
    {
        StartCoroutine(ie_CallTakePokemon());
    }

    IEnumerator ie_CallTakePokemon()
    {
        //Time.timeScale = 0.9f;
        stage = PlayerStage.TakePokemon;
        anim.Play("TakePokemon");
        transRotate.DOLocalMoveY(1.25f, 0.4f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transRotate.DOLocalMoveY(0f, 0.35f)
                    .SetEase(Ease.InQuad);
            });
        transRotate.DOLocalRotate(Vector3.up * 360, 0.75f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transRotate.localRotation = Quaternion.Euler(0, 0, 0);
            });
        yield return Yielders.Get(0.75f);
        //Time.timeScale = 1f;
        stage = PlayerStage.Run;
    }
    #endregion

    [HideInInspector] public int _countPokemon;
    #region Collission
    public void OnTriggerEnter(Collider other)
    {
        //VibrationsManager.instance.TriggerLightImpact();
        if (other.tag.Equals("Pokeball"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Ball, other.transform.position + transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            ballCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Pokeball);
        }
        else if (other.tag.Equals("Gem"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Gem, other.transform.position + transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            //GameManager.ins.data.gemCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Gem);
        }
        else if (other.tag.Equals("Energy"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Energy, other.transform.position + transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            energyCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Energy);
        }
        else if (other.tag.Equals("Evolution"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Evolution, other.transform.position + transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            evolutionCollected += 1;
        }
        else if (other.tag.Equals("Boom"))
        {
            SoundController.PlaySoundOneShot(SoundController.ins.get_boom);
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Boom, other.transform.position + transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            evolutionCollected -= 1;

            GetHitSpecialMode(evolutionCollected <= 0, false);
        }
        else if (other.tag.Equals("Key"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Key, other.transform.position + transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            keyCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Key);
        }
        else if (other.tag.Equals("GateLand"))
        {
            var o = other.GetComponent<GateLand>();
            if (o != null && o.gate.numRequire > 0 && CheckHasItem(o.gate.typeRequire))
            {
                var g = o.gate;
                if (_gateTrigger != g)
                {
                    _gateTrigger = g;
                    ThrowItem(g.typeRequire);
                }
            }
        }
        else if (other.tag.Equals("GateLevelUp"))
        {
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_CollectPokemon, other.transform.position);
            other.enabled = false;
            LevelUp();
            SoundController.PlaySoundOneShot(SoundController.ins.pokemon_levelup);
        }
        else if (other.tag.Equals("GateLevelDown"))
        {
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_LevelDown, other.transform.position);
            other.enabled = false;
            foreach (var t in listPokemon) t.LevelUp(-1, false);
            SoundController.PlaySoundOneShot(SoundController.ins.pokemon_leveldown);
        }
        else if (other.tag.Equals("GatePokemonAds"))
        {
            other.enabled = false;
            //AdsManager.Ins.ShowRewardedAd("pokemon_gate", () =>
            //{
            //    other.transform.parent.parent.GetComponent<Gate>().ActionOpenGatePokemon();
            //});
            
        }
        else if (other.tag.Equals("Trap"))
        {
            SoundController.PlaySoundOneShot(SoundController.ins.boy_get_hit);

            if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Combat_Normal) GetHit();
            else
            {
                evolutionCollected -= 8;
                GetHitSpecialMode(evolutionCollected <= 0);
                evolutionCollected = Mathf.Max(evolutionCollected, 0);
            }

            other.enabled = false;
            Timer.Schedule(this, 2f, () => { other.enabled = true; });
        }
        else if (other.tag.Equals("EndGameGate"))
        {
            var e = other.GetComponent<EndGame>();
            if (e != null)
            {
                _countPokemon = listPokemon.Count;
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
                other.GetComponent<Collider>().enabled = false;
                _endgame = e;
                foreach (WaveBall w in GameManager.ins.mapCurrent.listWave) {
                    w.gameObject.SetActive(false);
                }
                GameManager.ins.mapCurrent.listWave.Clear();
                PlayerEndgame(e);
                //CanvasManager.ins.canvasEvolution.OnClose();
            }
        }
        else if (other.tag.Equals("Limit"))
        {
            var e = other.GetComponent<LimitRoad>();
            if (e != null)
            {
                minX = e.minX;
                maxX = e.maxX;
            }
        }
    }

    public void LevelUp(int insLv = 1)
    {
        foreach (var t in listPokemon) t.LevelUp(insLv, false);
    }

    private void PlayerEndgame(EndGame e)
    {
        StartCoroutine(ie_PlayerEndGame(e));
    }

    IEnumerator ie_PlayerEndGame(EndGame e)
    {
        if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Combat_Normal
            || GameManager.ins.mapCurrent.typeEndGame == EndGameType.Chest)
        {
            Endgame3_CeilManager.ins.SetupListTmp(PlayerController.ins.listPokemon.Count);
            CameraController.ins.SetCameraEndGame_3_Step_1();
            stage = PlayerStage.EndGame;

            for (var i = 0; i < listPokemon.Count && i < 5; i++)
            {
                listPokemon[i].SetTriggerStatus(true);
                listPokemon[i].stage = PokemonStage.Endgame;
                _endgame.ceilManager.Merge(listPokemon[i]);
            }

        }

        if (listPokemon.Count > 0)
        {
            yield return Yielders.Get(0);
            if (Endgame3_CeilManager.ins._countCeilTmp > 0)
            {
                Timer.Schedule(this, 1.5f, () =>
                {
                    CanvasManager.ins.canvasFight.OnShow_2();
                });
            }
            else
            {
                Timer.Schedule(this, 1.5f, () =>
                {
                    CanvasManager.ins.canvasFight.OnShow_2();
                });
                PlayerController.ins.MoveToEndMerge();
            }
        }
        else
        {
            Timer.Schedule(this, 1.5f, () =>
            {
                CanvasManager.ins.canvasFight.OnShow_2();
            });
            PlayerController.ins.MoveToEndMerge();
        }
    }

    public void MoveToEndMerge()
    {
        anim.SetInteger("stage", 1);
        transform.DOMove(_endgame.transPlayer.position, 1f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                anim.SetInteger("stage", 0);
                CanvasFight.ins.Refresh();
            });
    }

    private bool CheckHasItem(ItemType typeRequire)
    {
        switch (typeRequire)
        {
            case ItemType.Energy:
                return energyCollected > 0;
            case ItemType.Evolution:
                return evolutionCollected > 0;
            case ItemType.Key:
                return keyCollected > 0;
            case ItemType.Pokeball:
                return ballCollected > 0;
        }
        return false;
    }

    public void AddItem(ItemType typeRequire, int value)
    {
        switch (typeRequire)
        {
            case ItemType.Energy:
                energyCollected += value;
                break;
            case ItemType.Evolution:
                evolutionCollected += value;
                break;
            case ItemType.Gem:
                //GameManager.ins.data.gemCollected += value;
                break;
            case ItemType.Key:
                keyCollected += value;
                break;
            case ItemType.Pokeball:
                ballCollected += value;
                break;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("GateLand"))
        {
            _gateTrigger = null;
            if (i_throwItem != null) StopCoroutine(i_throwItem);
        }
    }

    #endregion

    #region Support
    float rY;
    float _t;
    float rCam;
    private void Rotate(float value)
    {
        if (stage == PlayerStage.TakePokemon) return;
        if (Input.GetMouseButton(0))
        {
            if (value > 0)
            {
                _t = 0;
                if (rY < 0)
                {
                    rCam = 0;
                    rY = 0;
                }
                else
                {
                    rY += 300 * Time.deltaTime;
                    rCam += Time.deltaTime * 0.05f;
                    rCam = Mathf.Min(rCam, 0.05f);
                }
            }
            else if (value < 0)
            {
                _t = 0;
                if (rY > 0)
                {
                    rCam = 0;
                    rY = 0;
                }
                else
                {
                    rY -= 300 * Time.deltaTime;
                    rCam += Time.deltaTime * 0.05f;
                    rCam = Mathf.Min(rCam, 0.05f);
                }
            }
            else
            {
                _t += Time.deltaTime;

                if (_t >= 0f)
                {
                    rY = rY.Lerp(0, 1f);
                }
            }
        }
        else
        {
            rY = rY.Lerp(0, 1f);
        }

        transRotate.localRotation = Quaternion.Euler(new Vector3(0, Mathf.Clamp(rY, -15, 15), 0));
    }

    public Vector3[] CreatePath()
    {
        var map = GameManager.ins.mapCurrent;

        var path = new List<Vector3>();
        foreach (var t in map.listRoadItems)
        {
            foreach (var t1 in t.lstPoint)
            {
                path.Add(t1.position);
            }
        }
        return path.ToArray();
    }

    public Transform AddPokemon(Pokemon po)
    {
        listPokemon.Add(po);
        if (listPokemon.Count == 1)
        {
            return group_1.GetChild(0);
        }
        else if (listPokemon.Count == 2)
        {
            //Move old pokemon to new pos
            //...
            listPokemon[0].SetNewPos(group_2.GetChild(0));
            return group_2.GetChild(1);
        }
        else if (listPokemon.Count == 3)
        {
            //Move old pokemon to new pos
            //...
            listPokemon[0].SetNewPos(group_3.GetChild(0));
            listPokemon[1].SetNewPos(group_3.GetChild(1));
            return group_3.GetChild(2);
        }
        else if (listPokemon.Count == 4)
        {
            //Move old pokemon to new pos
            //...
            listPokemon[0].SetNewPos(group_4.GetChild(0));
            listPokemon[1].SetNewPos(group_4.GetChild(1));
            listPokemon[2].SetNewPos(group_4.GetChild(2));
            return group_4.GetChild(3);
        }
        else if (listPokemon.Count == 5)
        {
            //Move old pokemon to new pos
            //...
            listPokemon[0].SetNewPos(group_5.GetChild(0));
            listPokemon[1].SetNewPos(group_5.GetChild(1));
            listPokemon[2].SetNewPos(group_5.GetChild(2));
            listPokemon[3].SetNewPos(group_5.GetChild(3));
            return group_5.GetChild(4);
        }
        else if (listPokemon.Count == 6)
        {
            //Move old pokemon to new pos
            //...
            listPokemon[0].SetNewPos(group_6.GetChild(0));
            listPokemon[1].SetNewPos(group_6.GetChild(1));
            listPokemon[2].SetNewPos(group_6.GetChild(2));
            listPokemon[3].SetNewPos(group_6.GetChild(3));
            listPokemon[4].SetNewPos(group_6.GetChild(4));
            return group_6.GetChild(5);
        }
        return null;
    }

    public void RemovePokemon(Pokemon po)
    {
        listPokemon.Remove(po);
        ReLoadFormation();
    }

    public void ReLoadFormation()
    {
        if (listPokemon.Count == 1)
        {
            listPokemon[0].SetNewPos(group_1.GetChild(0));
        }
        else if (listPokemon.Count == 2)
        {
            //Move old pokemon to new pos
            //...
            listPokemon[0].SetNewPos(group_2.GetChild(0));
            listPokemon[1].SetNewPos(group_2.GetChild(1));
        }
        else if (listPokemon.Count == 3)
        {
            //Move old pokemon to new pos
            //...
            listPokemon[0].SetNewPos(group_3.GetChild(0));
            listPokemon[1].SetNewPos(group_3.GetChild(1));
            listPokemon[2].SetNewPos(group_3.GetChild(2));
        }
    }

    public void AddPokemonDie(PokemonType type)
    {
        foreach (var t in _pokemonDie)
        {
            if (t == type) return;
        }
        _pokemonDie.Add(type);
    }
    #endregion

    private GameObject _skinUsed;
    public void WearSkin(SkinId id)
    {
        var skin = GameConfig.ins.listAllSkin.skinData.Find(x => x.skinID == id);
        if (skin != null)
        {
            //if (GameManager.ins.data.charUsed == CharacterType.Cleopat)
            //{
            //    if (_skinUsed != null) SimplePool.Despawn(_skinUsed);
            //    var o = SimplePool.Spawn(skin.skinModel, Vector3.zero, Quaternion.identity);
            //    o.transform.SetParent(skinParent);
            //    o.transform.localScale = Vector3.one;
            //    o.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //    o.transform.localPosition = new Vector3(0, -0.185f, 0.037f);
            //    _skinUsed = o;
            //}
            //else
            //{
            //    if (_skinUsed != null) SimplePool.Despawn(_skinUsed);
            //    var o = SimplePool.Spawn(skin.skinModel, Vector3.zero, Quaternion.identity);
            //    o.transform.SetParent(skinParent);
            //    o.transform.localScale = Vector3.one;
            //    o.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //    o.transform.localPosition = new Vector3(0, -0.269f, 0.1f);
            //    _skinUsed = o;
            //}
        }
    }
}


public enum PlayerStage
{
    Idle,
    Run,
    TakePokemon,
    GetHit,
    Die,
    Victory,
    EndGame,
    Combat,
    IdleShop,
    EndGame_Win,
    EndGame_Lose
}