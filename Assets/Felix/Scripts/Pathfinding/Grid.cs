using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Grid : MonoBehaviour
    {
        private Node[,] grid;
        private float nodeDiameter;
        private int gridSizeX, gridSizeZ;
        
        public List<Node> path;

        public int MaxSize => gridSizeX * gridSizeZ;
        
        public bool onlyDisplayPath;
        [Space]
        public Vector2 gridSize;
        public float nodeRadius;

        public LayerMask obstructedMask;

        private void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
            gridSizeZ = Mathf.RoundToInt(gridSize.y / nodeDiameter);

            CreateGrid();
        }

        private void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeZ];
            Vector3 bottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 point = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    bool isObstructed = Physics.CheckSphere(point, nodeRadius, obstructedMask);

                    grid[x, z] = new Node(isObstructed, point, x, z);
                }
            }
        }

        public List<Node> GetNeighbourNodes(Node _node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = _node.gridX + x;
                    int checkY = _node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeZ)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromPoint(Vector3 _position)
        {
            float xPercentage = (_position.x + gridSize.x / 2) / gridSize.x;
            float zPercentage = (_position.z + gridSize.y / 2) / gridSize.y;
            xPercentage = Mathf.Clamp01(xPercentage);
            zPercentage = Mathf.Clamp01(zPercentage);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPercentage);
            int y = Mathf.RoundToInt((gridSizeZ - 1) * zPercentage);

            return grid[x, y];
        }
        
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.black;
                
                Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 0f, 0.5f);
                Gizmos.DrawCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
            }

            if (onlyDisplayPath)
            {
                if (path != null)
                {
                    foreach (Node n in path)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
                    }
                }
            }
            else
            {
                if (grid != null)
                {
                    foreach (Node n in grid)
                    {
                        Gizmos.color = n.isObstructed ? new Color(1f, 0f, 0f, 0.5f) : new Color(0f, 0f, 0f, 0.5f);
                    
                        if (path != null && path.Contains(n))
                            Gizmos.color = Color.cyan;
                    
                        Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
                    }
                }
            }
        }
    }
}
