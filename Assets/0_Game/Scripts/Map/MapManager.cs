using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform transPosFocus;
    public List<RoadItem> listRoadItems;
    public List<WaveBall> listWave;
    public EndGameType typeEndGame;
    public PokemonType typeBoss;
    public int numEnemys;

    public EndGame endgame;

    [ContextMenu("Set up Connection")]
    public void SetupConnection() {
        for (int i = 0; i < listRoadItems.Count - 1; i++) {
            listRoadItems[i].connectTo = listRoadItems[i + 1].gameObject;
        }
        for (int i = 0; i < listRoadItems.Count - 1; i++)
        {
            listRoadItems[i].ConnectRoad();
        }
    }
}
