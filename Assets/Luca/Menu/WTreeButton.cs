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
   private bool isSellable;
   [HideInInspector]public bool canBeUpgrades;
   [SerializeField] public bool firstWeapon;
   [SerializeField] private int price;
   [SerializeField] private int sellPricePourcentage;

   public WTreeButton previousUpgrades;

   private void Awake()
   {
      image = GetComponent<Image>();
      text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
      text.text = price.ToString();
   }

   void sellChangePrice(bool lbool)
   {
      if(!lbool) text.text = price.ToString();
      else text.text = ((int)(price * (sellPricePourcentage / 100f))).ToString(); 
   }

   public void selected()
   {
      if ((!isSellable && !cannotBeSelected && ScrapMetal.Instance.scrap >= price) || (isSellable && previousUpgrades != null))
         {
            if (isSellable) ScrapMetal.Instance.addMoneyServerRpc((int)(price * (sellPricePourcentage / 100f)));
            else ScrapMetal.Instance.addMoneyServerRpc(-price);
            
            
            if (firstWeapon)
            {
               if(!isSellable) UpgradeMenu.Instance.UnlockWeapon1ServerRpc(id);
               else UpgradeMenu.Instance.SellWeapon1ServerRpc(id);
               // UpgradeMenu.Instance.upgradeWeapon1(this);
            }
            else
            {
               if(!isSellable) UpgradeMenu.Instance.UnlockWeapon2ServerRpc(id);
               else UpgradeMenu.Instance.SellWeapon2ServerRpc(id);
               // UpgradeMenu.Instance.upgradeWeapon2(this);
            }
         }
   }
   

   public void buyed()
   {
      image.color = new Color(0, 0, 0, 0);
      cannotBeSelected = true;
      text.gameObject.SetActive(false);
      canBeUpgrades = false;
      isSellable = false;
   }

   public void turnOn()
   {
      image.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
      cannotBeSelected = false;
      text.gameObject.SetActive(true);
      canBeUpgrades = true;
      isSellable = false;
      sellChangePrice(false);
   }

   public void notSelectedYet()
   {
      image.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
      cannotBeSelected = true;
      text.gameObject.SetActive(false);
      canBeUpgrades = true;
      isSellable = false;
   }
   
   public void unselectable()
   {
      image.color = new Color(0, 0 ,0, 0.9f);
      cannotBeSelected = true;
      text.gameObject.SetActive(false);
      canBeUpgrades = false;
      isSellable = false;
   }
   
   public void sellable()
   {
      image.color = new Color(1f, 0.7f ,0.7f, 0.75f);
      cannotBeSelected = false;
      text.gameObject.SetActive(true);
      canBeUpgrades = false;
      isSellable = true;
      sellChangePrice(true);
   }
   
}
