using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Deprecated
public enum NoiseType
{
    PERLIN = 0,
    SIMPLEX = 1,
    VORONOI = 2
}

[System.Serializable]
public class ShaderVariables
{
    public Material mat;
    public float _Height;
    public float _VoronoiHeight;
    public float _PerlinSize;
    public NoiseType _NoiseType;

    public void SetVariables()
    {
        mat.SetFloat("_Height", _Height);
        mat.SetFloat("_VoronoiHeight", _VoronoiHeight);
        mat.SetFloat("_PerlinSize", _PerlinSize);
        switch (_NoiseType)
        {
            case NoiseType.PERLIN:
                mat.EnableKeyword("_NOISETYPE_PERLIN");
                mat.DisableKeyword("_NOISETYPE_SIMPLEX");
                mat.DisableKeyword("_NOISETYPE_VORONOI");
                break;

            case NoiseType.SIMPLEX:
                mat.DisableKeyword("_NOISETYPE_PERLIN");
                mat.EnableKeyword("_NOISETYPE_SIMPLEX");
                mat.DisableKeyword("_NOISETYPE_VORONOI");
                break;

            case NoiseType.VORONOI:
                mat.DisableKeyword("_NOISETYPE_PERLIN");
                mat.DisableKeyword("_NOISETYPE_SIMPLEX");
                mat.EnableKeyword("_NOISETYPE_VORONOI");
                break;
        }
        
    }

}
#endregion

public class Tile
{
    public static GameObject plane;
    public static float scaleX, scaleY;

    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Vector3[] vertPositions;

    public GameObject thisTile;

    public Tile(Vector3 startPos, int spaceSize, int X, int Y)
    {
        GameObject p = GameObject.Instantiate(plane, new Vector3(startPos.x + X * spaceSize, 0, startPos.z + Y * spaceSize), Quaternion.identity);
        thisTile = p;

        meshFilter = p.GetComponent<MeshFilter>();
        meshCollider = p.GetComponent<MeshCollider>();

        vertPositions = meshFilter.mesh.vertices;
    }

    public void ChangeMesh()
    {
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

public class Chunk
{
    int iX, iY;
    public Tile[,] chunkGrid = new Tile[size, size];
    const int size = 7;

    
    public Chunk(Vector3 startPos, int spaceSize)
    {
        iX--;
        for (int X = -Mathf.CeilToInt(size / 2); X < Mathf.FloorToInt(size / 2); X++)
        {
            iX++;
            iY = 0;
            for (int Y = -Mathf.CeilToInt(size / 2); Y < Mathf.FloorToInt(size / 2); Y++)
            {
                Tile p = new Tile(startPos, spaceSize, X, Y);
                chunkGrid[iX, iY] = p;
                iY++;
            }
        }
    }

    public void DestroyChunk()
    {
        iX--;
        for (int X = -Mathf.CeilToInt(size / 2); X < Mathf.FloorToInt(size / 2); X++)
        {
            iX++;
            iY = 0;
            for (int Y = -Mathf.CeilToInt(size / 2); Y < Mathf.FloorToInt(size / 2); Y++)
            {
                GameObject.Destroy(chunkGrid[X, Y].thisTile);
                iY++;
            }
        }
    }

    
}

public class GroundGenerator : MonoBehaviour
{
    public float scaleX = 1f;
    public float scaleY = 1f;
    float prevScaleX, prevScaleY;

    public ShaderVariables vars;
    public bool isFlat;

    public GameObject plane;
    const int size = 7;
    public int spaceSize = 10;
    const float chunckSpaceSize = 85;
    const float tileScale = 100;

    Chunk[,] chuncks = new Chunk[3, 3];

    Vector3[,] sVec = { { new Vector3(-size * chunckSpaceSize, 0, size * chunckSpaceSize), new Vector3(0, 0, size * chunckSpaceSize), new Vector3(size * chunckSpaceSize, 0, size * chunckSpaceSize) }, { new Vector3(-size * chunckSpaceSize, 0, 0), Vector3.zero, new Vector3(size * chunckSpaceSize, 0, 0) }, { new Vector3(-size * chunckSpaceSize, 0, -size * chunckSpaceSize), new Vector3(0, 0, -size * chunckSpaceSize), new Vector3(size * chunckSpaceSize, 0, -size * chunckSpaceSize) } };

    Bounds chunckLimits;

    int offX, offZ;

    Vector3 ChunkSize = new Vector3(size * tileScale, 0, size * tileScale);

    void Start()
    {
        prevScaleX = scaleX;
        prevScaleY = scaleY;

        Tile.plane = plane.transform.GetChild(0).gameObject;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                chuncks[i, j] = CreateChunck(sVec[i, j]);
            }
        }

        if (!isFlat) ChangeMeshes();

        chunckLimits = new Bounds(Vector3.zero, ChunkSize);
    }

    public void ApplyVars()
    {
        vars.SetVariables();
    }

    void Update()
    {
        if (!isFlat) return;
        else if (prevScaleX != scaleX || prevScaleY != scaleY) ChangeMeshes();

        if (transform.position.x > chunckLimits.max.x) offX = 1;
        else if (transform.position.x < chunckLimits.min.x) offX = -1;

        if (transform.position.z > chunckLimits.max.z) offZ = 1;
        else if (transform.position.z < chunckLimits.min.z) offZ = -1;


        if (offX == 1)
        {
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x + size * tileScale, 0, chunckLimits.center.z), ChunkSize); 
            offX = 0;
            updateSVec(size * tileScale, 0);
            chuncks[0, 0].DestroyChunk();
            chuncks[1, 0].DestroyChunk();
            chuncks[2, 0].DestroyChunk();

            chuncks[0, 0] = chuncks[0, 1];
            chuncks[1, 0] = chuncks[1, 1];
            chuncks[2, 0] = chuncks[2, 1];

            chuncks[0, 1] = chuncks[0, 2];
            chuncks[1, 1] = chuncks[1, 2];
            chuncks[2, 1] = chuncks[2, 2];

            chuncks[0, 2] = CreateChunck(sVec[0, 2]);
            chuncks[1, 2] = CreateChunck(sVec[1, 2]);
            chuncks[2, 2] = CreateChunck(sVec[2, 2]);
        }
        else if (offX == -1) 
        { 
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x - size * tileScale, 0, chunckLimits.center.z), ChunkSize); 
            offX = 0;
            updateSVec(-size * tileScale, 0);
            chuncks[0, 2].DestroyChunk();
            chuncks[1, 2].DestroyChunk();
            chuncks[2, 2].DestroyChunk();

            chuncks[0, 2] = chuncks[0, 1];
            chuncks[1, 2] = chuncks[1, 1];
            chuncks[2, 2] = chuncks[2, 1];

            chuncks[0, 1] = chuncks[0, 0];
            chuncks[1, 1] = chuncks[1, 0];
            chuncks[2, 1] = chuncks[2, 0];

            chuncks[0, 0] = CreateChunck(sVec[0, 0]);
            chuncks[1, 0] = CreateChunck(sVec[1, 0]);
            chuncks[2, 0] = CreateChunck(sVec[2, 0]);
        }

        if (offZ == 1) 
        { 
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x, 0, chunckLimits.center.z + size * tileScale), ChunkSize); 
            offZ = 0;
            updateSVec(0, size * tileScale);
            chuncks[2, 0].DestroyChunk();
            chuncks[2, 1].DestroyChunk();
            chuncks[2, 2].DestroyChunk();

            chuncks[2, 0] = chuncks[1, 0];
            chuncks[2, 1] = chuncks[1, 1];
            chuncks[2, 2] = chuncks[1, 2];
            
            chuncks[1, 0] = chuncks[0, 0];
            chuncks[1, 1] = chuncks[0, 1];
            chuncks[1, 2] = chuncks[0, 2];

            chuncks[0, 0] = CreateChunck(sVec[0, 0]);
            chuncks[0, 1] = CreateChunck(sVec[0, 1]);
            chuncks[0, 2] = CreateChunck(sVec[0, 2]);
        }
        else if (offZ == -1) 
        { 
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x, 0, chunckLimits.center.z - size * tileScale), ChunkSize); 
            offZ = 0;
            updateSVec(0, -size * tileScale);
            chuncks[0, 0].DestroyChunk();
            chuncks[0, 1].DestroyChunk();
            chuncks[0, 2].DestroyChunk();

            chuncks[0, 0]  = chuncks[1, 0];
            chuncks[0, 1]  = chuncks[1, 1];
            chuncks[0, 2]  = chuncks[1, 2];

            chuncks[1, 0] = chuncks[2, 0];
            chuncks[1, 1] = chuncks[2, 1];
            chuncks[1, 2] = chuncks[2, 2];

            chuncks[2, 0] = CreateChunck(sVec[2, 0]);
            chuncks[2, 1] = CreateChunck(sVec[2, 1]);
            chuncks[2, 2] = CreateChunck(sVec[2, 2]);
        }
    }

    void updateSVec(float xVal, float zVal)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3 vec = sVec[i, j];
                sVec[i, j] = new Vector3(vec.x + xVal, 0, vec.z + zVal);
            }
        }
    }

    Chunk CreateChunck(Vector3 startPos)
    {
        Chunk chunk = new Chunk(startPos, spaceSize);
        return chunk;
    }

    void ChangeMeshes()
    {
        prevScaleX = scaleX;
        prevScaleY = scaleY;
        Tile.scaleX = scaleX;
        Tile.scaleY = scaleY;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Chunk c = chuncks[i, j];
                int iX = 0;
                int iY = 0;
                iX--;
                for (int X = -Mathf.CeilToInt(size / 2); X < Mathf.FloorToInt(size / 2); X++)
                {
                    iX++;
                    iY = 0;
                    for (int Y = -Mathf.CeilToInt(size / 2); Y < Mathf.FloorToInt(size / 2); Y++)
                    {
                        c.chunkGrid[iX, iY].ChangeMesh();
                        iY++;
                    }
                }
            }
        }
    }
}
