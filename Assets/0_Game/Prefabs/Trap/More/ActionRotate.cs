using DG.Tweening;
using UnityEngine;

public class ActionRotate : MonoBehaviour
{
    public Vector3 rotateTo;
    public float time;

    private void OnEnable()
    {
        transform.DOLocalRotate(rotateTo, time, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }
}
