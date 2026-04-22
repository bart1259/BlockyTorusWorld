using UnityEngine;
using System.Collections.Generic;

public class ChunkMeshGenerator {

    public int MajorSegments { get; private set; }
    public int MinorSegments { get; private set; }
    public WorldData WorldData { get; private set; }
    public TextureMapper TextureMapper { get; private set; }

    public ChunkMeshGenerator(WorldData worldData, TextureMapper textureMapper) {
        WorldData = worldData;
        MajorSegments = worldData.MajorTorusSegments;
        MinorSegments = worldData.MinorTorusSegments;
        TextureMapper = textureMapper;
    }

    public static bool IsTransparent(int blockId) {
        return blockId == 0 || blockId == 11 || blockId == 12 || blockId == 13 || blockId == 14;
    }

    public static bool IsTransparentNotWater(int blockId) {
        return blockId == 0;
    }

    public Mesh GenerateMesh(int majorIndex, int minorIndex, bool collisionMesh = false) {


        void AddFace(int blockId, Vector3 center, Direction direction, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uv, float trimTop = 0.0f)
        {

            if (direction == Direction.Top) {
                vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y - trimTop, center.z + 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y - trimTop, center.z + 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y - trimTop, center.z - 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y - trimTop, center.z - 0.5f), majorIndex, minorIndex));
            }
            else if (direction == Direction.Bottom) {
                vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y, center.z + 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y, center.z + 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y, center.z - 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y, center.z - 0.5f), majorIndex, minorIndex));
            } else if (direction == Direction.Left || direction == Direction.Right) {
                vertices.Add(ToWorldPosition(new Vector3(center.x, center.y - trimTop + 0.5f, center.z + 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x, center.y - trimTop + 0.5f, center.z - 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x, center.y - 0.5f, center.z - 0.5f), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x, center.y - 0.5f, center.z + 0.5f), majorIndex, minorIndex));
            } else if (direction == Direction.Front || direction == Direction.Back) {
                vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y - trimTop + 0.5f, center.z), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y - trimTop + 0.5f, center.z), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y - 0.5f, center.z), majorIndex, minorIndex));
                vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y - 0.5f, center.z), majorIndex, minorIndex));
            }

            if (direction == Direction.Top || direction == Direction.Front || direction == Direction.Right) {
                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);
            }
            else if (direction == Direction.Bottom || direction == Direction.Back || direction == Direction.Left) {
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 4);
            }

            Bounds textureCoordinates = TextureMapper.GetTextureCoordinates(blockId);
            uv.Add(new Vector2(textureCoordinates.min.x, textureCoordinates.min.y));
            uv.Add(new Vector2(textureCoordinates.max.x, textureCoordinates.min.y));
            uv.Add(new Vector2(textureCoordinates.max.x, textureCoordinates.max.y));
            uv.Add(new Vector2(textureCoordinates.min.x, textureCoordinates.max.y));
        }

        void AddBillboardFaces(int blockId, Vector3 center, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uv)
        {
            if (collisionMesh) {
                return;
            }
            Bounds tc = TextureMapper.GetTextureCoordinates(blockId);

            // Diagonal 1: runs from (-x, -z) corner to (+x, +z) corner
            int b1 = vertices.Count;
            vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y - 0.5f, center.z - 0.5f), majorIndex, minorIndex)); // bottom-left
            vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y - 0.5f, center.z + 0.5f), majorIndex, minorIndex)); // bottom-right
            vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y + 0.5f, center.z + 0.5f), majorIndex, minorIndex)); // top-right
            vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y + 0.5f, center.z - 0.5f), majorIndex, minorIndex)); // top-left
            triangles.Add(b1 + 0); triangles.Add(b1 + 1); triangles.Add(b1 + 2);
            triangles.Add(b1 + 0); triangles.Add(b1 + 2); triangles.Add(b1 + 3);
            // triangles.Add(b1 + 0); triangles.Add(b1 + 2); triangles.Add(b1 + 1);
            // triangles.Add(b1 + 0); triangles.Add(b1 + 3); triangles.Add(b1 + 2);
            uv.Add(new Vector2(tc.min.x, tc.min.y));
            uv.Add(new Vector2(tc.max.x, tc.min.y));
            uv.Add(new Vector2(tc.max.x, tc.max.y));
            uv.Add(new Vector2(tc.min.x, tc.max.y));

            // Diagonal 2: runs from (+x, -z) corner to (-x, +z) corner
            int b2 = vertices.Count;
            vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y - 0.5f, center.z - 0.5f), majorIndex, minorIndex)); // bottom-left
            vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y - 0.5f, center.z + 0.5f), majorIndex, minorIndex)); // bottom-right
            vertices.Add(ToWorldPosition(new Vector3(center.x - 0.5f, center.y + 0.5f, center.z + 0.5f), majorIndex, minorIndex)); // top-right
            vertices.Add(ToWorldPosition(new Vector3(center.x + 0.5f, center.y + 0.5f, center.z - 0.5f), majorIndex, minorIndex)); // top-left
            triangles.Add(b2 + 0); triangles.Add(b2 + 1); triangles.Add(b2 + 2);
            triangles.Add(b2 + 0); triangles.Add(b2 + 2); triangles.Add(b2 + 3);
            // triangles.Add(b2 + 0); triangles.Add(b2 + 2); triangles.Add(b2 + 1);
            // triangles.Add(b2 + 0); triangles.Add(b2 + 3); triangles.Add(b2 + 2);
            uv.Add(new Vector2(tc.min.x, tc.min.y));
            uv.Add(new Vector2(tc.max.x, tc.min.y));
            uv.Add(new Vector2(tc.max.x, tc.max.y));
            uv.Add(new Vector2(tc.min.x, tc.max.y));
        }

        
        ChunkData chunkData = WorldData.GetChunk(majorIndex, minorIndex);
        ChunkData posMajorChunkData = WorldData.GetChunk(majorIndex + 1, minorIndex);
        ChunkData negMajorChunkData = WorldData.GetChunk(majorIndex - 1, minorIndex);
        ChunkData posMinorChunkData = WorldData.GetChunk(majorIndex, minorIndex + 1);
        ChunkData negMinorChunkData = WorldData.GetChunk(majorIndex, minorIndex - 1);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();  
        List<Vector2> uv = new List<Vector2>();

        int ChunkSize = WorldData.ChunkSize;
        int ChunkHeight = WorldData.ChunkHeight;

        for (int x = 0; x < ChunkSize; x++) {
            for (int y = 0; y < ChunkHeight; y++) {
                for (int z = 0; z < ChunkSize; z++) {
                    // Check if the block is not air
                    int blockId = chunkData.GetBlock(x, y, z);
                    if (blockId == 11) {
                        // Water rendering
                        bool waterAbove = y == ChunkHeight - 1 || chunkData.GetBlock(x, y + 1, z) == 11;
                        if (!waterAbove) {
                            AddFace(blockId, new Vector3(x, y, z), Direction.Top, ref vertices, ref triangles, ref uv, 0.05f);
                        }
                        bool airBelow = y == 0 || chunkData.GetBlock(x, y - 1, z) == 0;
                        if (airBelow) {
                            AddFace(blockId, new Vector3(x, y, z), Direction.Bottom, ref vertices, ref triangles, ref uv);
                        }
                        bool leftNeighborIsTransparent = x == 0
                            ? IsTransparentNotWater(negMajorChunkData.GetBlock(ChunkSize - 1, y, z))
                            : IsTransparentNotWater(chunkData.GetBlock(x - 1, y, z));
                        if (leftNeighborIsTransparent) {
                            AddFace(blockId, new Vector3(x - 0.5f, y, z), Direction.Left, ref vertices, ref triangles, ref uv);
                        }
                        bool rightNeighborIsTransparent = x == ChunkSize - 1
                            ? IsTransparentNotWater(posMajorChunkData.GetBlock(0, y, z))
                            : IsTransparentNotWater(chunkData.GetBlock(x + 1, y, z));
                        if (rightNeighborIsTransparent) {
                            AddFace(blockId, new Vector3(x + 0.5f, y, z), Direction.Right, ref vertices, ref triangles, ref uv);
                        }
                        bool frontNeighborIsTransparent = z == 0
                            ? IsTransparentNotWater(negMinorChunkData.GetBlock(x, y, ChunkSize - 1))
                            : IsTransparentNotWater(chunkData.GetBlock(x, y, z - 1));
                        if (frontNeighborIsTransparent) {
                            AddFace(blockId, new Vector3(x, y, z - 0.5f), Direction.Front, ref vertices, ref triangles, ref uv);
                        }
                        bool backNeighborIsTransparent = z == ChunkSize - 1
                            ? IsTransparentNotWater(posMinorChunkData.GetBlock(x, y, 0))
                            : IsTransparentNotWater(chunkData.GetBlock(x, y, z + 1));
                        if (backNeighborIsTransparent) {
                            AddFace(blockId, new Vector3(x, y, z + 0.5f), Direction.Back, ref vertices, ref triangles, ref uv);
                        }
                    }
                    else if (blockId == 12 || blockId == 13 || blockId == 14)
                    {
                        AddBillboardFaces(blockId, new Vector3(x, y, z), ref vertices, ref triangles, ref uv);
                    }
                    else if (blockId != 0) {
                        // Check top neighbor
                        if (y == ChunkHeight - 1 || IsTransparent(chunkData.GetBlock(x, y + 1, z))) {
                            // Add top face
                            AddFace(blockId, new Vector3(x, y + 0.5f, z), Direction.Top, ref vertices, ref triangles, ref uv);
                        }
                        if (y == 0 || IsTransparent(chunkData.GetBlock(x, y - 1, z))) {
                            // Add bottom face
                            AddFace(blockId, new Vector3(x, y - 0.5f, z), Direction.Bottom, ref vertices, ref triangles, ref uv);
                        }
                        bool leftNeighborIsTransparent = x == 0
                            ? IsTransparent(negMajorChunkData.GetBlock(ChunkSize - 1, y, z))
                            : IsTransparent(chunkData.GetBlock(x - 1, y, z));
                        if (leftNeighborIsTransparent) {
                            // Add left face
                            AddFace(blockId, new Vector3(x - 0.5f, y, z), Direction.Left, ref vertices, ref triangles, ref uv);
                        }
                        bool rightNeighborIsTransparent = x == ChunkSize - 1
                            ? IsTransparent(posMajorChunkData.GetBlock(0, y, z))
                            : IsTransparent(chunkData.GetBlock(x + 1, y, z));
                        if (rightNeighborIsTransparent) {
                            // Add right face
                            AddFace(blockId, new Vector3(x + 0.5f, y, z), Direction.Right, ref vertices, ref triangles, ref uv);
                        }
                        bool frontNeighborIsTransparent = z == 0
                            ? IsTransparent(negMinorChunkData.GetBlock(x, y, ChunkSize - 1))
                            : IsTransparent(chunkData.GetBlock(x, y, z - 1));
                        if (frontNeighborIsTransparent) {
                            // Add front face
                            AddFace(blockId, new Vector3(x, y, z - 0.5f), Direction.Front, ref vertices, ref triangles, ref uv);
                        }
                        bool backNeighborIsTransparent = z == ChunkSize - 1
                            ? IsTransparent(posMinorChunkData.GetBlock(x, y, 0))
                            : IsTransparent(chunkData.GetBlock(x, y, z + 1));
                        if (backNeighborIsTransparent) {
                            // Add back face
                            AddFace(blockId, new Vector3(x, y, z + 0.5f), Direction.Back, ref vertices, ref triangles, ref uv);
                        }
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Chunk" + majorIndex + "_" + minorIndex;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        return mesh;
    }

    // Maps a local chunk vertex to its 3D world position on the torus surface.
    // localVert.x/z are chunk-space horizontal coords; localVert.y is height above the surface.
    // majorIndex/minorIndex identify which chunk in the torus grid this vertex belongs to.
    public Vector3 ToWorldPosition(Vector3 localVert, int majorIndex, int minorIndex) {
        Vector2 unwrappedPosition = new Vector2(
            localVert.x + (majorIndex * WorldData.ChunkSize),
            localVert.z + (minorIndex * WorldData.ChunkSize)
        );

        // [0, size) -> [0, 2*pi)
        float majorAngle = 2 * Mathf.PI * unwrappedPosition.x / (WorldData.MajorTorusSegments * WorldData.ChunkSize);
        float minorAngle = 2 * Mathf.PI * unwrappedPosition.y / (WorldData.MinorTorusSegments * WorldData.ChunkSize);
        float height = localVert.y;
        float R = WorldData.MajorRadius;
        float r = WorldData.MinorRadius;
        // Torus 3D parameterization with height from the surface:
        // x = (R + (r + height) * cos(minor)) * cos(major)
        // y = (R + (r + height) * cos(minor)) * sin(major)
        // z = (r + height) * sin(minor)
        float tubeRadius = r + height;
        return new Vector3(
            (R + tubeRadius * Mathf.Cos(minorAngle)) * Mathf.Cos(majorAngle),
            (R + tubeRadius * Mathf.Cos(minorAngle)) * Mathf.Sin(majorAngle),
            tubeRadius * Mathf.Sin(minorAngle)
        );
    }

    // Inverse of ToWorldPosition: maps a 3D world position back to local chunk-space coordinates.
    // Returns a Vector3 where x/z are local horizontal coords within the chunk and y is height above the surface.
    public Vector3 WorldToLocalPosition(Vector3 worldPos, int majorIndex, int minorIndex) {
        float R = WorldData.MajorRadius;
        float r = WorldData.MinorRadius;

        // Recover major angle from the XY-plane projection
        float majorAngle = Mathf.Atan2(worldPos.y, worldPos.x);
        if (majorAngle < 0) majorAngle += 2 * Mathf.PI;

        // Distance from the world Z-axis gives us the radius in XY
        float distXY = Mathf.Sqrt(worldPos.x * worldPos.x + worldPos.y * worldPos.y);

        // Recover minor angle from the tube cross-section
        float minorAngle = Mathf.Atan2(worldPos.z, distXY - R);
        if (minorAngle < 0) minorAngle += 2 * Mathf.PI;

        // Tube radius encodes height: tubeRadius = r + (localY * 0.8)
        float tubeRadius = Mathf.Sqrt((distXY - R) * (distXY - R) + worldPos.z * worldPos.z);
        float localY = (tubeRadius - r);

        // Angles back to unwrapped grid coordinates
        float unwrappedX = majorAngle / (2 * Mathf.PI) * (WorldData.MajorTorusSegments * WorldData.ChunkSize);
        float unwrappedZ = minorAngle / (2 * Mathf.PI) * (WorldData.MinorTorusSegments * WorldData.ChunkSize);

        return new Vector3(
            unwrappedX - majorIndex * WorldData.ChunkSize,
            localY,
            unwrappedZ - minorIndex * WorldData.ChunkSize
        );
    }
}