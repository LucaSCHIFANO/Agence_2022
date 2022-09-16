using UnityEngine;

namespace Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        public Node parent;

        public bool toShowGui;
        public bool isObstructed;
        public Vector3 position;
        public int gridX;
        public int gridZ;

        public int gCost;
        public int hCost;

        private int heapIndex;
        
        public Node(bool _isObstructed, Vector3 _position, int _gridX, int _gridZ, bool _toShowGui = false)
        {
            isObstructed = _isObstructed;
            position = _position;

            gridX = _gridX;
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
