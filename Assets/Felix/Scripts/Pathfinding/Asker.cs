using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Grid = Pathfinding.Grid;

public class Asker : MonoBehaviour
{
    private Grid grid;

    private Vector3[] path;
    private int targetIndex;

    private float speed;
    
    private void Start()
    { 
        grid = FindObjectOfType<Grid>();
    }

    public void AskNewPath(Transform _target, float _speed)
    {
        speed = _speed;
        PathRequestManager.RequestPath(transform.position, transform, _target.position, OnPathFound);
    }
    
    public void AskNewPath(Vector3 _targetPosition, float _speed)
    {
        speed = _speed;
        PathRequestManager.RequestPath(transform.position, transform, _targetPosition, OnPathFound);
    }

    public void OnPathFound(Vector3[] _newPath, bool _pathSuccess)
    {
        if (_pathSuccess)
        {
            path = _newPath;
            targetIndex = 0;
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
                Gizmos.color = Color.black;
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
    }
}
