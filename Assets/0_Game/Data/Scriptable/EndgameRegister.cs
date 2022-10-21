using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndgameRegister", menuName = "ScriptableObject/EndgameRegister", order = 0)]

public class EndgameRegister : ScriptableObject
{
    public List<EndgameRegisterInfo> info = new List<EndgameRegisterInfo>();
}

[System.Serializable]
public class EndgameRegisterInfo
{
    public List<EndgameMonsterInfo> listMonster = new List<EndgameMonsterInfo>();
}

[System.Serializable]
public class EndgameMonsterInfo
{
    public PokemonType type;
    public int lv;
    public float scalePower;
    public Vector3 localPosition;
    public Vector3 localScale;
    public TypeAttack typeAttack;
    public GameObject objTmp;   
}

[System.Serializable]
public enum TypeAttack
{
    Melee,
    Range
}