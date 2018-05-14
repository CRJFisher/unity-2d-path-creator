using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(PathCreator))]
public class PathCreatorEditor : Editor {

    PathCreator creator;

    public Path currentPath = null;

    Path CurrentPath {
        get 
        {
            if (creator.currentSection == null) {
                creator.SetDefaultCurrentSection();
            }
            return creator.currentSection;
        }
        set {
            creator.currentSection = value;
        }
    }


    Path[] Paths
    {
        get
        {
            return creator.path;
        }
    }

    private int? selectedIdx = null;

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        // Check current path
        CheckSelected();

        switch (CurrentPath.pathType)
        {
            case (Path.PathType.ARC):
                // arc editor methods
                break;
            case (Path.PathType.BEZIER):
                break;
        }

        if (GUILayout.Button("Create new"))
        {
            Undo.RecordObject(creator, "Create new");
            creator.CreateNewPathSection();
        }
        if (GUILayout.Button("Delete all"))
        {
            Undo.RecordObject(creator, "Delete all");
            creator.DeletePath();
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

    private void SetNewSelected(Path[] paths)
    {
        if (selectedIdx != null) {
            paths[(int)selectedIdx].isSelected = false;
        }
        for (var i = 0; i < Paths.Length; i++)
        {
            if (paths[i].isSelected)
            {
                selectedIdx = i;
                CurrentPath = Paths[i];
            }
        }
    }

    private void CheckSelected() {

        List<Path> _paths = new List<Path>(Paths);
        List<Path> selectedPaths = _paths.Where(x => x.isSelected).ToList();

        if (selectedPaths.Count == 2) {
            Debug.Log(selectedIdx);
            SetNewSelected(Paths);
        }
    }

    private int? FindSelected() {
        int? selectedIx = null;

        for (int i = 0; i < Paths.Length; i++) {
            if (Paths[i].isSelected) selectedIx = i;
        }
        return selectedIx;
    }

    void OnSceneGUI()
    {
        selectedIdx = FindSelected();
        CheckSelected();
        Input();
        CurrentPath.Draw(creator);
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            CurrentPath.SceneShiftClick(mousePos);
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
        {
            CurrentPath.SceneRightClick(mousePos, creator);
        }

        if (guiEvent.type == EventType.MouseMove)
        {
            CurrentPath.SceneMouseMove(mousePos);
        }

        HandleUtility.AddDefaultControl(0);
    }

    void OnEnable()
    {
        creator = (PathCreator)target;

        creator.CreateNewPathSection();
        selectedIdx = 0;
    }
}
