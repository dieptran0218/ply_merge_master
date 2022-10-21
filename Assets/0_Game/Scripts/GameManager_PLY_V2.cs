using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_PLY_V2 : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager_PLY_V2 Instance;
    public int gemCollected = 2000;
    public int m_SlotPrice = 1000;
    public int m_MonsterPrice = 1000;
    public int totalCeilOpened = 4;
    public bool isGamePlaying = true;
    public Pokemon[] MonsterList;
    public Endgame_3_Ceil[] Ceil_List;
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Start()
    {
        SetUpMonsters();
    }

    private void SetUpMonsters()
    {
        foreach(Pokemon aPokemon in MonsterList)
        {
            aPokemon.Setup(aPokemon.info, false, Mode.Normal, PokemonStage.Endgame);
        }
    }

    public int AddGem(int value)
    {
        gemCollected += value;
        gemCollected = Mathf.Max(gemCollected, 0);
        if (CanvasInGame.ins != null
            && CanvasInGame.ins.gameObject.activeInHierarchy) CanvasInGame.ins.ReloadGem();
        return gemCollected;
    }

}
