using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Path
    {
        public readonly Vector3 startPosition;
        public readonly Vector3[] lookPoints;
        public readonly float[] timers;
        public readonly Line[] turnBoundaries;
        public readonly int finishLineIndex;

        public Path(Vector3[] _waypoints, Vector3 _startPos, float _turnDistance, float _speed)
        {
            startPosition = _startPos;
            lookPoints = _waypoints;
            timers = new float[lookPoints.Length];
            turnBoundaries = new Line[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            Vector2 previousPoint = V3ToV2(startPosition);
            for (int i = 0; i < lookPoints.Length; i++)
            {
                Vector2 currentPosition = V3ToV2(lookPoints[i]);
                Vector2 directionToCurrentPoint = (currentPosition - previousPoint).normalized;
                Vector2 turnBoundaryPoint = i == finishLineIndex ? currentPosition : currentPosition - directionToCurrentPoint * _turnDistance;

                timers[i] = Vector2.Distance(currentPosition, previousPoint) / _speed;

                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - directionToCurrentPoint * _turnDistance);
                previousPoint = turnBoundaryPoint;
            }
        }

        private Vector2 V3ToV2(Vector3 _v3)
        {
            return new Vector2(_v3.x, _v3.z);
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
