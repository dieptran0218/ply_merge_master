using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExposive : MonoBehaviour
{
    public Collider col;
    public GameObject efx_Trigger;

    private PokemonEvent main;
    private int eff;

    public void Setup(PokemonEvent m, GameObject efxExplosive, int eff)
    {
        efx_Trigger = efxExplosive;
        col = GetComponent<Collider>();
        col.enabled = false;
        this.main = m;
        this.eff = eff;
        col.enabled = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        var t = other.GetComponent<Pokemon>();
        if (t != null)
        {
            if (eff == 0)
            {
                if (t.isPlayerPokemon != main.mng.isPlayerPokemon)
                {
                    if(efx_Trigger != null) GameConfig.ins.SpawnFx(efx_Trigger, t.pokemonEvent.transHit.position);
                    t.KnockBackAttack(main.mng, 1.5f);
                }
            }
            else if(eff == 1)
            {
                if (t.isPlayerPokemon != main.mng.isPlayerPokemon)
                {
                    if (efx_Trigger != null) GameConfig.ins.SpawnFx(efx_Trigger, t.pokemonEvent.transHit.position);
                    t.KnockBackAttackFrezze(main.mng, 2f, 1f);
                }
            }
        }
    }

    public void OnDisable()
    {
        col.enabled = false;
    }
}
