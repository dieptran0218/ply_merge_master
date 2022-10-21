using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkinData", menuName = "ScriptableObject/SkinData", order = 1)]
public class ScriptableSkin : ScriptableObject
{
    public List<SkinData> skinData;
}

[System.Serializable]
public class SkinData {
    public SkinId skinID;
    public Sprite skinIcon;
    public GameObject skinModel;
    //public Transform skinModel;
    public bool isOpenAd;
    public int price;
    public int unlockLevel;
}
