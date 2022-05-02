using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        private Grid grid;

        public Transform startPoint, endPoint;
        
        private void Start()
        {
            grid = GetComponent<Grid>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                FindPath(startPoint.position, endPoint.position);
        }

        private void FindPath(Vector3 _startPosition, Vector3 _targetPosition)
        {
            Node startNode = grid.NodeFromPoint(_startPosition);
            Node targetNode = grid.NodeFromPoint(_targetPosition);

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();
            
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                
                closeSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return; // Good path
                }

                foreach (Node neighbour in grid.GetNeighbourNodes(currentNode))
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
                    }
                }
            }
        }

        private void RetracePath(Node _startNode, Node _targetNode)
        {
            List<Node> path = new List<Node>();

            Node currentNode = _targetNode;

            while (currentNode != _startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            
            path.Reverse();

            grid.path = path;
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