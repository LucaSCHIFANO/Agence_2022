using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour
{ 
    public GameObject Camera;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private TextMeshProUGUI _name;
    
    
    [Header("Health")]
    [SerializeField] protected float maxHP;
    protected float currentHP;
    
    [SerializeField][Range(0, 1)] protected float hpPourcent;
    
    [SerializeField] protected float timeBeforeRecov;
    protected float currentTimeRecov;
    [SerializeField] protected float recovPerSecond;
    
    public static NetworkedPlayer Local { get; set; }
    public PossessingType PossessingType = PossessingType.CHARACTER;
    

    public CharacterInputHandler CharacterInputHandler;
    public WeaponInputHandler WeaponInputHandler;
    // public CharacterInputHandler CarInputHandler;
    
    private Player _player;

    public override void Spawned()
    {
        _player = App.Instance.GetPlayer(Object.InputAuthority);
        _name.text = _player.Name;
        _mesh.material.color = _player.Color;
        CharacterInputHandler = GetComponent<CharacterInputHandler>();
        
        currentHP = maxHP;
        
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Spawned Local Player");
            Camera.SetActive(true);
        }
        else
        {
            Debug.Log("Spawned Remote Player");
        }
    }
    
    public void Unpossess(Transform exitPoint)
    {
        if (Object.HasInputAuthority)
            Camera.SetActive(true);
        
        transform.position = exitPoint.position;
        transform.rotation = exitPoint.rotation;

        GetComponent<CharacterController>().enabled = true;
    }

    public void Possess(Transform seat)
    {
        GetComponent<CharacterController>().enabled = false;

        if (Runner.IsServer)
        {
            transform.position = seat.position;
            transform.rotation = seat.rotation;
        }
        
        if (Object.HasInputAuthority)
            Camera.SetActive(false);
    }

    public void ChangeInputHandler(PossessingType possessingType, GameObject handler)
    {
        PossessingType = possessingType;
        if (possessingType == PossessingType.WEAPON)
        {
            WeaponInputHandler = handler.GetComponent<WeaponInputHandler>();
        }
        else if (possessingType == PossessingType.CAR)
        {
            // handler.GetComponent<CarInputHandler>();
        }
        else
        {
            WeaponInputHandler = null;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        HP();
    }

    void HP()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ReceiveDamage(10f);
        }

        if (currentTimeRecov <= 0) currentHP += Time.deltaTime * recovPerSecond;
        else currentTimeRecov -= Time.deltaTime;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        hpPourcent = currentHP / maxHP;
        
        CanvasInGame.Instance.actuBlood(Mathf.Abs(hpPourcent - 1));
    }

    public void ReceiveDamage(float damage)
    {
        currentHP -= damage;
        currentTimeRecov = timeBeforeRecov;
    }
}

public enum PossessingType
{
    CHARACTER,
    CAR,
    WEAPON
}