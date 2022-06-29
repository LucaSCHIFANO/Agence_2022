using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class UpdateMesh : MonoBehaviour
{
    public float noiseScaleX = 1f;
    public float noiseScaleY = 1f;
    float prevScaleX, prevScaleY;

    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Vector3[] vertPositions;

    bool calcul = false;
    void Start()
    {
        prevScaleX = noiseScaleX;
        prevScaleY = noiseScaleY;
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();

        vertPositions = meshFilter.mesh.vertices;

        ChangeMesh();
    }
    /*
    void Update()
    {
        if (prevScaleX != scaleX || prevScaleY != scaleY)
        {
            ChangeMesh();
        }
    }
    */
    public int div = 200;
    public float ScaleX = 1.0f;
    public float ScaleY = 1.0f;
    public float ScaleZ = 1.0f;
    public bool RecalculateNormals = false;
    private Vector3[] _baseVertices;
    public void Update()
    {
        if (prevScaleX != noiseScaleX || prevScaleY != noiseScaleY)
        {
            ChangeMesh();
        }

        
    }

    void ChangeMesh()
    {
        prevScaleX = noiseScaleX;
        prevScaleY = noiseScaleY;

        float invertscaleX = 1 - (noiseScaleX - 1);
        float invertscaleY = 1 - (noiseScaleY - 1);


        Vector3[] vertices = meshFilter.mesh.vertices;
        Vector3[] normals = meshFilter.mesh.normals;

        
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = vertPositions[i].y + (Mathf.PerlinNoise((vertPositions[i].x * noiseScaleX), (vertPositions[i].z * noiseScaleY))) - (Mathf.PerlinNoise(Time.deltaTime + (vertPositions[i].x * invertscaleX), Time.deltaTime + (vertPositions[i].y * invertscaleY)));
        }
        

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = meshFilter.mesh;
        
    }

    private void OnDrawGizmos()
    {
        return;
        if (!calcul) return;
        var mesh = GetComponent<MeshFilter>().mesh;
        
        if (!Application.isPlaying) return;
        
        for (int i = 0; i < mesh.vertices.Length / div; i++)
        {
            float val = Vector3.Dot(mesh.normals[i * div], Vector3.up);
            Gizmos.color = new Color(val, val, val, 1);
            Gizmos.DrawLine(mesh.vertices[i * div], mesh.vertices[i * div] + mesh.normals[i * div] * 10/*meshFilter.mesh.normals[i]*/);
        }
        calcul = false;
    }

}
