using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endgame_3_Ceil : MonoBehaviour, IElement
{
    public List<Pokemon> _mainPokemon = new List<Pokemon>();
    public CeilInfo info;
    public bool isTmp;
    public GameObject ceilLayout;
    public GameObject fx_CanMerge;
    public bool isTrash;

    private bool isHold;

    #region Init
    public void SpawnPokemonInCeil_Tmp(Pokemon obj, GameObject ceilLayout)
    {
        ShowCanMerge(false);
        Clear();
        isTmp = true;
        _mainPokemon.Add(obj);
        this.ceilLayout = ceilLayout;

        info = new CeilInfo(0);
        info.type = obj.info.type;
        if (obj.info.lv == 1) info.levelUpdate = 1;
        else if (obj.info.lv == 2) info.levelUpdate = 3;
        else if (obj.info.lv == 3) info.levelUpdate = 5;
    }

    public void SpawnPokemonInCeil_1(PokemonInfo obj, CeilInfo info, bool isTmp = false)
    {
        ShowCanMerge(false);
        Clear();
        this.info = info;
        this.isTmp = isTmp;

        var po1 = SimplePool.Spawn(GameConfig.ins.objPrefabPokemon, Vector3.zero, Quaternion.identity).GetComponent<Pokemon>();
        po1.transform.SetParent(transform);
        po1.transform.localPosition = new Vector3(0f, 0f, -0.5f);
        po1.Setup(obj, true, Mode.Normal, PokemonStage.Endgame);
        po1.SetTriggerStatus(true);
        _mainPokemon.Add(po1);
        po1.transform.localScale = Vector3.one * 1.25f;
        po1.transform.localRotation = Quaternion.Euler(0, 90, 90);
        po1.transform.GetChild(0).localRotation = Quaternion.Euler(0, -180, 0);

    }

    public void SpawnPokemonInCeil_1(PokemonInfo obj, bool isTmp = false)
    {
        ShowCanMerge(false);
        Clear();
        this.isTmp = isTmp;

        var po1 = SimplePool.Spawn(GameConfig.ins.objPrefabPokemon, Vector3.zero, Quaternion.identity).GetComponent<Pokemon>();
        po1.transform.SetParent(transform);
        po1.transform.localPosition = new Vector3(0f, 0f, -0.5f);
        po1.Setup(obj, true, Mode.Normal, PokemonStage.Endgame);
        po1.SetTriggerStatus(true);
        _mainPokemon.Add(po1);
        po1.transform.localScale = Vector3.one * 1.25f;
        po1.transform.localRotation = Quaternion.Euler(0, 90, 90);
        po1.transform.GetChild(0).localRotation = Quaternion.Euler(0, -180, 0);
        PlayerController.ins.listPokemon.Add(po1);
    }

    public void SpawnPokemonInCeil_2(PokemonInfo obj, CeilInfo info, bool isTmp = false)
    {
        ShowCanMerge(false);
        Clear();

        this.info = info;
        this.isTmp = isTmp;

        var po1 = SimplePool.Spawn(GameConfig.ins.objPrefabPokemon, Vector3.zero, Quaternion.identity).GetComponent<Pokemon>();
        po1.transform.SetParent(transform);
        po1.transform.localPosition = new Vector3(0f, -GetDistanceInCeil(obj.lv), -0.5f);
        po1.Setup(obj, true, Mode.Normal, PokemonStage.Endgame);
        po1.SetTriggerStatus(true);
        _mainPokemon.Add(po1);
        po1.transform.localScale = Vector3.one * 1.25f;
        po1.transform.GetChild(0).localRotation = Quaternion.Euler(0, -180, 0);
        po1.transform.localRotation = Quaternion.Euler(0, 90, 90);

        var po2 = SimplePool.Spawn(GameConfig.ins.objPrefabPokemon, Vector3.zero, Quaternion.identity).GetComponent<Pokemon>();
        po2.transform.SetParent(transform);
        po2.transform.localPosition = new Vector3(0f, GetDistanceInCeil(obj.lv), -0.5f);
        po2.Setup(obj, true, Mode.Normal, PokemonStage.Endgame);
        po2.SetTriggerStatus(true);
        _mainPokemon.Add(po2);
        po2.transform.localScale = Vector3.one * 1.25f;
        po2.transform.localRotation = Quaternion.Euler(0, 90, 90);
        po2.transform.GetChild(0).localRotation = Quaternion.Euler(0, -180, 0);
    }

    public void ShowCanMerge(bool b)
    {
        fx_CanMerge.SetActive(b);
        fx_CanMerge.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowCanMerge_Only(bool b)
    {
        fx_CanMerge.SetActive(b);
        fx_CanMerge.transform.GetChild(0).gameObject.SetActive(true);
    }

    float GetDistanceInCeil(int lv)
    {
        if (lv == 1) return 0.35f;
        else return 0.4f;
    }

    public void Clear()
    {
        transform.DespawnAllChild(fx_CanMerge);
        _mainPokemon.Clear();
    }
    #endregion

    #region merge

    private Endgame_3_Ceil _ceilTrigger;
    Vector3 _rootPos;

    public Endgame_3_Ceil Press()
    {
        _rootPos = transform.position;
        isHold = true;
        Endgame3_CeilManager.ins.isSelected = true;
        Endgame3_CeilManager.ins.ReloadCheckMerge(this);
        return this;
    }

    public void Drop()
    {
        isMove = false;
        isHold = false;

        Endgame3_CeilManager.ins.isSelected = false;

        //Nếu va chạm với 1 ô nào đó
        if (_ceilTrigger != null)
        {
            //Kiểm tra xem nếu merge được với ô đó
            var check = CheckMerge();
            if(check == -2)
            {

                if (isTmp)
                {
                    ceilLayout.SetActive(false);
                    Destroy(gameObject);
                }
                else
                {
                    //xóa ô đã merge khỏi bảng dữ liệu
                    info.ClearData();
                    Clear();
                    BackToOrigin();
                }

                GameConfig.ins.SpawnFx(GameConfig.ins.fx_Smoke, _ceilTrigger.transform.position);
            }
            else if (check == 1)
            {
                Merge();
                CanvasFight.ins.Refresh();

                if (Tutorial.ins != null && Tutorial.ins.step == 3)
                {
                    Tutorial.ins.step = 4;
                }

                if (Tutorial.ins != null && Tutorial.ins.step == 2)
                {
                    Tutorial.ins.step = 3;
                }
            }
            //Nếu không -> trở về vị trí cũ
            else
            {
                if(check == -1)
                {
                    SwitchMerge();
                    //BackToOrigin();
                }
                else
                {
                    SwitchMerge();
                    if (Tutorial.ins != null && Tutorial.ins.step == 3)
                    {
                        Tutorial.ins.step = 4;
                    }
                }
                //BackToOrigin();
            }
        }
        else
        {
            BackToOrigin();
        }
        Endgame3_CeilManager.ins.ReloadCheckMerge();
    }

    int CheckMerge()
    {
        if (_ceilTrigger == null)
        {
            return -1;
        }
        else
        {
            if(_ceilTrigger.isTrash)
            {
                return -2;
            }
            //Không thể kéo chính => tạm
            if (!isTmp && _ceilTrigger.isTmp) return -1;

            //tạm => chính || chính => chính || tạm => tạm
            else
            {
                if (info.CheckCanMerge(_ceilTrigger.info)) return 1;
                if (isTmp && !_ceilTrigger.isTmp) return -1;
                return 0;
            }
        }
    }

    void SwitchMerge()
    {
        //Doi data
        //var tmp = GameHelper.DeepClone(_ceilTrigger.info);
        //_ceilTrigger.info.CloneData(info);
        //info.CloneData(tmp);

        //Sinh lai của _ceilTrigger
        var lv = _ceilTrigger.info.GetLevelPokemon();
        var objPokemon = GameConfig.ins.GetPokemon(_ceilTrigger.info.type, lv);
        var count = _ceilTrigger.info.GetNumPokemon();

        if (count == 1)
        {
            _ceilTrigger.SpawnPokemonInCeil_1(objPokemon, _ceilTrigger.info, _ceilTrigger.isTmp);
        }
        else if (count == 2)
        {
            _ceilTrigger.SpawnPokemonInCeil_2(objPokemon, _ceilTrigger.info, _ceilTrigger.isTmp);
        }

        //Sinh lai của mình
        lv = info.GetLevelPokemon();
        objPokemon = GameConfig.ins.GetPokemon(info.type, lv);
        count = info.GetNumPokemon();

        if (count == 1)
        {
            SpawnPokemonInCeil_1(objPokemon, info, isTmp);
        }
        else if (count == 2)
        {
            SpawnPokemonInCeil_2(objPokemon, info, isTmp);
        }


        BackToOrigin();
    }

    void Merge()
    {
        if (_ceilTrigger.info.levelUpdate != 0) _ceilTrigger.info.levelUpdate++;
        else _ceilTrigger.info.CloneData(info);

        var lv = _ceilTrigger.info.GetLevelPokemon();
        var objPokemon = GameConfig.ins.GetPokemon(_ceilTrigger.info.type, lv);
        var count = _ceilTrigger.info.GetNumPokemon();

        if (count == 1)
        {
            _ceilTrigger.SpawnPokemonInCeil_1(objPokemon, _ceilTrigger.info, _ceilTrigger.isTmp);
        }
        else if (count == 2)
        {
            _ceilTrigger.SpawnPokemonInCeil_2(objPokemon, _ceilTrigger.info, _ceilTrigger.isTmp);
        }

        if (isTmp)
        {
            ceilLayout.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            //xóa ô đã merge khỏi bảng dữ liệu
            info.ClearData();

            Clear();
            BackToOrigin();
        }

        if (CheckMergeTmpDone() && !isFisrtDone)
        {
            isFisrtDone = true;

            if (CanvasFight.ins.stage == 1)
            {
                PlayerController.ins.MoveToEndMerge();
                CanvasManager.ins.canvasFight.OnShow_1_2();
            }
        }

        Endgame3_CeilManager.ins.ReloadCheckMerge();
        GameConfig.ins.SpawnFx(GameConfig.ins.fx_MergeLevelUp, _ceilTrigger.transform.position);
    }

    public void SpawnMergeFx()
    {
        GameConfig.ins.SpawnFx(GameConfig.ins.fx_MergeLevelUp, transform.position);
    }

    //Merge vào ô trống (khi player đi đến đích)
    public void Merge(PokemonInfo obj)
    {
        var data = info;
        if (data != null)
        {
            data.levelUpdate += 1;
        }

        data.type = obj.type;
        var lv = data.GetLevelPokemon();
        var objPokemon = GameConfig.ins.GetPokemon(obj.type, lv);
        var count = data.GetNumPokemon();

        if (count == 1)
        {
            SpawnPokemonInCeil_1(objPokemon, info);
        }
        else if (count == 2)
        {
            SpawnPokemonInCeil_2(objPokemon, info);
        }
    }

    void BackToOrigin()
    {
        transform.position = _rootPos;
    }

    private bool isMove;
    private Vector3 _target;
    public void MoveTo(Vector3 dir)
    {
        isMove = true;
        _target = dir;
    }

    void Update()
    {
        if (isMove)
        {
            transform.position = Vector3.LerpUnclamped(transform.position, _target + Vector3.up * 0.5f, 5f * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isHold) return;
        if (other.tag.Equals("Element"))
        {
            var t = other.GetComponent<Endgame_3_Ceil>();
            if (t != null)
            {
                _ceilTrigger = t;
                SetFx(true);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!isHold) return;
        if (other.tag.Equals("Element"))
        {
            var t = other.GetComponent<Endgame_3_Ceil>();
            if (t == _ceilTrigger)
            {
                _ceilTrigger = null;
                SetFx(false);
            }
        }
    }

    void SetFx(bool b)
    {

    }

    bool isFisrtDone;
    public bool CheckMergeTmpDone()
    {
        foreach (var t in Endgame3_CeilManager.ins.sprCeilsTmp)
        {
            if (t.gameObject.activeInHierarchy) return false;
        }
        return true;
    }
    #endregion

    public void BlockInput(bool b)
    {
        GetComponent<Collider>().enabled = !b;
    }
}
