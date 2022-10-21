using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMove : MonoBehaviour
{
    public float xMin;
    public float xMax;
    public bool startToMin;
    public bool startToMax;
    public float timeAround;

    private void OnEnable()
    {
        if (startToMin) MoveToMinFirst();
        else if (startToMax) MoveToMaxFirst();
    }

    public void MoveToMinFirst()
    {
        transform.DOLocalMoveX(xMin, timeAround * 0.25f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.DOLocalMoveX(xMax, timeAround * 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    transform.DOLocalMoveX(0, timeAround * 0.25f)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            MoveToMinFirst();
                        });
                });
            });
    }

    public void MoveToMaxFirst()
    {
        transform.DOLocalMoveX(xMax, timeAround * 0.25f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.DOLocalMoveX(xMin, timeAround * 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    transform.DOLocalMoveX(0, timeAround * 0.25f)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            MoveToMaxFirst();
                        });
                });
            });
    }
}
