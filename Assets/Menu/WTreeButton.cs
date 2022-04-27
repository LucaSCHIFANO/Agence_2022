using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WTreeButton : MonoBehaviour
{
   private Image image;
   private bool cannotBeSelected;

   private void Start()
   {
      image = GetComponent<Image>();
   }

   public void selected()
   {
      if(!cannotBeSelected) UpgradeMenu.Instance.upgradeWeapon1(this);
      
   }

   public void turnOn()
   {
      image.color = Color.white;
   }

   public void notSelectedYet()
   {
      image.color = Color.gray;
      cannotBeSelected = false;
   }
   
   public void unselectable()
   {
      image.color = Color.black;
      cannotBeSelected = true;
   }
   
}
