using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

public class WinManager : MonoBehaviour
{
    [SerializeField] protected WinRef reference;
    
    [Header("Needed Boss")]
    [SerializeField] protected Spline spline;
    [SerializeField] protected Boss boss;
    [SerializeField] protected GameObject bossVisual;
    [SerializeField] protected GameObject camera;
    
    [Header("Needed Truck")]
    [SerializeField] protected GameObject truckVisual;
    
    [SerializeField] protected GameObject camera2;

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
            Vector3 pos = new Vector3(bossVisual.transform.position.x + Random.Range(plusOuMoins.x, plusOuMoins.y), bossVisual.transform.position.y + Random.Range(plusOuMoins.x, plusOuMoins.y), bossVisual.transform.position.z + Random.Range(plusOuMoins.x, plusOuMoins.y));

            Instantiate(explosion, pos, bossVisual.transform.rotation);
            yield return new WaitForSeconds(Random.Range(timeBtwExplosion.x, timeBtwExplosion.y));
        }
        
    }
    
    IEnumerator fadeOutToEnd()
    {
        yield return new WaitForSeconds(timeBeforeFonduAuNoir);
        
        blackScreen.SetActive(true);
        blackScreen.GetComponent<Animator>().SetTrigger("ActivateFadeIn");
        //blackScreen.GetComponent<Animator>().SetTrigger("ChangerState");
        
        yield return new WaitForSeconds(1.2f);

        App.Instance.Session.LoadMap(MapIndex.Win);
    }
    
    
    public void callTheGameOver()
    {
        camera2.SetActive(true);

        StartCoroutine(explosionTheGameOver());
        StartCoroutine(fadeOutTheGame());
        
    }
    
    IEnumerator explosionTheGameOver()
    {
        while(true)
        {
            Vector3 pos = new Vector3(truckVisual.transform.position.x + Random.Range(plusOuMoins.x, plusOuMoins.y), truckVisual.transform.position.y + Random.Range(plusOuMoins.x, plusOuMoins.y), truckVisual.transform.position.z + Random.Range(plusOuMoins.x, plusOuMoins.y));

            Instantiate(explosion, pos, truckVisual.transform.rotation);
            yield return new WaitForSeconds(Random.Range(timeBtwExplosion.x, timeBtwExplosion.y));
        }
        
    }
    
    IEnumerator fadeOutTheGame()
    {
        yield return new WaitForSeconds(timeBeforeFonduAuNoir);
        
        blackScreen.SetActive(true);
        blackScreen.GetComponent<Animator>().SetTrigger("ActivateFadeIn");
        //blackScreen.GetComponent<Animator>().SetTrigger("ChangerState");
        
        yield return new WaitForSeconds(1.2f);

        App.Instance.Session.LoadMap(MapIndex.GameOver);
    }
    
}
