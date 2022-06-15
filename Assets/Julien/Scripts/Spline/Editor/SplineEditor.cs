using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    bool drawDebugList = false;
    
    private void OnSceneGUI()
    {
        serializedObject.Update();
        SerializedProperty _segmentsProperty = serializedObject.FindProperty("_segments");
        
        for (int i = 0; i < _segmentsProperty.arraySize; i++)
        {
            SerializedProperty _pointASerialized = _segmentsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("pointA");
            SerializedProperty _pointBSerialized = _segmentsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("pointB");
            SerializedProperty _pointCSerialized = _segmentsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("pointC");
            SerializedProperty _pointDSerialized = _segmentsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("pointD");
            
            Vector3 previousPointAPos = _pointASerialized.vector3Value;
            
            if (i <= 0)
            {
                _pointASerialized.vector3Value =
                    Handles.PositionHandle(_pointASerialized.vector3Value, Quaternion.identity);
            }
            else
            {
                _pointASerialized.vector3Value = _segmentsProperty.GetArrayElementAtIndex(i - 1).FindPropertyRelative("pointD").vector3Value;
            }

            _pointBSerialized.vector3Value =
                _pointBSerialized.vector3Value + (_pointASerialized.vector3Value - previousPointAPos);
            
            _pointBSerialized.vector3Value =
                Handles.PositionHandle(_pointBSerialized.vector3Value, Quaternion.identity);
            
            Vector3 previousPointDPos = _pointDSerialized.vector3Value;
            _pointDSerialized.vector3Value =
                Handles.PositionHandle(_pointDSerialized.vector3Value, Quaternion.identity);
            

            _pointCSerialized.vector3Value =
                _pointCSerialized.vector3Value + (_pointDSerialized.vector3Value - previousPointDPos);
            _pointCSerialized.vector3Value =
                Handles.PositionHandle(_pointCSerialized.vector3Value, Quaternion.identity);
            
            
            Handles.color = Color.yellow;
            Handles.DrawLine(_pointASerialized.vector3Value, _pointBSerialized.vector3Value);
            Handles.DrawLine(_pointDSerialized.vector3Value, _pointCSerialized.vector3Value);
            Handles.color = Color.white;
            
            Handles.DrawBezier(_pointASerialized.vector3Value, _pointDSerialized.vector3Value, _pointBSerialized.vector3Value, _pointCSerialized.vector3Value, Color.green, Texture2D.whiteTexture, 1);
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty restartWhenEndedProperty = serializedObject.FindProperty("restartWhenEnded");
        SerializedProperty visual = serializedObject.FindProperty("visual");
        SerializedProperty movementSpeed = serializedObject.FindProperty("movementSpeed");
        SerializedProperty _segmentsProperty = serializedObject.FindProperty("_segments");
        
        EditorGUILayout.PropertyField(restartWhenEndedProperty);
        EditorGUILayout.PropertyField(visual);
        EditorGUILayout.PropertyField(movementSpeed);

        if (GUILayout.Button("Add Points"))
        {
            _segmentsProperty.InsertArrayElementAtIndex(_segmentsProperty.arraySize);

            if (_segmentsProperty.arraySize-1 > 0)
            {
                SerializedProperty newSegment =
                    _segmentsProperty.GetArrayElementAtIndex(_segmentsProperty.arraySize - 1);
                SerializedProperty previousSegment =
                    _segmentsProperty.GetArrayElementAtIndex(_segmentsProperty.arraySize - 2);

                newSegment.FindPropertyRelative("pointB").vector3Value =
                    previousSegment.FindPropertyRelative("pointD").vector3Value + new Vector3(1, 0, 1);

                newSegment.FindPropertyRelative("pointD").vector3Value =
                    previousSegment.FindPropertyRelative("pointD").vector3Value + new Vector3(1, 0, 1);

                newSegment.FindPropertyRelative("pointC").vector3Value =
                    newSegment.FindPropertyRelative("pointD").vector3Value + new Vector3(1, 0, 1);
            }
        }

        GUI.enabled = _segmentsProperty.arraySize > 0;
        if (GUILayout.Button("Remove Last Point"))
        {
            if (_segmentsProperty.arraySize > 0)
                _segmentsProperty.DeleteArrayElementAtIndex(_segmentsProperty.arraySize - 1);
        }
        GUI.enabled = true;

        drawDebugList = EditorGUILayout.Toggle("Debug", drawDebugList);
        if (drawDebugList)
        {
            EditorGUILayout.PropertyField(_segmentsProperty);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
