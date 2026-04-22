using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisualizeChunkMeshRegion : MonoBehaviour {

    public int Seed = 0;
    public int ChunkSize = 16;
    public int ChunkHeight = 16;
    public int MajorTorusSegments = 32;
    public int MinorTorusSegments = 32;
    public int MajorRadius = 10;
    public int MinorRadius = 1;
    public int MinMajorIndex = 0;
    public int MaxMajorIndex = 0;
    public Material Material = null;

    public void Visualize() {
        WorldDataGenerator worldDataGenerator = new WorldDataGenerator(Seed, ChunkSize, ChunkHeight, MajorTorusSegments, MinorTorusSegments);
        WorldData worldData = new WorldData(worldDataGenerator, MajorRadius, MinorRadius, MajorTorusSegments, MinorTorusSegments, ChunkSize, ChunkHeight, 16);

        // Delete all children of the object
        foreach (Transform child in transform) {
            DestroyImmediate(child.gameObject, true);
        }

        TextureMapper textureMapper = new TextureMapper(2, 2);
        textureMapper.SetTextureCoordinates(1, new Vector2Int(0, 0));
        textureMapper.SetTextureCoordinates(2, new Vector2Int(1, 0));
        textureMapper.SetTextureCoordinates(3, new Vector2Int(0, 1));
        textureMapper.SetTextureCoordinates(4, new Vector2Int(1, 1));
        print(textureMapper.GetTextureCoordinates(1));
        ChunkMeshGenerator chunkMeshGenerator = new ChunkMeshGenerator(worldData, textureMapper);

        for (int majorIndex = MinMajorIndex; majorIndex <= MaxMajorIndex; majorIndex++) {
            for (int minorIndex = 0; minorIndex < MinorTorusSegments; minorIndex++) {
                GameObject chunk = new GameObject("Chunk" + majorIndex + "_" + minorIndex);
                chunk.transform.parent = transform;
                Mesh mesh = chunkMeshGenerator.GenerateMesh(majorIndex, minorIndex);
                chunk.AddComponent<MeshFilter>().sharedMesh = mesh;
                chunk.AddComponent<MeshRenderer>().material = Material;
            }
        }
    }
}