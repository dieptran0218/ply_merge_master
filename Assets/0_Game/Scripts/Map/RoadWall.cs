using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoadWall : MonoBehaviour
{
    public List<TextMeshPro> listValue;
    public List<Transform> listLevelScore;

    public void OnEnable()
    {
        transform.GetChild(0).DOLocalMove(Vector3.zero, 2f)
            .SetEase(Ease.OutQuart);

        var rootValue = GameManager.ins.data.GetWallBonus();
        for(var i = 0; i < listValue.Count; i++)
        {
            listValue[i].text = (rootValue - (0.2 * (listValue.Count - i - 1))).ToString("0.0");
        }
    }

    public float GetIns(int lv)
    {
        if (lv >= listValue.Count) return float.Parse(listValue[listValue.Count - 1].text);
        if (lv < 0) return float.Parse(listValue[0].text);
        return float.Parse(listValue[lv].text);
    }
}
