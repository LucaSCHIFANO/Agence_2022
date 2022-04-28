using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WTreeButton : MonoBehaviour
{
   private Image image;
   private bool cannotBeSelected;
   public bool firstWeapon;

   private void Awake()
   {
      image = GetComponent<Image>();
   }

   public void selected()
   {
      if (!cannotBeSelected)
      {
         if(firstWeapon) UpgradeMenu.Instance.upgradeWeapon1(this);
         else UpgradeMenu.Instance.upgradeWeapon2(this);
      }
      
   }

   public void buyed()
   {
      image.color = new Color(0, 0, 0, 0);
      cannotBeSelected = true;
   }

   public void turnOn()
   {
      image.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
      cannotBeSelected = false;
   }

   public void notSelectedYet()
   {
      image.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
      cannotBeSelected = true;
   }
   
   public void unselectable()
   {
      image.color = new Color(0, 0 ,0, 0.9f);
      cannotBeSelected = true;
   }
   
}
