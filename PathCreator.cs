using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathCreator : MonoBehaviour {

    //[HideInInspector]
    [SerializeField]
    public Path[] path = new Path[0];

    [SerializeField]
    public Path currentSection;

    [SerializeField]
    public Path CurrentSection
    {
        get
        {
            return currentSection != null ? CurrentSection : nextSection;
        }
        set {
            currentSection = value;
        }
    }

    public void SetDefaultCurrentSection() {
        this.currentSection = nextSection;
    }

    [SerializeField]
    public Path nextSection = new BezierPath(new Vector2());

	public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public static float anchorDiameter = .1f;
    public float controlDiameter = .075f;
    public bool displayControlPoints = true;

    public void CreateNewPathSection()
    {
        //path = new Path(transform.position);
        int pathLength = path.Length;
        var newPath = new Path[pathLength+1];
        var i = 0;
        for (; i < path.Length; i++) {
            newPath[i] = path[i];
        }
        newPath[i] = nextSection;
        path = newPath;
        //Camera camera1 = SceneView.currentDrawingSceneView.camera;
        //Vector3 sceneCameraPos = camera1.transform.position;
        Vector2 nextPathLocation = new Vector2(0, 0); // TODO: get this from elsewhere 
        nextSection = new BezierPath(nextPathLocation);
    }

    public void DeletePath() {
        path = new Path[0];
    }

    void Reset()
    {
        CreateNewPathSection();
    }
}
