using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    public PokemonEvent main;

    public void OnTriggerEnter(Collider other)
    {
        var t = other.GetComponent<Pokemon>();
        if(t != null)
        {
            if (main.fx_TriggerSkill != null) 
                GameConfig.ins.SpawnFx(main.fx_TriggerSkill, t.pokemonEvent.transHit.position);
            t.KnockBackAttack(main.mng, 0.1f);
        }
    }
}
