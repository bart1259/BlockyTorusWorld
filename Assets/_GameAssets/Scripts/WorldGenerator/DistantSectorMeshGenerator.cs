using UnityEngine;
using System.Collections.Generic;

public class DistantSectorMeshGenerator {

    public int MajorSegments { get; private set; }
    public int MinorSegments { get; private set; }
    public WorldData WorldData { get; private set; }
    public TextureMapper TextureMapper { get; private set; }
    public Texture2D Texture { get; private set; }
    public int SectorCount { get; private set; }

    private Dictionary<int, Color> CachedAverageColors;

    public DistantSectorMeshGenerator(WorldData worldData, TextureMapper textureMapper, Texture2D blockTextures) {
        WorldData = worldData;
        MajorSegments = worldData.MajorTorusSegments;
        MinorSegments = worldData.MinorTorusSegments;
        TextureMapper = textureMapper;
        Texture = blockTextures;

        CachedAverageColors = new Dictionary<int, Color>();
    }

    private Color GetAverageColor(int blockId) {
        if (CachedAverageColors.ContainsKey(blockId)) {
            return CachedAverageColors[blockId];
        }
        Bounds textureCoordinates = TextureMapper.GetTextureCoordinates(blockId);
        Vector3 averageColor = Vector3.zero;
        int minPixelX = (int)(textureCoordinates.min.x * Texture.width);
        int maxPixelX = (int)(textureCoordinates.max.x * Texture.width);
        int minPixelY = (int)(textureCoordinates.min.y * Texture.height);
        int maxPixelY = (int)(textureCoordinates.max.y * Texture.height);
        int pixelCount = 0;
        for (int i = minPixelX; i < maxPixelX; i++) {
            for (int j = minPixelY; j < maxPixelY; j++) {
                Color color = Texture.GetPixel(i, j);
                averageColor += new Vector3(color.r, color.g, color.b);
                pixelCount++;
            }
        }
        CachedAverageColors[blockId] = new Color(averageColor.x / pixelCount, averageColor.y / pixelCount, averageColor.z / pixelCount);
        return CachedAverageColors[blockId];
    }

    public Texture2D GetSectorTexture(int sectorIndex) {
        Texture2D texture = new Texture2D(WorldData.MajorBlockCount / WorldData.SectorCount, WorldData.MinorBlockCount);
        for (int i = 0; i < WorldData.MajorBlockCount / WorldData.SectorCount; i++) {
            for (int j = 0; j < WorldData.MinorBlockCount; j++) {
                int globalMajor = sectorIndex * (WorldData.MajorBlockCount / WorldData.SectorCount) + i;
                int globalMinor = j;
                int chunkX = globalMajor / WorldData.ChunkSize;
                int chunkY = globalMinor / WorldData.ChunkSize;
                int blockX = globalMajor % WorldData.ChunkSize;
                int blockY = globalMinor % WorldData.ChunkSize;

                ChunkData chunkData = WorldData.GetChunk(chunkX, chunkY);
                short block = chunkData.GetTopBlock(blockX, blockY);
                texture.SetPixel(i, j, GetAverageColor(block));
            }
        }
   
        texture.filterMode = FilterMode.Point;
        texture.Apply(true, false);
        return texture;
    }

    public Mesh GenerateSectorMesh(int sectorIndex, int majorResolution, int minorResolution) {
        // Simple torus sector
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int minMajorSegment = sectorIndex * majorResolution;
        int maxMajorSegment = (sectorIndex + 1) * majorResolution;

        for (int i = minMajorSegment; i < maxMajorSegment + 1; i++) {
            float majorAngle = i * 2 * Mathf.PI / (majorResolution * WorldData.SectorCount);
            for (int j = 0; j < minorResolution + 1; j++) {
                // Get height of block
                float blockX = WorldData.MajorBlockCount * i / (float)majorResolution / (float)WorldData.SectorCount;
                float blockZ = WorldData.MinorBlockCount * j / (float)minorResolution;
                ChunkData chunkData = WorldData.GetChunk((int)(blockX / WorldData.ChunkSize), (int)(blockZ / WorldData.ChunkSize));
                int height = chunkData.GetHeight((int)(blockX % WorldData.ChunkSize), (int)(blockZ % WorldData.ChunkSize));

                float heightFromSurface = WorldData.MinorRadius + height;

                float minorAngle = j * 2 * Mathf.PI / (minorResolution);
                float x = (WorldData.MajorRadius + heightFromSurface * Mathf.Cos(minorAngle)) * Mathf.Cos(majorAngle);
                float y = (WorldData.MajorRadius + heightFromSurface * Mathf.Cos(minorAngle)) * Mathf.Sin(majorAngle);
                float z = heightFromSurface * Mathf.Sin(minorAngle);

                vertices.Add(new Vector3(x, y, z));
                float u = (i - minMajorSegment) / (float)majorResolution;
                float v = j / (float)minorResolution;
                uvs.Add(new Vector2(u, v));
            }
        }

        for (int major = 0; major < majorResolution; major++) {
            for (int minor = 0; minor < minorResolution; minor++) {
                int rowLength = minorResolution + 1;
                int a = major * rowLength + minor;
                int b = (major + 1) * rowLength + minor;
                int c = (major + 1) * rowLength + (minor + 1);
                int d = major * rowLength + (minor + 1);

                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);

                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(d);
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = "TorusSector_" + sectorIndex;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        return mesh;
    }
}