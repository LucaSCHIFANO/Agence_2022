using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

public class Asker : MonoBehaviour
{
    public Path path;
    public bool pathEnd { get; private set; }

    private Action<Vector3[]> callback;

    private float speed;
    [SerializeField] private float turnDistance;
    [SerializeField] private float turnSpeed;

    private void Start()
    {
        pathEnd = true;
    }

    public void AskNewPath(Transform _target, float _speed, Action<Vector3[]> _callback)
    {
        speed = _speed;
        callback = _callback;
        PathRequestManager.RequestPath(new PathRequest(transform.position, _target.position, OnPathFound));
    }
    
    public void AskNewPath(Vector3 _targetPosition, float _speed, Action<Vector3[]> _callback)
    {
        speed = _speed;
        callback = _callback;
        PathRequestManager.RequestPath(new PathRequest(transform.position, _targetPosition, OnPathFound));
    }

    public void AskNewPathAtEnd(Vector3 _targetPosition)
    {
        if (path == null) return;
        
        PathRequestManager.RequestPath(new PathRequest(path.lookPoints[^1], _targetPosition, OnPathFoundAtEnd));
    }

    public void OnPathFoundAtEnd(Vector3[] _path, bool _pathSuccess)
    {
        if (_pathSuccess)
        {
            StopCoroutine("FollowPath");
            Vector3[] newPathPoints = new Vector3[_path.Length + path.lookPoints.Length];

            for (int i = 0; i < newPathPoints.Length; i++)
            {
                if (i < path.lookPoints.Length)
                {
                    newPathPoints[i] = path.lookPoints[i];
                }
                else
                {
                    newPathPoints[i] = _path[i - path.lookPoints.Length];
                }
            }

            path = new Path(newPathPoints, path.startPosition, turnDistance, speed);
            StartCoroutine("FollowPath");
        }
    }

    public void OnPathFound(Vector3[] _newPath, bool _pathSuccess)
    {
        if (_pathSuccess)
        {
            path = new Path(_newPath, transform.position, turnDistance, speed);
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
        pathEnd = false;

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

        pathEnd = true;
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
