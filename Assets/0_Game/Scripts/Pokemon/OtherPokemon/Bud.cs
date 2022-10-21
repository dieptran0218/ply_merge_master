using DG.Tweening;

public class Bud : Pokemon
{
    public override void SkillAction()
    {
        
    }

    public override void AttackAction()
    {
        if (_target == null) return;
        if (_typeAttack == TypeAttack.Range)
        {
            if (GameManager.ins.mapCurrent.typeEndGame == EndGameType.Chest)
            {
                var o = GetBullet(pokemonEvent.typeBullet);
                var b = SimplePool.Spawn(o, pokemonEvent.transCanon.position, _transRotate.rotation).transform;

                var t = CrystalManager.ins.GetTarget();
                b.LookAt(t);

                b.DOMove(t, 8f)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        CrystalManager.ins.SpawnGem(t, info.lv);
                        b.GetComponent<ProjectileMover>().OnCollision(t);

                        _target.GetHitAttack(_dam);
                    });
            }
            else
            {
                if (info.lv == 3)
                {
                    //Bắn 3 phát đạn lên trời
                    if (stage_Anim == PokemonAnimStage.Attack_Skill)
                    {
                        for (var i = 0; i < pokemonEvent.listCanon.Count; i++)
                        {
                            var o = GetBullet(pokemonEvent.typeBullet);
                            var b = SimplePool.Spawn(o, pokemonEvent.transCanon.position, _transRotate.rotation).transform;

                            var t = _target.pokemonEvent.transHit.transform.position;
                            b.LookAt(t);

                            b.DOMove(t, 8f)
                                .SetSpeedBased(true)
                                .SetEase(Ease.Linear)
                                .OnComplete(() =>
                                {
                                    b.GetComponent<ProjectileMover>().OnCollision(_target.pokemonEvent.transHit.position);

                                    if (!_isBuffing) _target.GetHitAttack(_dam);
                                    else _target.KnockBackAttack(this, 0.3f);
                                });
                        }
                    }
                }
                else
                {
                    //Bắn 3 phát đạn thường
                    for (var i = 0; i < pokemonEvent.listCanon.Count; i++)
                    {
                        var o = GetBullet(pokemonEvent.typeBullet);
                        var b = SimplePool.Spawn(o, pokemonEvent.transCanon.position, _transRotate.rotation).transform;

                        var t = _target.pokemonEvent.transHit.transform.position;
                        b.LookAt(t);

                        b.DOMove(t, 8f)
                            .SetSpeedBased(true)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                b.GetComponent<ProjectileMover>().OnCollision(_target.pokemonEvent.transHit.position);

                                if (!_isBuffing) _target.GetHitAttack(_dam);
                                else _target.KnockBackAttack(this, 0.3f);
                            });
                    }
                }
            }
        }
    }
}
