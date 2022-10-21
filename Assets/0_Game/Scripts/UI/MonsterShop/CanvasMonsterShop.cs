using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class CanvasMonsterShop : MonoBehaviour
{
    public Animator anim;
    public Transform content;
    public MonsterShopItem itemPrefab;
    private List<MonsterShopItem> itemList = new List<MonsterShopItem>();

    private void Start()
    {
        Init();
    }

    public void OnClose() {
        anim.Play("Hide");
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        StartCoroutine(ie_Close());
    }

    private IEnumerator ie_Close() {
        yield return new WaitForSeconds(0.4f);
        gameObject.SetActive(false);
    }

    public void OnOpen()
    {
        gameObject.SetActive(true);
        UpdateState();
    }

    public void Init()
    {
        for (int i = 0; i < GameConfig.ins.PokemonList.Count; i++)
        {
            var item = SimplePool.Spawn(itemPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<MonsterShopItem>();
            item.transform.SetParent(content);
            item.transform.localScale = Vector3.one;
            itemList.Add(item);
            item.Setup(GameConfig.ins.PokemonList[i].type);
            //item.transform.SetAsLastSibling();
        }

        UpdateState();

    }

    private void UpdateState() {
        foreach (MonsterShopItem item in itemList) {
            item.SetupState(item.type);
        }   
    }
}
