using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisualizeChunkMesh : MonoBehaviour {

    public int Seed = 0;
    public int ChunkSize = 16;
    public int ChunkHeight = 16;
    public int MajorTorusSegments = 32;
    public int MinorTorusSegments = 32;
    public int MajorRadius = 10;
    public int MinorRadius = 1;
    public int MajorIndex = 0;
    public int MinorIndex = 0;

    public void Visualize() {
        WorldDataGenerator worldDataGenerator = new WorldDataGenerator(Seed, ChunkSize, ChunkHeight, MajorTorusSegments, MinorTorusSegments);
        WorldData worldData = new WorldData(worldDataGenerator, MajorRadius, MinorRadius, MajorTorusSegments, MinorTorusSegments, ChunkSize, ChunkHeight, 16);

        TextureMapper textureMapper = new TextureMapper(2, 2);
        textureMapper.SetTextureCoordinates(1, new Vector2Int(0, 0));
        textureMapper.SetTextureCoordinates(2, new Vector2Int(1, 0));
        textureMapper.SetTextureCoordinates(3, new Vector2Int(0, 1));
        textureMapper.SetTextureCoordinates(4, new Vector2Int(1, 1));

        ChunkMeshGenerator chunkMeshGenerator = new ChunkMeshGenerator(worldData, textureMapper);
        Mesh mesh = chunkMeshGenerator.GenerateMesh(MajorIndex, MinorIndex);
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void OnValidate()
    {
        ChunkSize = Mathf.Max(1, ChunkSize);
        ChunkHeight = Mathf.Max(1, ChunkHeight);
        MajorTorusSegments = Mathf.Max(1, MajorTorusSegments);
        MinorTorusSegments = Mathf.Max(1, MinorTorusSegments);

        if (!isActiveAndEnabled)
        {
            return;
        }

        Visualize();
    }
}