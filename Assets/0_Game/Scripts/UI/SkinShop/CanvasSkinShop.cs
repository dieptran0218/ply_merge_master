using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class CanvasSkinShop : SingletonMonoBehaviour<CanvasSkinShop>
{
    //public ScriptableSkin skinDataConfig;
    public GameObject itemPrefab;
    public RectTransform contentHolder;
    public List<RectTransform> contentHolderList = new List<RectTransform>();
    public RectTransform transContent;
    public Page_Dot_UI pageDotUI;
    public PageSwiper pageSwiper;
    public List<SkinItem> itemList;
    public GameObject adsBtn;
    public ShopTab curTab;
    public List<GameObject> onTabUI;

    public Transform modelHolder;

    public Text rewardTxt;
    public Text gemCollected;

    public Animator anim;

    private int sumReward;
    private bool isInit;
    private GameObject currentModel;

    private void OnEnable()
    {
        transContent.anchoredPosition = new Vector2(100f, 0);
        gemCollected.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
    }

    public void Init()
    {
        isInit = true;
        curTab = ShopTab.Character;
        int pageCount = Mathf.CeilToInt(GameConfig.ins.listCharacter.skinCharData.Count / 8f);
        SetupChilContent(pageCount);
        for (int i = 0; i < GameConfig.ins.listCharacter.skinCharData.Count; i++)
        {
            var item = Instantiate(itemPrefab);
            int currentPage = Mathf.FloorToInt(i / 8f);
            item.transform.SetParent(contentHolderList[currentPage]);
            item.transform.localScale = Vector3.one;
            SkinItem itemSkin = item.GetComponent<SkinItem>();
            itemSkin.Setup(GameConfig.ins.listCharacter.skinCharData[i].skinCharID);
            itemList.Add(itemSkin);
        }

        pageDotUI.Init();
        pageSwiper.Init();
    }

    public void ReloadUI()
    {
        foreach (var i in itemList)
        {
            if (curTab == ShopTab.Character)
            {
                i.SetupCharUI();
            }
            else if (curTab == ShopTab.Skin)
            {
                i.SetupSkinUI();
            }
            else if (curTab == ShopTab.Dance)
            {
                i.SetupDanceUI();
            }
        }
        gemCollected.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
    }

    void SetupChilContent(int pageCount)
    {
        for (int i = 0; i < pageCount; i++)
        {
            RectTransform contentChild = Instantiate(contentHolder);
            contentHolderList.Add(contentChild);
            contentChild.parent = transContent.transform;
            contentChild.transform.localScale = Vector3.one;
        }
    }

    public void OnClose()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        CameraController.ins.EndCameraShop();
        PlayerController.ins.stage = PlayerStage.Combat;
        PlayerController.ins.ReloadAnimation();
        PlayerController.ins.LoadCharacter();
        ScreenSwitch.ins.Show(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void OnOpen()
    {
        CanvasManager.ins.canvasHome.OnClose();
        gameObject.SetActive(true);
        PlayerController.ins.stage = PlayerStage.IdleShop;
        PlayerController.ins.ReloadAnimation();
        CountReward();
        ChangePlayerModel(GameManager.ins.data.charUsed);
        //ReloadUI();
        if (!isInit)
        {
            Init();
        }
        else
        {
            ReloadUI();
        }
    }

    private void CountReward()
    {
        List<WallUpgradeData> priceList = GameConfig.ins.wall.wallUpgradeData;

        int fakeMoney = GameManager.ins.data.gemCollected;
        int fakeWallLevel = GameManager.ins.data.wallLevel;
        while (fakeMoney > 0)
        {
            if (fakeWallLevel < priceList.Count - 1)
            {
                fakeMoney -= priceList[fakeWallLevel].price;
                fakeWallLevel++;
            }
            else
            {
                fakeMoney -= priceList[GameConfig.ins.wall.wallUpgradeData.Count - 1].price + 50 * (fakeWallLevel - GameConfig.ins.wall.wallUpgradeData.Count);
                fakeWallLevel++;
            }
        }

        if (fakeWallLevel > GameManager.ins.data.wallUpgradeLevel)
        {
            GameManager.ins.data.wallUpgradeLevel = fakeWallLevel;
        }
        if (GameManager.ins.data.wallUpgradeLevel > priceList.Count - 3)
        {
            sumReward = (priceList[GameConfig.ins.wall.wallUpgradeData.Count - 1].price + 50 * (GameManager.ins.data.wallUpgradeLevel - GameConfig.ins.wall.wallUpgradeData.Count + 3)) * 3 + 100;
        }
        else
        {
            sumReward = priceList[GameManager.ins.data.wallUpgradeLevel].price + priceList[GameManager.ins.data.wallUpgradeLevel + 1].price + priceList[GameManager.ins.data.wallUpgradeLevel + 2].price;
        }
        rewardTxt.text = "x" + sumReward;
    }

    public void OnAdsClick()
    {
        SoundController.ins.UI_Click();
        VibrationsManager.instance.TriggerLightImpact();
        //AdsManager.Ins.ShowRewardedAd("shop_offer_gem", AdsDone);
    }

    public void OnTabBtn(int tabIndex)
    {
        Debug.Log(tabIndex);
        ShopTab tab = (ShopTab)tabIndex;
        if (tab == curTab) return;
        curTab = tab;
        foreach (GameObject item in onTabUI)
        {
            item.SetActive(false);
        }
        onTabUI[tabIndex].SetActive(true);
        foreach (RectTransform t in contentHolderList)
        {
            GameObject.Destroy(t.gameObject);
        }
        contentHolderList.Clear();
        itemList.Clear();
        if (curTab == ShopTab.Character)
        {
            isInit = true;
            int pageCount = Mathf.CeilToInt(GameConfig.ins.listCharacter.skinCharData.Count / 8f);
            SetupChilContent(pageCount);
            for (int i = 0; i < GameConfig.ins.listCharacter.skinCharData.Count; i++)
            {
                var item = Instantiate(itemPrefab);
                int currentPage = Mathf.FloorToInt(i / 8f);
                item.transform.SetParent(contentHolderList[currentPage]);
                item.transform.localScale = Vector3.one;
                SkinItem itemSkin = item.GetComponent<SkinItem>();
                itemSkin.Setup(GameConfig.ins.listCharacter.skinCharData[i].skinCharID);
                itemList.Add(itemSkin);
            }
        }
        else if (curTab == ShopTab.Skin)
        {
            isInit = true;
            int pageCount = Mathf.CeilToInt(GameConfig.ins.listAllSkin.skinData.Count / 8f);
            SetupChilContent(pageCount);
            for (int i = 0; i < GameConfig.ins.listAllSkin.skinData.Count; i++)
            {
                var item = Instantiate(itemPrefab);
                int currentPage = Mathf.FloorToInt(i / 8f);
                item.transform.SetParent(contentHolderList[currentPage]);
                item.transform.localScale = Vector3.one;
                SkinItem itemSkin = item.GetComponent<SkinItem>();
                itemSkin.Setup(GameConfig.ins.listAllSkin.skinData[i].skinID);
                itemList.Add(itemSkin);
            }
        }
        else if (curTab == ShopTab.Dance)
        {
            isInit = true;
            int pageCount = Mathf.CeilToInt(GameConfig.ins.listDance.danceData.Count / 8f);
            SetupChilContent(pageCount);
            for (int i = 0; i < GameConfig.ins.listDance.danceData.Count; i++)
            {
                var item = Instantiate(itemPrefab);
                int currentPage = Mathf.FloorToInt(i / 8f);
                item.transform.SetParent(contentHolderList[currentPage]);
                item.transform.localScale = Vector3.one;
                SkinItem itemSkin = item.GetComponent<SkinItem>();
                itemSkin.Setup(GameConfig.ins.listDance.danceData[i].danceID);
                itemList.Add(itemSkin);
            }
        }
        pageDotUI.Init();
        pageSwiper.Init();
    }

    public void AdsDone()
    {
        EfxManager.ins.GetGemFx(adsBtn.transform.position, gemCollected.transform.position);
        GameManager.ins.data.wallUpgradeLevel += 3;
        StartCoroutine(ie_GetCoin());
        //ReloadUI();
        CountReward();
    }

    public void TurnOffAllBg()
    {
        foreach (SkinItem i in itemList)
        {
            i.chooseBg.SetActive(false);
        }
    }

    public void ChangePlayerModel(CharacterType id)
    {
        modelHolder.DespawnAllChild();
        var o = GameConfig.ins.listCharacter.skinCharData.Find(x => x.skinCharID == id);
        if(o != null)
        {
            var p = SimplePool.Spawn(o.charModel, modelHolder.position, Quaternion.identity).transform;
            p.SetParent(modelHolder);
            p.localPosition = Vector3.zero;
            p.localRotation = Quaternion.Euler(0, 0, 0);
            p.localScale = Vector3.one;
            p.GetComponent<Animator>().Play("Dance_2");
        }
    }

    private IEnumerator ie_CloseShop()
    {
        yield return Yielders.Get(1f);
        gameObject.SetActive(false);
    }

    private IEnumerator ie_GetCoin()
    {
        int tempCoin = GameManager.ins.data.gemCollected;
        GameManager.ins.data.AddGem(sumReward);
        //FirebaseManager.Ins.earn_virtual_currency("Gem", sumReward, "ads_shop_offer");
        yield return Yielders.Get(1f);
        for (int i = 0; i < 50; i++)
        {
            gemCollected.text = GameHelper.ConvertNumber(tempCoin + i * (int) (sumReward / 50));
            yield return Yielders.Get(0.01f);
        }
        gemCollected.text = GameHelper.ConvertNumber(GameManager.ins.data.gemCollected);
    }

    [System.Serializable]
    public enum ShopTab
    {
        Character,
        Skin,
        Dance
    }
}
