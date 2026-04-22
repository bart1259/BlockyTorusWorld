using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisualizeTorusMesh))]
public class VisualizeTorusMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VisualizeTorusMesh visualizeTorusMesh = (VisualizeTorusMesh)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Visualize Mesh"))
        {
            visualizeTorusMesh.Visualize();
            EditorUtility.SetDirty(visualizeTorusMesh);
        }
    }
}
