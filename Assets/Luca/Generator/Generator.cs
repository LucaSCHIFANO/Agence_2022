using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
    [SerializeField] private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>(); // green then orange then red
    [SerializeField] private List<GameObject> textButtonOverCloke = new List<GameObject>(); // green then orange then red

    [Header("Pourcentage")]
    [SerializeField] private float minimumDist = 0; // 0 au plus pret du sommet
    [SerializeField] private float maximumDist = 850; // ~840 au plus pret du sommet
    [SerializeField] private float overclokePourcent; // ~840 au plus pret du sommet
    [SerializeField] private float overclokePourcentOther; // ~840 au plus pret du sommet
    
    

    private void Start()
    {
        onClickTriangle(false);
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
            if (distTop < 10)
            {
                myLines[i].gameObject.SetActive(false);
                return;
            }
            
            var vectorTop = listSommets[i].position - upgradePoint.transform.position;
            
            int numberOfPoint = (int) (distTop / lineThinkness);
            myLines[i].LineThickness = lineThinkness;

            var pointlist = new List<Vector2>();
            for (int j = 1; j <= numberOfPoint; j++)
            {
                pointlist.Add(new Vector2((vectorTop.x / numberOfPoint) * j, (vectorTop.y / numberOfPoint) * j));
            }

            myLines[i].Points = pointlist.ToArray();
            
            if(distTop < colorDistance[0]) myLines[i].color = Color.green;
            else if(distTop < colorDistance[1]) myLines[i].color = new Color(0.9f, 0.5f, 0.04f);
            else myLines[i].color = Color.red;

            var pourcent = (((maximumDist - distTop) / maximumDist)*100).ToString("F2");  // max distance 800 min distance 0
            switch (i)
            {
                case 0:
                    textList[0].text = "Att : " + pourcent + "%";
                    break;
                case 1:
                    textList[1].text = "Def : " + pourcent + "%";
                    break;
                case 2:textList[2].text = "Spd : " + pourcent + "%";
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
