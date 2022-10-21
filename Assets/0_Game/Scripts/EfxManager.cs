using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EfxManager : SingletonMonoBehaviour<EfxManager>
{
    public Image gem;
    public Transform efxHolder;

    public void GetGemFx(Vector3 startPos, Vector3 endPos)
    {
        for (int i = 0; i < Random.Range(15, 25); i++)
        {
            float randomX = Random.Range(-125f, 125f);
            float randomY = Random.Range(-50f, 50f);
            //Vector3 firstDesPos = new Vector3(startPos.x + randomX, startPos.y + randomY, 0);
            var tempImg = SimplePool.Spawn(gem.gameObject, startPos, Quaternion.identity).GetComponent<RectTransform>();
            tempImg.transform.SetParent(efxHolder);
            tempImg.position = startPos;
            tempImg.transform.localScale = Vector3.one * 0.65f;
            Vector3 firstDesPos = new Vector3(tempImg.position.x + randomX, tempImg.position.y + randomY, 0);
            tempImg.transform.DOMove(firstDesPos, Random.Range(0.3f, 0.8f)).SetEase(Ease.InQuad).OnComplete(() => {
                tempImg.transform.DOMove(new Vector3(endPos.x, endPos.y, 0), Random.Range(0.8f, 1.2f)).SetEase(Ease.InQuad).OnComplete(() => {
                    SoundController.PlaySoundOneShot(SoundController.ins.gem_collect);
                    SimplePool.Despawn(tempImg.gameObject);
                });
            });
        }
    }

    public void GetGemFx_2(Vector3 startPos, Vector3 endPos)
    {
        for (int i = 0; i < Random.Range(5, 10); i++)
        {
            var ran = Random.insideUnitCircle * Random.Range(200, 300);
            //Vector3 firstDesPos = new Vector3(startPos.x + randomX, startPos.y + randomY, 0);
            var tempImg = SimplePool.Spawn(gem.gameObject, startPos, Quaternion.identity).GetComponent<RectTransform>();
            tempImg.transform.SetParent(efxHolder);
            tempImg.position = startPos;
            tempImg.transform.localScale = Vector3.one * 0.35f;
            Vector3 firstDesPos = new Vector3(tempImg.position.x + ran.x, tempImg.position.y + ran.y, 0);
            tempImg.transform.DOMove(firstDesPos, Random.Range(0.3f, 0.8f)).SetEase(Ease.InQuad).OnComplete(() => {
                tempImg.transform.DOMove(new Vector3(endPos.x, endPos.y, 0), Random.Range(1f, 1.5f)).SetEase(Ease.InQuad).OnComplete(() => {
                    SoundController.PlaySoundOneShot(SoundController.ins.gem_collect);
                    SimplePool.Despawn(tempImg.gameObject);
                });
            });
        }
    }

    public Transform startPos;
    public Transform endPos;
    [ContextMenu("test")]
    public void Play()
    {
        GetGemFx(startPos.position, endPos.position);
    }
}
