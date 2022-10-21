using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallData", menuName = "ScriptableObject/UpgradeWallData", order = 1)]
public class ScriptableWall : ScriptableObject
{
    public List<WallUpgradeData> wallUpgradeData;
}

[System.Serializable]
public class WallUpgradeData
{
    public int price;
}