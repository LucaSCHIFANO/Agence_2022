using System.Collections;
using Pathfinding;
using UnityEngine;
using Unity.Netcode;

public class Asker : MonoBehaviour
{
    private Path path;

    private float speed;
    [SerializeField] private float turnDistance;

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
            path = new Path(_newPath, transform.position, turnDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {


        while (true)
        {
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
