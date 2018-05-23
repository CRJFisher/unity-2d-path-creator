using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathCreator : MonoBehaviour {

    [SerializeField]
    public List<PathSection> paths = new List<PathSection>();

    [HideInInspector]
    public PathSection currentSection;

    [SerializeField]
    public PathSection nextSection;

    void Start()
    {
        nextSection = ScriptableObject.CreateInstance<BezierPath>();
    }

    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public static float anchorDiameter = .1f;
    public float controlDiameter = .075f;
    public bool displayControlPoints = true;

    public void CreateNewPathSection()
    {
        Debug.Log("CreateNewPathSection");

        //Camera camera1 = SceneView.currentDrawingSceneView.camera;
        //Vector3 sceneCameraPos = camera1.transform.position;
        Vector2 nextPathLocation = new Vector2(0, 0); // TODO: get this from elsewhere 
        paths.Add(new BezierPath());
    }

    void Reset()
    {
        CreateNewPathSection();
    }
}
