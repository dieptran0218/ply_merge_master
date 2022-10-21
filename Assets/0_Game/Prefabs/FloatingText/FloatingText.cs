using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    public TextMeshPro txtDam;
    public Transform transRotate;
    public Transform trans;

    public void SetText(string txt)
    {
        trans.DOKill();
        transRotate.rotation = Camera.main.transform.rotation;
        txtDam.text = txt;
        Animation();
    }

    public void Animation()
    {
        var time = 0.75f;
        trans.localScale = Vector3.zero;
        trans.DOScale(Vector3.one, time)
            .OnComplete(() =>
            {
                SimplePool.Despawn(gameObject);
            });

        trans.localPosition = Vector3.zero;
        trans.DOLocalMoveY(1f, time);
    }
}
