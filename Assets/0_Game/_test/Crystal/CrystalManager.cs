using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrystalManager : SingletonMonoBehaviour<CrystalManager>
{
    public int hp = 1000;
    public List<CrystalElement> listCrystal;
    public TextMeshPro txtTime;
    public Transform transCircle;
    public Animator animChest;

    private int _time;
    private List<GameObject> _listGem = new List<GameObject>();
    private int _damOneShot;
    private int _countDam;
    private bool _canSpawnGem;

    public void OnStart(int time)
    {
        var scale = ((GameManager.ins.data.level - 1) / 5);
        var s = (0.4f + scale * 0.05f);
        s = Mathf.Min(s, 0.6f);
        transform.localScale = Vector3.one * s;
        hp = (int)((scale + 1) * 300f + 500);
        hp = Mathf.Min(hp, 2500);

        txtTime.gameObject.SetActive(true);
        _time = time;

        _canSpawnGem = true;
        _damOneShot = hp / listCrystal.Count;
        _countDam = _damOneShot;

        _listGem.Clear();
        StartCoroutine(ie_OnStart());
    }

    IEnumerator ie_OnStart()
    {
        txtTime.text = _time.ToString();

        yield return Yielders.Get(1f);

        transCircle.DOLocalRotate(new Vector3(0, 180, 360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        while (_time > 0)
        {
            yield return Yielders.Get(1f);
            _time--;
            txtTime.text = _time.ToString();
        }
        txtTime.gameObject.SetActive(false);
        StopAttack();
    }

    [ContextMenu("test destroy")]
    public void DestroyCrystal()
    {
        if (listCrystal.Count == 0)
        {
            return;
        }
        var o = listCrystal[0];
        listCrystal.RemoveAt(0);
        o.DestroyCrystal(transform.localScale.x * 0.8f);

        if(listCrystal.Count == 0)
        {
            StopAttack();
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            animChest.Play("Open");
        }
    }

    public Vector3 GetTarget()
    {
        return transform.position + Vector3.up * 1.5f + Random.insideUnitSphere;
    }

    public void SpawnGem(Vector3 pos, int dam)
    {
        StartCoroutine(ie_SpawnGem(pos));
        GetDam(5);
    }

    void GetDam(int dam)
    {
        if (hp <= 0)
        {
            return;
        }
        hp -= dam;

        if (1000 - hp > _countDam)
        {
            _countDam += _damOneShot;
            DestroyCrystal();
        }
        if (hp == 0)
        {
            while (listCrystal.Count > 0) DestroyCrystal();
            StopAttack();
        }
    }

    public void StopAttack()
    {
        _canSpawnGem = false;
        PlayerController.ins._endgame._lstPokemonBoss.Clear();
        PlayerController.ins.CheckEndGame();
        foreach (var t in PlayerController.ins.listPokemon)
        {
            t.stage = PokemonStage.Win;
        }
    }

    IEnumerator ie_SpawnGem(Vector3 pos)
    {
        if (!_canSpawnGem) yield break;
        var l = new List<GameObject>();

        var c = Random.Range(1, 4);
        for (var k = 0; k < c; k++)
        {
            var o = SimplePool.Spawn(GameConfig.ins.prefab_GemKill, Vector3.zero, Quaternion.identity).GetComponent<Rigidbody>();
            o.transform.position = pos;
            o.transform.localScale = Vector3.one * 0.75f;
            var f = new Vector3(Random.Range(-1f, 1f), Random.Range(3f, 7f), Random.Range(-1f, 1f));
            o.isKinematic = false;
            o.AddForce(f, ForceMode.Impulse);
            f = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
            o.angularVelocity = f;
            l.Add(o.gameObject);
            _listGem.Add(o.gameObject);
        }

        GameManager.ins.data.AddGem(GameManager.ins.data.level);
        CanvasInGame.ins.AddGem(GameManager.ins.data.level);

        yield return Yielders.Get(2f);

        for (var i = 0; i < l.Count; i++)
        {
            l[i].GetComponent<Rigidbody>().isKinematic = true;
            l[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    IEnumerator ie_CollectGem()
    {
        StopAttack();

        var p = Camera.main.ScreenToWorldPoint(CanvasInGame.ins.objGem.transform.position + Vector3.forward * 15);
        var ng = new GameObject();
        ng.transform.position = p;

        for (var i = 0; i < _listGem.Count; i++)
        {
            _listGem[i].GetComponent<Rigidbody>().isKinematic = true;
            _listGem[i].transform.DOMove(p, Random.Range(0.5f, 1f))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    CanvasInGame.ins.animGem.Play();
                    PlayerController.ins.AddItem(ItemType.Gem, 1);
                    CanvasInGame.ins.ReloadGem();
                    SimplePool.Despawn(_listGem[i]);
                });
            yield return Yielders.Get(Random.Range(0.02f, 0.06f));
        }
    }

}
