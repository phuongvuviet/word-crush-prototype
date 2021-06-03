using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardDataGenerator))]
public class BoardGeneratorEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUILayout.Button("Generate"))
        {
            BoardDataGenerator generator = target as BoardDataGenerator;
            generator.GenerateBoard();
        }
        serializedObject.ApplyModifiedProperties();

    }
}
