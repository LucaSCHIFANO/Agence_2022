using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Color = UnityEngine.Color;

public class Generator : MonoBehaviour
{
    /*[SerializeField] protected OnClickTriangle triangleButton;

    [SerializeField] public GameObject upgradePoint;
    [SerializeField] public List<Transform> listSommets = new List<Transform>();
    [SerializeField] private List<UILineRenderer> myLines = new List<UILineRenderer>();
    [SerializeField] private float lineThinkness;
    [SerializeField] private List<float> colorDistance = new List<float>(); // green then orange then red
    [SerializeField] private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>(); // att def spd
    [SerializeField] private List<GameObject> textButtonOverCloke = new List<GameObject>(); // att def spds

    [Header("Pourcentage")] 
    private float minimumDist = 0; 
    private float maximumDist = 0; 
    private float maximumHeight = 0;
    private float maximumWidth = 0;

    [HideInInspector] public bool isOvercloaking;
    [HideInInspector] public int overCloakeInt;
    [SerializeField] private float overclokePourcent; // pourcent en overcloke
    [SerializeField] private float overclokePourcentOther; // pourcent quand autre en overcloke

    [HideInInspector] public List<float> pourcentageList = new List<float>(); // 0 atk 1 def 2 spd
    [HideInInspector] public List<float> pourcentageListWOutChange = new List<float>(); // 0 atk 1 def 2 spd
    [SerializeField] private int pourcentageUpgrades;
    public int getPourcentUpgrade
    {
        get => pourcentageUpgrades; 
    }


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

    void Start()
    {
        maximumDist = Vector2.Distance(listSommets[1].position, listSommets[2].position);

        for (int i = 0; i < 3; i++)
        {
            pourcentageList.Add(0);
            pourcentageListWOutChange.Add(0);
        }

        onClickTriangle(false);
        gameObject.SetActive(false);
    }

    /*public void quitShop()
    {
        CanvasInGame.Instance.showGen(false);

        NetworkedPlayer _playerController = App.Instance.Session.Runner
            .GetPlayerObject(App.Instance.Session.Runner.LocalPlayer).GetComponent<NetworkedPlayer>();
                
        if (_playerController.Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        if(triangleButton.canMove) onClickTriangle(true);
    }

    public void onClickTriangle(bool boul)
    {
        isOvercloaking = false; 
        
        if (boul) upgradePoint.transform.position = Input.mousePosition;

        for (int i = 0; i < myLines.Count; i++)
        {
            myLines[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < listSommets.Count; i++)
        {
            myLines[i].transform.position = upgradePoint.transform.position;

            float distTop = Vector3.Distance(upgradePoint.transform.position, listSommets[i].position);

            if (distTop < 10 || distTop < 10)
            {
                myLines[i].gameObject.SetActive(false);
            }

            myLines[i].LineThickness = lineThinkness;
          
            
            var pourcent =
                (((maximumDist - distTop) / maximumDist) * 100).ToString("F2"); // max distance 800 min distance 0
            
            pourcentageListWOutChange[i] = (((maximumDist - distTop) / maximumDist) * 100);
            
            switch (i)
            {
                case 0:
                    textList[0].text = "Att : " + pourcent + "%";
                    pourcentageList[0] = ((maximumDist - distTop) / maximumDist) * 100;
                    break;
                case 1:
                    textList[1].text = "Def : " + pourcent + "%";
                    pourcentageList[1] = ((maximumDist - distTop) / maximumDist) * 100;
                    break;
                case 2:
                    textList[2].text = "Spd : " + pourcent + "%";
                    pourcentageList[2] = ((maximumDist - distTop) / maximumDist) * 100;
                    break;
            }
        }
        
        setLine();
    }

    void setLine()
    {
        for (int i = 0; i < listSommets.Count; i++)
        {
            float distTop = Vector3.Distance(upgradePoint.transform.position, listSommets[i].position);

            var pointlist = new List<Vector2>();

            pointlist.Add(Vector2.zero);

            pointlist.Add(-listSommets[i].transform.InverseTransformPoint(upgradePoint.transform.position));

            myLines[i].Points = pointlist.ToArray();

            if ((distTop / maximumDist) * 100 < colorDistance[0])
                myLines[i].color = Color.Lerp(Color.green, new Color(0.7f, 0.49f, 0.11f),
                    distTop / ((maximumDist * colorDistance[0]) * 0.01f));

            else if ((distTop / maximumDist) * 100 < colorDistance[1])

                myLines[i].color = Color.Lerp(new Color(0.7f, 0.49f, 0.11f), new Color(0.36f, 0.02f, 0f),
                    (distTop - ((maximumDist * colorDistance[0]) * 0.01f)) /
                    (((maximumDist * colorDistance[1]) * 0.01f) - ((maximumDist * colorDistance[0]) * 0.01f)));

            else myLines[i].color = new Color(0.36f, 0.02f, 0f);
        }
    }

    public void visuelChange(Vector2 newPos, bool isOver, int leint)
    {
        Debug.Log(isOver + " " + leint);
        if (isOver) overcloking(leint);
        else
        {
            textList[0].text = "Att : " + pourcentageListWOutChange[0].ToString("F2") + "%";
            textList[1].text = "Def : " + pourcentageListWOutChange[1].ToString("F2") + "%";
            textList[2].text = "Spd : " + pourcentageListWOutChange[2].ToString("F2") + "%";

            upgradePoint.transform.position = new Vector2(
                Mathf.Lerp(listSommets[1].transform.position.x, listSommets[2].transform.position.x, newPos.x),
                Mathf.Lerp(listSommets[0].transform.position.y, listSommets[1].transform.position.y, newPos.y));

            onClickTriangle(false);
        }
    }


    public void overcloking(int position)
    {
        isOvercloaking = true;
        overCloakeInt = position;
        
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
                case 2:
                    textList[2].text = "Spd : " + pourcentProv + "%";
                    break;
            }
        }
    }*/
}
