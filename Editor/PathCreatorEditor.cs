using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(PathCreator))]
public class PathCreatorEditor : Editor {

    PathCreator creator;

    List<PathSection> PathSections
    {
        get
        {
            return creator.paths;
        }
    }

    public int numberOfSelectedSections = 0;

    // Triggered by user events in the scene view
    void OnSceneGUI()
    {
        Debug.Log("On scene gui");
        if (creator.currentSection == null) {
            return;
        }
        Input();
        creator.currentSection.Draw(creator);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        // Check current path
        CheckSelected();

        //if (creator.currentSection == null)
        //{
        //    return;
        //}

        if (GUILayout.Button("Create new"))
        {
            Undo.RecordObject(creator, "Create new");
            creator.CreateNewPathSection();
        }
        if (GUILayout.Button("Show selected"))
        {
            for (var i = 0; i < PathSections.Count(); i++) {
                if (PathSections[i].isSelected) {
                    Debug.Log(i);
                    Debug.Log(creator.currentSection.GetType());
                }
            }
        }
        if (GUILayout.Button("Show selectedIdx"))
        {            
            Debug.Log(numberOfSelectedSections);
        }

        //bool isClosed = GUILayout.Toggle(Path.IsClosed, "Closed");
        //if (isClosed != Path.IsClosed)
        //{
        //    Undo.RecordObject(creator, "Toggle closed");
        //    Path.IsClosed = isClosed;
        //}

        //bool autoSetControlPoints = GUILayout.Toggle(Path.AutoSetControlPoints, "Auto Set Control Points");
        //if (autoSetControlPoints != Path.AutoSetControlPoints)
        //{
        //    Undo.RecordObject(creator, "Toggle auto set controls");
        //    Path.AutoSetControlPoints = autoSetControlPoints;
        //}

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }

    private void CheckSelected()
    {
        List<PathSection> selectedPaths = PathSections
            .Where(x => x.isSelected).ToList();
        if (selectedPaths.Count != numberOfSelectedSections || 
            selectedPaths.Count == 2)
        {
            numberOfSelectedSections = 1;
            SetNewSelected();
        }
    }

    private void SetNewSelected()
    {
        if (creator.currentSection != null)
        {
            creator.currentSection.isSelected = false;
        }
        foreach (var p in PathSections)
        {
            if (p.isSelected)
            {
                creator.currentSection = p;
            }
        }
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            creator.currentSection.SceneShiftClick(mousePos, creator);
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
        {
            creator.currentSection.SceneRightClick(mousePos, creator);
        }

        if (guiEvent.type == EventType.MouseMove)
        {
            creator.currentSection.SceneMouseMove(mousePos);
        }

        HandleUtility.AddDefaultControl(0);
    }

    void OnEnable()
    {
        Debug.Log("On enable");
        creator = (PathCreator)target;
    }
}
