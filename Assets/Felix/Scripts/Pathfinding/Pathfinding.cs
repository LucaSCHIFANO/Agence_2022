using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        private Grid grid;

        private void Awake()
        {
            grid = GetComponent<Grid>();
        }

        public void FindPath(PathRequest _request, Action<PathResult> _callback, bool _isAStar)
        {
            if (_isAStar)
            {
                FindPathAStar(_request, _callback);
            }
            else
            {
                FindPathJPS(_request, _callback);
            }
        }
        
        public void FindPathAStar(PathRequest _request, Action<PathResult> _callback)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Vector3[] wayPoints = Array.Empty<Vector3>();
            bool pathSuccess = false;
            
            Node startNode = grid.NodeFromPoint(_request.startPoint);
            Node targetNode = grid.NodeFromPoint(_request.endPoint);

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
                        if (neighbour.isObstructed || closeSet.Contains(neighbour))
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
                wayPoints = RetracePath(startNode, targetNode, true);
                pathSuccess = wayPoints.Length > 0;
            }

            stopwatch.Stop();
            print("A Star" + (pathSuccess ? "succeed, " : "failed, ") + "time: " + stopwatch.ElapsedMilliseconds + "ms");

            _callback(new PathResult(wayPoints, pathSuccess, _request.callback));
        }

        public void FindPathJPS(PathRequest _request, Action<PathResult> _callback)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            Vector3[] wayPoints = Array.Empty<Vector3>();
            bool pathSuccess = false;
            
            Node startNode = grid.NodeFromPoint(_request.startPoint);
            Node targetNode = grid.NodeFromPoint(_request.endPoint);
            
            if (!startNode.isObstructed && !targetNode.isObstructed)
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

                    List<Node> neighbours = grid.PruneNeighbours(currentNode, targetNode);

                    foreach (Node neighbour in neighbours)
                    {
                        if (neighbour.isObstructed || closeSet.Contains(neighbour))
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

            if (pathSuccess)
            {
                wayPoints = RetracePath(startNode, targetNode, false);
                pathSuccess = wayPoints.Length > 0;
            }
            
            stopwatch.Stop();
            print("JPS " + (pathSuccess ? "succeed, " : "failed, ") + "time: " + stopwatch.ElapsedMilliseconds + "ms");
            
            _callback(new PathResult(wayPoints, pathSuccess, _request.callback));
        }

        
        
        private Vector3[] RetracePath(Node _startNode, Node _targetNode, bool _isAStar)
        {
            List<Node> path = new List<Node>();

            Node currentNode = _targetNode;

            while (currentNode != _startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            
            Vector3[] waypoints;

            if (_isAStar)
            {
                waypoints = SimplifyPath(path);
            }
            else
            {
                waypoints = new Vector3[path.Count];
                for (int i = 0; i < waypoints.Length; i++)
                {
                    waypoints[i] = path[i].position;
                }
            }
            
            Array.Reverse(waypoints);
            return waypoints;
        }

        private Vector3[] SimplifyPath(List<Node> _path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            
            Vector2 oldDirection = Vector2.zero;

            for (int i = 1; i < _path.Count; i++)
            {
                Vector2 newDirection = new Vector3(_path[i - 1].gridX - _path[i].gridX, _path[i - 1].gridZ - _path[i].gridZ);

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
            int distZ = Mathf.Abs(_a.gridZ - _b.gridZ);

            if (distX > distZ)
                return 14 * distZ + 10 * (distX - distZ);

            return 14 * distX + 10 * (distZ - distX);
        }
    }
}