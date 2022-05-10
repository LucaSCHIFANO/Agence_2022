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

        public int MaxSize => gridSizeX * gridSizeZ;

        public bool displayGrid = true;
        [Space]
        public Vector2 gridSize;
        public float nodeRadius;

        public LayerMask obstructedMask;

        private void Awake()
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

        public Node[] OptimizedNodesFromTransform(Transform _object)
        {
            List<Node> nodes = new List<Node>();

            // TODO:
            // Add rotation to the scale
            
            // Add 0.5f to always round to upper int value
            int nbX = Mathf.RoundToInt(_object.localScale.x / nodeRadius / 2 + 0.5f);
            int nbZ = Mathf.RoundToInt(_object.localScale.z / nodeRadius / 2 + 0.5f);

            for (int i = -nbX; i <= nbX; i+=nbX*2)
            {
                for (int w = -nbZ; w <= nbZ; w++)
                {
                    Node node = NodeFromPoint(new Vector3(_object.position.x + i * _object.localScale.x / (nbX * 2), _object.position.y, _object.position.z + w * _object.localScale.z / (nbZ * 2)));
                    
                    if (!nodes.Contains(node))
                        nodes.Add(node);
                }
            }
            
            for (int i = -nbZ; i <= nbZ; i+=nbZ*2)
            {
                for (int w = -nbX; w <= nbX; w++)
                {
                    Node node = NodeFromPoint(new Vector3(_object.position.x + w * _object.localScale.x / (nbX * 2), _object.position.y, _object.position.z + i * _object.localScale.z / (nbZ * 2)));
                    
                    if (!nodes.Contains(node))
                        nodes.Add(node);
                }
            }

            return nodes.ToArray();
        }
        
        public Node[] OptimizedNodesFromTransform(Vector3 _position, Transform _object)
        {
            List<Node> nodes = new List<Node>();

            int nbX = Mathf.RoundToInt(_object.localScale.x / nodeRadius / 2 + 0.5f);
            int nbZ = Mathf.RoundToInt(_object.localScale.z / nodeRadius / 2 + 0.5f);

            for (int i = -nbX; i <= nbX; i+=nbX*2)
            {
                for (int w = -nbZ; w <= nbZ; w++)
                {
                    //Node node = NodeFromPoint(new Vector3(_position.x + i * _object.localScale.x / (nbX * 2), _position.y, _position.z + w * _object.localScale.z / (nbZ * 2)));
                    Node node = NodeFromPoint(_position + _object.right * (i * _object.localScale.x / (nbX * 2)) + _object.forward * (w * _object.localScale.z / (nbZ * 2)));
                    
                    if (!nodes.Contains(node))
                        nodes.Add(node);
                }
            }
            
            for (int i = -nbZ; i <= nbZ; i+=nbZ*2)
            {
                for (int w = -nbX; w <= nbX; w++)
                {
                    //Node node = NodeFromPoint(new Vector3(_position.x + w * _object.localScale.x / (nbX * 2), _position.y, _position.z + i * _object.localScale.z / (nbZ * 2)));
                    Node node = NodeFromPoint(_position + _object.right * (w * _object.localScale.x / (nbX * 2)) + _object.forward * (i * _object.localScale.z / (nbZ * 2)));
                    
                    if (!nodes.Contains(node))
                        nodes.Add(node);
                }
            }

            return nodes.ToArray();
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
            
            if (grid != null && displayGrid)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = n.isObstructed ? new Color(1f, 0f, 0f, 0.5f) : new Color(0f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
            
        }
    }
}
