using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ReservoirParts : MonoBehaviour
{
    [System.Serializable]
    public class Part
    {
        public Collider col;
        public Color color;
        public float maxResistance;
        internal float curResistance;
        internal Vector3 leakPos;
        bool isLeaking;
        NetworkObject leakPrefab;
        NetworkObject leak;

        public void Init(NetworkObject leakObj)
        {
            curResistance = maxResistance;
            leakPrefab = leakObj;
        }

        public void GetHit(float value, RaycastHit hitPoint, GameObject hitGo)
        {
            if (isLeaking) { return; }
            curResistance -= value;
            if (curResistance <= 0)
            {
                isLeaking = true;
                App.Instance.Session.Runner.Spawn(leakPrefab, hitPoint.point, Quaternion.Euler(hitPoint.normal), onBeforeSpawned : (runner, obj) => {
                    leak = obj;
                    obj.transform.SetParent(hitGo.transform);
                    obj.transform.forward = -hitPoint.normal;
                    obj.GetComponent<Leak>().Part = this;
                    leakPos = hitPoint.point;
                    Leak l = obj.GetComponent<Leak>();
                    TruckFuel[] t = FindObjectsOfType<TruckFuel>(); //TO CHANGE
                    foreach (TruckFuel f in t)
                    {
                        l.Damage = f.AddConstDamage(l.DamageName);
                    }
                });
            }
        }

        public void Repair()
        {
            isLeaking = false;
            //Destroy(leak);
            App.Instance.Session.Runner.Despawn(leak);
        }
    }

    [SerializeField] LayerMask partlayer;
    [SerializeField] NetworkObject leak;


    [SerializeField] bool showGizmos;
    [Range(0.0f, 1.0f)]
    [SerializeField] float allPartsColorAlpha;
    [SerializeField] List<Part> parts;


    RaycastHit hit;

    [HideInInspector]
    public Camera cam; //DEBUG

    private void Awake()
    {
        foreach (Part part in parts) part.Init(leak); 
    }


    public void HitReservoir(RaycastHit hit)
    {
        foreach (Part part in parts)
        {
            BoxCollider c = part.col as BoxCollider;
            if (hit.collider != null && hit.collider == c) part.GetHit(5, hit, c.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            foreach (Part part in parts)
            {
                BoxCollider c = part.col as BoxCollider;
                Vector3 s = c.gameObject.transform.localScale;
                Vector3 vec = new Vector3(c.size.x * (s.x), c.size.y * s.y, c.size.z * (s.z));
                Gizmos.color = new Color(part.color.r, part.color.g, part.color.b, part.color.a * allPartsColorAlpha);
                if (hit.collider != null && hit.collider == c) Gizmos.color = Color.black;
                Gizmos.DrawCube(c.transform.position + c.center, vec); //Assume part collider is a BoxCollider
            }
        }
    }

}
