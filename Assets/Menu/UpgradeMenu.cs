using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : NetworkBehaviour
{
    public List<GameObject> screenList = new List<GameObject>();

    private int intUpgrade = 4;

    [Header("Forteresse")] [SerializeField]
    private List<Image> upgradesFVisu = new List<Image>(); //upgrades forteresse mais les visus

    [SerializeField]
    private List<GameObject> upgradesFButton = new List<GameObject>(); //upgrades forteresse mais les buttons +

    [SerializeField] private List<TextMeshProUGUI> listPriceF = new List<TextMeshProUGUI>();
    private List<int> upgradesF = new List<int>(); //upgrades forteresse en int

    [Header("Camion")] [SerializeField]
    private List<Image> upgradesCVisu = new List<Image>(); //upgrades forteresse mais les visus

    [SerializeField]
    private List<GameObject> upgradesCButton = new List<GameObject>(); //upgrades forteresse mais les buttons +

    [SerializeField] private List<TextMeshProUGUI> listPriceC = new List<TextMeshProUGUI>();
    private List<int> upgradesC = new List<int>(); //upgrades forteresse en int

    [Header("Weapons1")] 
    public List<WTreeButton> listAllButton1 = new List<WTreeButton>();
    private int upgardesLevel = 1;
    [HideInInspector] public WTreeButton lastUpgrade1;

    [Header("Weapons2")]
    public List<WTreeButton> listAllButton2 = new List<WTreeButton>();
    private int upgardesLevel2 = 1;
    [HideInInspector] public WTreeButton lastUpgrade2;


    [Header("Price Upgrade --- GD")] public List<listInt> FPrice = new List<listInt>();
    public List<listInt> CPrice = new List<listInt>();

    #region Singleton

    private static UpgradeMenu instance;

    public static UpgradeMenu Instance
    {
        get => instance;
        set => instance = value;
    }

    #endregion

    private NetworkList<int> upgradesForteresseServer;
    private NetworkList<int> upgradesCamionServer;
    private NetworkList<int> unlockedWeapon1Server;
    private NetworkList<int> unlockedWeapon2Server;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;


        upgradesForteresseServer = new NetworkList<int>();
        upgradesCamionServer = new NetworkList<int>();
        unlockedWeapon1Server = new NetworkList<int>();
        unlockedWeapon2Server = new NetworkList<int>();
        
        for (int i = 0; i < 3; i++)
        {
            upgradesF.Add(0);
            upgradesC.Add(0);
        }
        
    }


    private void Start()
    {
        if (IsHost)
        {
            for (int i = 0; i < 3; i++)
            {
                upgradesForteresseServer.Add(0);
                upgradesCamionServer.Add(0);
            }
        }

        #region pour tout set et que ca bug pas

        gotoScreen(1);
        visuF();
        gotoScreen(2);
        visuC();
        gotoScreen(3);
        upgradeWeapon1(listAllButton1[0]);
        gotoScreen(4);
        upgradeWeapon2(listAllButton2[0]);
        gotoScreen(0);

        #endregion
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            upgradesForteresseServer.OnListChanged += UpgradesForteresseServerOnChanged;
            upgradesCamionServer.OnListChanged += UpgradesCamionServerOnChanged;
            unlockedWeapon1Server.OnListChanged += UnlockWeapon1ServerOnChanged;
            unlockedWeapon2Server.OnListChanged += UnlockWeapon2ServerOnChanged;
        }
    }


    public void gotoScreen(int lint)
    {
        for (int i = 0; i < screenList.Count; i++)
        {
            if (i == lint) screenList[i].SetActive(true);
            else screenList[i].SetActive(false);
        }
    }

    public void upgradeForteresse(int lint)
    {
        if (FPrice[lint].intList[upgradesF[lint]] <= ScrapMetal.Instance.scrap)
        {
            ScrapMetal.Instance.addMoneyServerRpc(-FPrice[lint].intList[upgradesF[lint]]);

            UpgradeForteresseServerRpc(lint);
        }
    }

    public void upgradeCamion(int lint)
    {
        if (CPrice[lint].intList[upgradesC[lint]] <= ScrapMetal.Instance.scrap)
        {
            ScrapMetal.Instance.addMoneyServerRpc(-CPrice[lint].intList[upgradesC[lint]]);

            UpgradeCamionServerRpc(lint);
        }
    }


    public void upgradeWeapon1(WTreeButton buttonTree)
    {
        disableAllWeapon1();

        var currentButtonTree = buttonTree;
        bool finished = false;

        while (!finished)
        {
            currentButtonTree.buyed();
            if (currentButtonTree.previousUpgrades != null) currentButtonTree = currentButtonTree.previousUpgrades;
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

    private void disableAllWeapon1()
    {
        for (int i = 0; i < listAllButton1.Count; i++)
        {
            listAllButton1[i].unselectable();
        }
    }



    public void upgradeWeapon2(WTreeButton buttonTree)
    {
        disableAllWeapon2();
      
        var currentButtonTree = buttonTree;
        bool finished = false;

        while (!finished)
        {
            currentButtonTree.buyed();
            if (currentButtonTree.previousUpgrades != null) currentButtonTree = currentButtonTree.previousUpgrades;
            else finished = true;
        }
      
        for (int i = 0; i < listAllButton2.Count; i++)
        {
            if (listAllButton1[i].previousUpgrades != null)
            {
                if (listAllButton2[i].previousUpgrades == buttonTree) listAllButton2[i].turnOn();
                else if (listAllButton2[i].previousUpgrades.canBeUpgrades) listAllButton2[i].notSelectedYet();
            }
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


    [System.Serializable]
    public class listInt
    {
        public string newName;
        public List<int> intList;
    }


    #region Online

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void UpgradeForteresseServerRpc(int upgradeIndex, ServerRpcParams serverRpcParams = default)
    {
        upgradesForteresseServer[upgradeIndex]++;
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void UpgradeCamionServerRpc(int upgradeIndex, ServerRpcParams serverRpcParams = default)
    {
        upgradesCamionServer[upgradeIndex]++;
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void UnlockWeapon1ServerRpc(int weaponIdToUnlock, ServerRpcParams serverRpcParams = default)
    {
        unlockedWeapon1Server.Add(weaponIdToUnlock);
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void UnlockWeapon2ServerRpc(int weaponIdToUnlock, ServerRpcParams serverRpcParams = default)
    {
        unlockedWeapon2Server.Add(weaponIdToUnlock);
    }

    private void UpgradesForteresseServerOnChanged(NetworkListEvent<int> newList)
    {
        Debug.Log($"upgradesF[{newList.Index}] = {newList.Value}");

        upgradesF[newList.Index] = newList.Value;
        visuF();
    }

    private void UpgradesCamionServerOnChanged(NetworkListEvent<int> newList)
    {
        Debug.Log($"upgradesC[{newList.Index}] = {newList.Value}");

        upgradesC[newList.Index] = newList.Value;
        visuC();
    }

    private void UnlockWeapon1ServerOnChanged(NetworkListEvent<int> newList)
    {
        WTreeButton weaponBuyed = listAllButton1.Find((button) =>
        {
            return button.id == newList.Value;
        });
        
        upgradeWeapon1(weaponBuyed);
    }

    private void UnlockWeapon2ServerOnChanged(NetworkListEvent<int> newList)
    {
        WTreeButton weaponBuyed = listAllButton2.Find((button) =>
        {
            return button.id == newList.Value;
        });
        
        upgradeWeapon2(weaponBuyed);
    }

    #endregion
}