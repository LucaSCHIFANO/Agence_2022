using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    int iX, iY;
    GameObject[,] chunkGrid = new GameObject[7, 7];

    public Chunk(Vector3 startPos, GameObject plane)
    {
        iX--;
        for (int X = -3; X < 4; X++)
        {
            iX++;
            iY = 0;
            for (int Y = -3; Y < 4; Y++)
            {
                GameObject p = GameObject.Instantiate(plane, new Vector3(startPos.x + X * 10, 0, startPos.z + Y * 10), Quaternion.identity);
                chunkGrid[iX, iY] = p;
                iY++;
            }
        }
    }

    public void DestroyChunk()
    {
        for (int X = 0; X < 7; X++)
        {
            for (int Y = 0; Y < 7; Y++)
            {
                GameObject.Destroy(chunkGrid[X, Y]);
            }
        }
    }
}


public class GroundGenerator : MonoBehaviour
{
    public GameObject plane;

    Chunk[,] chuncks = new Chunk[3, 3];

    Vector3[,] sVec = { { new Vector3(-70, 0, 70), new Vector3(0, 0, 70), new Vector3(70, 0, 70) }, { new Vector3(-70, 0, 0), Vector3.zero, new Vector3(70, 0, 0) }, { new Vector3(-70, 0, -70), new Vector3(0, 0, -70), new Vector3(70, 0, -70) } };

    Bounds chunckLimits;

    int offX, offZ;

    Vector3 ChunkSize = new Vector3(70, 0, 70);

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                chuncks[i, j] = CreateChunck(sVec[i, j]);
            }
        }

        chunckLimits = new Bounds(Vector3.zero, ChunkSize);
    }


    void Update()
    {
        if (transform.position.x > chunckLimits.max.x) offX = 1;
        else if (transform.position.x < chunckLimits.min.x) offX = -1;

        if (transform.position.z > chunckLimits.max.z) offZ = 1;
        else if (transform.position.z < chunckLimits.min.z) offZ = -1;


        if (offX == 1)
        {
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x + 70, 0, chunckLimits.center.z), ChunkSize); 
            offX = 0;
            updateSVec(70, 0);
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
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x - 70, 0, chunckLimits.center.z), ChunkSize); 
            offX = 0;
            updateSVec(-70, 0);
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
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x, 0, chunckLimits.center.z + 70), ChunkSize); 
            offZ = 0;
            updateSVec(0, 70);
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
            chunckLimits = new Bounds(new Vector3(chunckLimits.center.x, 0, chunckLimits.center.z - 70), ChunkSize); 
            offZ = 0;
            updateSVec(0, -70);
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
        Chunk chunk = new Chunk(startPos, plane);
        return chunk;
    }
}
