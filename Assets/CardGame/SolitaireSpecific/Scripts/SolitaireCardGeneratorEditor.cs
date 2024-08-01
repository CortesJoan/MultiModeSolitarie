using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SolitaireCardGenerator))]
public class SolitaireCardGeneratorEditor : Editor
{
    private SolitaireCardGenerator solitaireCardGenerator;

    public void OnEnable()
    {
        solitaireCardGenerator = target as SolitaireCardGenerator;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Game"))
        {
            Undo.RecordObject(solitaireCardGenerator, "Generate New Game");
            solitaireCardGenerator.GenerateGame();
            PrefabUtility.RecordPrefabInstancePropertyModifications(solitaireCardGenerator);
        }
        if (GUILayout.Button("Reset"))
        {
            solitaireCardGenerator.Clear();
        }
    }
}