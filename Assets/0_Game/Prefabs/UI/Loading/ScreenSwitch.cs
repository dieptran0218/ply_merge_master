using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSwitch : MonoBehaviour
{
    public static ScreenSwitch ins;
    public CanvasGroup group;

    private void Awake()
    {
        if (ins != null) Destroy(gameObject);
        else
        {
            ins = this;
            DontDestroyOnLoad(this);
        }
    }

    public void ShowOn(Action c)
    {
        group.gameObject.SetActive(true);
        group.DOKill();
        group.alpha = 0;
        group.DOFade(1, 0.3f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Timer.Schedule(this, 1f, () =>
                {
                    if (c != null) c.Invoke();
                });
            });
    }

    public void Show(Action c = null, float alpha = 0)
    {
        group.gameObject.SetActive(true);
        StartCoroutine(ie_Show(alpha, c));
    }

    IEnumerator ie_Show(float alpha, Action c)
    {
        group.alpha = alpha;
        if (alpha < 0.9f)
        {
            group.DOFade(1, 0.45f)
            .SetEase(Ease.Linear);
            yield return Yielders.Get(0.45f);
        }

        if (c != null)
        {
            c?.Invoke();
        }

        group.DOFade(0, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                group.gameObject.SetActive(false);
            });
    }
}
