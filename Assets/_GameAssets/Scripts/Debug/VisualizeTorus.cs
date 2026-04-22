using UnityEngine;

public class VisualizeTorus : MonoBehaviour {

    public float MajorRadius = 10.0f;
    public float MinorRadius = 1.0f;
    public int MajorSegments = 100;
    public int MinorSegments = 10;

    private Torus torus;

    private void OnDrawGizmos()
    {
        if (torus == null || torus.MajorRadius != MajorRadius || torus.MinorRadius != MinorRadius || torus.MajorSegments != MajorSegments || torus.MinorSegments != MinorSegments)
        {
            torus = new Torus(MajorRadius, MinorRadius, MajorSegments, MinorSegments);
        }

        for (int i = 0; i < torus.Vertices.Count; i++)
        {
            Gizmos.DrawSphere(torus.Vertices[i], 0.1f);
        }
    }

}