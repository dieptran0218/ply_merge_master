using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    public List<GameObject> listMaps;
    private int oldMap;
    public GameObject GetMap(int mapId)
    {
        if (mapId < 20) return listMaps[mapId];
        else
        {
            var t = mapId % 6;

            //Màn thường
            if (t < 4)
            {
                var res = 0;
                while (true)
                {
                    var tmp = Random.Range(0, 3);
                    var tmp_2 = Random.Range(0, 4);
                    res = tmp * 6 + tmp_2;
                    if (res != oldMap) break;
                }
                oldMap = res;
                if (res == 0) return listMaps[res + 1];
                else return listMaps[res];
            }
            //Màn boss
            else if (t == 4)
            {
                return listMaps[Random.Range(1, 4) * 6 - 2];
            }
            //Màn Chess
            else
            {
                return listMaps[Random.Range(1, 4) * 6 - 1];
            }

        }
    }
}
