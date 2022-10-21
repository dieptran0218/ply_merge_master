using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PrefabRegister : SingletonMonoBehaviour<PrefabRegister>
{
    public string linkPrefabs = "";
    public string links = "Assets/Game";
    public List<ElementInfo> prefabs = new List<ElementInfo>();

    Dictionary<string, GameObject> _dicPrefabs = new Dictionary<string, GameObject>();

    private void OnEnable()
    {
        _dicPrefabs.Clear();
        for(var i = 0; i < prefabs.Count; i++)
        {
            _dicPrefabs.Add(prefabs[i].id, prefabs[i].prefab);
        }
    }

    public GameObject GetPrefab(string id)
    {
        try
        {
            return _dicPrefabs[id];
            //return prefabs.Find(x => x.id.Equals(id)).prefab;
        }
        catch (Exception e)
        {
            Debug.Log(id);
            return null;
        }
    }

    #region delete
    //[ContextMenu("Refresh Enum")]
    //public void CreateEnum()
    //{
    //    List<GameObject> objs = GetAllPrefabObj(linkPrefabs);

    //    //Create Enum
    //    #region Enum Create
    //    string content = "";
        
    //    for (var i = 0; i < objs.Count; i++)
    //    {
    //        if (i < objs.Count - 1) content += objs[i].name + ",\n";
    //        else content += objs[i].name + "\n";
    //    }
    //    string dataClass = "using System;using UnityEngine;\n[Serializable]\npublic class Prefabs\n{" + content + "\n}";
    //    //string dataClass = "[System.Serializable]\npublic enum ElementRegister {\n" + content + "\n}";
    //    string path = Application.dataPath;
    //    string fileName = path + "/_PrefabData/MapEditor/ElementRegister.cs";
    //    Debug.Log(dataClass);
    //    if (File.Exists(fileName))
    //    {
    //        File.WriteAllText(fileName, dataClass);
    //    }
    //    else
    //    {
    //        StreamWriter sr;
    //        sr = File.CreateText(fileName);
    //        sr.WriteLine(dataClass);
    //        sr.Close();
    //        AssetDatabase.ImportAsset(fileName);
    //        AssetDatabase.Refresh();
    //    }
    //    #endregion
    //}
    #endregion

#if UNITY_EDITOR
    [ContextMenu("Load All Asset")]
    public void Refresh()
    {
        prefabs.Clear();
        List<GameObject> objs = GetAllPrefabObj(linkPrefabs);
        foreach (var o in objs)
        {
            prefabs.Add(new ElementInfo());
            prefabs[prefabs.Count - 1].prefab = o;
            prefabs[prefabs.Count - 1].id = o.name;
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        PrefabUtility.ApplyPrefabInstance(gameObject, InteractionMode.AutomatedAction);
    }

    public List<GameObject> GetAllPrefabObj(string path)
    {
        var list = AssetDatabase.FindAssets("t:prefab").ToList().Select(AssetDatabase.GUIDToAssetPath).ToList();
        list = list.FindAll(s => s.StartsWith(path));
        var listObj = new List<GameObject>();
        foreach (var assetPath in list)
        {
            listObj.Add(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath));
        }
        return listObj;

    }
#endif
}


[System.Serializable]
public class ElementInfo
{
    public string id;
    public GameObject prefab;
}