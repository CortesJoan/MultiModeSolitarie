using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SolvableSolitaireCardGenerator))]
public class SolvableSolitaireCardGeneratorEditor : Editor
{
    private SolvableSolitaireCardGenerator solitaireCardGenerator;
    private float progress = 0;

    public void OnEnable()
    {
        solitaireCardGenerator = target as SolvableSolitaireCardGenerator;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Game"))
        {
            Undo.RecordObject(solitaireCardGenerator, "Generate New Game");

            // Start measuring the progress
            progress = 0;
            EditorUtility.DisplayProgressBar("Generating Solvable Solitaire Game", "In Progress...", progress);

            // Generate the game
            solitaireCardGenerator.GenerateGame();

            // Progress done
            progress = 1;
            EditorUtility.ClearProgressBar();

            PrefabUtility.RecordPrefabInstancePropertyModifications(solitaireCardGenerator);
        }

        // Display the progress bar
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Progress", GUILayout.Width(70));
  //      EditorUtility.ClearProgressBar(progress, "");
        EditorGUILayout.EndHorizontal();
    }
}