using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    [SerializeField] protected Vector2 timeBtwExplosion;

    [SerializeField] protected Vector2 plusOuMoins;

    [SerializeField] protected GameObject explosion;
    
    [Header("BlackScreen")]
    [SerializeField] protected float timeBeforeFonduAuNoir;
    [SerializeField] protected GameObject blackScreen;
    
    private void Awake()
    {
        (reference as IReferenceHead<WinManager>).Set(this);
    }

    public void callTheEnd()
    {
        spline.getSpeed = 0.0000000001f;
        camera.SetActive(true);
        for (int i = 0; i < boss.listWeapon.Count; i++)
        {
            boss.listWeapon[i].DisableWeapon();
        }

        StartCoroutine(explosionToEnd());
        StartCoroutine(fadeOutToEnd());
        
    }
    
    IEnumerator explosionToEnd()
    {
        while(true)
        {
            Vector3 pos = new Vector3(boss.transform.position.x + Random.Range(plusOuMoins.x, plusOuMoins.y), boss.transform.position.y + Random.Range(plusOuMoins.x, plusOuMoins.y), boss.transform.position.z + Random.Range(plusOuMoins.x, plusOuMoins.y));

            Instantiate(explosion, pos, boss.transform.rotation);
            yield return new WaitForSeconds(Random.Range(timeBtwExplosion.x, timeBtwExplosion.y));
        }
        
    }
    
    IEnumerator fadeOutToEnd()
    {
        yield return new WaitForSeconds(timeBeforeFonduAuNoir);
        
        blackScreen.SetActive(true);
        blackScreen.GetComponent<Animator>().SetTrigger("ChangerState");
        
        yield return new WaitForSeconds(1.2f);

        App.Instance.Session.LoadMap(MapIndex.Win);
    }
    
    
}
