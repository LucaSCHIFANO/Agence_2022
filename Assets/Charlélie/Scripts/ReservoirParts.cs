using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
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
        GameObject leakPrefab;
        GameObject leak;
        public void Init(GameObject leakObj)
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
                leak = Instantiate(leakPrefab, hitPoint.point, Quaternion.Euler(hitPoint.normal));
                leak.transform.forward = -hitPoint.normal;
                leak.transform.parent = hitGo.transform;
                leak.GetComponent<Leak>().Part = this;
                leakPos = hitPoint.point;
            }
        }

        public void Repair()
        {
            isLeaking = false;
            Destroy(leak);
        }
    }

    [SerializeField] LayerMask partlayer;
    [SerializeField] GameObject leak;


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

    void Start()
    {
        
    }


    void Update()
    {
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, partlayer);

        if (Input.GetMouseButtonDown(0)) HitReservoir(hit);
    }

    void HitReservoir(RaycastHit hit)
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
