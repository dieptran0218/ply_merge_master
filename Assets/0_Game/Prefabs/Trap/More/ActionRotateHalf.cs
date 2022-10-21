using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRotateHalf : MonoBehaviour
{
    public Vector3 rotateMin;
    public Vector3 rotateMax;
    public bool startToMin;
    public bool startToMax;
    public float timeAround;
    public float timeDelay;

    private void OnEnable()
    {
        if (startToMin) MoveToMinFirst();
        else if (startToMax) MoveToMaxFirst();
    }

    public void MoveToMinFirst()
    {
        transform.DOLocalRotate(rotateMin, timeAround * 0.25f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.DOLocalRotate(Vector3.zero, timeAround * 0.25f)
                .SetEase(Ease.Linear)
                .SetDelay(timeDelay)
                .OnComplete(() =>
                {
                    transform.DOLocalRotate(rotateMax, timeAround * 0.25f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        transform.DOLocalRotate(Vector3.zero, timeAround * 0.25f)
                            .SetEase(Ease.Linear)
                            .SetDelay(timeDelay)
                            .OnComplete(() =>
                            {
                                MoveToMinFirst();
                            });
                    });
                });
            });
    }

    public void MoveToMaxFirst()
    {
        transform.DOLocalRotate(rotateMax, timeAround * 0.25f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.DOLocalRotate(rotateMin, timeAround * 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    transform.DOLocalRotate(Vector3.zero, timeAround * 0.25f)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            MoveToMaxFirst();
                        });
                });
            });
    }
}
