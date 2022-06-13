using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ReservoirParts : MonoBehaviour
{
    [System.Serializable]
    class Part
    {
        public Collider col;
        public Color color;
        public float maxResistance;
        internal float curResistance;
        internal Vector3 leakPos;
        bool isLeaking;
        GameObject _Decal;
        GameObject decalPrefab;
        public void Init(GameObject decal)
        {
            curResistance = maxResistance;
            decalPrefab = decal;
        }

        public void GetHit(float value, RaycastHit hitPoint)
        {
            if (isLeaking) { Repair(); return; }
            curResistance -= value;
            if (curResistance <= 0)
            {
                isLeaking = true;
                _Decal = Instantiate(decalPrefab, hitPoint.point, Quaternion.Euler(hitPoint.normal));
                _Decal.transform.forward = -hitPoint.normal;
                //_Decal.transform.parent = hitGo.transform;
                leakPos = hitPoint.point;
            }
        }

        public void Repair()
        {
            isLeaking = false;
            Destroy(_Decal);
        }
    }

    [SerializeField] LayerMask partlayer;
    public GameObject decal;


    [SerializeField] bool showGizmos;
    [Range(0.0f, 1.0f)]
    [SerializeField] float allPartsColorAlpha;
    [SerializeField] List<Part> parts;


    RaycastHit hit;

    private void Awake()
    {
        foreach (Part part in parts) part.Init(decal);
    }

    void Start()
    {
        
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Mathf.Infinity, partlayer);

        if (Input.GetMouseButtonDown(0)) HitReservoir(hit);
    }

    void HitReservoir(RaycastHit hit)
    {
        foreach (Part part in parts)
        {
            BoxCollider c = part.col as BoxCollider;
            if (hit.collider != null && hit.collider == c) part.GetHit(5, hit);
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            foreach (Part part in parts)
            {
                BoxCollider c = part.col as BoxCollider;
                Vector3 vec = new Vector3(c.size.x * (transform.localScale.x * 2), c.size.y * transform.localScale.y, c.size.z * (transform.localScale.z / 2));
                Gizmos.color = new Color(part.color.r, part.color.g, part.color.b, part.color.a * allPartsColorAlpha);
                if (hit.collider != null && hit.collider == c) Gizmos.color = Color.black;
                Gizmos.DrawCube(c.bounds.center, vec); //Assume part collider is a BoxCollider
            }
        }
    }

}
