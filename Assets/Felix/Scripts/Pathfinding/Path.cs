using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Path
    {
        public readonly Vector3[] lookPoints;
        public readonly Line[] turnBoundaries;
        public readonly int finishLineIndex;

        public Path(Vector3[] waypoints, Vector3 startPos, float turnDistance)
        {
            lookPoints = waypoints;
            turnBoundaries = new Line[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;


            Vector2 previousPoint = V3ToV2(startPos);
            for (int i = 0; i < lookPoints.Length; i++)
            {
                Vector2 currentPosition = V3ToV2(lookPoints[i]);
                Vector2 directionToCurrentPoint = (currentPosition - previousPoint).normalized;
                Vector2 turnBoundaryPoint = i == finishLineIndex ? currentPosition : currentPosition - directionToCurrentPoint * turnDistance;

                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - directionToCurrentPoint * turnDistance);
                previousPoint = turnBoundaryPoint;
            }
        }

        private Vector2 V3ToV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;

            foreach (Vector3 point in lookPoints)
            {
                Gizmos.DrawCube(point + Vector3.up, Vector3.one);
            }

            foreach (Line line in turnBoundaries)
            {
                line.DrawWithGizmos(10);
            }
        }
    }
}
