using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisualizeChunkMeshRegion))]
public class VisualizeChunkMeshRegionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VisualizeChunkMeshRegion visualizeChunkMeshRegion = (VisualizeChunkMeshRegion)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Visualize Mesh"))
        {
            visualizeChunkMeshRegion.Visualize();
            EditorUtility.SetDirty(visualizeChunkMeshRegion);
        }

        GUILayout.Label("Map size: " + (visualizeChunkMeshRegion.ChunkSize * visualizeChunkMeshRegion.MajorTorusSegments) + "x" + (visualizeChunkMeshRegion.ChunkSize * visualizeChunkMeshRegion.MinorTorusSegments));
    }
}
