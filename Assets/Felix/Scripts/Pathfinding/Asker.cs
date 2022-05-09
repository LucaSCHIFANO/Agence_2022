using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Grid = Pathfinding.Grid;

public class Asker : MonoBehaviour
{
    // TEMP
    private Grid gridTemp;
    
    private Node[] objectNodes;
    
    private Vector3[] path;
    private int targetIndex;
    
    public Transform target;

    public float speed;

    private void Start()
    {
        //PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        gridTemp = FindObjectOfType<Grid>();
        objectNodes = gridTemp.OptimizedNodesFromTransform(transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            objectNodes = gridTemp.OptimizedNodesFromTransform(transform);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PathRequestManager.RequestPath(transform.position, transform, target.position, OnPathFound);
        }
    }

    public void OnPathFound(Vector3[] _newPath, bool _pathSuccess)
    {
        if (_pathSuccess)
        {
            path = _newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;

                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }

        if (objectNodes != null)
        {
            for (int i = 0; i < objectNodes.Length; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(objectNodes[i].position, Vector3.one);
            }
        }
    }
}
