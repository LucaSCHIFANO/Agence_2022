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
    }

    [SerializeField] LayerMask partlayer;

    [SerializeField] bool showGizmos;
    [Range(0.0f, 1.0f)]
    [SerializeField] float allPartsColorAlpha;
    [SerializeField] List<Part> parts;


    RaycastHit hit;

    void Start()
    {
        
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Mathf.Infinity, partlayer);
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
