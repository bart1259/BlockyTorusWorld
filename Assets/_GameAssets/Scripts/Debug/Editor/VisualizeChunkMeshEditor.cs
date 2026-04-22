using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisualizeChunkMesh))]
public class VisualizeChunkMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VisualizeChunkMesh visualizeChunkMesh = (VisualizeChunkMesh)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Visualize Mesh"))
        {
            visualizeChunkMesh.Visualize();
            EditorUtility.SetDirty(visualizeChunkMesh);
        }
    }
}
