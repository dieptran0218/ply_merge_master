using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GateType type;
    public GateColor color;

    [Header("Object Register")]
    public GameObject objActive;
    public GameObject objDeactive;
    public GameObject objRequire;
    public GameObject objLand;
    public TextMeshPro txtRequire;

    [Header("Mesh")]
    public MeshRenderer meshGate;
    public MeshRenderer meshNumber;
    public MeshRenderer meshLand;
    public MeshRenderer meshDoor;

    public ItemType typeRequire;
    public int numRequire;

    [Header("=== Data Reward ===")]
    public GameObject objClaim;

    [Header("Pokemon Gate")]
    public PokemonType typePokemon;
    public PokemonLevel startlevel;
    private List<Pokemon> _pokemon = new List<Pokemon>();

    [Header("Key Gate")]
    public GameObject objChestReward;
    public SpriteRenderer sprTypeReward;
    public TextMeshPro txtTypeReward;
    public ItemType typeChestReward;
    public int minValue;
    public int maxValue;
    private int chestReward;
    private Transform objChest;

    [Header("Levelup Gate")]
    public GameObject fxGate;

    private bool _isActived;
    private float rootScale;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            GameConfig.ins != null
            && PlayerController.ins != null
            && GameManager.ins.mapCurrent != null
        );
        _isActived = false;
        //chỉ load 1 lần lúc khởi tạo
        if(objLand != null) objLand.SetActive(numRequire > 0);
        ReloadData();
        LoadMaterial();

        //Sinh Obj claim tùy thuộc vào loại gate
        SpawnObjClaim();

        
    }

    public void ReloadData()
    {
        if (objActive != null) objActive.SetActive(numRequire == 0);
        if (objDeactive != null) objDeactive.SetActive(numRequire > 0);
        if (objRequire != null) objRequire.SetActive(numRequire > 0);
        if (fxGate != null) fxGate.SetActive(numRequire == 0);
        if (objChestReward != null) objChestReward.SetActive(false);

        if (txtRequire != null && numRequire > 0)
        {
            txtRequire.text = numRequire.ToString();
        }
    }

    #region Init
    public void SpawnObjClaim()
    {
        switch (type)
        {
            case GateType.Gate_Pokemon:
                SpawnPokemon();
                rootScale = _pokemon[0].transform.localScale.x;
                return;
            case GateType.Gate_Key:
                SpawnChest();
                return;
        }
    }

    public void SpawnPokemon()
    {
        if(typeRequire != ItemType.Ads || GameManager.ins.data.level >= 20) typePokemon = RandomType();


        _pokemon.Clear();
        GameManager.ins.PreLoadPokemon(typePokemon);
        PokemonInfo info = null;

        if(GameManager.ins.isTest)
        {
            info = GameConfig.ins.GetPokemon(GameManager.ins.pokemonTest, GetLevelPokemon());
        }
        else
        {
            info = GameConfig.ins.GetPokemon(typePokemon, GetLevelPokemon());
        }

        var num = GetNumPokemon();
        for (var i = 0; i < num; i++)
        {
            var o = SimplePool.Spawn(GameConfig.ins.objPrefabPokemon, transform.position, Quaternion.identity).transform;
            o.SetParent(objClaim.transform);
            o.localPosition = Vector3.zero;
            o.GetComponent<Pokemon>().Setup(info, true, GameManager.ins.GetMode());
            o.GetComponent<Collider>().isTrigger = true;
            o.GetComponent<Rigidbody>().isKinematic = true;
            _pokemon.Add(o.GetComponent<Pokemon>());
            _scale = _pokemon[i].transform.localScale;
            _stepScale = 0.1f / numRequire;
            _step = 0;

            if(num == 1)
            {
                o.transform.localPosition = Vector3.zero;
            }
            else
            {
                o.localPosition = Vector3.right * (i == 0 ? -1 : 1) * 0.5f;
            }

            o.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public PokemonType RandomType()
    {
        if(GameManager.ins.data.level == 0)
        {
            return PokemonType.Bee;
        }
        var r = Random.Range(0, GameManager.ins.data.pokemonCollected.Count);
        return GameManager.ins.data.pokemonCollected[r];
    }

    public void SpawnChest()
    {
        if (objChestReward != null) objChestReward.SetActive(true);
        chestReward = Random.Range(minValue, maxValue);
        txtTypeReward.text = chestReward.ToString();
        sprTypeReward.sprite = GameConfig.ins.GetItemIcon(typeChestReward).icon;
        var o = SimplePool.Spawn(GameConfig.ins.prefab_Chest, transform.position, Quaternion.identity).transform;
        o.SetParent(objClaim.transform);
        o.transform.localScale = Vector3.one * 0.75f;
        o.localPosition = Vector3.zero;
        objChest = o.transform;
        _scale = objChest.localScale;
    }

    private float _stepScale;
    private int _step;
    public void ScalePokemon()
    {
        _step += 1;
        if (_step > 5) return;
        foreach (var t in _pokemon)
        {
            t.transform.DOKill();
            t.transform.localScale = _scale;

            _scale = Vector3.one * rootScale * (1 + (_step * _stepScale));
            _scale_2 = Vector3.one * rootScale * (1 + (_step * _stepScale * 2));

            t.transform.DOScale(_scale_2, 0.15f)
                .OnComplete(() =>
                {
                    t.transform.DOScale(_scale, 0.15f);
                });
        }
    }

    private Vector3 _scale;
    private Vector3 _scale_2;
    public void ScaleChest()
    {
        objChest.DOKill();
        objChest.localScale = _scale;

        _scale = objChest.localScale * 1.04f;
        _scale_2 = objChest.transform.localScale * 1.08f;

        objChest.DOScale(_scale_2, 0.15f)
            .OnComplete(() =>
            {
                objChest.DOScale(_scale, 0.15f);
            });
    }

    #endregion

    #region Gate Open / Complete
    public void ReloadUI()
    {
        txtRequire.text = numRequire.ToString();
        if (type == GateType.Gate_Key)
        {
            objClaim.transform.localScale += Vector3.one * 0.01f;
        }
    }

    public void ActivateOpen()
    {
        if (_isActived) return;
        _isActived = true;

        ReloadData();

        //Dosomething with another Gate
        //...
        switch (type)
        {
            case GateType.Gate_Pokemon:
                ActionOpenGatePokemon();
                return;
            case GateType.Gate_Key:
                ActionOpenGateKey();
                return;
            case GateType.Gate_Levelup:
                ActionOpenGateLevelup();
                return;
        }
    }

    public void ActionOpenGatePokemon()
    {
        SoundController.PlaySoundOneShot(SoundController.ins.get_pokemon);
        GameConfig.ins.SpawnFx(GameConfig.ins.fx_CollectPokemon, transform.position);
        //cho pokemon đi theo player
        foreach(var t in _pokemon)
        {
            t.MoveToPlayer();
        }
    }

    public void ActionOpenGateKey()
    {
        //Mở rương
        SoundController.PlaySoundOneShot(SoundController.ins.open_chest);
        objClaim.SetActive(false);
        GameConfig.ins.SpawnFx(GameConfig.ins.fx_CollectPokemon, objClaim.transform.position);


        SoundController.PlaySoundOneShot(SoundController.ins.firework);
        var pos = PlayerController.ins.objFocus.position + PlayerController.ins.trans.forward * 5.5f + new Vector3(-1, 5.5f, 0);
        GameConfig.ins.SpawnFx(GameConfig.ins.fx_FireWork_OpenChest
            , pos);
        EfxManager.ins.GetGemFx_2(Camera.main.WorldToScreenPoint(pos), CanvasInGame.ins.icoGem.position);

        pos = PlayerController.ins.objFocus.position + PlayerController.ins.trans.forward * 5.5f + new Vector3(1, 5.5f, 0);
        GameConfig.ins.SpawnFx(GameConfig.ins.fx_FireWork_OpenChest
            , pos);
        EfxManager.ins.GetGemFx_2(Camera.main.WorldToScreenPoint(pos), CanvasInGame.ins.icoGem.position);

        pos = PlayerController.ins.objFocus.position + PlayerController.ins.trans.forward * 5.5f + new Vector3(0, 6f, 0);
        GameConfig.ins.SpawnFx(GameConfig.ins.fx_FireWork_OpenChest
            , pos);
        EfxManager.ins.GetGemFx_2(Camera.main.WorldToScreenPoint(pos), CanvasInGame.ins.icoGem.position);

        PlayerController.ins.AddItem(typeChestReward, chestReward);
        CanvasInGame.ins.AddGem(chestReward);
    }

    public void ActionOpenGateLevelup()
    {
        //Mở trigger
        objActive.SetActive(true);
        objDeactive.SetActive(false);
        objClaim.SetActive(true);
        objRequire.SetActive(false);
    }
    #endregion

    #region Support
    public void LoadMaterial()
    {
        var dataMesh = GameConfig.ins.listGateMaterial.Find(x => x.typeColor == color);
        if(meshGate != null) meshGate.material = dataMesh.matGate;
        if(meshNumber != null) meshNumber.material = dataMesh.matGate;
        if(meshDoor != null) meshDoor.material = dataMesh.matDoor;
        if(meshLand != null) meshLand.material = dataMesh.matLand;
    }

    public int GetNumPokemon()
    {
        if(startlevel == PokemonLevel.Level_2 
            || startlevel == PokemonLevel.Level_4)
        {
            return 2;
        }
        return 1;
    }

    public int GetLevelPokemon()
    {
        if(startlevel == PokemonLevel.Level_5)
        {
            return 3;
        }
        else if (startlevel == PokemonLevel.Level_4
            || startlevel == PokemonLevel.Level_3)
        {
            return 2;
        }
        return 1;
    }
    #endregion

}
