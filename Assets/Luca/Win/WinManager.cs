using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WinManager : MonoBehaviour
{
    [SerializeField] protected WinRef reference;
    
    [Header("Needed")]
    [SerializeField] protected Spline spline;
    [SerializeField] protected Boss boss;
    
    [SerializeField] protected GameObject camera;

    [Header("Explosion")]
    [SerializeField] protected int numberOfExplosion;
    [SerializeField] protected float timeBtwExplosion;
    
    [SerializeField] protected Vector2 plusOuMoins;

    [SerializeField] protected GameObject explosion;
    
    private void Awake()
    {
        (reference as IReferenceHead<WinManager>).Set(this);
    }

    public void callTheEnd()
    {
        spline.getSpeed = 0;
        camera.SetActive(true);
        for (int i = 0; i < boss.listWeapon.Count; i++)
        {
            boss.listWeapon[i].DisableWeapon();
        }

        StartCoroutine(explosionToEnd());
        
    }
    
    IEnumerator explosionToEnd()
    {
        for (int i = 0; i < numberOfExplosion; i++)
        {
            Vector3 pos = new Vector3(boss.transform.position.x + Random.Range(plusOuMoins.x, plusOuMoins.y), boss.transform.position.y + Random.Range(plusOuMoins.x, plusOuMoins.y), boss.transform.position.z + Random.Range(plusOuMoins.x, plusOuMoins.y));

            Instantiate(explosion, pos, boss.transform.rotation);
            yield return new WaitForSeconds(timeBtwExplosion);
        }
        
        App.Instance.Session.LoadMap(MapIndex.Win);
    }
    
    
}
