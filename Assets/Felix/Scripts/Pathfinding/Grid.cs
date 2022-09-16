using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Grid : MonoBehaviour
    {
        private Node[,] grid;

        private Vector3 gridPosition;

        private float nodeDiameter;
        private int gridSizeX, gridSizeZ;

        public int MaxSize => gridSizeX * gridSizeZ;

        public bool displayGrid;
        public bool displayObstructed;
        [Space]
        public Vector3 gridSize;
        public float nodeRadius;

        public LayerMask obstructedMask;

        private void OnEnable()
        {
            gridPosition = transform.position;
        }
        
        private void Awake()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
            gridSizeZ = Mathf.RoundToInt(gridSize.z / nodeDiameter);

            gridPosition = transform.position;

            grid = CreateGrid();
        }

        private Node[,] CreateGrid()
        {
            Node[,] newGrid = new Node[gridSizeX, gridSizeZ];
            Vector3 bottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 point = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    bool isObstructed = Physics.CheckSphere(point, nodeRadius, obstructedMask);

                    bool toShowGui = true; // TODO

                    newGrid[x, z] = new Node(isObstructed, point, x, z, toShowGui);
                }
            }

            return newGrid;
        }

        public List<Node> GetNeighbourNodes(Node _node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0)
                        continue;

                    int checkX = _node.gridX + x;
                    int checkZ = _node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(grid[checkX, checkZ]);
                    }
                }
            }

            return neighbours;
        }

        public List<Node> PruneNeighbours(Node _currentNode, Node _destinationNode)
        {
            List<Node> returnNeighbours = new List<Node>();
            
            List<Node> neighbours = GetNeighbourNodes(_currentNode);

            foreach (Node neighbour in neighbours)
            {
                int x = Mathf.Clamp(neighbour.gridX - _currentNode.gridX, -1, 1);
                int y = Mathf.Clamp(neighbour.gridZ - _currentNode.gridZ, -1, 1);

                Node jumpPoint = Jump(_currentNode, x, y, _destinationNode);
                
                if (jumpPoint != null)
                    returnNeighbours.Add(jumpPoint);
            }

            return returnNeighbours;
        }

        private Node Jump(Node _currentNode, int _xDirection, int _yDirection, Node _destination)
        {
            int xJumpPosition = _currentNode.gridX + _xDirection;
            int yJumpPosition = _currentNode.gridZ + _yDirection;

            if (!IsWalkable(xJumpPosition, yJumpPosition))
                return null;

            Node jumpPoint = grid[xJumpPosition, yJumpPosition];

            if (jumpPoint == _destination)
                return jumpPoint;

            // Horizontals
            if (_xDirection != 0 && _yDirection == 0)
            {
                if (!IsWalkable(_currentNode.gridX, _currentNode.gridZ + 1) &&
                    IsWalkable(_currentNode.gridX + _xDirection, _currentNode.gridZ + 1))
                {
                    return jumpPoint;
                }
                else if (!IsWalkable(_currentNode.gridX, _currentNode.gridZ - 1) &&
                         IsWalkable(_currentNode.gridX + _xDirection, _currentNode.gridZ - 1))
                {
                    return jumpPoint;
                }
            }
            // Verticals
            else if (_xDirection == 0 && _yDirection != 0)
            {
                if (!IsWalkable(_currentNode.gridX + 1, _currentNode.gridZ) &&
                    IsWalkable(_currentNode.gridX + 1, _currentNode.gridZ + _yDirection))
                {
                    return jumpPoint;
                }
                else if (!IsWalkable(_currentNode.gridX - 1, _currentNode.gridZ) &&
                         IsWalkable(_currentNode.gridX - 1, _currentNode.gridZ + _yDirection))
                {
                    return jumpPoint;
                }
            }
            // Diagonals
            else if (_xDirection != 0 && _yDirection != 0)
            {
                if (!IsWalkable(_currentNode.gridX + _xDirection, _currentNode.gridZ))
                {
                    return jumpPoint;
                }
                else if (!IsWalkable(_currentNode.gridX, _currentNode.gridZ + _yDirection))
                {
                    return jumpPoint;
                }

                if (Jump(jumpPoint, _xDirection, 0, _destination) != null ||
                    Jump(jumpPoint, 0, _yDirection, _destination) != null)
                {
                    return jumpPoint;
                }
            }

            return Jump(jumpPoint, _xDirection, _yDirection, _destination);
        }

        public Node NodeFromPoint(Vector3 _position)
        {
            float xPercentage = (_position.x + gridSize.x / 2 - gridPosition.x) / gridSize.x;
            float zPercentage = (_position.z + gridSize.z / 2 - gridPosition.z) / gridSize.z;
            xPercentage = Mathf.Clamp01(xPercentage);
            zPercentage = Mathf.Clamp01(zPercentage);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPercentage);
            int z = Mathf.RoundToInt((gridSizeZ - 1) * zPercentage);

            return grid[x, z];
        }
        
        public Node NodeFromPoint(Vector2 _position)
        {
            float xPercentage = (_position.x + gridSize.x / 2 - gridPosition.x) / gridSize.x;
            float yPercentage = (_position.y + gridSize.z / 2 - gridPosition.z) / gridSize.z;
            xPercentage = Mathf.Clamp01(xPercentage);
            yPercentage = Mathf.Clamp01(yPercentage);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPercentage);
            int y = Mathf.RoundToInt((gridSizeZ - 1) * yPercentage);

            return grid[x, y];
        }
        
        private bool IsWalkable(int _gridX, int _gridY)
        {
            if (_gridX < 0 || _gridX > gridSize.x - 1 || _gridY < 0 || _gridY > gridSize.z - 1)
                return false;

            return !grid[_gridX, _gridY].isObstructed;
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
            
            for (int x = -nbX; x <= nbX; x+=nbX*2)
            {
                for (int z = -nbZ; z <= nbZ; z++)
                {
                    //Node node = NodeFromPoint(new Vector3(_position.x + i * _object.localScale.x / (nbX * 2), _position.y, _position.z + w * _object.localScale.z / (nbZ * 2)));
                    Node node = NodeFromPoint(_position + _object.right * (x * _object.localScale.x / (nbX * 2)) +
                                              _object.forward * (z * _object.localScale.z / (nbZ * 2)));

                    if (!nodes.Contains(node))
                        nodes.Add(node);
                }
            }
            
            for (int z = -nbZ; z <= nbZ; z+=nbZ*2)
            {
                for (int x = -nbX; x <= nbX; x++)
                {
                
                    //Node node = NodeFromPoint(new Vector3(_position.x + w * _object.localScale.x / (nbX * 2), _position.y, _position.z + i * _object.localScale.z / (nbZ * 2)));
                    Node node = NodeFromPoint(_position + _object.right * (x * _object.localScale.x / (nbX * 2)) + 
                                              _object.forward * (z * _object.localScale.z / (nbZ * 2)));
                    
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
                
                Gizmos.DrawWireCube(gridPosition, new Vector3(gridSize.x, 1, gridSize.z));
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 0f, 0.5f);
                Gizmos.DrawCube(gridPosition, new Vector3(gridSize.x, 1, gridSize.z));
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
                    else
                        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);

                    Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }
}
