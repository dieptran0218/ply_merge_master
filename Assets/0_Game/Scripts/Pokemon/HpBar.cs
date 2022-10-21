using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public PokemonEvent _main;
    public SpriteRenderer hpProgress;
    private Transform transCam;

    public SpriteRenderer spr;
    public Sprite sprGreen;
    public Sprite sprRed;

    private void OnEnable()
    {
        transCam = Camera.main.transform.parent;

        var t = transform.localScale;
        t.x = -t.x;
        transform.localScale = t;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => _main != null && _main.mng != null);
        if (_main.mng.isPlayerPokemon) spr.sprite = sprGreen;
        else spr.sprite = sprRed;
    }

    private void Update()
    {
        transform.rotation = transCam.rotation;
    }

    public void ShowHp()
    {
        gameObject.SetActive(true);
    }

    public void SetHp(int curHp, int maxHp)
    {
        if (_main.mng.isPlayerPokemon) transform.GetChild(0).localRotation = Quaternion.Euler(0, 180, 0);
        else transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);

        var fill = (float)curHp / maxHp;
        var c = hpProgress.size;
        c.x = fill;
        hpProgress.size = c;
    }
}
