using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Grid : MonoBehaviour
    {
        private Node[,,] grid;

        private float nodeDiameter;
        private int gridSizeX, gridSizeY, gridSizeZ;

        public int MaxSize => gridSizeX * gridSizeY * gridSizeZ;

        public bool displayGrid = true;
        public bool displayObstructed = false;
        [Space]
        public Vector3 gridSize;
        public float nodeRadius;

        public LayerMask obstructedMask;

        private void Awake()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
            gridSizeZ = Mathf.RoundToInt(gridSize.z / nodeDiameter);

            grid = CreateGrid();
        }

        private Node[,,] CreateGrid()
        {
            Node[,,] newGrid = new Node[gridSizeX, gridSizeY, gridSizeZ];
            Vector3 bottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSizeY / 2 - Vector3.forward * gridSize.z / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    for (int y = 0; y < gridSizeY; y++)
                    {
                        Vector3 point = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                        bool isObstructed = Physics.CheckSphere(point, nodeRadius, obstructedMask);

                        bool isWalkable = !isObstructed && y - 1 >= 0 && newGrid[x, y - 1, z].isObstructed;

                        if (y == 0 && !isObstructed)
                            isWalkable = true;
                        
                        bool toShowGui = true; // TODO

                        newGrid[x, y, z] = new Node(isObstructed, isWalkable, point, x, y, z, toShowGui);
                    }
                }
            }

            return newGrid;
        }
        
        public List<Node> GetNeighbourNodes(Node _node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            continue;

                        int checkX = _node.gridX + x;
                        int checkY = _node.gridY + y;
                        int checkZ = _node.gridZ + z;

                        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                        {
                            neighbours.Add(grid[checkX, checkY, checkZ]);
                        }
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromPoint(Vector3 _position)
        {
            float xPercentage = (_position.x + gridSize.x / 2 - transform.position.x) / gridSize.x;
            float yPercentage = (_position.y + gridSize.y / 2 - transform.position.y) / gridSize.y;
            float zPercentage = (_position.z + gridSize.z / 2 - transform.position.z) / gridSize.z;
            xPercentage = Mathf.Clamp01(xPercentage);
            yPercentage = Mathf.Clamp01(yPercentage);
            zPercentage = Mathf.Clamp01(zPercentage);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPercentage);
            int y = Mathf.RoundToInt((gridSizeY - 1) * yPercentage);
            int z = Mathf.RoundToInt((gridSizeZ - 1) * zPercentage);

            return grid[x, y, z];
        }

        public Node WalkableNodeFromPoint(Vector3 _position)
        {
            float xPercentage = (_position.x + gridSize.x / 2 - transform.position.x) / gridSize.x;
            float yPercentage = (_position.y + gridSize.y / 2 - transform.position.y) / gridSize.y;
            float zPercentage = (_position.z + gridSize.z / 2 - transform.position.z) / gridSize.z;
            xPercentage = Mathf.Clamp01(xPercentage);
            yPercentage = Mathf.Clamp01(yPercentage);
            zPercentage = Mathf.Clamp01(zPercentage);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPercentage);
            int y = Mathf.RoundToInt((gridSizeY - 1) * yPercentage);
            int z = Mathf.RoundToInt((gridSizeZ - 1) * zPercentage);

            Node rNode = grid[x, y, z];

            if (rNode.isObstructed)
            {
                for (int i = y+1; i < gridSizeY; i++)
                {
                    rNode = grid[x, i, z];

                    if (rNode.isWalkable)
                        break;
                }
            }
            else if (!rNode.isWalkable)
            {
                for (int i = y-1; i >= 0; i--)
                {
                    rNode = grid[x, i, z];

                    if (rNode.isWalkable)
                        break;
                }
            }
            
            return rNode;
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
            int nbY = Mathf.RoundToInt(_object.localScale.y / nodeRadius / 2 + 0.5f);
            int nbZ = Mathf.RoundToInt(_object.localScale.z / nodeRadius / 2 + 0.5f);

            for (int y = -nbY; y <= nbY; y+=nbY*2)
            {
                for (int x = -nbX; x <= nbX; x+=nbX*2)
                {
                    for (int z = -nbZ; z <= nbZ; z++)
                    {
                    
                        //Node node = NodeFromPoint(new Vector3(_position.x + i * _object.localScale.x / (nbX * 2), _position.y, _position.z + w * _object.localScale.z / (nbZ * 2)));
                        Node node = NodeFromPoint(_position + _object.right * (x * _object.localScale.x / (nbX * 2)) +
                                                  _object.up * (y * _object.localScale.y / (nbY * 2)) +
                                                  _object.forward * (z * _object.localScale.z / (nbZ * 2)));

                        if (!nodes.Contains(node))
                            nodes.Add(node);
                    }
                }
            }

            for (int y = -nbY; y <= nbY; y+=nbY*2)
            {
                for (int z = -nbZ; z <= nbZ; z+=nbZ*2)
                {
                    for (int x = -nbX; x <= nbX; x++)
                    {
                    
                        //Node node = NodeFromPoint(new Vector3(_position.x + w * _object.localScale.x / (nbX * 2), _position.y, _position.z + i * _object.localScale.z / (nbZ * 2)));
                        Node node = NodeFromPoint(_position + _object.right * (x * _object.localScale.x / (nbX * 2)) + 
                                                  _object.up * (y * _object.localScale.y / (nbY * 2)) +
                                                  _object.forward * (z * _object.localScale.z / (nbZ * 2)));
                        
                        if (!nodes.Contains(node))
                            nodes.Add(node);
                    }
                }
            }

            return nodes.ToArray();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.black;
                
                Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, gridSize.z));
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 0f, 0.5f);
                Gizmos.DrawCube(transform.position, new Vector3(gridSize.x, gridSize.y, gridSize.z));
            }
            
            if (grid != null && displayGrid)
            {
                foreach (Node n in grid)
                {
                    if (!n.toShowGui)
                        continue;
                        
                    if (n.isObstructed && displayObstructed)
                        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    else if (n.isObstructed && !displayObstructed)
                        continue;
                    else if (n.isWalkable)
                        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                    else
                        continue;

                    Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }
}
