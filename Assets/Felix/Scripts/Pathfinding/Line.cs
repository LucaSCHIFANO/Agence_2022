using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public struct Line
    {
        private const float verticalLineGradient = 1e5f;

        private float gradient;
        private float intercept;
        private Vector2 pointOnLine1;
        private Vector2 pointOnLine2;

        private float gradientPerpendicular;

        private bool approachSide;
        
        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) : this()
        {
            float dx = pointOnLine.x - pointPerpendicularToLine.x;
            float dy = pointOnLine.y - pointPerpendicularToLine.y;

            gradientPerpendicular = dx == 0 ? verticalLineGradient : dy / dx;
            gradient = gradientPerpendicular == 0 ? verticalLineGradient : -1 / gradientPerpendicular;
            intercept = pointOnLine.y - gradient * pointOnLine.x;

            pointOnLine1 = pointOnLine;
            pointOnLine2 = pointOnLine + new Vector2(1, gradient);

            approachSide = GetSide(pointPerpendicularToLine);
        }

        private bool GetSide(Vector2 p)
        {
            return (p.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) >
                   (p.y - pointOnLine1.y) * (pointOnLine2.x - pointOnLine1.x);
        }

        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != approachSide;
        }

        public void DrawWithGizmos(float length)
        {
            Vector3 lineDirection = new Vector3(1, 0, gradient).normalized;
            Vector3 lineCenter = new Vector3(pointOnLine1.x, 0, pointOnLine1.y) + Vector3.up;
            Gizmos.DrawLine(lineCenter - lineDirection * length / 2f, lineCenter + lineDirection * length / 2f);
        }
    }
}
