using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestMesh : MonoBehaviour
{
    public int currVertice;
    public float sphereSize = 2.0f;
    MeshFilter meshFilter;
    Mesh mesh;

    [Range(1, 200)]
    public int resolution = 10;

    private int currentResolution;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        Vector3[] newV = new Vector3[mesh.vertexCount + 5];
        int[] newT = new int[mesh.triangles.Length + 24];
        mesh.vertices.CopyTo(newV, 0);

        //24 TOP
        Vector3 vertA = newV[3];
        Vector3 vertB = newV[5];
        Vector3 vertC = new Vector3(-0.50f, 0.50f, 0.0f/*vertA.x, vertA.y, (vertA.z + vertB.z) / 2*/);
        newV[mesh.vertexCount - 5] = vertC;

        //25 BOTTOM
        vertA = newV[1];
        vertB = newV[7];
        vertC = new Vector3(vertA.x, vertA.y, (vertA.z + vertB.z) / 2);
        newV[mesh.vertexCount - 4] = vertC;

        //26 LEFT
        vertA = newV[3];
        vertB = newV[1];
        vertC = new Vector3(vertA.x, vertA.y, (vertA.z + vertB.z) / 2);
        newV[mesh.vertexCount - 3] = vertC;

        //27 RIGHT
        vertA = newV[5];
        vertB = newV[7];
        vertC = new Vector3(vertA.x, vertA.y, (vertA.z + vertB.z) / 2);
        newV[mesh.vertexCount - 2] = vertC;

        //28 MIDDLE
        vertA = newV[26];
        vertB = newV[27];
        vertC = new Vector3(vertA.x, vertA.y, (vertA.z + vertB.z) / 2);
        newV[mesh.vertexCount - 1] = vertC;

        
        mesh.vertices = newV;

        int[] tris = new int[]
        {
            //TL
            //B
            26, 28, 3,
            //T
            3, 24, 28,

            //TR
            //B
            24, 28, 27,
            //T
            24, 5, 27,

            //BL
            //B
            26, 1, 25,
            //T
            26, 28, 25,

            //BR
            //B
            28, 25, 7,
            //T
            28, 27, 7,
        };

        tris.CopyTo(newT, newT.Length - 24);
        mesh.triangles.CopyTo(newT, 0);
        //mesh.triangles = newT;
        /*
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Debug.Log(mesh.triangles[i] + ", " + mesh.triangles[i + 1] + ", " + mesh.triangles[i + 2]);
        }
        */
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Debug.Log(i + " => " + mesh.vertices[i]);
        }
    }


    void Update()
    {
        if (currVertice > meshFilter.mesh.vertexCount) currVertice = meshFilter.mesh.vertexCount - 1;
        else if (currVertice < 0) currVertice = 0;

        if (resolution != currentResolution)
        {
            //CreateGrid();
        }
        
    }

    private void CreateGrid()
    {
        currentResolution = resolution;
        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        float stepSize = 1f / resolution;
        for (int v = 0, y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++, v++)
            {
                vertices[v] = new Vector3(x * stepSize - 0.5f, y * stepSize - 0.5f);
            }
        }
        mesh.vertices = vertices;
        int[] triangles = new int[resolution * resolution * 6];
        for (int t = 0, v = 0, y = 0; y < resolution; y++, v++)
        {
            for (int x = 0; x < resolution; x++, v++, t += 6)
            {
                triangles[t] = v;
                triangles[t + 1] = v + resolution + 1;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + resolution + 1;
                triangles[t + 5] = v + resolution + 2;
            }
        }
        mesh.triangles = triangles;
    }


    private void OnDrawGizmos()
    {
        //Vector3 vec = transform.TransformPoint(meshFilter.mesh.vertices[currVertice]);/*transform.position + meshFilter.sharedMesh.vertices[currVertice];
        //Gizmos.DrawSphere(vec, sphereSize);
    }
}
