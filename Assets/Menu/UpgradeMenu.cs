using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
   public List<GameObject> screenList = new List<GameObject>();
   
   [Header("Forteresse")]
   private List<int> upgradesF = new List<int>(); //upgrades forteresse en int
   [SerializeField] private List<Image> upgradesFVisu = new List<Image>(); //upgrades forteresse mais les visus
   [SerializeField] private List<GameObject> upgradesFButton = new List<GameObject>(); //upgrades forteresse mais les buttons +


   private void Start()
   {
      for (int i = 0; i < 3; i++)
      {
         upgradesF.Add(0);
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
      upgradesF[lint]++;
      updateVisu();
   }


   private void updateVisu()
   {
      #region Forteresse

      var multi = 1;
      for (int i = 0; i < upgradesF.Count; i++)
      {
         var step = 0;
         for (int j = 3*(multi-1); j < 3 * multi && step < 3; j++)
         {
            var j2 = j;
            if (j >= 3) j2 -= 3 * (multi-1); 
            
            if(upgradesF[i] > j2) upgradesFVisu[j].color = Color.red;
            
            step++;
         }

         if(upgradesF[i]==3) upgradesFButton[i].SetActive(false);
         multi++;
      }
      #endregion
   }
   
}
