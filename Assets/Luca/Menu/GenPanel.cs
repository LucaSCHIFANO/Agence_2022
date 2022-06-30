using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GenPanel : NetworkBehaviour
{
    
    [Header("Interact")]
    private NetworkedPlayer _playerController;
    [Networked] private bool isPossessed { get; set; }
    
    
    
    #region Singleton

    private static GenPanel instance;

    public static GenPanel Instance
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



    public void quitPanel(){

        Vector2 newPos = SetGoodValue();
        sendUpgradePosRpc(Generator.Instance.pourcentageListWOutChange.ToArray(),
            Generator.Instance.pourcentageList.ToArray(), newPos, Generator.Instance.isOvercloaking, Generator.Instance.overCloakeInt);
        //Part2Rpc(Generator.Instance.pourcentageList);
        
        _playerController.ChangeInputHandler(PossessingType.CHARACTER, gameObject);
        CanvasInGame.Instance.showGen(false);
        
        if (_playerController.Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _playerController.enabled = true;
            isPossessed = false;
        }

    }

    private Vector2 SetGoodValue()
    {
        Generator gen = Generator.Instance; 
       var maximumHeight = gen.listSommets[1].transform.position.y - gen.listSommets[0].transform.position.y;
       var actualHeight = gen.upgradePoint.transform.position.y - gen.listSommets[0].transform.position.y;

       var pourcentDistanceH = (actualHeight / maximumHeight);

       var maximumWidth = gen.listSommets[2].transform.position.x - gen.listSommets[1].transform.position.x;
       var actualWidth = gen.upgradePoint.transform.position.x - gen.listSommets[1].transform.position.x;

       var pourcentDistanceW = (actualWidth / maximumWidth);

       return new Vector2(pourcentDistanceW, pourcentDistanceH);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void sendUpgradePosRpc(float[] pourcent, float[] pourcentChanged, Vector2 newPos, NetworkBool isover, int leint)
    {
        var value1 = new List<float>(pourcent);
        var value2 = new List<float>(pourcentChanged);
        
        Generator.Instance.pourcentageListWOutChange = value1;
        Generator.Instance.pourcentageList = value2;
        
        Generator.Instance.visuelChange(newPos, isover, leint);
        
    }

    /*[Rpc(RpcSources.All, RpcTargets.All)]
    private void Part2Rpc(List<float> pourcentChanged)
    {
        Generator.Instance.pourcentageList = pourcentChanged;
        Generator.Instance.visuelChange();
    }*/



    public void Interact(NetworkedPlayer other)
    {
        if (isPossessed) return;

        if (other.Object.HasInputAuthority)
        {
            other.ChangeInputHandler(PossessingType.NONE, gameObject);
            CanvasInGame.Instance.showGen(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _playerController = other;
            other.enabled = false;
            isPossessed = true;
        }


    }
}
