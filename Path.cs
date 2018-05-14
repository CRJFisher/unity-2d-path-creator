using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Path {

    public enum PathType {
        BEZIER,
        ARC
    }

    public bool isSelected;

    public void SetIsSelectedFalse() {
        this.isSelected = false;
    }

    [SerializeField] //, HideInInspector]
    public List<Vector2> points;

    public PathType pathType;

    //Path (PathType type) {
    //    this.pathType = type;
    //}
    
    private int numPoints;

    public virtual void SceneShiftClick(Vector2 mousePos) {}

    public virtual void SceneRightClick(Vector2 mousePos, PathCreator creator) {}

    public virtual void SceneMouseMove(Vector2 mousePos) {}

    public virtual Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1) { return new Vector2[0]; }

    public virtual void Draw(PathCreator creator) {}
}