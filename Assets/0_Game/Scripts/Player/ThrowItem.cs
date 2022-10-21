using DG.Tweening;
using UnityEngine;

public class ThrowItem : MonoBehaviour
{
    public Transform transParent;

    public void Setup(ItemType type, Gate _gateTrigger)
    {
        SoundController.PlaySoundOneShot(SoundController.ins.throw_item);
        var o = GameConfig.ins.GetItemIcon(type);
        if (o != null)
        {
            transParent.DespawnAllChild();
            var t = SimplePool.Spawn(o.model, transform.position, Quaternion.identity).transform;
            t.SetParent(transParent);
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.Euler(0, 0, 0);
        }
        Throw(_gateTrigger, type);
    }

    public void Throw(Gate _gateTrigger, ItemType type)
    {
        transform.DOLocalRotateQuaternion(Random.rotation, 0.5f)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Yoyo);

        transform.DOMove(_gateTrigger.objClaim.transform.position + Vector3.up * 0.5f, 25)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SoundController.PlaySoundOneShot(SoundController.ins.ball_impact);
                if (_gateTrigger.numRequire > 0)
                    _gateTrigger.numRequire--;
                _gateTrigger.ReloadUI();
                if (_gateTrigger.numRequire == 0)
                {
                    _gateTrigger.ActivateOpen();
                }
                else
                {
                    if (_gateTrigger.typeRequire == ItemType.Key)
                    {
                        _gateTrigger.ScaleChest();
                    }
                    else if (_gateTrigger.typeRequire == ItemType.Pokeball)
                    {
                        _gateTrigger.ScalePokemon();
                    }
                }
                GameConfig.ins.SpawnFx(GameConfig.ins.fx_ThrowTrigger, transform.transform.position);
                SimplePool.Despawn(gameObject);
            });
    }
}


