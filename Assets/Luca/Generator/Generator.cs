using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Generator : MonoBehaviour
{
    [SerializeField] protected GameObject upgradePoint;
    [SerializeField] private List<Transform> listSommets = new List<Transform>();
    [SerializeField] private List<UILineRenderer> myLines = new List<UILineRenderer>();
    [SerializeField] private float lineThinkness;
    [SerializeField] private List<float> colorDistance = new List<float>(); // green then orange then red

    private void Start()
    {
        //onClickTriangle(true);
    }

    public void onClickTriangle()
    {
        upgradePoint.transform.position = Input.mousePosition;

        for (int i = 0; i < listSommets.Count; i++)
        {
            myLines[i].transform.position = upgradePoint.transform.position;
            
            float distTop = Vector3.Distance(upgradePoint.transform.position, listSommets[i].position);
            var vectorTop = new Vector3(listSommets[i].position.x - upgradePoint.transform.position.x,
                listSommets[i].position.y - upgradePoint.transform.position.y);
            
            int numberOfPoint = (int) (distTop / lineThinkness);
            myLines[i].LineThickness = lineThinkness;

            var pointlist = new List<Vector2>();
            for (int j = 1; j < numberOfPoint; j++)
            {
                pointlist.Add(new Vector2((vectorTop.x / numberOfPoint) * j, (vectorTop.y / numberOfPoint) * j));
            }

            myLines[i].Points = pointlist.ToArray();
            
            if(distTop < colorDistance[0]) myLines[i].color = Color.green;
            else if(distTop < colorDistance[1]) myLines[i].color = new Color(0.9f, 0.5f, 0.04f);
            else myLines[i].color = Color.red;
            
            // 730 max
            //530 les autres
            
            
        }
    }
}
