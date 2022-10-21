using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndgameBoss : MonoBehaviour
{
    public Animator anim;
    public GameObject objBall;
    public Rigidbody rigid;
    public Transform transSpawnBall;

    private bool isDied = false;

    private List<Pokemon> _listMonster;
    public void CallMonster(List<Pokemon> listMonster)
    {
        anim.Play("Throw");
        this._listMonster = listMonster;
    }

    public void Spawn()
    {
        foreach(var t in _listMonster)
        {
            var target = t.transform.position;
            var o = Instantiate(objBall).transform;
            o.position = transSpawnBall.position;
            o.localScale = Vector3.one * 0.4f;

            o.DOLocalRotateQuaternion(Random.rotation, 0.25f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);

            o.DOScale(Vector3.one, 1f).SetEase(Ease.Linear);

            var path = new List<Vector3>();
            path.Add(o.position);
            path.Add((o.position + target) * 0.5f + Vector3.up * 3f);
            path.Add(target);
            o.DOPath(path.ToArray(), 1.25f, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Destroy(o.gameObject);
                    GameConfig.ins.SpawnFx(GameConfig.ins.fx_Smoke, target + Vector3.up * 0.5f);
                    t.gameObject.SetActive(true);
                });
        }
    }

    public void GetHit()
    {
        rigid.isKinematic = false;
        rigid.AddForce(transform.forward * 5 + Vector3.up * 5f, ForceMode.Impulse);
        isDied = true;
        transform.DOLocalRotate(new Vector3(0, 180, 0), 0.2f);
        anim.speed = 0.8f;
        anim.Play("Die");
    }

    public void Update()
    {
        if (isDied) return;
        if(Vector3.Distance(transform.position , PlayerController.ins.trans.position) < 1f)
        {
            GetHit();
        }
    }
}
