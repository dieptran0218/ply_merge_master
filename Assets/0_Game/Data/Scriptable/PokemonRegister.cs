using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon_Register", menuName = "Pokemon_Register", order = 0)]
public class PokemonRegister : ScriptableObject
{
    public List<PokemonInfo> info = new List<PokemonInfo>();
}

[System.Serializable]
public class PokemonInfo
{
    public PokemonType type;
    public int lv;
    [Space]
    public bool isMelee;
    public bool isRange;
    public int numMeleeType;
    [Space]
    public GameObject objPrefab;
    public int dam;
    public int hp;
    public float range;
    public float speedAttack;
    public int castSkillCountEnemy;
    public int castSkillCountPlayer;
    public float skillTime;

    [Space]
    public float bulletScale;
}

[System.Serializable]

public class PokemonImgData
{
    public PokemonType type;
    public Sprite img;
    public Sprite blurImg;
    public Sprite singleImg;
    public Sprite blurSingleImg;
}