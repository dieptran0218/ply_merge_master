using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PokemonEvent : MonoBehaviour
{
    public Transform transCanon;
    public Transform transHit;
    public HpBar hpBar;
    public SpriteRenderer _hp1;
    public SpriteRenderer _hp2;
    public Projectile typeBullet;
    public GameObject fx_PowerUp;

    public GameObject fx_TriggerSkill;

    [Header(" ===== Bub Serires ======")]
    public List<Transform> listCanon;

    [Header(" ===== Snake Serires ======")]
    public List<GameObject> listFire;

    [Header(" ===== Bomb Serires ======")]
    public GameObject objCooldown;
    public int timeCooldown;
    public TextMeshPro txtTimeCooldown;

    [Header(" ===== Gloom Serires ======")]
    public Transform transTornado;

    [Header(" ===== Dragon Serires ======")]
    public Transform transTornado_1;
    public Transform transTornado_2;
    public Transform p1;
    public Transform p2;
    public Transform pEnd;

    [Header(" ===== Bat Serires ======")]
    public GameObject efx_BatSkill;

#if UNITY_EDITOR
    [ContextMenu("remove")]
    public void rm()
    {
        DestroyImmediate(GetComponent<Rigidbody>());
        DestroyImmediate(GetComponent<Collider>());
        UnityEditor.EditorUtility.SetDirty(gameObject);
    }
#endif

    [HideInInspector] public Pokemon mng;
    public void setup(Pokemon mng)
    {
        this.mng = mng;
        hpBar.gameObject.SetActive(false);
        _hp1 = hpBar.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        _hp2 = hpBar.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void ShowHpBar()
    {
        _hp1.DOKill();
        _hp2.DOKill();
        _hp1.color = GameHelper.GetColorAlpha(_hp1.color, 1);
        _hp2.color = GameHelper.GetColorAlpha(_hp2.color, 1);
        hpBar.gameObject.SetActive(true);
    }

    public void HideHpBar()
    {
        _hp1.color = GameHelper.GetColorAlpha(_hp1.color, 0);
        _hp2.color = GameHelper.GetColorAlpha(_hp2.color, 0);
                hpBar.gameObject.SetActive(false);
    }

    public void SetHp(int curHp, int maxHp)
    {
        hpBar.SetHp(curHp, maxHp);
    }

    public void Evt_Attack()
    {
        mng.AttackAction();
    }

    public void Evt_EndAttack()
    {
        mng.ClearStage();
    }

    public void Evt_Died()
    {
        Timer.Schedule(this, 1f, () => { mng.gameObject.SetActive(false); });
    }

    [HideInInspector] public int idleCount = 0;
    public void Evt_Idle()
    {
    }

    public void Evt_None()
    {
        mng.anim.SetInteger("stage", -1);
    }

    public void Evt_SkillStart()
    {
        mng.SetSpeed(0.5f, 1f);
        foreach (var t in listFire)
        {
            t.SetActive(true);
        }
    }

    public void Evt_SkillEnd()
    {
        foreach (var t in listFire)
        {
            t.SetActive(false);
        }
        mng.SetSpeed(1f, 1f);
    }

    //Bomb series
    public void Explosive()
    {
        objCooldown.SetActive(true);
        StartCoroutine(ie_Explosive());
    }

    IEnumerator ie_Explosive()
    {
        timeCooldown = 3;
        while (timeCooldown >= 0)
        {
            txtTimeCooldown.text = timeCooldown + "";
            timeCooldown--;
            yield return Yielders.Get(1f);
        }

        hpBar.gameObject.SetActive(false);

        mng.stage = PokemonStage.Endgame;
        mng.SetAnimation(PokemonAnimStage.Attack_Skill);

        if (mng.info.lv == 3)
        {
            GameConfig.ins.SpawnFx(GameConfig.ins.fx_Poison_Screen, mng.transform.position, 5f, 5f);
        }

        yield return Yielders.Get(1f);

        if (mng.info.lv == 1)
        {
            var t = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Explosive, mng.transform.position + Vector3.up * 0.01f, 0.5f);
            var s = t.gameObject.AddComponent<BombExposive>();
            s.Setup(this, fx_TriggerSkill, 0);
        }
        else if (mng.info.lv == 2)
        {
            var t = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Ice_Explosive, mng.transform.position + Vector3.up * 0.01f, 0.75f);
            var s = t.gameObject.AddComponent<BombExposive>();
            s.Setup(this, fx_TriggerSkill, 1);
        }
        else
        {
            var t = GameConfig.ins.SpawnFx(GameConfig.ins.fx_Poison_Explosive, mng.transform.position + Vector3.up * 0.01f);
            if (mng.isPlayerPokemon)
            {
                foreach (var k in PlayerController.ins._endgame._lstPokemonBoss)
                {
                    k.PoisonHit(3f, (int)(mng._dam * 0.1f));
                }
            }
            else
            {
                foreach (var k in PlayerController.ins.listPokemon)
                {
                    k.PoisonHit(3f, (int)(mng._dam * 0.1f));
                }
            }
        }

        mng.gameObject.SetActive(false);
        mng.Die();
    }

    private void Start()
    {
        mng = transform.parent.GetComponent<Pokemon>();
    }

}
