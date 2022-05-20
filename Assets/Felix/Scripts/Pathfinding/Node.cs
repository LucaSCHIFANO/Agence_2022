using UnityEngine;

namespace Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        public Node parent;

        public bool toShowGui;
        public bool isObstructed;
        public bool isWalkable;
        public Vector3 position;
        public int gridX;
        public int gridY;
        public int gridZ;

        public int gCost;
        public int hCost;

        private int heapIndex;
        
        public Node(bool _isObstructed, bool _isWalkable, Vector3 _position, int _gridX, int _gridY, int _gridZ, bool _toShowGui = false)
        {
            isObstructed = _isObstructed;
            isWalkable = _isWalkable;
            position = _position;

            gridX = _gridX;
            gridY = _gridY;
            gridZ = _gridZ;

            toShowGui = _toShowGui;
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);

            if (compare == 0)
                compare = hCost.CompareTo(nodeToCompare.hCost);

            return -compare;
        }

        public int HeapIndex
        {
            get { return heapIndex;}
            set { heapIndex = value; }
        }
    }
}
