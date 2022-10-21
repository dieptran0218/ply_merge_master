using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveBall : MonoBehaviour
{
    private List<GameObject> ballList = new List<GameObject>();
    private float rangeYTop = 0.6f;
    private float rangeYDown = 0.55f;

    private float timeDiff = 0.15f;
    private float duration = 1.2f;

    private bool isStart = false;

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++) {
            ballList.Add(transform.GetChild(i).gameObject);
        }
        //StartTrigger();
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => (GameManager.ins.mapCurrent != null));
        GameManager.ins.mapCurrent.listWave.Add(this);
        yield return new WaitUntil(() => Vector3.Distance(PlayerController.ins.trans.position, transform.position) < 15f);
        StartTrigger();
        yield return new WaitUntil(() => Vector3.Distance(PlayerController.ins.trans.position, transform.position) > 20f);
        GameManager.ins.mapCurrent.listWave.Remove(this);
        gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if (!isStart && Vector3.Distance(PlayerController.ins.playerGroup.transform.position, gameObject.transform.position) < 10f ) {
    //        isStart = true;
    //    }

    //    if (isStart && Vector3.Distance(PlayerController.ins.playerGroup.transform.position, gameObject.transform.position) > 20f) {
    //        //GameObject.Destroy(gameObject);
    //        gameObject.SetActive(false);
    //        GameManager.ins.mapCurrent.listWave.Remove(this);
    //    }
    //}

    private void StartTrigger()
    {
        StartCoroutine(ie_Swing());
    }

    private IEnumerator ie_Swing()
    {
        foreach (var t in ballList)
        {
            BallSwing(t.transform);
            yield return Yielders.Get(timeDiff);
        }
    }

    void BallSwing(Transform t)
    {
        t.transform.DOLocalMoveY(0.1f + t.transform.localPosition.y, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                t.transform.DOLocalMoveY(-0.1f + t.transform.localPosition.y, duration)
                .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        BallSwing(t);
                    });
            });
    }
}

