using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class BezierPath : PathSection
{
    
    bool autoSetControlPoints;

    const float segmentSelectDistanceThreshold = .1f;

    int selectedSegmentIndex = -1;

    public BezierPath()
    {
        Vector2 centre = new Vector2(0, 0);
        points = new List<Vector2>
        {
            centre + Vector2.left,
            centre+(Vector2.left+Vector2.up)*.5f,
            centre + (Vector2.right+Vector2.down)*.5f,
            centre + Vector2.right
        };
        pathType = PathType.BEZIER;
    }

    public Vector2 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public bool AutoSetControlPoints
    {
        get
        {
            return autoSetControlPoints;
        }
        set
        {
            if (autoSetControlPoints != value)
            {
                autoSetControlPoints = value;
                if (autoSetControlPoints)
                {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    public int NumPoints
    {
        get
        {
            return points.Count;
        }
    }

    public int NumSegments
    {
        get
        {
            return points.Count / 3;
        }
    }

    public void AddSegment(Vector2 anchorPos)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * .5f);
        points.Add(anchorPos);

        if (autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(points.Count - 1);
        }
    }

    public void SplitSegment(Vector2 anchorPos, int segmentIndex)
    {
        //Undo.RecordObject(creator, "Split segment");
        points.InsertRange(segmentIndex * 3 + 2, new Vector2[] { Vector2.zero, anchorPos, Vector2.zero });
        if (autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(segmentIndex * 3 + 3);
        }
        else
        {
            AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
        }
    }

    public void DeleteSegment(int anchorIndex)
    {
        if (NumSegments > 2)
        {
            if (anchorIndex == 0)
            {
                points.RemoveRange(0, 3);
            }
            else if (anchorIndex == points.Count - 1)
            {
                points.RemoveRange(anchorIndex - 2, 3);
            }
            else
            {
                points.RemoveRange(anchorIndex - 1, 3);
            }
        }
    }

    public Vector2[] GetPointsInSegment(int i)
    {
        return new Vector2[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
    }

    public void MovePoint(int i, Vector2 pos)
    {
        Vector2 deltaMove = pos - points[i];
        if (i % 3 == 0 || !autoSetControlPoints)
        {
            points[i] = pos;
            if (autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(i);
            }
            else
            {

                if (i % 3 == 0)
                {
                    if (i + 1 < points.Count)
                    {
                        points[LoopIndex(i + 1)] += deltaMove;
                    }
                    if (i - 1 >= 0)
                    {
                        points[LoopIndex(i - 1)] += deltaMove;
                    }
                }
                else
                {
                    bool nextPointIsAnchor = (i + 1) % 3 == 0;
                    int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                    int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                    if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
                    {
                        float dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                        Vector2 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                        points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
                    }
                }
            }
        }
    }

    public override Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
    {
        List<Vector2> evenlySpacedPoints = new List<Vector2>
        {
            points[0]
        };
        Vector2 previousPoint = points[0];
        float dstSinceLastEvenPoint = 0;

        for (int segmentIndex = 0; segmentIndex < NumSegments; segmentIndex++)
        {
            Vector2[] p = GetPointsInSegment(segmentIndex);
            float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength / 2f;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            while (t <= 1)
            {
                t += 1f / divisions;
                Vector2 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                while (dstSinceLastEvenPoint >= spacing)
                {
                    float overshootDst = dstSinceLastEvenPoint - spacing;
                    Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    dstSinceLastEvenPoint = overshootDst;
                    previousPoint = newEvenlySpacedPoint;
                }

                previousPoint = pointOnCurve;
            }
        }
        return evenlySpacedPoints.ToArray();
    }


    void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
    {
        for (int i = updatedAnchorIndex - 3; i <= updatedAnchorIndex + 3; i += 3)
        {
            if (i >= 0 && i < points.Count)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControls();
    }

    void AutoSetAllControlPoints()
    {
        for (int i = 0; i < points.Count; i += 3)
        {
            AutoSetAnchorControlPoints(i);
        }

        AutoSetStartAndEndControls();
    }

    void AutoSetAnchorControlPoints(int anchorIndex)
    {
        Vector2 anchorPos = points[anchorIndex];
        Vector2 dir = Vector2.zero;
        float[] neighbourDistances = new float[2];

        if (anchorIndex - 3 >= 0)
        {
            Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }
        if (anchorIndex + 3 >= 0)
        {
            Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
            dir -= offset.normalized;
            neighbourDistances[1] = -offset.magnitude;
        }

        dir.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if (controlIndex >= 0 && controlIndex < points.Count)
            {
                points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
            }
        }
    }

    void AutoSetStartAndEndControls()
    {
        points[1] = (points[0] + points[2]) * .5f;
        points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
    }

    int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }

    public override void SceneShiftClick(Vector2 mousePos, PathCreator creator)
    {
        if (selectedSegmentIndex != -1)
        {

            SplitSegment(mousePos, selectedSegmentIndex);
        }
        else 
        {
            Undo.RecordObject(creator, "Add segment");
            AddSegment(mousePos);
        }
    }

    public override void SceneRightClick(Vector2 mousePos, PathCreator creator)
	{
        float minDstToAnchor = PathCreator.anchorDiameter * .5f;
        int closestAnchorIndex = -1;

        for (int i = 0; i < NumPoints; i+=3)
        {
            float dst = Vector2.Distance(mousePos, this[i]);
            if (dst < minDstToAnchor)
            {
                minDstToAnchor = dst;
                closestAnchorIndex = i;
            }
        }

        if (closestAnchorIndex != -1)
        {
            Undo.RecordObject(creator, "Delete segment");
            DeleteSegment(closestAnchorIndex);
        }
	}

    public override void SceneMouseMove(Vector2 mousePos) {
        float minDstToSegment = segmentSelectDistanceThreshold;
        int newSelectedSegmentIndex = -1;

        for (int i = 0; i < this.NumSegments; i++)
        {
            Vector2[] segPoints = GetPointsInSegment(i);
            float dst = HandleUtility.DistancePointBezier(mousePos, segPoints[0], segPoints[3], segPoints[1], segPoints[2]);
            if (dst < minDstToSegment)
            {
                minDstToSegment = dst;
                newSelectedSegmentIndex = i;
            }
        }

        if (newSelectedSegmentIndex != selectedSegmentIndex)
        {
            selectedSegmentIndex = newSelectedSegmentIndex;
            HandleUtility.Repaint();
        }
    }

    public override void Draw(PathCreator creator)
	{
        Debug.Log("draw");
        for (int i = 0; i < NumSegments; i++)
        {
            Vector2[] segPoints = GetPointsInSegment(i);
            if (creator.displayControlPoints)
            {
                Handles.color = Color.black;
                Handles.DrawLine(segPoints[1], segPoints[0]);
                Handles.DrawLine(segPoints[2], segPoints[3]);
            }
            Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentCol : creator.segmentCol;
            Handles.DrawBezier(segPoints[0], segPoints[3], segPoints[1], segPoints[2], segmentCol, null, 2);
        }


        for (int i = 0; i < NumPoints; i++)
        {
            if (i % 3 == 0 || creator.displayControlPoints)
            {
                Handles.color = (i % 3 == 0) ? creator.anchorCol : creator.controlCol;
                float handleSize = (i % 3 == 0) ? PathCreator.anchorDiameter : creator.controlDiameter;
                Vector2 newPos = Handles.FreeMoveHandle(this.points[i], Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);
                if (this.points[i] != newPos)
                {
                    Undo.RecordObject(creator, "Move point");
                    MovePoint(i, newPos);
                }
            }
        }
	}
}
