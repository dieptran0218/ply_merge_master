using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataManager
{
    public int gemCollected;
    public int wallLevel;
    public int wallUpgradeLevel;
    public int level;

    //Data Reward + Buy
    //buy unit
    public int countBuyUnit;
    public int countBuySlot;
    public int countInsPrize = 2;

    //win reward
    public int countInsReward = 2;

    [HideInInspector] public int lastSpinTime;

    public SkinId skinUsed;
    public List<SkinId> skinCollected = new List<SkinId>();
    public SkinId skinAdsId;

    public CharacterType charUsed;
    public List<CharacterType> charCollected = new List<CharacterType>();
    public CharacterType charAdsId;

    public DanceId danceUsed;
    public List<DanceId> danceCollected = new List<DanceId>();
    public DanceId danceAdsId;

    public List<PokemonType> pokemonCollected = new List<PokemonType>();
    public List<PokemonType> pokemonAds = new List<PokemonType>();
    public PokemonType curPokemon;
    public int loadPokemonPercent;
    public int currentPokemon = 2;
    public CharacterType charType;

    public int totalTimePlayed;
    public int failCount;

    public int totalCeilOpened = 4;
    public List<CeilInfo> endgame_CeilInfo = new List<CeilInfo>();

    public SkinData GetSkin(SkinId id)
    {
        var t = GameConfig.ins.listAllSkin.skinData.Find(x => x.skinID == id);
        return t;
    }

    public DanceData GetDance(DanceId id) 
    {
        var t = GameConfig.ins.listDance.danceData.Find(x => x.danceID == id);
        return t;
    }

    public SkinCharData GetChar(CharacterType id)
    {
        var t = GameConfig.ins.listCharacter.skinCharData.Find(x => x.skinCharID == id);
        return t;
    }

    public int GetCharIndex(CharacterType type)
    {
        for (int i = 0; i < GameConfig.ins.listCharacter.skinCharData.Count; i++) {
            if (GameConfig.ins.listCharacter.skinCharData[i].skinCharID == type)
                return i;
        }
        return -1;
    }

    public float GetWallBonus()
    {
        return wallLevel  * 0.2f + 2.2f;
    }

    public PokemonType GetRandomPokemon()
    {
        return pokemonCollected[Random.Range(0, pokemonCollected.Count)];
    }

    public int AddGem(int value)
    {
        gemCollected += value;
        gemCollected = Mathf.Max(gemCollected, 0);
        if(CanvasInGame.ins != null 
            && CanvasInGame.ins.gameObject.activeInHierarchy) CanvasInGame.ins.ReloadGem();
        return gemCollected;
    }

    public void AddSlot()
    {
        var tmp = new CeilInfo(endgame_CeilInfo.Count);
        endgame_CeilInfo.Add(tmp);
        totalCeilOpened = endgame_CeilInfo.Count;
    }

    public void AddUnit()
    {
        var ran = Random.Range(0, pokemonCollected.Count);
        if (GameManager.ins.data.level == 0)
        {
            ran = 2;
        }
        foreach (var t in endgame_CeilInfo)
        {
            if(t.levelUpdate == 0)
            {
                t.levelUpdate = 1;
                t.isOpen = true;
                if (GameManager.ins.data.level == 0)
                {
                    t.type = PokemonType.Bee;
                }
                else t.type = pokemonCollected[ran];
                return;
            }
        }
    }
}

[System.Serializable]
public class CeilInfo
{
    public int id;
    public bool isOpen;
    public int levelUpdate;
    public int preLevelUpdate;
    public PokemonType type;

    public CeilInfo(int id)
    {
        this.id = id;
        isOpen = true;
        levelUpdate = 0;
        preLevelUpdate = levelUpdate;
        type = PokemonType.None;
    }
    
    public int GetNumPokemon()
    {
        if (levelUpdate == 0) return -1;
        if (levelUpdate == 2 || levelUpdate == 4) return 2;
        return 1;
    }

    public int GetLevelPokemon()
    {
        if (levelUpdate == 0) return -1;
        if (levelUpdate <= 2) return 1;
        else if (levelUpdate <= 4) return 2;
        else return 3;
    }

    public bool CheckMaxLv()
    {
        return levelUpdate >= 5;
    }

    public bool CheckCanMerge(CeilInfo info)
    {
        if (info.levelUpdate == 0) return true;

        if(type == info.type && levelUpdate == info.levelUpdate)
        {
            return true;
        }
        return false;
    }

    public void ClearData()
    {
        isOpen = true;
        levelUpdate = 0;
        preLevelUpdate = levelUpdate;
        type = PokemonType.None;
    }

    public void CloneData(CeilInfo clone)
    {
        this.isOpen = clone.isOpen;
        this.levelUpdate = clone.levelUpdate;
        this.preLevelUpdate = clone.preLevelUpdate;
        this.type = clone.type;
    }
}
