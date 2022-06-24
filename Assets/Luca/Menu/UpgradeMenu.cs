using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UpgradeMenu : NetworkBehaviour
{
    public List<GameObject> screenList = new List<GameObject>();
    public List<GameObject> screenListInRed = new List<GameObject>();

    private int intUpgrade = 4;

    [Header("Forteresse")] [SerializeField]
    private List<Image> upgradesFVisu = new List<Image>(); //upgrades forteresse mais les visus

    [SerializeField]
    private List<GameObject> upgradesFButton = new List<GameObject>(); //upgrades forteresse mais les buttons +

    [SerializeField] private List<TextMeshProUGUI> listPriceF = new List<TextMeshProUGUI>();
    private List<int> upgradesF = new List<int>(); //upgrades forteresse en int

    [Header("Camion")] [SerializeField]
    private List<Image> upgradesCVisu = new List<Image>(); //upgrades camion mais les visus

    [SerializeField]
    private List<GameObject> upgradesCButton = new List<GameObject>(); //upgrades camion mais les buttons +

    [SerializeField] private List<TextMeshProUGUI> listPriceC = new List<TextMeshProUGUI>();
    [HideInInspector] public List<int> upgradesC = new List<int>(); //upgrades camion en int

    [Header("Weapons1")] 
    public List<WTreeButton> listAllButton1 = new List<WTreeButton>();
    [HideInInspector] public WTreeButton lastUpgrade1;
    [SerializeField] GameObject turret1;

    [Header("Weapons2")]
    public List<WTreeButton> listAllButton2 = new List<WTreeButton>();
    [HideInInspector] public WTreeButton lastUpgrade2;
    [SerializeField] GameObject turret2;

    
    [Header("Weapon Both")]
    public List<WScriptable> allWeapon = new List<WScriptable>();
    
    [Header("Price Upgrade --- GD")] public List<listInt> FPrice = new List<listInt>();
    public List<listInt> CPrice = new List<listInt>();


    private bool sellMode;

    [Header("Repair")] [SerializeField] protected HPTruck truck;
    [SerializeField] protected TextMeshProUGUI fullText;
    [SerializeField] protected TextMeshProUGUI partialText;
    [SerializeField] protected float fullRepairPrice;
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected TextMeshProUGUI hpText;

    [SerializeField] private TruckFuel fuel;

    #region Singleton

    private static UpgradeMenu instance;

    public static UpgradeMenu Instance
    {
        get => instance;
        set => instance = value;
    }

    #endregion

    [Networked(OnChanged = nameof(UpgradesForteresseServerOnChanged)), Capacity(3)] private NetworkArray<int> upgradesForteresseServer => default;
    [Networked(OnChanged = nameof(UpgradesCamionServerOnChanged)), Capacity(3)] private NetworkArray<int> upgradesCamionServer => default;
    [Networked(OnChanged = nameof(UnlockWeapon1ServerOnChanged)), Capacity(5)] private NetworkLinkedList<int> unlockedWeapon1Server => default;
    [Networked(OnChanged = nameof(UnlockWeapon2ServerOnChanged)), Capacity(5)] private NetworkLinkedList<int> unlockedWeapon2Server => default;
    
    private int sizeWeapon1;
    private int sizeWeapon2;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;


        for (int i = 0; i < 3; i++)
        {
            upgradesF.Add(0);
            upgradesC.Add(0);
        }
        
    }


    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            for (int i = 0; i < 3; i++)
            {
                upgradesForteresseServer.Set(i, 0);
                upgradesCamionServer.Set(i,0);
            }
            
            unlockedWeapon1Server.Add(0);
            unlockedWeapon2Server.Add(0);
        }

        #region pour tout set et que ca bug pas

        gotoScreen(1);
        visuF();
        gotoScreen(2);
        visuC();
        gotoScreen(3);
        upgradeWeapon(listAllButton1[0], listAllButton1[0].firstWeapon);
        gotoScreen(4);
        upgradeWeapon(listAllButton2[0], listAllButton2[0].firstWeapon);
        gotoScreen(0);
        gameObject.SetActive(false);

        #endregion

    }

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            upgradesForteresseServer.OnListChanged += UpgradesForteresseServerOnChanged;
            upgradesCamionServer.OnListChanged += UpgradesCamionServerOnChanged;
            unlockedWeapon1Server.OnListChanged += UnlockWeapon1ServerOnChanged;
            unlockedWeapon2Server.OnListChanged += UnlockWeapon2ServerOnChanged;
        }
    }*/

    public void gotoScreen(int lint)
    {
        for (int i = 0; i < screenList.Count; i++)
        {
            
            if (i == lint)
            {
                if (i != 0 && i != 1) screenListInRed[i].SetActive(true); 
                screenList[i].SetActive(true);
            }
            else
            {
                if (i != 0 && i != 1) screenListInRed[i].SetActive(false); 
                screenList[i].SetActive(false);
            }
            
            if(i == 5) forRepair(); 
        }

        if (sellMode)
        {
            sellMode = false;
            upgradeWeapon(lastUpgrade1, true);
            upgradeWeapon(lastUpgrade2, false);
        }
        
    }

    public void upgradeForteresse(int lint)
    {
        
        if (FPrice[lint].intList[upgradesF[lint]] <= ScrapMetal.Instance.scrapLeft)
        {
            ScrapMetal.Instance.addMoneyServerRpc(-FPrice[lint].intList[upgradesF[lint]]);

            UpgradeForteresseServerRpc(lint);
        }
    }

    public void upgradeCamion(int lint)
    {
        if (CPrice[lint].intList[upgradesC[lint]] <= ScrapMetal.Instance.scrapLeft)
        {
            ScrapMetal.Instance.addMoneyServerRpc(-CPrice[lint].intList[upgradesC[lint]]);

            UpgradeCamionServerRpc(lint);
        }
    }


    void upgradeWeapon(WTreeButton buttonTree, bool firstWeapon)
    {
        if (firstWeapon)
        {
            disableAllWeapon1();
      
            var currentButtonTree = buttonTree;
            lastUpgrade1 = currentButtonTree;
            bool finished = false;

            if (!sellMode)
            {
                while (!finished)
                {
                    currentButtonTree.buyed();
                    if (currentButtonTree.previousUpgrades != null)
                        currentButtonTree = currentButtonTree.previousUpgrades;
                    else finished = true;
                }

                for (int i = 0; i < listAllButton1.Count; i++)
                {
                    if (listAllButton1[i].previousUpgrades != null)
                    {
                        if (listAllButton1[i].previousUpgrades == buttonTree) listAllButton1[i].turnOn();
                        else if (listAllButton1[i].previousUpgrades.canBeUpgrades) listAllButton1[i].notSelectedYet();
                    }
                }
            }
            else
            {
                if (currentButtonTree.previousUpgrades != null)
                {
                    //currentButtonTree = currentButtonTree.previousUpgrades;
                    currentButtonTree.sellable();
                    lastUpgrade1 = currentButtonTree;
                    if (currentButtonTree.previousUpgrades != null) currentButtonTree = currentButtonTree.previousUpgrades;
                    else finished = true;
                    
                }
                else finished = true;
                
                
                while (!finished)
                {
                    currentButtonTree.notSelectedYet();
                    if (currentButtonTree.previousUpgrades != null) currentButtonTree = currentButtonTree.previousUpgrades;
                    else finished = true;
                }
            }

            upgradeWeaponTurret(turret1, lastUpgrade1.id);
        }
        else
        {
            disableAllWeapon2();
      
            var currentButtonTree = buttonTree;
            lastUpgrade2 = currentButtonTree;
            bool finished = false;

            if (!sellMode)
            {
                while (!finished)
                {
                    currentButtonTree.buyed();
                    if (currentButtonTree.previousUpgrades != null)
                        currentButtonTree = currentButtonTree.previousUpgrades;
                    else finished = true;
                }

                for (int i = 0; i < listAllButton2.Count; i++)
                {
                    if (listAllButton2[i].previousUpgrades != null)
                    {
                        if (listAllButton2[i].previousUpgrades == buttonTree) listAllButton2[i].turnOn();
                        else if (listAllButton2[i].previousUpgrades.canBeUpgrades) listAllButton2[i].notSelectedYet();
                    }
                }
            }
            else
            {
                if (currentButtonTree.previousUpgrades != null)
                {
                    //currentButtonTree = currentButtonTree.previousUpgrades;
                    currentButtonTree.sellable();
                    lastUpgrade2 = currentButtonTree;
                    if (currentButtonTree.previousUpgrades != null) currentButtonTree = currentButtonTree.previousUpgrades;
                    else finished = true;
                    
                }
                else finished = true;
                
                
                while (!finished)
                {
                    currentButtonTree.notSelectedYet();
                    if (currentButtonTree.previousUpgrades != null) currentButtonTree = currentButtonTree.previousUpgrades;
                    else finished = true;
                }
            }

            upgradeWeaponTurret(turret2, lastUpgrade2.id);
        }
    }

    private void disableAllWeapon1()
    {
        for (int i = 0; i < listAllButton1.Count; i++)
        {
            listAllButton1[i].unselectable();
        }
    }

    private void disableAllWeapon2()
    {
        for (int i = 0; i < listAllButton2.Count; i++)
        {
            listAllButton2[i].unselectable();
        }
    }


    /*private void disablgrayAllWeapon1()
    {
       for (int i = 0; i < UpgradeW1Button.Count; i++)
       {
          for (int j = 0; j < UpgradeW1Button[i].buttons.Count; j++)
          {
             UpgradeW1Button[i].buttons[j].notSelectedYet();
          }
       }
    }*/

    /*private void updateVisu()
    {
       visuF();
       visuC();
    }*/


    private void visuF()
    {
        var multi = 1;
        for (int i = 0; i < upgradesF.Count; i++)
        {
            var step = 0;
            for (int j = intUpgrade * (multi - 1); j < intUpgrade * multi && step < intUpgrade; j++)
            {
                var j2 = j;
                if (j >= intUpgrade) j2 -= intUpgrade * (multi - 1);

                if (upgradesF[i] > j2 && step == 0) upgradesFVisu[j].color = new Color(1, 1, 1, 0);
                else if (upgradesF[i] > j2 && step != 0) upgradesFVisu[j].color = Color.red;

                step++;
            }

            if (upgradesF[i] == intUpgrade) upgradesFButton[i].SetActive(false);
            multi++;
        }


        for (int i = 0; i < listPriceF.Count; i++)
        {
            if (listPriceF[i].IsActive()) listPriceF[i].text = FPrice[i].intList[upgradesF[i]].ToString();
        }
    }

    private void visuC()
    {

        fuel.changeMaxFuel();

        var multi = 1;
        for (int i = 0; i < upgradesC.Count; i++)
        {
            var step = 0;
            for (int j = intUpgrade * (multi - 1); j < intUpgrade * multi && step < intUpgrade; j++)
            {
                var j2 = j;
                if (j >= intUpgrade) j2 -= intUpgrade * (multi - 1);

                if (upgradesC[i] > j2 && step == 0) upgradesCVisu[j].color = new Color(1, 1, 1, 0);
                else if (upgradesC[i] > j2 && step != 0) upgradesCVisu[j].color = Color.red;

                step++;
            }

            if (upgradesC[i] == intUpgrade) upgradesCButton[i].SetActive(false);
            multi++;
        }

        for (int j = 0; j < listPriceF.Count; j++)
        {
            if (listPriceC[j].IsActive()) listPriceC[j].text = CPrice[j].intList[upgradesC[j]].ToString();
        }
    }
    

    void upgradeWeaponTurret(GameObject turret, int id)
    {
       
        turret.GetComponent<WeaponUltima>().actuAllStats(allWeapon[id]);
        
        // turret.GetComponent<TestControlWeapon>().actuGauge();
    }

    
    /*public void quitUpgrade()
    {
        gotoScreen(0);
        Shop.Instance.quitShop();
    }*/

    public void changeSellMode(bool firstweapon)
    {
        sellMode = !sellMode;
        
        if (firstweapon)
        {
            WTreeButton prov = lastUpgrade1;
            disableAllWeapon1();
            
            if (sellMode)
            {
                var finished = false;
                lastUpgrade1.sellable();
                if (prov.previousUpgrades != null) prov = prov.previousUpgrades;
                else finished = true;

                while (!finished)
                {
                    prov.notSelectedYet();
                    if (prov.previousUpgrades != null) prov = prov.previousUpgrades;
                    else finished = true;
                }
            }else upgradeWeapon(lastUpgrade1, true);
        }
        else
        {
            WTreeButton prov = lastUpgrade2;
            disableAllWeapon2();
            
            if (sellMode)
            {
                var finished = false;
                lastUpgrade2.sellable();
                if (prov.previousUpgrades != null) prov = prov.previousUpgrades;
                else finished = true;

                while (!finished)
                {
                    prov.notSelectedYet();
                    if (prov.previousUpgrades != null) prov = prov.previousUpgrades;
                    else finished = true;
                }
            }else upgradeWeapon(lastUpgrade2, false);
        }
        
    }

    public void forRepair()
    {
        hpSlider.value = truck.currenthealth / truck.maxhealth;
        hpText.text = truck.currenthealth + " / " + truck.maxhealth;

        float prePriceFull = (truck.currenthealth / truck.maxhealth);
        float priceFull = fullRepairPrice - (fullRepairPrice * prePriceFull);
        float pricePartial = fullRepairPrice / 10;
        
        
        fullText.text = "Full repair : " + ((int)priceFull).ToString() + " metals";
        
        if(priceFull > pricePartial) partialText.text = "Partial repair : " + ((int)pricePartial).ToString() + " metals";
        else partialText.text = "Partial repair : " + ((int)priceFull).ToString() + " metals";
    }
    
    public void healTruck(bool fullHeal)
    {
        int price1 = (int)fullRepairPrice / 10;
        int price2 = (int)(fullRepairPrice - (fullRepairPrice * (truck.currenthealth / truck.maxhealth)));

        if (fullHeal) if(price2 <= ScrapMetal.Instance.scrapLeft) ScrapMetal.Instance.addMoneyServerRpc(-price2);
            
        else
        {
            var cheap = Mathf.Min(price1, price2);
            if(cheap <= ScrapMetal.Instance.scrapLeft) ScrapMetal.Instance.addMoneyServerRpc(-cheap);

        }
        
        truck.heal(fullHeal);
        forRepair();
    }
    

    [System.Serializable]
    public class listInt
    {
        public string newName;
        public List<int> intList;
    }


    
    
    #region Online

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void UpgradeForteresseServerRpc(int upgradeIndex)
    {
        upgradesForteresseServer.Set(upgradeIndex, upgradesForteresseServer[upgradeIndex]+1);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void UpgradeCamionServerRpc(int upgradeIndex)
    {
        upgradesCamionServer.Set(upgradeIndex, upgradesCamionServer[upgradeIndex]+1);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void UnlockWeapon1ServerRpc(int weaponIdToUnlock, RpcInfo info = default)
    {
        unlockedWeapon1Server.Add(weaponIdToUnlock);
        
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void SellWeapon1ServerRpc(int weaponIdToUnlock, RpcInfo info = default)
    {
        unlockedWeapon1Server.Remove(weaponIdToUnlock);
    }


    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void UnlockWeapon2ServerRpc(int weaponIdToUnlock)
    {
        unlockedWeapon2Server.Add(weaponIdToUnlock);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void SellWeapon2ServerRpc(int weaponIdToUnlock)
    {
        unlockedWeapon2Server.Remove(weaponIdToUnlock);
    }

    public static void UpgradesForteresseServerOnChanged(Changed<UpgradeMenu> changed)
    {
        changed.Behaviour.ForteresseChange();
    }

    private void ForteresseChange()
    {
        upgradesF = upgradesForteresseServer.ToList();
        visuF();
    }

    public static void UpgradesCamionServerOnChanged(Changed<UpgradeMenu> changed)
    {
        changed.Behaviour.CamionChange();
    }
    
    private void CamionChange()
    {
        upgradesC = upgradesCamionServer.ToList();
        visuC();
    }
    

    public static void UnlockWeapon1ServerOnChanged(Changed<UpgradeMenu> changed)
    {
        changed.Behaviour.weapon1Change();
    }

    private void weapon1Change()
    {
        
        WTreeButton weaponBuyed = listAllButton1[unlockedWeapon1Server.Last()];
        
        sellMode = unlockedWeapon1Server.Count < sizeWeapon1;
        upgradeWeapon(weaponBuyed, weaponBuyed.firstWeapon);
        sizeWeapon1 = unlockedWeapon1Server.Count;
    }
    
    
    public static void UnlockWeapon2ServerOnChanged(Changed<UpgradeMenu> changed)
    {
        changed.Behaviour.weapon2Change();
    }
    
    private void weapon2Change()
    {
        
        WTreeButton weaponBuyed = listAllButton2[unlockedWeapon2Server.Last()];
        
        sellMode = unlockedWeapon2Server.Count < sizeWeapon2;
        upgradeWeapon(weaponBuyed, weaponBuyed.firstWeapon);
        sizeWeapon2 = unlockedWeapon2Server.Count;
    }
    
    #endregion
}

