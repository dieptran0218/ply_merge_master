using DG.Tweening;
using System.Collections;
using UnityEngine;
using MoreMountains.NiceVibrations;
using System.Collections.Generic;

public class Pokemon : MonoBehaviour
{
    public PokemonInfo info;
    public Animator anim;
    public Transform _targetFocus;
    public Rigidbody rigid;
    public PokemonStage stage;
    public Mode mode;

    [Header("=== Parameter stage ===")]
    [SerializeField] protected float _speed;
    [HideInInspector] public bool _isDied;
    [HideInInspector] public Transform _transRotate;
    public PokemonEvent pokemonEvent;
    [HideInInspector] public bool _isFirstInPlayer;
    [HideInInspector] public bool isPlayerPokemon;
    [HideInInspector] public int index;
    protected float _minX;
    protected float _maxX;
    protected float _speedAttack = 0.75f;
    protected float _timeCooldown;
     public int _hp;
     public int _dam;
    protected float _range;
    protected int _maxHp;
    protected bool _isAttacking;
    protected Pokemon _target;
    protected TypeAttack _typeAttack;
    protected float _animSpeed;
    protected float _atkSpeed;
    protected int _countAttack;
    protected bool _isBuffing;
    protected float _timeSwitchTarget;
    protected float _curRange;
    protected bool _isTriggerWall;
    protected bool _isSkilling;
    //0: melee
    //1: range
    protected int priorityTarget;

    //Pollen
    protected bool _isDash;

    public List<Pokemon> _listTrigger = new List<Pokemon>();

    #region Init
    public void Setup(PokemonInfo po, bool isPlayer, Mode mode, PokemonStage stage = PokemonStage.Idle)
    {
        this.mode = mode;
        this.isPlayerPokemon = isPlayer;
        this.stage = stage;
        this.info = po;

        _isFirstInPlayer = false;
        _isAttacking = false;
        _animSpeed = 1f;
        _maxHp = po.hp;
        _hp = po.hp;
        _dam = po.dam;
        //if(!isPlayerPokemon && GameManager.ins.data.level == 0)
        //{
        //    _dam = 1;
        //}
        _range = po.range;
        _speedAttack = po.speedAttack;
        _timeCooldown = _speedAttack;
        _isDied = false;
        _isBuffing = false;
        _listTrigger.Clear();
        _timeSwitchTarget = 0;
        _isDash = false;
        _curRange = _range;
        _isTriggerWall = false;
        priorityTarget = 0;
        _isSkilling = false;

        if (_typeAttack == TypeAttack.Range)
        {
            _curRange = Random.Range(0.9f, 1.1f) * _range;
        }

        //spawn pokemon + reload anim
        {
            var o = SimplePool.Spawn(po.objPrefab, transform.position, Quaternion.identity).transform;
            transform.DespawnAllChild();
            o.SetParent(transform);
            o.transform.localRotation = Quaternion.Euler(180, 90, 90);
            _transRotate = o;
            pokemonEvent = o.transform.GetComponent<PokemonEvent>();
            pokemonEvent.setup(this);
            o.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            anim = pokemonEvent.GetComponent<Animator>();
            if (PlayerController.ins != null)
            {
                _minX = PlayerController.ins.minX;
                _maxX = PlayerController.ins.maxX;
            }
        }

        var col = GetComponent<CapsuleCollider>();
        col.radius = GetRadiusCollider(info.lv);
        //col.isTrigger = true;
    }

    public void SetTriggerStatus(bool b)
    {
        if (rigid != null)
        {
            rigid.isKinematic = b;
            GetComponent<Collider>().isTrigger = b;
        }
    }

    //Setup để view lúc xếp map
    public void SetupTmp(PokemonInfo po, bool isPlayer, Mode mode, PokemonStage stage = PokemonStage.Idle)
    {
        //spawn pokemon + reload anim
        var o = Instantiate(po.objPrefab, transform.position, Quaternion.identity).transform;
        transform.RemoveAllChildOnEditor();
        o.SetParent(transform);
        o.transform.localRotation = Quaternion.Euler(180, 90, 90);
        _transRotate = o;
        o.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    float GetRadiusCollider(int lv)
    {
        if (lv == 1) return 0.25f;
        else if (lv == 2) return 0.35f;
        else return 0.5f;
    }
    #endregion

    #region Run Time
    Vector3 _dir;
    public virtual void Update()
    {
        if (!GameManager_PLY_V2.Instance.isGamePlaying) return;


        if (_isDied)
        {
            //SetAnimation(PokemonAnimStage.Die);
            DOTween.Sequence(transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            }));
            return;
        }

        //Special Setup
        if (mode == Mode.Normal)
        {
            //Khi player bắt đầu chạm đến endgame
            if (stage == PokemonStage.Endgame)
            {
                return;
            }

            //Khi giết hết tất cả kẻ địch
            if (stage == PokemonStage.Win)
            {
                return;
            }

            if (stage == PokemonStage.Frighten)
            {
                SetAnimation(PokemonAnimStage.Move);
                return;
            }

            if (stage == PokemonStage.Get_Hit) return;

            if (stage == PokemonStage.Stun)
            {
                SetAnimation(PokemonAnimStage.Idle);
                return;
            }

            if (info.type == PokemonType.Dragon && info.lv == 3)
            {
                anim.SetFloat("idle_stage", _listTrigger.Count > 0 ? 1 : 0);
            }

            //ATTACK
            if (stage == PokemonStage.Attack_Normal)
            {
                CheckNear();

                //Có 2 trường hợp có thể đánh:
                //1. có kẻ địch chạm collider với mình (ưu tiên cao hơn)
                //2. có kẻ địch vào trong tầm đánh (ưu tiên thấp hơn)
                //3. Nếu quá lâu không thể tấn công vào target thì tìm mục tiêu khác
                _timeSwitchTarget += Time.deltaTime;

                if (_listTrigger.Count > 0)
                {
                    if (_target != null && !_target._isDied)
                    {
                        if (!CheckRange(_target.transform) && !_listTrigger.Contains(_target))
                        {
                            _target = null;
                            _target = _listTrigger[0];
                            _timeSwitchTarget = 0;
                        }
                    }
                }

                if (_target == null || _target._isDied || _timeSwitchTarget >= 5f)
                {
                    _timeSwitchTarget = 0;
                    _target = GetTarget();
                }

                if (_target == null) return;

                if (CheckRange(_target.transform) || _listTrigger.Contains(_target))
                {
                    rigid.isKinematic = true;
                    rigid.velocity = Vector3.zero;

                    if (_timeCooldown > (_speedAttack / _animSpeed) && !_isAttacking)
                    {
                        
                            _isAttacking = true;

                            _typeAttack = GetTypeAttack(_target.transform);
                            _timeCooldown = Random.Range(-0.15f, 0.15f);

                            _transRotate.DOLookAt(_target.transform.position, 0.3f)
                                .SetEase(Ease.InQuart)
                                .OnComplete(() =>
                                {
                                    AttackNormal();
                                });
                        
                    }
                    else
                    {
                        if (!_isAttacking)
                        {
                            if(stage_Anim != PokemonAnimStage.Idle)
                            {
                                SetAnimation(PokemonAnimStage.Idle);
                            }

                            //_transRotate.LookAt(_target.transform.position);
                            _timeCooldown += Time.deltaTime;
                        }
                    }
                }
                else
                {
                    if (_listTrigger.Count > 0)
                    {
                        rigid.isKinematic = true;
                        rigid.velocity = Vector3.zero;
                    }
                    else
                    {
                        rigid.isKinematic = false;
                        SetAnimation(PokemonAnimStage.Move);
                        _transRotate.LookAt(_target.transform.position);
                        _dir = (_target.transform.position - transform.position).normalized;
                        rigid.velocity = _dir * _speed * 0.5f;
                    }
                }
                return;
            }

            ////MOVE
            //{
            //    //Chỉ có pokemon của đi theo player mới thực hiện các lệnh đằng sau
            //    if (_targetFocus == null) return;

            //    if (!_isFirstInPlayer)
            //    {
            //        _isFirstInPlayer = true;
            //        stage = PokemonStage.Move;
            //        PlayerController.ins.CallTakePokemon();
            //        _transRotate.DOLocalRotate(Vector3.up * 360, 0.75f, RotateMode.FastBeyond360)
            //            .SetEase(Ease.Linear)
            //            .OnComplete(() =>
            //            {
            //                _transRotate.localRotation = Quaternion.Euler(0, 180, 0);
            //                transform.SetParent(PlayerController.ins.playerGroup);
            //            });
            //    }

            //    if (stage == PokemonStage.Move)
            //    {
            //        SetAnimation(PokemonAnimStage.Move);
            //        //follow focus pos + look at player
            //        if (Vector3.Distance(transform.position, _targetFocus.position) > 0.05f)
            //        {
            //            transform.position = Vector3.Lerp(transform.position, _targetFocus.position, _speed * Time.deltaTime);
            //            transform.localPosition
            //                = new Vector3(Mathf.Clamp(transform.localPosition.x, _minX, _maxX),
            //                transform.localPosition.y, transform.localPosition.z);
            //        }
            //        //_transRotate.rotation = Quaternion.Euler(PlayerController.ins.transRotate.rotation.eulerAngles);
            //    }
            //}
        }
        else
        {
            //Chỉ có pokemon của đi theo player mới thực hiện các lệnh đằng sau
            if (_targetFocus == null) return;

            if (!_isFirstInPlayer)
            {
                _isFirstInPlayer = true;
                stage = PokemonStage.Move;
                PlayerController.ins.CallTakePokemon();
                _transRotate.DOLocalRotate(Vector3.up * 360, 0.75f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _transRotate.localRotation = Quaternion.Euler(0, 180, 0);
                        transform.SetParent(PlayerController.ins.playerGroup);
                    });
            }

            if (stage == PokemonStage.Move)
            {
                SetAnimation(PokemonAnimStage.Move);
                //follow focus pos + look at player
                if (Vector3.Distance(transform.position, _targetFocus.position) > 0.05f)
                {
                    transform.position = Vector3.Lerp(transform.position, _targetFocus.position, _speed * Time.deltaTime);
                    transform.localPosition
                        = new Vector3(Mathf.Clamp(transform.localPosition.x, _minX, _maxX),
                        transform.localPosition.y, transform.localPosition.z);
                }
                _transRotate.rotation = Quaternion.Euler(PlayerController.ins.transRotate.rotation.eulerAngles);
            }
        }
    }

    protected bool CheckRange(Transform target, float distance = -1)
    {
        //if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Chest)
        //{
        //    if (_curRange < 2f) _curRange = 2f;
        //}

        if (Vector3.Distance(transform.position, target.position)
            <= (distance < 0 ? (_listTrigger.Count > 0 ? 1.5f : info.range) : distance))
        {
            return true;
        }
        return false;
    }

    TypeAttack GetTypeAttack(Transform _target)
    {
        if (info.isRange)
        {
            if (Vector3.Distance(_target.position, transform.position) < 1.5f && info.isMelee)
            {
                info.numMeleeType = 1;
                return TypeAttack.Melee;

            }
            return TypeAttack.Range;
        }
        return TypeAttack.Melee;
    }

    public void SetWin()
    {
        rigid.isKinematic = true;
        rigid.freezeRotation = true;
    }

    public void MoveToPlayer()
    {
        if (PlayerController.ins.listPokemon.Contains(this)) return;
        _targetFocus = PlayerController.ins.AddPokemon(this);
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void SetNewPos(Transform newPos)
    {
        _targetFocus = newPos;
    }

    public void LevelUp(int v, bool canDie)
    {
        if (info.lv + v <= 0 && canDie)
        {
            //set Die
            Die();
            return;
        }

        var curLv = info.lv + v;
        curLv = Mathf.Min(curLv, 3);
        curLv = Mathf.Max(curLv, 1);

        var po = GameConfig.ins.GetPokemon(info.type, curLv);
        info = po;

        transform.DisableAllChild();
        var scale = po.objPrefab.transform.localScale;
        var o = SimplePool.Spawn(po.objPrefab, transform.position, Quaternion.identity).transform;
        o.SetParent(transform);
        var oldRot = pokemonEvent.transform.parent.localRotation;
        o.localPosition = Vector3.zero;
        o.localRotation = oldRot;
        o.localScale = scale;
        _transRotate = o;

        pokemonEvent = o.transform.GetChild(0).GetComponent<PokemonEvent>();

        anim = pokemonEvent.GetComponent<Animator>();
        pokemonEvent.setup(this);
    }
    #endregion

    #region Get Hit
    public void GetHitAttack(int dam)
    {
        if (_isDied || PlayerController.ins.CheckEndGame() || _isSkilling) return;
        
        _hp -= dam;
        pokemonEvent.SetHp(_hp > 0 ? _hp : 0, _maxHp);

        ShowHpBar();

        //if (isPlayerPokemon)
        //    VibrationsManager.instance.TriggerLightImpact();
        if (_hp <= 0)
        {
            Die();
            pokemonEvent.hpBar.gameObject.SetActive(false);
        }
    }

    void ShowHpBar()
    {
        if (i_ShowHp != null) StopCoroutine(i_ShowHp);
        i_ShowHp = StartCoroutine(ie_ShowHpBar());
    }

    Coroutine i_ShowHp;
    IEnumerator ie_ShowHpBar()
    {
        pokemonEvent.ShowHpBar();
        yield return Yielders.Get(2f);
        pokemonEvent.HideHpBar();
    }

    public void KnockBackAttack(Pokemon _attacker, float f = 0.5f)
    {
        if (_isSkilling) return;

        GetHitAttack((int)(_attacker._dam * 1.2f));
        _isAttacking = false;
        f *= Random.Range(0.8f, 1.2f);
        stage = PokemonStage.Get_Hit;
        //SetAnimation(PokemonAnimStage.Get_Hit);

        transform.DOKill();

        var dir = (_attacker.transform.position - transform.position).normalized;

        if (!_isTriggerWall)
        {
            transform.DOMove(transform.position - dir * f, 0.25f)
                .OnComplete(() =>
                {
                    SetAnimation(PokemonAnimStage.Idle);
                });
        }

        Timer.Schedule(this, 0.5f, () =>
        {
            stage = PokemonStage.Attack_Normal;
            SetAnimation(PokemonAnimStage.Idle);

            _isAttacking = false;
        });
    }

    public void KnockBackAttackFrezze(Pokemon _attacker, float timeFrezze, float f = 0.5f)
    {
        if (_isSkilling) return;
        var trans =
            GameConfig.ins.SpawnFx(
                GameConfig.ins.fx_Frezze
                , pokemonEvent.transHit.position
                , 1.5f);

        trans.SetParent(transform);

        GetHitAttack((int)(_attacker._dam * 1.5f));
        _isAttacking = false;
        f *= Random.Range(0.8f, 1.2f);
        stage = PokemonStage.Get_Hit;
        //SetAnimation(PokemonAnimStage.Get_Hit);

        transform.DOKill();

        var dir = (_attacker.transform.position - transform.position).normalized;

        if (!_isTriggerWall)
        {
            transform.DOMove(transform.position - dir * f, 0.25f)
                .OnComplete(() =>
                {
                    SetAnimation(PokemonAnimStage.Idle);
                });
        }

        Timer.Schedule(this, 0.25f, () =>
        {
            Frezze(timeFrezze);
        });
    }

    public void PoisonHit(float time, int dps)
    {
        StartCoroutine(ie_PoisonHit(time, dps));
    }

    IEnumerator ie_PoisonHit(float time, int dps)
    {
        while (time > 0)
        {
            GetHitAttack(dps);
            yield return Yielders.Get(1f);
            time--;
        }
    }

    #region Special Stage
    public void Stun(float time)
    {
        var trans = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Stun
            , pokemonEvent.transform.position + Vector3.up * 0.2f
            , 1
            , time);
        stage = PokemonStage.Stun;
        Timer.Schedule(this, time, () =>
        {
            stage = PokemonStage.Attack_Normal;
            SetAnimation(PokemonAnimStage.Idle);
        });
    }

    public void Frezze(float time)
    {
        stage = PokemonStage.Stun;
        Timer.Schedule(this, time, () =>
        {
            SetSpeed(1, 1);
            stage = PokemonStage.Attack_Normal;
            SetAnimation(PokemonAnimStage.Idle);
        });
    }

    public void frighten(float time)
    {
        SetSpeed(0.3f, 1f);
        var rot = _transRotate.rotation.eulerAngles;
        rot.y = -rot.y;
        _transRotate.rotation = Quaternion.Euler(rot);

        var trans = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Frighten
            , pokemonEvent.transform.position + Vector3.up
            , 1
            , time);
        trans.localScale = Vector3.one * 2f;
        trans.SetParent(transform);
        stage = PokemonStage.Frighten;

        if (!_isTriggerWall)
        {
            transform.DOMove(transform.position - _transRotate.forward * 2f, time * 0.5f)
                .OnComplete(() =>
                {
                    SetSpeed(1f, 1f);
                    stage = PokemonStage.Attack_Normal;
                    SetAnimation(PokemonAnimStage.Idle);
                });
        }
    }
    #endregion

    public bool GetHit()
    {
        if (_isDied) return true;
        LevelUp(-1, true);
        return _isDied;
    }

    public void Die()
    {
        if (_isDied) return;

        _isDied = true;
        stage = PokemonStage.Die;
        transform.SetParent(null);
        if (isPlayerPokemon)
        {
            PlayerController.ins.RemovePokemon(this);
            PlayerController.ins.AddPokemonDie(info.type);
            Timer.Schedule(this, 1f, () =>
            {
                transform.DOLocalMoveY(transform.localPosition.y - 2f, 3f)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            });
        }
        else
        {
            Timer.Schedule(this, 1f, () =>
            {
                transform.DOLocalMoveY(transform.localPosition.y - info.lv * 0.15f, 0.5f);
                StartCoroutine(SpawnGem());
            });
            EndGame.Instance._lstPokemonBoss.Remove(this);
        }

        rigid.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;

        //Call anim die
        SetAnimation(PokemonAnimStage.Die);
        //gameObject.SetActive(false);

        PlayerController.ins.CheckEndGame();
    }

    IEnumerator SpawnGem()
    {
        var l = new List<GameObject>();

        var c = Random.Range(2, 5) * 2;
        for (var k = 0; k < c; k++)
        {
            var o = SimplePool.Spawn(GameConfig.ins.prefab_GemKill, Vector3.zero, Quaternion.identity).GetComponent<Rigidbody>();
            o.transform.position = transform.position;
            o.transform.localScale = Vector3.one * 0.75f;
            var f = new Vector3(Random.Range(-1f, 1f), Random.Range(3f, 7f), Random.Range(-1f, 1f));
            o.isKinematic = false;
            o.AddForce(f, ForceMode.Impulse);
            f = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
            o.angularVelocity = f;
            l.Add(o.gameObject);
        }

        GameManager_PLY_V2.Instance.AddGem((info.lv + 1) * 5 * 4);
        CanvasInGame.ins.AddGem((info.lv + 1) * 5 * 4);

        yield return Yielders.Get(2f);

        for (var i = 0; i < l.Count; i++)
        {
            l[i].GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void Revive()
    {
        _isDied = false;
        stage = PokemonStage.Idle;
        SetAnimation(PokemonAnimStage.Idle);
    }
    #endregion

    #region Attack
    void AttackNormal()
    {
        SetAnimation(PokemonAnimStage.Attack_Melee);
    }

    void AttackRanger()
    {
        SetAnimation(PokemonAnimStage.Attack_Range);
    }

    void CastSkill()
    {
        SetAnimation(PokemonAnimStage.Attack_Skill);
        SkillAction();
    }

    #region CastSkill
    //Thực hiện cùng lúc với khi gọi skill
    public virtual void SkillAction()
    {
        switch (info.type)
        {
            case PokemonType.Bat:
                {
                    BatSkill();
                    return;
                }
            case PokemonType.Bee:
                {
                    var curScale = transform.localScale;
                    pokemonEvent.fx_PowerUp.SetActive(true);
                    Timer.Schedule(this, 3f, () => { pokemonEvent.fx_PowerUp.SetActive(false); });
                    transform.DOScale(curScale * 1.25f, 0.3f)
                        .OnComplete(() =>
                        {
                            transform.DOScale(curScale, 0.3f);
                        });
                    return;
                }
            case PokemonType.Chick:
                {
                    BuffSpeed(3);
                    if (isPlayerPokemon)
                    {
                        foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                        {
                            if (t != null && !t._isDied && Vector3.Distance(t.transform.position, transform.position) <= 2f)
                            {
                                t.KnockBackAttack(this, 2f);
                            }
                        }
                    }
                    else
                    {
                        foreach (var t in PlayerController.ins.listPokemon)
                        {
                            if (t != null && !t._isDied && Vector3.Distance(t.transform.position, transform.position) <= 2f)
                            {
                                t.KnockBackAttack(this, 2f);
                            }
                        }
                    }
                    var o = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Bird_Skill, transform.position, 10, 1.5f);
                    o.SetParent(transform);
                    return;
                }
            case PokemonType.Egg:
                {
                    BuffSpeed(5);
                    return;
                }
            case PokemonType.Seed:
                {
                    var o =
                        GameConfig.ins.SpawnFx(
                            GameConfig.ins.fx_Sprout_Skill,
                            transform.position, 2, 5f);
                    o.SetParent(transform);
                    BuffSpeed(2);
                    return;
                }
            case PokemonType.Ghost:
                {
                    BuffSpeed(2);
                    return;
                }
            case PokemonType.Spider:
                {
                    BuffSpeed(5);
                    return;
                }
            case PokemonType.Wolf:
                {
                    BuffSpeed(5);
                    return;
                }
            case PokemonType.Practice:
                {
                    BuffSpeed(5);
                    return;
                }
            case PokemonType.Shell:
                {
                    Timer.Schedule(this, 0.2f, () =>
                    {
                        var o = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Bird_Skill, transform.position, 10, 2.3f);
                        o.SetParent(transform);
                    });
                    return;
                }
            case PokemonType.Pollen:
                {
                    priorityTarget = 1;
                    _countAttack = -1000;
                    SetTriggerStatus(true);
                    _isDash = true;
                    transform.DOScale(Vector3.one * 2f, 0.5f);
                    transform.DOMove(transform.position + transform.forward * 5f, 0.5f)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() =>
                        {
                            _isDash = false;
                            SetTriggerStatus(false);
                            stage = PokemonStage.Attack_Normal;
                            SetAnimation(PokemonAnimStage.Idle);
                            _isAttacking = false;
                        });
                    return;
                }
            case PokemonType.Scropling:
                {
                    BuffSpeed(3);
                    return;
                }
            case PokemonType.Golem:
                {
                    Timer.Schedule(this, 0.2f, () =>
                    {
                        var o = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Bird_Skill, transform.position, 10, 1.5f);
                        o.SetParent(transform);
                    });
                    return;
                }
            case PokemonType.Fungi:
                {
                    BuffSpeed(5);
                    return;
                }
            case PokemonType.Dragon:
                {
                    if (_listTrigger.Count == 0)
                    {
                        DragonSkill();
                    }
                    else DragonSkill_2();
                    return;
                }
            case PokemonType.Sun_Blossom:
                {
                    ShowEfx(1f);
                    return;
                }
            case PokemonType.Bud:
                {
                    ShowEfx(1f);
                    return;
                }
            case PokemonType.Earthworm:
                {
                    ShowEfx(2f);
                    return;
                }
            case PokemonType.Snakelet:
                {
                    ShowEfx(1.5f);
                    return;
                }
            case PokemonType.Gloom:
                {
                    ShowEfx(1f);
                    return;
                }
        }
    }

    //Event Attack của skill (có thể có hoặc không)
    public virtual void AttackAction()
    {
        if (_target == null) return;
        if (_typeAttack == TypeAttack.Melee)
        {
            if (false)
            {
                //CrystalManager.ins.SpawnGem(pokemonEvent.transCanon.position, info.lv);
            }
            else
            {
                if (_target != null && !_target._isDied)
                {
                    _target.GetHitAttack(_dam);
                    if (info.lv == 3)
                    {
                        //Event attack khi dùng skill, thường chỉ diễn anim thôi
                        if (stage_Anim == PokemonAnimStage.Attack_Skill)
                        {
                            if (info.type == PokemonType.Bee)
                            {
                                GameConfig.ins.SpawnFx(GameConfig.ins.fx_Bee_Skill,
                                    transform.position + _transRotate.forward * 2f + Vector3.up * 0.5f, 8);
                                CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                if (isPlayerPokemon)
                                {
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if ((_listTrigger.Contains(t) || CheckRange(t.transform))
                                            && checkAngle(t.transform, 180))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if ((_listTrigger.Contains(t) || CheckRange(t.transform))
                                            && checkAngle(t.transform, 180))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                            }
                            else if (info.type == PokemonType.Seed)
                            {
                                CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                if (isPlayerPokemon)
                                {
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.Stun(2f);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.Stun(2f);
                                        }
                                    }
                                }
                            }
                            else if (info.type == PokemonType.Ghost)
                            {
                                PhantomSkill();
                            }
                            else if (info.type == PokemonType.Wolf)
                            {
                                GameConfig.ins.SpawnFx(GameConfig.ins.fx_Bee_Skill,
                                    transform.position + _transRotate.forward * 2f + Vector3.up * 0.5f, 12);
                                CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                if (isPlayerPokemon)
                                {
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                            }
                            else if (info.type == PokemonType.Practice)
                            {
                                GameConfig.ins.SpawnFx(GameConfig.ins.fx_Bee_Skill,
                                    transform.position + _transRotate.forward * 2f + Vector3.up * 0.5f, 12);
                                CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                if (isPlayerPokemon)
                                {
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                            }
                            else if (info.type == PokemonType.Shell)
                            {
                                CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                if (isPlayerPokemon)
                                {
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if (CheckRange(t.transform, 1.5f))
                                        {
                                            t.KnockBackAttack(this, 0.5f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_small_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if (CheckRange(t.transform, 1.5f))
                                        {
                                            t.KnockBackAttack(this, 0.5f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_small_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                            }
                            else if (info.type == PokemonType.Scropling)
                            {
                                GameConfig.ins.SpawnFx(GameConfig.ins.fx_Scopion_Earthquake,
                                    transform.position, 1.25f);
                                CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                if (isPlayerPokemon)
                                {
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if (CheckRange(t.transform, 3f))
                                        {
                                            t.KnockBackAttack(this, 2f);
                                            GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                            }
                            else if (info.type == PokemonType.Golem)
                            {
                                CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                if (isPlayerPokemon)
                                {
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if (CheckRange(t.transform, 2f))
                                        {
                                            t.SetSpeed(0, 0);
                                            t.KnockBackAttackFrezze(this, 2f, 2f);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if (CheckRange(t.transform, 2f))
                                        {
                                            t.SetSpeed(0, 0);
                                            t.KnockBackAttackFrezze(this, 2f, 2f);
                                        }
                                    }
                                }
                            }
                            else if (info.type == PokemonType.Gloom)
                            {
                                Gloom_Skill();
                            }
                        }
                        //Event attack khi dùng đòn đánh thường
                        else
                        {
                            if (_isBuffing)
                            {
                                if (isPlayerPokemon)
                                {
                                    CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                    foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
                                    {
                                        if ((_listTrigger.Contains(t) || CheckRange(t.transform))
                                            && checkAngle(t.transform, 120))
                                        {
                                            t.KnockBackAttack(this, 1f);
                                            GameConfig.ins.SpawnFx(
                                                pokemonEvent.fx_TriggerSkill != null ? pokemonEvent.fx_TriggerSkill : GameConfig.ins.fx_HitMelee_Big_1
                                                , t.pokemonEvent.transHit.position);
                                        }
                                    }
                                }
                                else
                                {
                                    CameraShake.Instance.Vibrate(0.1f, 0.15f);
                                    foreach (var t in PlayerController.ins.listPokemon)
                                    {
                                        if ((_listTrigger.Contains(t) || CheckRange(t.transform))
                                            && checkAngle(t.transform, 120))
                                        {
                                            t.KnockBackAttack(this, 1f);
                                            GameConfig.ins.SpawnFx(
                                                pokemonEvent.fx_TriggerSkill != null ? pokemonEvent.fx_TriggerSkill : GameConfig.ins.fx_HitMelee_Big_1
                                                , t.pokemonEvent.transHit.position
                                            );
                                        }
                                    }
                                }

                            }
                            else
                            {
                                _target.KnockBackAttack(this, 0.2f);
                                GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_small_1, _target.pokemonEvent.transHit.position);
                            }
                        }
                    }
                    return;
                }
            }
        }
        else
        {
            if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Chest)
            {
                var o = GetBullet(pokemonEvent.typeBullet);
                var b = SimplePool.Spawn(o, pokemonEvent.transCanon.position, _transRotate.rotation).transform;

                var t = CrystalManager.ins.GetTarget();
                b.LookAt(t);

                b.DOMove(t, 8f)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        CrystalManager.ins.SpawnGem(t, info.lv);
                        b.GetComponent<ProjectileMover>().OnCollision(t);

                        _target.GetHitAttack(_dam);
                    });
            }
            else
            {

                if (_target != null && !_target._isDied)
                {
                    _target.GetHitAttack(_dam);
                    if (info.lv == 3)
                    {
                        //Event attack khi dùng skill, thường chỉ diễn anim thôi
                        if (stage_Anim == PokemonAnimStage.Attack_Skill)
                        {
                            if (info.type == PokemonType.Bud)
                            {
                                Bud_Skill();
                            }
                            else if (info.type == PokemonType.Dragon)
                            {

                            }
                            else if (info.type == PokemonType.Sun_Blossom)
                            {
                                PixieSkill();
                            }
                            else if (info.type == PokemonType.Earthworm)
                            {
                                _target = GetTarget();
                                SpawnBullet(info.bulletScale);
                            }
                            else if (info.type == PokemonType.Gloom)
                            {
                                Gloom_Skill();
                            }
                        }
                        //Event attack khi dùng đòn đánh thường
                        else
                        {
                            if (_isBuffing)
                            {
                                if (isPlayerPokemon)
                                {
                                    SpawnBullet(info.bulletScale);
                                }
                                else
                                {
                                    SpawnBullet(info.bulletScale);
                                }
                            }
                            else
                            {
                                SpawnBullet(info.bulletScale);
                            }
                        }
                    }
                    else
                    {
                        SpawnBullet(info.bulletScale);
                    }
                    return;
                }
            }
        }
    }

    public void SpawnBullet(float scale = 1f)
    {
        if (info.type == PokemonType.Bud || (info.type == PokemonType.Snakelet && info.lv == 3))
        {
            for (var i = 0; i < pokemonEvent.listCanon.Count; i++)
            {
                var o = GetBullet(pokemonEvent.typeBullet);
                var b = SimplePool.Spawn(o, pokemonEvent.listCanon[i].position, _transRotate.rotation).transform;

                b.localScale = scale * Vector3.one;

                var t = _target.pokemonEvent.transHit.transform.position;
                b.LookAt(t);

                var scr = b.GetComponent<ProjectileMover>();
                scr.SetupTarget(_target, 0.1f * transform.localScale.x * (info.lv + 1));

                b.DOMove(t, 8f)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if (scr._target != null && !scr._target._isDied)
                        {
                            scr.OnCollision(_target.pokemonEvent.transHit.position);
                            if (!_isBuffing) _target.GetHitAttack(_dam);
                            else _target.KnockBackAttack(this, 0.3f);
                        }
                        else
                        {
                            scr.Destroy();
                        }
                    });
            }
        }
        else if (info.type == PokemonType.Sun_Blossom)
        {
            var o = GetBullet(pokemonEvent.typeBullet);
            var b = SimplePool.Spawn(o, pokemonEvent.transCanon.position, _transRotate.rotation).transform;

            var t = _target.pokemonEvent.transHit.transform.position;
            b.LookAt(t);

            var scr = b.GetComponent<ProjectileMover>();
            scr.SetupTarget(_target, 1f);

            b.DOMove(t, 6f)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    scr.OnCollision(_target.pokemonEvent.transHit.position);

                    if (scr._target != null && !scr._target._isDied)
                    {
                        GameConfig.ins.SpawnFx(GameConfig.ins.fx_HpHeal, scr._target.transform.position, 1f);
                        scr._target.HealHp(_dam);
                        _target = null;
                    }
                });
        }
        else
        {
            var o = GetBullet(pokemonEvent.typeBullet);
            var b = SimplePool.Spawn(o, pokemonEvent.transCanon.position, _transRotate.rotation).transform;
            b.localScale = info.bulletScale * Vector3.one * scale;
            var t = _target.pokemonEvent.transHit.transform.position;
            b.LookAt(t);

            var scr = b.GetComponent<ProjectileMover>();
            scr.SetupTarget(_target, info.bulletScale * scale);

            if (info.lv == 2 && info.type == PokemonType.Gloom)
            {
                b.DOScale(b.localScale * 1.25f, 1f);
            }

            b.DOMove(t, 8f)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    scr.OnCollision(_target.pokemonEvent.transHit.position);

                    if (scr._target != null && !scr._target._isDied)
                    {
                        if (!_isBuffing) _target.GetHitAttack(_dam);
                        else _target.KnockBackAttack(this, 0.3f);
                    }
                });
        }
    }

    #region Bat Skill
    public void BatSkill()
    {
        var oldScale = pokemonEvent.transform.localScale;
        pokemonEvent.efx_BatSkill.transform.SetParent(transform);
        BombExposive s = pokemonEvent.efx_BatSkill.gameObject.GetComponent<BombExposive>();
        if (s == null)
        {
            s = pokemonEvent.efx_BatSkill.gameObject.AddComponent<BombExposive>();
        }
        s.Setup(pokemonEvent, pokemonEvent.fx_TriggerSkill, 0);

        var path = new List<Vector3>();
        path.Add(transform.position);
        if (isPlayerPokemon)
        {
            for (var i = 0; i <= 5 && i < PlayerController.ins._endgame._lstPokemonBoss.Count; i++)
            {
                path.Add(PlayerController.ins._endgame._lstPokemonBoss[i].transform.position);
            }
        }
        else
        {
            for (var i = 0; i <= 5 && i < PlayerController.ins.listPokemon.Count; i++)
            {
                path.Add(PlayerController.ins.listPokemon[i].transform.position);
            }
        }
        path.Add(transform.position);
        pokemonEvent.efx_BatSkill.SetActive(true);

        _isSkilling = true;
        pokemonEvent.transform.DOScale(0f, 0.5f)
            .OnKill(() =>
            {
                pokemonEvent.transform.DOScale(oldScale, 0.5f);
            })
            .OnComplete(() =>
            {
                transform.DOPath(path.ToArray(), 15f, PathType.CatmullRom)
                    .SetEase(Ease.Linear)
                    .SetSpeedBased(true)
                    .OnKill(() =>
                    {
                        pokemonEvent.transform.DOScale(oldScale, 0.5f);
                    })
                    .OnComplete(() =>
                    {
                        pokemonEvent.transform.DOScale(oldScale, 0.5f)
                            .OnKill(() =>
                            {
                                pokemonEvent.transform.DOScale(oldScale, 0.5f);
                            })
                            .OnComplete(() =>
                            {
                                pokemonEvent.efx_BatSkill.SetActive(false);
                                _isSkilling = false;
                                ClearStage();
                            });
                    });
            });
    }
    #endregion

    #region Bud Bullet Skill
    public void Bud_Skill()
    {
        for (var i = 0; i < pokemonEvent.listCanon.Count; i++)
        {
            var o = GetBullet(pokemonEvent.typeBullet);
            var t = _target.pokemonEvent.transHit.transform.position
                + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            var b = SimplePool.Spawn(o, pokemonEvent.listCanon[i].position, _transRotate.rotation).transform;
            b.localScale = Vector3.one * 1.1f;
            var pos = pokemonEvent.listCanon[i].position + Vector3.up * 10f;
            b.LookAt(pos);
            var scr = b.GetComponent<ProjectileMover>();
            scr.SetupTarget(_target, 1.1f);

            b.DOMove(pos, 12f)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    scr.Destroy();

                    b = SimplePool.Spawn(o, t + Vector3.up * 10, _transRotate.rotation).transform;
                    b.localScale = Vector3.one * 1.1f;
                    b.LookAt(t);
                    scr = b.GetComponent<ProjectileMover>();
                    scr.SetupTarget(_target, 1.1f);

                    b.DOMove(t, 10f)
                        .SetDelay(Random.Range(0f, 1f))
                        .SetSpeedBased(true)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            scr.OnCollision(_target.pokemonEvent.transHit.position);
                            HitArea(b.position, _dam, 1f, true);
                        });
                });
        }
    }
    #endregion

    #region Sun_Pixie Skill
    public void PixieSkill()
    {
        if (isPlayerPokemon)
        {
            foreach (var t in PlayerController.ins.listPokemon)
            {
                if (t == this) continue;
                if (t != null && !t._isDied)
                {
                    t.HealHp(_dam);
                    GameConfig.ins.SpawnFx(GameConfig.ins.fx_HpHeal, t.transform.position);
                }
            }
        }
        else
        {
            foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
            {
                if (t == this) continue;
                if (t != null && !t._isDied)
                {
                    t.HealHp(_dam);
                    GameConfig.ins.SpawnFx(GameConfig.ins.fx_HpHeal, t.transform.position);
                }
            }
        }
    }
    #endregion

    #region Gloom Skill
    public void Gloom_Skill()
    {
        var t = Instantiate(GameConfig.ins.fx_Slash, pokemonEvent.transTornado.position, Quaternion.identity).transform;
        var pos = t.position;
        pos.y = _target.transform.position.y;
        t.position = pos;
        t.LookAt(_target.transform);
        BombExposive s = t.gameObject.GetComponent<BombExposive>();
        if (s == null)
        {
            s = t.gameObject.AddComponent<BombExposive>();
        }
        s.Setup(pokemonEvent, pokemonEvent.fx_TriggerSkill, 0);


        t.localScale = Vector3.one * 0.25f;
        t.DOScale(new Vector3(1, 1.15f, 1) * 8f, 1f)
            .SetEase(Ease.Linear);

        t.DOMove(_transRotate.forward * 15f + pokemonEvent.transTornado.position, 10f)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Destroy(t.gameObject);
            });
    }
    #endregion

    #region Dragon Skill
    public void DragonSkill()
    {
        var l1 = new List<Vector3>();
        var l2 = new List<Vector3>();

        pokemonEvent.pEnd.position = _target.transform.position;
        var dis = Vector3.Distance(transform.position, pokemonEvent.pEnd.position);
        pokemonEvent.p1.localPosition = new Vector3(2.5f, 0f, dis);
        pokemonEvent.p2.localPosition = new Vector3(-2.5f, 0f, dis);

        //
        l1.Add(pokemonEvent.transTornado_1.position);
        l1.Add(pokemonEvent.p1.position);
        l1.Add(pokemonEvent.pEnd.position);

        l2.Add(pokemonEvent.transTornado_2.position);
        l2.Add(pokemonEvent.p2.position);
        l2.Add(pokemonEvent.pEnd.position);

        var o1 = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Tornado_Orange, l1[0]);
        o1.transform.position = l1[0];
        o1.localScale = 0.5f * Vector3.one;
        BombExposive s = o1.gameObject.GetComponent<BombExposive>();
        if (s == null)
        {
            s = o1.gameObject.AddComponent<BombExposive>();
        }
        s.Setup(pokemonEvent, pokemonEvent.fx_TriggerSkill, 0);
        o1.DOScale(Vector3.one * 1.25f, 2f);
        o1.DOPath(l1.ToArray(), 1.5f, PathType.CatmullRom)
            .SetEase(Ease.Linear);

        var o2 = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Tornado_Orange, l1[0]);
        o2.SetParent(transform);
        o2.transform.position = l2[0];
        o2.localScale = 0.5f * Vector3.one;
        BombExposive s2 = o2.gameObject.GetComponent<BombExposive>();
        if (s2 == null)
        {
            s2 = o2.gameObject.AddComponent<BombExposive>();
        }
        s2.Setup(pokemonEvent, pokemonEvent.fx_TriggerSkill, 0);

        o2.DOScale(Vector3.one * 1.25f, 2f);
        o2.DOPath(l2.ToArray(), 1.5f, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Timer.Schedule(this, 1.5f, () =>
                {
                    SimplePool.Despawn(o1.gameObject);
                    SimplePool.Despawn(o2.gameObject);
                });
            });
    }

    public void DragonSkill_2()
    {
        var o = new GameObject();
        o.transform.position = transform.position;

        var l1 = new List<Vector3>();
        var l2 = new List<Vector3>();
        l1.Add(pokemonEvent.transTornado_1.position);
        l2.Add(pokemonEvent.transTornado_2.position);

        var o1 = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Tornado_Orange, l1[0]);
        o1.transform.position = l1[0];
        o1.localScale = 0.5f * Vector3.one;
        o1.SetParent(o.transform);
        o1.localPosition = Vector3.zero;
        o1.DOScale(Vector3.one * 1.5f, 2f);
        BombExposive s = o1.gameObject.GetComponent<BombExposive>();
        if (s == null)
        {
            s = o1.gameObject.AddComponent<BombExposive>();
        }
        s.Setup(pokemonEvent, pokemonEvent.fx_TriggerSkill, 0);

        var o2 = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Tornado_Orange, l1[0]);
        o2.transform.position = l2[0];
        o2.localScale = 0.5f * Vector3.one;
        o2.SetParent(o.transform);
        o2.localPosition = Vector3.zero;
        o2.DOScale(Vector3.one * 1.5f, 2f);
        BombExposive s2 = o2.gameObject.GetComponent<BombExposive>();
        if (s2 == null)
        {
            s2 = o2.gameObject.AddComponent<BombExposive>();
        }
        s2.Setup(pokemonEvent, pokemonEvent.fx_TriggerSkill, 0);

        var time = 3f;
        o1.DOLocalMoveX(2.75f, time).SetEase(Ease.Linear);
        o2.DOLocalMoveX(-2.75f, time).SetEase(Ease.Linear);

        o.transform.DORotate(Vector3.up * 1080, time, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SimplePool.Despawn(o1.gameObject);
                SimplePool.Despawn(o2.gameObject);
            });
    }
    #endregion

    #region Phantom Skill
    public void PhantomSkill()
    {
        var o = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Ghost_Skill, transform.position + Vector3.up * 0.5f, 1.5f, 2f);

        var pos = o.transform.position;
        pos.y = _target.transform.position.y;
        o.transform.position = pos;

        o.LookAt(_target.transform);
        o.transform.localScale = Vector3.one * 0.5f;
        o.SetParent(transform);

        if (isPlayerPokemon)
        {
            foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
            {
                if (CheckRange(t.transform, 2f) && checkAngle(t.transform, 150))
                {
                    t.frighten(3f);
                }
            }
        }
        else
        {
            foreach (var t in PlayerController.ins.listPokemon)
            {
                if (CheckRange(t.transform, 2f) && checkAngle(t.transform, 150))
                {
                    t.frighten(3f);
                }
            }
        }
    }
    #endregion

    //Sử dụng chung cho tất cả các con skill buff speed + atk
    #region Buff Speed

    public void ShowEfx(float time)
    {
        StartCoroutine(ie_ShowEfx(time));
    }

    IEnumerator ie_ShowEfx(float time)
    {
        var oldScale = _transRotate.localScale;
        _transRotate.DOScale(oldScale * 1.2f, 0.75f);
        pokemonEvent.fx_PowerUp.SetActive(true);

        var _time = false;
        Timer.Schedule(this, time, () => { _time = true; });
        yield return new WaitUntil(() => _time);

        _transRotate.DOScale(oldScale, 0.75f);
        pokemonEvent.fx_PowerUp.SetActive(false);
    }

    [ContextMenu("test skill")]
    public void BuffSpeed(int maxAttack)
    {
        StartCoroutine(ie_BuffSpeed(maxAttack));
    }

    IEnumerator ie_BuffSpeed(int maxAttack = 5)
    {
        var oldScale = _transRotate.localScale;
        _transRotate.DOScale(oldScale * 1.2f, 0.75f);
        pokemonEvent.fx_PowerUp.SetActive(true);
        SetSpeed(1.3f, 2f);
        _isBuffing = true;
        _countAttack = 0;
        _timeCooldown = 1f;

        //Đánh 5 đòn hoặc qua 10 giây
        var _time = false;
        Timer.Schedule(this, 10, () => { _time = true; });
        yield return new WaitUntil(() => _countAttack > maxAttack || _time);

        _countAttack = 0;
        _transRotate.DOScale(oldScale, 0.75f);
        pokemonEvent.fx_PowerUp.SetActive(false);
        _isBuffing = false;
        SetSpeed(1f, 1f);
    }
    #endregion
    #endregion

    #region Support
    public void CheckNear()
    {
        _listTrigger.Clear();
        if (isPlayerPokemon)
        {
            foreach (var t in GameManager_PLY_V2.Instance.MonsterList)
            {
                if (Vector3.Distance(transform.position, t.transform.position) <= 1.5f)
                {
                    _listTrigger.Add(t);
                }
            }
        }
        else
        {
            foreach (var t in PlayerController.ins.listPokemon)
            {
                if (Vector3.Distance(transform.position, t.transform.position) <= 1.5f)
                {
                    _listTrigger.Add(t);
                }
            }
        }
    }

    public void HealHp(int dam)
    {
        _hp += dam;
        ShowHpBar();
        if (_hp > _maxHp) _hp = _maxHp;
        pokemonEvent.SetHp(_hp, _maxHp);
    }

    public void HitArea(Vector3 posInfict, int dam, float range, bool isKnockBack = false)
    {
        if (isPlayerPokemon)
        {
            foreach (var t in PlayerController.ins._endgame._lstPokemonBoss)
            {
                if (Vector3.Distance(posInfict, t.transform.position) < range)
                {
                    if (isKnockBack)
                    {
                        t.KnockBackAttack(this, 0.3f);
                    }
                    else
                    {
                        t.GetHitAttack(_dam);
                    }
                }
            }
        }
        else
        {
            foreach (var t in PlayerController.ins.listPokemon)
            {
                if (Vector3.Distance(posInfict, t.transform.position) < range)
                {
                    if (isKnockBack)
                    {
                        t.KnockBackAttack(this, 0.3f);
                    }
                    else
                    {
                        t.GetHitAttack(_dam);
                    }
                }
            }
        }
    }

    protected virtual Pokemon GetTarget()
    { 

        if (isPlayerPokemon)
        {
            foreach (Pokemon aPokemon in GameManager_PLY_V2.Instance.MonsterList)
            {
                if (aPokemon != null)
                {
                    return aPokemon;
                }
            }
        }
        else
        {
            foreach(Pokemon aPokemon in PlayerController.ins.listPokemon)
            {
                if (aPokemon != null)
                {
                    return aPokemon;
                }
            }
        }

        return null;
    }

    public Pokemon OverLapWithDeg(float angl)
    {
        for (var i = 0; i < _listTrigger.Count; i++)
        {
            if (_listTrigger != null && !_listTrigger[i]._isDied && checkAngle(_listTrigger[i].transform, angl))
            {
                return _listTrigger[i];
            }
        }
        return null;
    }

    public virtual bool checkAngle(Transform target, float angl)
    {
        var dir = _transRotate.position - target.position;
        var ang = Vector3.Angle(_transRotate.forward, dir);
        var a = 180 - angl / 2;
        var b = 180 + angl / 2;
        ang %= 360;
        if (Mathf.Abs(ang) > a && Mathf.Abs(ang) < b)
        {
            return true;
        }
        return false;
    }

    public GameObject GetBullet(Projectile type)
    {
        return GameConfig.ins.prefab_Bullet.Find(x => x.type == type).obj;
    }
    #endregion

    #endregion

    #region Collssion
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Pokeball"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Ball, other.transform.position + _transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            PlayerController.ins.ballCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Pokeball);
        }
        else if (other.tag.Equals("Gem"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Gem, other.transform.position + _transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            GameManager.ins.data.gemCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Gem);

        }
        else if (other.tag.Equals("Energy"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Energy, other.transform.position + _transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            PlayerController.ins.energyCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Energy);
        }
        else if (other.tag.Equals("Evolution"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Evolution, other.transform.position + _transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            PlayerController.ins.evolutionCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Evolution);
        }
        else if (other.tag.Equals("Boom"))
        {
            SoundController.PlaySoundOneShot(SoundController.ins.get_boom);
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Boom, other.transform.position + _transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            PlayerController.ins.evolutionCollected -= 1;
            CanvasInGame.ins.ReLoad(ItemType.Boom);
        }
        else if (other.tag.Equals("Key"))
        {
            SoundController.ins.TriggerItem();
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Get_Key, other.transform.position + _transRotate.forward * 0.75f);
            Destroy(other.gameObject);
            PlayerController.ins.keyCollected += 1;
            CanvasInGame.ins.ReLoad(ItemType.Key);
        }
        else if (other.tag.Equals("Wall"))
        {
            if (info.type == PokemonType.Bat && info.lv == 3 && _isSkilling) return;

            if (!InputManager.ins.canTouch)
            {
                transform.DOKill();
                rigid.velocity = Vector3.zero;
                //Die();
                _isTriggerWall = true;
            }
        }
        else if (_isDash && other.tag.Equals("Pokemon"))
        {
            var t = other.GetComponent<Pokemon>();
            if (t != null)
            {
                CameraShake.Instance.Vibrate(0.15f, 0.15f);
                t.KnockBackAttack(this, 1f);
                GameConfig.ins.SpawnFx(GameConfig.ins.fx_HitMelee_Big_1, t.pokemonEvent.transHit.position);
            }
        }
        else if (other.tag.Equals("Trap"))
        {
            Die();
        }
    }
    #endregion

    #region Animation
    public PokemonAnimStage ani_Apply;
    public PokemonAnimStage stage_Anim;

    public void LateUpdate()
    {
        if (GameManager_PLY_V2.Instance == null
            || !GameManager_PLY_V2.Instance.isGamePlaying)
        {
            return;
        }

        ReloadAnimation();
    }

    protected int _oldMelee;
    public void ReloadAnimation()
    {
        //Nếu trạng thái animation thay đổi
        if (ani_Apply == stage_Anim)
            return;

        //Bật animation sau mỗi frame
        ani_Apply = stage_Anim;

        switch (stage_Anim)
        {
            case PokemonAnimStage.Idle:
                anim.SetInteger("stage", 0);
                break;
            case PokemonAnimStage.Get_Hit:
                anim.SetInteger("stage", 1);
                break;
            case PokemonAnimStage.Die:
                anim.SetInteger("stage", 2);
                break;
            case PokemonAnimStage.Move:
                anim.SetInteger("stage", 3);
                break;
            case PokemonAnimStage.Attack_Melee:
                if (info.numMeleeType == 0) info.numMeleeType = 1;
                anim.SetInteger("melee_stage", (_oldMelee++ % info.numMeleeType));
                anim.SetInteger("stage", 4);
                _isAttacking = false; 
                break;
            case PokemonAnimStage.Attack_Range:
                anim.SetInteger("stage", 5);
                break;
            case PokemonAnimStage.Attack_Skill:
                if (info.type == PokemonType.Dragon)
                {
                    anim.SetInteger("skill_stage", _listTrigger.Count > 0 ? 1 : 0);
                }
                anim.SetInteger("stage", 6);
                break;
            default:
                Debug.LogError("Lỗi animationState Player" + stage.ToString());
                break;
        }
    }

    public void SetSpeed(float speed, float atkSpeed)
    {
        _animSpeed = speed;
        _atkSpeed = atkSpeed;
    }

    public void SetAnimation(PokemonAnimStage stage)
    {
        if (_isAttacking
            && stage != PokemonAnimStage.Get_Hit
            && (
            //stage_Anim == PokemonAnimStage.Attack_Melee|| 
            stage_Anim == PokemonAnimStage.Attack_Skill
                || stage_Anim == PokemonAnimStage.Attack_Range))
        {
            return;
        }

        anim.speed = _animSpeed;
        if (_isDied)
        {
            anim.speed = 1f;
        }
        this.stage_Anim = stage;
    }

    public void ClearStage()
    {
        //Cái này sau cho vào end attack event
        _isAttacking = false;
        _timeCooldown = 0;
        SetAnimation(PokemonAnimStage.Idle);
    }
    #endregion

    public void ScaleDam(float percent)
    {
        _dam = (int)(_dam * percent);
    }
}

[System.Serializable]
public enum PokemonStage
{
    Idle,
    Move,
    Attack_Normal,
    Attack_Boss,
    Attack_Special,
    Attack_Endgame,
    Die,
    Endgame,
    Win,
    Get_Hit,
    Attack_Melee,
    Attack_Range,
    Attack_Skill,
    Stun,
    Frighten
}

public enum PokemonAnimStage
{
    Idle,
    Move,
    Attack_Normal,
    Attack_Boss,
    Attack_Special,
    Attack_Endgame,
    Die,
    Endgame,
    Win,
    Get_Hit,
    Attack_Melee,
    Attack_Range,
    Attack_Skill
}

[System.Serializable]
public enum Mode
{
    Normal,
    Chest,
    BigBoss
}
