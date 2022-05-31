using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

public class Asker : MonoBehaviour
{
    private Path path;

    private Action<Vector3[]> callback;

    private float speed;
    [SerializeField] private float turnDistance;
    [SerializeField] private float turnSpeed;

    public void AskNewPath(Transform _target, float _speed, Action<Vector3[]> _callback)
    {
        speed = _speed;
        callback = _callback;
        PathRequestManager.RequestPath(transform.position, transform, _target.position, OnPathFound);
    }
    
    public void AskNewPath(Vector3 _targetPosition, float _speed, Action<Vector3[]> _callback)
    {
        speed = _speed;
        callback = _callback;
        PathRequestManager.RequestPath(transform.position, transform, _targetPosition, OnPathFound);
    }

    public void OnPathFound(Vector3[] _newPath, bool _pathSuccess)
    {
        if (_pathSuccess)
        {
            path = new Path(_newPath, transform.position, turnDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");

            callback?.Invoke(_newPath);
        }
    }

    private IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
                
                
            }
            
            yield return null;
        }
        
        /*if (path.Length <= 0)
            yield break;

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
        }*/
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
