
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : SingletonMonoBehaviour<GameConfig>
{
    public GameObject prefab_Chest;
    public GameObject prefab_roadWall;
    public Sprite sprIconAds;
    public Sprite sprIconAdsLoading;
    public Sprite sprButtonOff;
    public EndgameRegister endgameRegister;
    public GameObject prefab_GemKill;
    public GameObject floatingText;
    public GameObject prefab_Crystal;


    #region Pokemon
    public GameObject objPrefabPokemon;
    public PokemonRegister pokemonRegister;
    public List<BulletRegister> prefab_Bullet;

    public GameObject prefab_Bullet_End;
    public GameObject prefab_Kamezoko;

    public List<PokemonImgData> PokemonList;
    public PokemonInfo GetPokemon(PokemonType type, int lv)
    {
        var t = pokemonRegister.info.Find(x => x.type == type && x.lv == lv);
        if (t != null)
        {
            return t;
        }
        else {
            //type = GameManager.ins.data.pokemonCollected[Random.Range(0, GameManager.ins.data.pokemonCollected.Count - 1)];
            t = pokemonRegister.info.Find(x => x.type == type && x.lv == lv);
            return t;
        }
    }

    public PokemonInfo GetBossRandom(PokemonType type, int lv)
    {
        return null;
        //var check1 = GameManager.ins.data.pokemonCollected.Find(x => x == type);
        //var check2 = GameManager.ins.data.pokemonAds.Find(x => x == type);

        //if(check1 != PokemonType.None || check2 != PokemonType.None)
        //{
        //    foreach(var t in GameConfig.ins.PokemonList)
        //    {
        //        if (t.type == PokemonType.None) continue;
        //        check1 = GameManager.ins.data.pokemonCollected.Find(x => x == t.type);
        //        check2 = GameManager.ins.data.pokemonAds.Find(x => x == t.type);
        //        if(check1 == PokemonType.None && check2 == PokemonType.None)
        //        {
        //            return pokemonRegister.info.Find(x => x.type == t.type && x.lv == lv);
        //        }
        //    }
        //    var c = System.Enum.GetNames(typeof(PokemonType)).Length;
        //    var po = Random.Range(1, c).ToEnum<PokemonType>();
        //    return pokemonRegister.info.Find(x => x.type == po && x.lv == lv);
        //}

        //return pokemonRegister.info.Find(x => x.type == type && x.lv == lv);
    }

    #endregion

    #region Item
    public List<ItemIcon> itemIcons;

    public ItemIcon GetItemIcon(ItemType  type)
    {
        var o = itemIcons.Find(x => x.type == type);
        if(o != null)
        {
            return o;
        }
        return null;
    }
    #endregion

    #region Gate
    public List<GateMaterial> listGateMaterial;
    #endregion

    #region WallUpgrade
    public ScriptableWall wall;
    #endregion

    #region Skin
    public ScriptableSkin listAllSkin;
    public ScriptableCharacter listCharacter;
    public ScriptableDance listDance;
    #endregion

    #region EFX
    public GameObject fx_Get_Gem;
    public GameObject fx_Get_Ball;
    public GameObject fx_Get_Energy;
    public GameObject fx_Get_Evolution;
    public GameObject fx_Get_Boom;
    public GameObject fx_Get_Key;
    public GameObject fx_CollectPokemon;
    public GameObject fx_LevelUp;
    public GameObject fx_LevelDown;
    public GameObject fx_UpWall;
    public GameObject fx_FireWork_OpenChest;
    public GameObject fx_FireWork_OpenChest_2;
    public GameObject fx_FireWork_End;
    public GameObject fx_ThrowTrigger;
    public GameObject fx_MergeLevelUp;

    //Pokemon Skill
    public GameObject fx_HitMelee_small_1;
    public GameObject fx_HitMelee_Big_1;
    public GameObject fx_Bee_Skill;
    public GameObject fx_Bird_Skill;
    public GameObject fx_Sprout_Skill;
    public GameObject fx_Ghost_Skill;
    public GameObject fx_Stun;
    public GameObject fx_Frighten;
    public GameObject fx_Frezze;
    public GameObject fx_HpHeal;
    public GameObject fx_Explosive;
    public GameObject fx_Ice_Explosive;
    public GameObject fx_Poison_Explosive;
    public GameObject fx_Poison_Screen;
    public GameObject fx_Slash;
    public GameObject fx_Tornado_Orange;
    public GameObject fx_Scopion_Earthquake;
    public GameObject fx_Smoke;
    public GameObject fx_UnlockSlot;

    public Transform SpawnFx(GameObject fx, Vector3 pos, float scale = 1f, float timeAlice = 2f)
    {
        var o = SimplePool.Spawn(fx, pos, Quaternion.identity);
        o.transform.localScale = Vector3.one * scale;
        Timer.Schedule(this, timeAlice, () =>
        {
            SimplePool.Despawn(o);
        });
        return o.transform;
    }
    #endregion

    public void ShowText(string str, Vector3 pos, Transform parent, float scale = 1f)
    {
        var o = SimplePool
            .Spawn(floatingText, pos, Quaternion.identity)
            .GetComponent< FloatingText>();
        o.SetText(str);
        o.transform.SetParent(parent);
    }
}

[System.Serializable]
public class ItemIcon
{
    public ItemType type;
    public GameObject model;
    public Sprite icon;
}

[System.Serializable] 
public class GateMaterial
{
    public GateColor typeColor;
    public Material matGate;
    public Material matLand;
    public Material matDoor;
}

[System.Serializable]
public class BulletRegister
{
    public Projectile type;
    public GameObject obj;
}