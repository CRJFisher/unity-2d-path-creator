using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PathSection : ScriptableObject {

    public enum PathType {
        BEZIER,
        ARC
    }

    [SerializeField]
    public bool isSelected;

    [SerializeField] //, HideInInspector]
    public List<Vector2> points;

    [SerializeField]
    public PathType pathType;
    
    private int numPoints;

    public abstract void SceneShiftClick(Vector2 mousePos, PathCreator creator);

    public abstract void SceneRightClick(Vector2 mousePos, PathCreator creator);

    public abstract void SceneMouseMove(Vector2 mousePos);

    public abstract Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1);

    public abstract void Draw(PathCreator creator);
}