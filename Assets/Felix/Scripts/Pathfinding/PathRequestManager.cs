using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        private static PathRequestManager instance;

        private Pathfinding pathfinding;
    
        private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        private PathRequest currentPathRequest;

        private bool isProcessingPath;

        private void Awake()
        {
            instance = this;
            pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Vector3 _startPoint, Transform _startTransform, Vector3 _endPoint, Action<Vector3[], bool> _callback)
        {
            PathRequest newRequest = new PathRequest(_startPoint, _startTransform, _endPoint, _callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;

                pathfinding.StartFindPath(currentPathRequest.startPoint, currentPathRequest.startTransform, currentPathRequest.endPoint);
            }
        }

        public void FinishedProcessingPath(Vector3[] _path, bool _success)
        {
            if (currentPathRequest.callback != null && currentPathRequest.startTransform != null)
                currentPathRequest.callback(_path, _success);
            isProcessingPath = false;
            TryProcessNext();
        }
        
        private struct PathRequest
        {
            public Vector3 startPoint;
            public Transform startTransform;
            public Vector3 endPoint;
            public Action<Vector3[], bool> callback;

            public PathRequest(Vector3 _startPoint, Transform _startTransform, Vector3 _endPoint, Action<Vector3[], bool> _callback)
            {
                startPoint = _startPoint;
                startTransform = _startTransform;
                endPoint = _endPoint;
                callback = _callback;
            }
        }
    }   
}
