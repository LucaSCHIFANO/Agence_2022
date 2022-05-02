using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WTreeButton : MonoBehaviour
{
   public int id;
   
   private Image image;
   private TextMeshProUGUI text;
   private bool cannotBeSelected;
   [HideInInspector]public bool canBeUpgrades;
   [SerializeField] private bool firstWeapon;
   [SerializeField] private int price;

   public WTreeButton previousUpgrades;

   private void Awake()
   {
      image = GetComponent<Image>();
      text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
      text.text = price.ToString();
   }

   public void selected()
   {
      if (!cannotBeSelected && ScrapMetal.Instance.scrap >= price)
      {
         if (firstWeapon)
         {
            UpgradeMenu.Instance.UnlockWeapon1ServerRpc(id);
            // UpgradeMenu.Instance.upgradeWeapon1(this);
         }
         else
         {
            UpgradeMenu.Instance.UnlockWeapon2ServerRpc(id);
            // UpgradeMenu.Instance.upgradeWeapon2(this);
         }

         ScrapMetal.Instance.addMoneyServerRpc(-price);
      }
      
   }

   public void buyed()
   {
      image.color = new Color(0, 0, 0, 0);
      cannotBeSelected = true;
      text.gameObject.SetActive(false);
      canBeUpgrades = false;
   }

   public void turnOn()
   {
      image.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
      cannotBeSelected = false;
      text.gameObject.SetActive(true);
      canBeUpgrades = true;
   }

   public void notSelectedYet()
   {
      image.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
      cannotBeSelected = true;
      text.gameObject.SetActive(false);
      canBeUpgrades = true;
   }
   
   public void unselectable()
   {
      image.color = new Color(0, 0 ,0, 0.9f);
      cannotBeSelected = true;
      text.gameObject.SetActive(false);
      canBeUpgrades = false;
   }
   
}