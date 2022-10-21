using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ChestBox : MonoBehaviour
{
    public Transform transSpawn;
    public GameObject prefab_Gem;
    public Vector3 force;

    [ContextMenu("test")]
    public void OpenChest()
    {
        SpawnGem();
    }

    public void SpawnGem()
    {
        SoundController.PlaySoundOneShot(SoundController.ins.open_chest);
        StartCoroutine(ie_SpawnGem());
    }

    IEnumerator ie_SpawnGem()
    {
        for (var k = 0; k < 10; k++)
        {
            var c = Random.Range(2, 5);
            for (var i = 0; i < c; i++)
            {
                var o = SimplePool.Spawn(prefab_Gem, Vector3.zero, Quaternion.identity).transform;
                o.position = transSpawn.position;
                o.localScale = Vector3.one * 1f;
                var f = new Vector3(Random.Range(-2f, 2f), Random.Range(9f, 11f), Random.Range(-2f, 2f));
                o.GetComponent<Rigidbody>().AddForce(f, ForceMode.Impulse);
                Timer.Schedule(this, 4f, () => { Destroy(o.gameObject); });
            }
            GameManager.ins.data.AddGem(2 * GameManager.ins.data.level);
            CanvasInGame.ins.AddGem(2 * GameManager.ins.data.level);
            yield return Yielders.Get(0.2f);
        }
    }
}
