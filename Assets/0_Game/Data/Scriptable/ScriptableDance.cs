using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DanceData", menuName = "ScriptableObject/DanceData", order = 1)]
public class ScriptableDance : ScriptableObject
{
    public List<DanceData> danceData;
}

[System.Serializable]
public class DanceData
{
    public DanceId danceID;
    public Sprite danceIcon;
    public int price;
    public int unlockLevel;
}
