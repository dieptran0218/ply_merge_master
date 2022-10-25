using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endgame3_CeilManager : SingletonMonoBehaviour<Endgame3_CeilManager>
{
    public static Endgame3_CeilManager Instance;
    public List<SpriteRenderer> sprCeils;
    public List<SpriteRenderer> sprCeilsTmp;
    public GameObject objCube;
    public List<Endgame_3_Ceil> listCeilManager;
    public List<Endgame_3_Ceil> listCeilTmp = new List<Endgame_3_Ceil>();

    public GameObject btnRemove;
    public bool isSelected = false;

    private void OnEnable()
    {
        //foreach (var t in sprCeilsTmp) t.gameObject.SetActive(false);
        StartCoroutine(ie_CheckMerge());
    }

    private void Start()
    {
        //Init player monster
        var objPokemon = GameConfig.ins.GetPokemon(PokemonType.Bee, 2);
        GameManager_PLY_V2.Instance.Ceil_List[3].SpawnPokemonInCeil_1(objPokemon);
        objPokemon = GameConfig.ins.GetPokemon(PokemonType.Chick, 1);
        GameManager_PLY_V2.Instance.Ceil_List[2].SpawnPokemonInCeil_2(objPokemon);
        objPokemon = GameConfig.ins.GetPokemon(PokemonType.Egg, 1);
        GameManager_PLY_V2.Instance.Ceil_List[1].SpawnPokemonInCeil_1(objPokemon);

        objPokemon = GameConfig.ins.GetPokemon(PokemonType.Spider, 1);
        GameManager_PLY_V2.Instance.Ceil_Tmp_List[3].SpawnPokemonInCeil_1(objPokemon);
        objPokemon = GameConfig.ins.GetPokemon(PokemonType.Chick, 1);
        GameManager_PLY_V2.Instance.Ceil_Tmp_List[2].SpawnPokemonInCeil_1(objPokemon);
        objPokemon = GameConfig.ins.GetPokemon(PokemonType.Chick, 1);
        GameManager_PLY_V2.Instance.Ceil_Tmp_List[1].SpawnPokemonInCeil_1(objPokemon);
        objPokemon = GameConfig.ins.GetPokemon(PokemonType.Ghost, 1);
        GameManager_PLY_V2.Instance.Ceil_Tmp_List[0].SpawnPokemonInCeil_1(objPokemon);
        objPokemon = GameConfig.ins.GetPokemon(PokemonType.Bee, 1);
        GameManager_PLY_V2.Instance.Ceil_Tmp_List[4].SpawnPokemonInCeil_1(objPokemon);

        //objPokemon = GameConfig.ins.GetPokemon(PokemonType.Bee, 1);
        //GameManager_PLY_V2.Instance.Ceil_Tmp_List[3].SpawnPokemonInCeil_1(objPokemon);
    }

    IEnumerator ie_CheckMerge()
    {
        yield return new WaitUntil(() => CanvasFight.ins != null);
        while(CanvasFight.ins.gameObject.activeInHierarchy)
        {
            if(!isSelected) ReloadCheckMerge();
            yield return Yielders.Get(1f);
        }
    }

    public void ReloadCheckMerge()
    {
        foreach (var t in listCeilManager)
        {
            if (t != null) t.ShowCanMerge(CheckCanMerge(t));
        }

        foreach (var t in listCeilTmp)
        {
            if(t != null) t.ShowCanMerge(CheckCanMerge(t));
        }
    }

    public void ReloadCheckMerge(Endgame_3_Ceil ceil)
    {
        foreach (var t in listCeilManager)
        {
            if (t != null && t != ceil)
            {
                t.ShowCanMerge_Only(CheckCanMerge_Only(t, ceil));
            }
        }

        foreach (var t in listCeilTmp)
        {
            if (t != null) t.ShowCanMerge_Only(CheckCanMerge_Only(t, ceil));
        }
    }

    public void HideAllMerge()
    {
        foreach (var t in listCeilManager)
        {
            if (t != null) t.ShowCanMerge(false);
        }

        foreach (var t in listCeilTmp)
        {
            if (t != null) t.ShowCanMerge(false);
        }
    }

    public bool CheckCanMerge(Endgame_3_Ceil ceil)
    {
        foreach(var t in listCeilManager)
        {
            if (ceil != t
                && t != null
                && ceil.info.levelUpdate > 0
                && ceil.info.levelUpdate == t.info.levelUpdate
                && ceil.info.type == t.info.type)
            {
                return true;
            }
        }

        foreach(var t in listCeilTmp)
        {
            if (ceil != t
                && t != null
                && ceil.info.levelUpdate > 0
                && ceil.info.levelUpdate == t.info.levelUpdate
                && ceil.info.type == t.info.type)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckCanMerge_Only(Endgame_3_Ceil origin, Endgame_3_Ceil ceil_2)
    {
        if (origin != ceil_2
                && ceil_2 != null
                && ceil_2.info.levelUpdate > 0
                && ceil_2.info.levelUpdate == origin.info.levelUpdate
                && ceil_2.info.type == origin.info.type)
        {
            return true;
        }
        return false;
    }

    public void Setup(int numCeilOpened)
    {
        listCeilManager.Clear();

        //Mở các ô đã nhận được
        for (var i = 0; i < sprCeils.Count; i++)
        {
            if (i < numCeilOpened)
            {
                sprCeils[i].color = GameHelper.GetColorAlpha(Color.white, 1);
                sprCeils[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (i < numCeilOpened + 1)
            {
                sprCeils[i].color = GameHelper.GetColorAlpha(Color.white, 0.3f);
                sprCeils[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                sprCeils[i].color = GameHelper.GetColorAlpha(Color.white, 0f);
                sprCeils[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        //Load lại các pokemon đã từng lưu trước đó
        var data = GameManager.ins.data.endgame_CeilInfo;

        foreach(var t in sprCeils)
        {
            t.transform.DespawnAllChild(t.transform.GetChild(0).gameObject);
        }

        foreach(var t in data)
        {
            var type = t.type;
            var lv = t.GetLevelPokemon();
            var objPokemon = GameConfig.ins.GetPokemon(type, lv);
            var count = t.GetNumPokemon();

            var ceil = SimplePool.Spawn(objCube, Vector3.zero, Quaternion.identity).transform;
            ceil.SetParent(this.sprCeils[t.id].transform);
            ceil.localPosition = Vector3.forward * 0.5f;
            ceil.localRotation = Quaternion.Euler(Vector3.zero);
            ceil.localScale = Vector3.one;

            var scr = ceil.GetComponent<Endgame_3_Ceil>();
            listCeilManager.Add(scr);

            if(count == 1)
            {
                scr.SpawnPokemonInCeil_1(objPokemon, t);
            }
            else if(count == 2)
            {
                //scr.SpawnPokemonInCeil_2(objPokemon, t);
            }
            else
            {
                scr.info = t;
            }
        }
        ReloadCheckMerge();
    }

    public void Refresh()
    {
        Setup(GameManager.ins.data.totalCeilOpened);
    }

    [HideInInspector] public int _countCeilTmp;
    public void Merge(Pokemon pokemon)
    {
        var check = CheckMerge(pokemon);

        //case 1: có ô trống
        if(check.Item1 == 1)
        {
            check.Item2.info.preLevelUpdate += 1;

            pokemon.transform.SetParent(check.Item2.transform);
            pokemon._transRotate.LookAt(check.Item2.transform);

            pokemon.transform.DOLocalMove(Vector3.zero, 5f)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true)
                .OnComplete(() =>
                {
                    pokemon.transform.localRotation = Quaternion.Euler(0, 90, 90);
                    pokemon.transform.GetChild(0).localRotation = Quaternion.Euler(0, 180, 0);
                    check.Item2.Merge(pokemon.info);
                    ReloadCheckMerge();
                });
        }

        //case 2: không có ô trống
        else if(check.Item1 == 2)
        {
            //Tạo 1 ceil rỗng ở hàng đợi sau đó cho pokemon chạy vô
            _countCeilTmp++;

            sprCeilsTmp[listCeilTmp.Count].gameObject.SetActive(true);
            var ceil = SimplePool.Spawn(objCube, Vector3.zero, Quaternion.identity).transform;
            ceil.SetParent(this.sprCeilsTmp[listCeilTmp.Count].transform);
            ceil.localPosition = Vector3.forward * 0.5f;
            ceil.localRotation = Quaternion.Euler(Vector3.zero);
            ceil.localScale = Vector3.one;
            var scr = ceil.GetComponent<Endgame_3_Ceil>();
            scr.SpawnPokemonInCeil_Tmp(pokemon, sprCeilsTmp[listCeilTmp.Count].gameObject);
            listCeilTmp.Add(scr);
            scr._mainPokemon.Add(pokemon);
            pokemon.transform.SetParent(ceil);
            pokemon._transRotate.LookAt(ceil);

            pokemon.transform.DOLocalMove(Vector3.back * 0.5f, 5f)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true)
                .OnComplete(() =>
                {
                    pokemon.transform.localRotation = Quaternion.Euler(0, 90, 90);
                    pokemon.transform.GetChild(0).localRotation = Quaternion.Euler(0, 180, 0);
                    pokemon.SetAnimation(PokemonAnimStage.Idle);
                    ReloadCheckMerge();
                });
        }
    }

    public (int, Endgame_3_Ceil) CheckMerge(Pokemon pokemon)
    {
        var check = false;
        var i = 0;
        var data = GameManager.ins.data.endgame_CeilInfo;

        for(; i < data.Count; i++) 
        {
            if(data[i].levelUpdate == 0 && data[i].preLevelUpdate == 0)
            {
                check = true;
                break;
            }
        }

        //Có ô đang trống
        //status: 1
        if(check)
        {
            return (1, listCeilManager[i]);
        }
        //Tất cả các ô đã fill
        else
        {
            var find = data.Find(x =>
                    x.type == pokemon.info.type
                    && x.GetNumPokemon() == 1
                    && x.GetLevelPokemon() == pokemon.info.lv
                    && x.CheckMaxLv());

            if(find != null)
            {
                //Có thể merge
            }
            else
            {
                //không tìm được mục tiêu merge
            }
        }

        return (2, null);
    }

    public void SetupListTmp(int numRequire)
    {
        numRequire = Mathf.Max(numRequire, 3);
        numRequire = Mathf.Min(numRequire, 5);
        foreach (var t in sprCeilsTmp) t.gameObject.SetActive(false);
        if(numRequire == 3)
        {
            for(var i = 0; i < 3; i++)
            {
                sprCeilsTmp[i].gameObject.SetActive(false);
                sprCeilsTmp[i].transform.localScale = Vector3.one * 0.8f;
                sprCeilsTmp[i].transform.localPosition = Vector3.up * (i - 1) * 1.55f;
            }
        }
        else if(numRequire == 4)
        {
            var p = 1.7f;
            for (var i = 0; i < 4; i++)
            {
                sprCeilsTmp[i].gameObject.SetActive(false);
                sprCeilsTmp[i].transform.localScale = Vector3.one * 0.6f;
                sprCeilsTmp[i].transform.localPosition = Vector3.up * (p - 1.15f * i);
            }
        }
        else if (numRequire == 5)
        {
            var p = 1.9f;
            for (var i = 0; i < 5; i++)
            {
                sprCeilsTmp[i].gameObject.SetActive(false);
                sprCeilsTmp[i].transform.localScale = Vector3.one * 0.5f;
                sprCeilsTmp[i].transform.localPosition = Vector3.up * (p - 0.95f * i);
            }
        }
    }
}
