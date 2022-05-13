using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdateMesh : MonoBehaviour
{
    public float scaleX = 1f;
    public float scaleY = 1f;
    float prevScaleX, prevScaleY;

    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Vector3[] vertPositions;
    void Start()
    {
        prevScaleX = scaleX;
        prevScaleY = scaleY;
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        vertPositions = meshFilter.mesh.vertices;

        ChangeMesh();
    }

    void Update()
    {
        if (prevScaleX != scaleX || prevScaleY != scaleY)
        {
            ChangeMesh();
        }
    }

    private void FixedUpdate()
    {
        
        
    }

    void ChangeMesh()
    {
        prevScaleX = scaleX;
        prevScaleY = scaleY;

        float invertscaleX = 1 - (scaleX - 1);
        float invertscaleY = 1 - (scaleY - 1);


        Vector3[] vertices = meshFilter.mesh.vertices;
        Vector3[] normals = meshFilter.mesh.normals;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = vertPositions[i].y + (Mathf.PerlinNoise((vertPositions[i].x * scaleX), (vertPositions[i].z * scaleY))) - (Mathf.PerlinNoise(Time.deltaTime + (vertPositions[i].x * invertscaleX), Time.deltaTime + (vertPositions[i].y * invertscaleY)));
        }


        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = meshFilter.mesh;
    }
}
