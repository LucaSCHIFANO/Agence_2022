using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
   public List<GameObject> screenList = new List<GameObject>();

   private int intUpgrade = 4;
   
   [Header("Forteresse")]
   [SerializeField] private List<Image> upgradesFVisu = new List<Image>(); //upgrades forteresse mais les visus
   [SerializeField] private List<GameObject> upgradesFButton = new List<GameObject>(); //upgrades forteresse mais les buttons +
   [SerializeField] private List<TextMeshProUGUI> listPriceF = new List<TextMeshProUGUI>();
   private List<int> upgradesF = new List<int>(); //upgrades forteresse en int
   
   [Header("Camion")]
   [SerializeField] private List<Image> upgradesCVisu = new List<Image>(); //upgrades forteresse mais les visus
   [SerializeField] private List<GameObject> upgradesCButton = new List<GameObject>(); //upgrades forteresse mais les buttons +
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


   [Header("Price Upgrade --- GD")]
   public List<listInt> FPrice = new List<listInt>();
   public List<listInt> CPrice = new List<listInt>();


   #region Singleton
   private static UpgradeMenu instance;
   public static UpgradeMenu Instance { get => instance; set => instance = value; }
   #endregion

   private void Awake()
   {
      if (Instance == null)
         Instance = this;
   }


   private void Start()
   {
      for (int i = 0; i < 3; i++)
      {
         upgradesF.Add(0);
         upgradesC.Add(0);
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
         ScrapMetal.Instance.addMoney(-FPrice[lint].intList[upgradesF[lint]]);

         upgradesF[lint]++;
         visuF();
      }
   }
   
   public void upgradeCamion(int lint)
   {
      if (CPrice[lint].intList[upgradesC[lint]] <= ScrapMetal.Instance.scrap)
      {
         ScrapMetal.Instance.addMoney(-CPrice[lint].intList[upgradesC[lint]]);

         upgradesC[lint]++;
         visuC();
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

   
   
   
   

   
   private void updateVisu()
   {
      visuF();
      visuC();
   }

   
   
   
   
   
   
   

   private void visuF()
   {
      var multi = 1;
      for (int i = 0; i < upgradesF.Count; i++)
      {
         var step = 0;
         for (int j = intUpgrade*(multi-1); j < intUpgrade * multi && step < intUpgrade; j++)
         {
            var j2 = j;
            if (j >= intUpgrade) j2 -= intUpgrade * (multi-1); 
            
            if(upgradesF[i] > j2 && step==0) upgradesFVisu[j].color = new Color(1,1,1,0);
            else if(upgradesF[i] > j2 && step!=0) upgradesFVisu[j].color = Color.red;
            
            step++;
         }

         if(upgradesF[i]==intUpgrade) upgradesFButton[i].SetActive(false);
         multi++;
      }


      for (int i = 0; i < listPriceF.Count; i++)
      {
         if(listPriceF[i].IsActive()) listPriceF[i].text = FPrice[i].intList[upgradesF[i]].ToString();
      }
   }
   
   private void visuC()
   {
      var multi = 1;
      for (int i = 0; i < upgradesC.Count; i++)
      {
         var step = 0;
         for (int j = intUpgrade*(multi-1); j < intUpgrade * multi && step < intUpgrade; j++)
         {
            var j2 = j;
            if (j >= intUpgrade) j2 -= intUpgrade * (multi-1); 
            
            if(upgradesC[i] > j2 && step==0) upgradesCVisu[j].color = new Color(1,1,1,0);
            else if(upgradesC[i] > j2 && step!=0) upgradesCVisu[j].color = Color.red;
            
            step++;
         }

         if(upgradesC[i]==intUpgrade) upgradesCButton[i].SetActive(false);
         multi++;
         
         for (int j = 0; j < listPriceF.Count; j++)
         {
            if(listPriceC[j].IsActive()) listPriceC[j].text = CPrice[j].intList[upgradesC[j]].ToString();
         }
      }
   }

   
   
   [System.Serializable]
   public class listInt
   {
      public string newName;
      public List<int> intList;
   }
}
