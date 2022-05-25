using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public List<HPAntenna> listAntenna = new List<HPAntenna>();
    public List<WeaponBase> listWeapon = new List<WeaponBase>();

    #region Singleton
    private static Boss instance;
    public static Boss Instance { get => instance; set => instance = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    
    
    public void checkAntenna()
    {
        for (int i = 0; i < listAntenna.Count; i++)
        {
            if(listAntenna[i].broken) listWeapon[i].DisableWeapon();
        }
    }
}
