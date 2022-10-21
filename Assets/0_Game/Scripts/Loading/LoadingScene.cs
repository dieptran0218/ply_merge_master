using DG.Tweening;
using UnityEngine.UI;

public class LoadingScene : SingletonMonoBehaviour<LoadingScene>
{
    public Image imgProgress;

    public void SetPercent(float to, float time)
    {
        imgProgress.DOFillAmount(to, time)
            .SetEase(Ease.Linear);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
