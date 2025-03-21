using UnityEditor;
using UnityEngine;
using UnityEditor.TerrainTools;


namespace FileManagement
{
    [CustomEditor(typeof(FileManager))]
    public class FileManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FileManager fileManager = (FileManager)target;

            GUILayout.Space(20);
            GUILayout.Label("Version");
            EditorGUILayout.LabelField(fileManager.CurrentVersion);
            GUILayout.Space(10);
            if (GUILayout.Button("Increment Major Version")) { fileManager.IncrementMajorVersion(); }
            if (GUILayout.Button("Decrement Major Version")) { fileManager.DecrementMajorVersion(); }
            GUILayout.Space(10);
            if (GUILayout.Button("Increment Minor Version")) { fileManager.IncrementMinorVersion(); }
            if (GUILayout.Button("Decrement Minor Version")) { fileManager.DecrementMinorVersion(); }
            GUILayout.Space(10);
            if (GUILayout.Button("Increment Patch Version")) { fileManager.IncrementPatchVersion(); }
            if (GUILayout.Button("Decrement Patch Version")) { fileManager.DecrementPatchVersion(); }

            GUILayout.Space(20);
            GUILayout.Label("Debugging");
            GUILayout.Space(10);
            if(GUILayout.Button("Print Data")) { fileManager.PrintData(); }

        }
    }
}
