using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisualizeTorusMesh : MonoBehaviour {
    public float MajorRadius = 10.0f;
    public float MinorRadius = 1.0f;
    public int MajorSegments = 100;
    public int MinorSegments = 10;

    public void Visualize()
    {
        Torus torus = new Torus(MajorRadius, MinorRadius, MajorSegments, MinorSegments);
        Mesh mesh = torus.GenerateMesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        Visualize();
    }
}