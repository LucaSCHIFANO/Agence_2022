using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        private Grid grid;

        private void Awake()
        {
            grid = GetComponent<Grid>();
        }

        public void FindPath(PathRequest _request, Action<PathResult> _callback)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            Vector3[] wayPoints = new Vector3[0];
            bool pathSuccess = false;
            
            Node startNode = grid.NodeFromPoint(_request.startPoint);
            Node targetNode = grid.WalkableNodeFromPoint(_request.endPoint);

            /*Node[] optiNodes = grid.OptimizedNodesFromTransform(targetNode.position, _startTransform);

            bool isTargetNodeObstructed = false;
            
            foreach (Node node in optiNodes)
            {
                if (node.isObstructed)
                {
                    isTargetNodeObstructed = true;
                    break;
                }
            }*/
            
            if (!startNode.isObstructed && !targetNode.isObstructed/* && !isTargetNodeObstructed*/)
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
                        if (neighbour.isObstructed || !neighbour.isWalkable || closeSet.Contains(neighbour))
                            continue;

                        /*optiNodes = grid.OptimizedNodesFromTransform(neighbour.position, _startTransform);

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
                            continue;*/
                        
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

            if (pathSuccess)
            {
                wayPoints = RetracePath(startNode, targetNode);
                pathSuccess = wayPoints.Length > 0;
            }

            stopwatch.Stop();
            print(stopwatch.ElapsedMilliseconds);

            _callback(new PathResult(wayPoints, pathSuccess, _request.callback));
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
            
            Vector3 oldDirection = Vector3.zero;

            for (int i = 1; i < _path.Count; i++)
            {
                Vector3 newDirection = new Vector3(_path[i - 1].gridX - _path[i].gridX, _path[i - 1].gridY - _path[i].gridY, _path[i - 1].gridZ - _path[i].gridZ);

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
            int distZ = Mathf.Abs(_a.gridZ - _b.gridZ);

            if (distZ > distX && distZ > distY)
                return distZ / 2 + distY + distX;
            else if (distY > distX && distY > distZ)
                return distY / 2 + distZ + distX;
            else if (distX > distY && distY > distZ)
                return distX / 2 + distZ + distY;
            
            
            return distY + distZ + distX;

            /*if (distX > distZ)
            {
                if (distY > distX)
                {
                    return 14 * distZ + 14 * distX + 10 * (distY - distZ - distX);
                }
                else
                {
                    return 14 * distZ + 14 * distY + 10 * (distX - distY - distZ);
                }
            }
            else
            {
                if (distY > distZ)
                {
                    return 14 * distX + 14 * distZ + 10 * (distY - distZ - distX);
                }
                else
                {
                    return 14 * distX + 14 * distY + 10 * (distZ - distY - distZ);
                }
            }*/
        }
    }
}