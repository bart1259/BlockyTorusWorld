using UnityEngine;
using System.Collections.Generic;

public class Torus
{
    public float MajorRadius { get; private set; }
    public float MinorRadius { get; private set; }
    public int MajorSegments { get; private set; }
    public int MinorSegments { get; private set; }

    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }

    public Torus(float majorRadius, float minorRadius, int majorSegments, int minorSegments)
    {
        MajorRadius = majorRadius;
        MinorRadius = minorRadius;
        MajorSegments = majorSegments;
        MinorSegments = minorSegments;
        Vertices = new List<Vector3>();
        Triangles = new List<int>();

        GenerateMeshData();
    }

    public void GenerateMeshData()
    {
        Vertices.Clear();
        Triangles.Clear();

        for (int i = 0; i < MajorSegments; i++)
        {
            for (int j = 0; j < MinorSegments; j++)
            {
                float angle = i * 2 * Mathf.PI / MajorSegments;
                float minorAngle = j * 2 * Mathf.PI / MinorSegments;
                float x = (MajorRadius + MinorRadius * Mathf.Cos(minorAngle)) * Mathf.Cos(angle);
                float y = (MajorRadius + MinorRadius * Mathf.Cos(minorAngle)) * Mathf.Sin(angle);
                float z = MinorRadius * Mathf.Sin(minorAngle);
                Vertices.Add(new Vector3(x, y, z));
            }
        }

        for (int i = 0; i < MajorSegments; i++)
        {
            for (int j = 0; j < MinorSegments; j++)
            {
                int current = i * MinorSegments + j;
                int nextMinor = i * MinorSegments + (j + 1) % MinorSegments;
                int nextMajor = ((i + 1) % MajorSegments) * MinorSegments + j;
                int nextMajorNextMinor = ((i + 1) % MajorSegments) * MinorSegments + (j + 1) % MinorSegments;

                Triangles.AddRange(new int[] { current, nextMajor, nextMinor });
                Triangles.AddRange(new int[] { nextMinor, nextMajor, nextMajorNextMinor });
            }
        }
    }

    public Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Torus";
        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        return mesh;
    }

    public Mesh GenerateMeshSector(int sectorIndex, int maxSectors) {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int segmentsPerSector = MajorSegments / maxSectors;
        int maxMajorSegments = segmentsPerSector * (sectorIndex + 1);
        int minMajorSegments = maxMajorSegments - segmentsPerSector;

        for (int i = minMajorSegments; i <= maxMajorSegments; i++) {
            for (int j = 0; j < MinorSegments; j++) {
                float angle = i * 2 * Mathf.PI / MajorSegments;
                float minorAngle = j * 2 * Mathf.PI / MinorSegments;
                float x = (MajorRadius + MinorRadius * Mathf.Cos(minorAngle)) * Mathf.Cos(angle);
                float y = (MajorRadius + MinorRadius * Mathf.Cos(minorAngle)) * Mathf.Sin(angle);
                float z = MinorRadius * Mathf.Sin(minorAngle);

                vertices.Add(new Vector3(x, y, z));
            }
        }

        for (int ri = 0; ri < segmentsPerSector; ri++) {
            for (int j = 0; j < MinorSegments; j++) {
                int current = ri * MinorSegments + j;
                int nextMinor = ri * MinorSegments + (j + 1) % MinorSegments;
                int nextMajor = (ri + 1) * MinorSegments + j;
                int nextMajorNextMinor = (ri + 1) * MinorSegments + (j + 1) % MinorSegments;

                triangles.AddRange(new int[] { current, nextMajor, nextMinor });
                triangles.AddRange(new int[] { nextMinor, nextMajor, nextMajorNextMinor });
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "TorusSector_" + sectorIndex;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        return mesh;
    }
}
