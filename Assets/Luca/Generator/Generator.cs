using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Generator : NetworkBehaviour
{
    [Header("Triangle Panel")]
    [SerializeField] protected GameObject upgradePoint;
    [SerializeField] private List<Transform> listSommets = new List<Transform>();
    [SerializeField] private List<UILineRenderer> myLines = new List<UILineRenderer>();
    [SerializeField] private float lineThinkness;
    [SerializeField] private List<float> colorDistance = new List<float>(); // green then orange then red
    [SerializeField] private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>(); // att def spd
    [SerializeField] private List<GameObject> textButtonOverCloke = new List<GameObject>(); // att def spd

    [Header("Pourcentage")]
    private float minimumDist = 0; // 0 au plus pret du sommet
    private float maximumDist = 850; // ~840 au plus loin du sommet
    [SerializeField] private float overclokePourcent; // pourcent en overcloke
    [SerializeField] private float overclokePourcentOther; // pourcent quand autre en overcloke

    [HideInInspector] public List<float> pourcentageList = new List<float>(); // 0 atk 1 def 2 spd


    #region Singleton

    private static Generator instance;

    public static Generator Instance
    {
        get => instance;
        set => instance = value;
    }

    #endregion


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }

    private void Start()
    {
        maximumDist = Vector2.Distance(listSommets[1].position, listSommets[2].position);

        for (int i = 0; i < 3; i++)
        {
            pourcentageList.Add(0);    
        }
        
        onClickTriangle(false);
        gameObject.SetActive(false);
    }

    public void onClickTriangle(bool boul)
    {
        if(boul) upgradePoint.transform.position = Input.mousePosition;
        
        for (int i = 0; i < myLines.Count; i++)
        {
            myLines[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < listSommets.Count; i++)
        {
            myLines[i].transform.position = upgradePoint.transform.position;
            
            float distTop = Vector3.Distance(upgradePoint.transform.position, listSommets[i].position);
            Vector3 vectorTop = listSommets[i].position - upgradePoint.transform.position;

            if (distTop < 10 || distTop < 10)
            {
                myLines[i].gameObject.SetActive(false);
            }
            
            myLines[i].LineThickness = lineThinkness;

            var pointlist = new List<Vector2>();
            
            pointlist.Add(Vector2.zero);

            pointlist.Add(-listSommets[i].transform.InverseTransformPoint(upgradePoint.transform.position));

            myLines[i].Points = pointlist.ToArray();
            
            if(distTop < colorDistance[0]) myLines[i].color = Color.green;
            else if(distTop < colorDistance[1]) myLines[i].color = new Color(0.9f, 0.5f, 0.04f);
            else myLines[i].color = Color.red;
            
            var pourcent = (((maximumDist - distTop) / maximumDist)*100).ToString("F2");  // max distance 800 min distance 0
            switch (i)
            {
                case 0:
                    textList[0].text = "Att : " + pourcent + "%";
                    pourcentageList[0] = ((maximumDist - distTop) / maximumDist)*100;
                    break;
                case 1:
                    textList[1].text = "Def : " + pourcent + "%";
                    pourcentageList[1] = ((maximumDist - distTop) / maximumDist)*100;
                    break;
                case 2:textList[2].text = "Spd : " + pourcent + "%";
                    pourcentageList[2] = ((maximumDist - distTop) / maximumDist)*100;
                    break;
            }

            
            Debug.Log(distTop);
        }
    }

    public void overcloking(int position)
    {
        upgradePoint.transform.position = textButtonOverCloke[position].transform.position;
        
        for (int i = 0; i < myLines.Count; i++)
        {
            myLines[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < textButtonOverCloke.Count; i++)
        {
            var pourcentProv = 0f;
            if (position == i) pourcentProv = overclokePourcent;
            else pourcentProv = overclokePourcentOther;

            switch (i)
            {
                case 0:
                    textList[0].text = "Att : " + pourcentProv + "%";
                    break;
                case 1:
                    textList[1].text = "Def : " + pourcentProv + "%";
                    break;
                case 2:textList[2].text = "Spd : " + pourcentProv + "%";
                    break;
            }
        }
    }
    
    
}
