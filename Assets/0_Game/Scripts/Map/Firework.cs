using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    public List<Transform> listPos;
    public GameObject objFirework;

    public void Setup()
    {
        StartCoroutine(ie_ShowFirework());
    }

    IEnumerator ie_ShowFirework()
    {
        while(true)
        {
            var ran = Random.Range(1, 4);
            for (var i = 0; i < ran; i++)
            {
                var t = SimplePool.Spawn(objFirework, Vector3.zero, Quaternion.identity).transform;
                t.position = listPos[Random.Range(0, listPos.Count)].position;
                t.localScale = Vector3.one * Random.Range(1.5f, 2.5f);
                Timer.Schedule(this, 1f, () => { SimplePool.Despawn(t.gameObject); });
                yield return Yielders.Get(0.25f);
            }
            yield return Yielders.Get(1f);
        }
    }

}
