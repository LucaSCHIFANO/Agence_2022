using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnClickTriangle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    [Header("Triangle Panel")] 
    protected bool isClicked;
    protected bool isInside;
    public bool canMove;
    
    
    public void OnPointerDown(PointerEventData eventData){
        isClicked = true;
    }
 
    public void OnPointerUp(PointerEventData eventData){
        isClicked = false;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInside = false;
    }

    private void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    private void Update()
    {
        if (isClicked && isInside) canMove = true;
        else canMove = false;

    }
}
