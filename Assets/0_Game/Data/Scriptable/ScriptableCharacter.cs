using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkinCharData", menuName = "ScriptableObject/SkinCharData", order = 1)]
public class ScriptableCharacter : ScriptableObject
{
    public List<SkinCharData> skinCharData;
}

[System.Serializable]
public class SkinCharData
{
    public CharacterType skinCharID;
    public Sprite charIcon;
    public GameObject charModel;
    public int price;
    public int unlockLevel;
}
