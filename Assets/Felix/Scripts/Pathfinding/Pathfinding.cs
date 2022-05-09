using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        private Grid grid;
        private PathRequestManager pathRequestManager;
        
        private void Awake()
        {
            grid = GetComponent<Grid>();
            pathRequestManager = GetComponent<PathRequestManager>();
        }

        public void StartFindPath(Vector3 _startPosition, Transform _startTransform, Vector3 _targetPosition)
        {
            StartCoroutine(FindPath(_startPosition, _startTransform, _targetPosition));
        }
        
        private IEnumerator FindPath(Vector3 _startPosition, Transform _startTransform, Vector3 _targetPosition)
        {
            Vector3[] wayPoints = new Vector3[0];
            bool pathSuccess = false;
            
            // TO OPTIMIZE
            // the order we test the nodes
            // front nodes in first, ect...
            
            Node startNode = grid.NodeFromPoint(_startPosition);
            Node targetNode = grid.NodeFromPoint(_targetPosition);

            Node[] optiNodes = grid.OptimizedNodesFromTransform(targetNode.position, _startTransform);

            bool isTargetNodeObstructed = false;
            
            foreach (Node node in optiNodes)
            {
                if (node.isObstructed)
                {
                    isTargetNodeObstructed = true;
                    break;
                }
            }
            
            if (!startNode.isObstructed && !targetNode.isObstructed && !isTargetNodeObstructed)
            {
                Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
                HashSet<Node> closeSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();

                    closeSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;
                        break; // Good path
                    }

                    foreach (Node neighbour in grid.GetNeighbourNodes(currentNode))
                    {
                        if (neighbour.isObstructed || closeSet.Contains(neighbour))
                            continue;

                        optiNodes = grid.OptimizedNodesFromTransform(neighbour.position, _startTransform);

                        bool isNodeObstructed = false;
                        
                        foreach (Node node in optiNodes)
                        {
                            if (node.isObstructed || closeSet.Contains(neighbour))
                            {
                                isNodeObstructed = true;
                                break;
                            }
                        }
                        
                        if (isNodeObstructed)
                            continue;
                        
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);

                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

            yield return null;

            if (pathSuccess)
            {
                wayPoints = RetracePath(startNode, targetNode);
            }

            pathRequestManager.FinishedProcessingPath(wayPoints, pathSuccess);
            
        }

        private Vector3[] RetracePath(Node _startNode, Node _targetNode)
        {
            List<Node> path = new List<Node>();

            Node currentNode = _targetNode;

            while (currentNode != _startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            Vector3[] waypoints = SimplifyPath(path);
            
            Array.Reverse(waypoints);
            return waypoints;
        }

        private Vector3[] SimplifyPath(List<Node> _path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            
            Vector2 oldDirection = Vector2.zero;

            for (int i = 1; i < _path.Count; i++)
            {
                Vector2 newDirection = new Vector2(_path[i - 1].gridX - _path[i].gridX, _path[i - 1].gridY - _path[i].gridY);

                if (newDirection != oldDirection)
                {
                    waypoints.Add(_path[i].position);
                }

                oldDirection = newDirection;
            }

            return waypoints.ToArray();
        }
        
        private int GetDistance(Node _a, Node _b)
        {
            int distX = Mathf.Abs(_a.gridX - _b.gridX);
            int distY = Mathf.Abs(_a.gridY - _b.gridY);

            if (distX > distY)
            {
                return 14 * distY + 10 * (distX - distY);
            }
            else
            {
                return 14 * distX + 10 * (distY - distX);
            }
        }
    }
}