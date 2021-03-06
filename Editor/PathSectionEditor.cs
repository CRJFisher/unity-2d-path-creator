﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PathSection), true)]
public class PathSectionsHolderEditor : PropertyDrawer {

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var amountRect = new Rect(position.x, position.y, 30, position.height);
        var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
        var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        if (property != null && property.objectReferenceValue != null) {
            SerializedObject propObj = new SerializedObject(property.objectReferenceValue);

            BezierPath path = ((BezierPath)property.objectReferenceValue);

            EditorGUI.BeginChangeCheck();

            SerializedProperty propSelected = propObj.FindProperty("isSelected");
            SerializedProperty propPathType = propObj.FindProperty("pathType");

            EditorGUILayout.PropertyField(propSelected, true);
            EditorGUILayout.PropertyField(propPathType, true);
            if (EditorGUI.EndChangeCheck())
            {
                propObj.ApplyModifiedProperties();
            }

            //if (((BezierPath)property.objectReferenceValue).isSelected) {
                
            //}

            //BezierPath path = (BezierPath)property.objectReferenceValue;

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            //EditorGUI.PropertyField(amountRect, propObj.FindProperty("pathType"), GUIContent.none);
            //EditorGUI.PropertyField(unitRect, propObj.FindProperty("isSelected"), GUIContent.none);
            propObj.Update();
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}