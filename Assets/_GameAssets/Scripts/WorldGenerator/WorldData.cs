using System.Collections.Generic;

public class WorldData {
    // Effectively a cache for all the chunk data
    public Dictionary<int, Dictionary<int, ChunkData>> Chunks { get; private set; }
    public WorldDataGenerator WorldDataGenerator { get; private set; }

    public int MajorTorusSegments { get; private set; }
    public int MinorTorusSegments { get; private set; }
    public int MajorBlockCount => MajorTorusSegments * ChunkSize;
    public int MinorBlockCount => MinorTorusSegments * ChunkSize;
    public int ChunkSize { get; private set; }
    public int ChunkHeight { get; private set; }
    public int MajorRadius { get; private set; }
    public int MinorRadius { get; private set; }
    public int SectorCount { get; private set; }

    public WorldData(WorldDataGenerator worldDataGenerator, int majorRadius, int minorRadius, int majorTorusSegments, int minorTorusSegments, int chunkSize, int chunkHeight, int sectorCount) {
        WorldDataGenerator = worldDataGenerator;
        Chunks = new Dictionary<int, Dictionary<int, ChunkData>>();
        MajorRadius = majorRadius;
        MinorRadius = minorRadius;
        MajorTorusSegments = majorTorusSegments;
        MinorTorusSegments = minorTorusSegments;
        ChunkSize = chunkSize;
        ChunkHeight = chunkHeight;
        SectorCount = sectorCount;

        // Pre-generate all chunks
        for (int majorIndex = 0; majorIndex < MajorTorusSegments; majorIndex++) {
            for (int minorIndex = 0; minorIndex < MinorTorusSegments; minorIndex++) {
                GenChunk(majorIndex, minorIndex);
            }
        }

        // Place all structures
        for (int majorIndex = 0; majorIndex < MajorTorusSegments; majorIndex++) {
            for (int minorIndex = 0; minorIndex < MinorTorusSegments; minorIndex++) {
                WorldDataGenerator.PlaceStructures(this, majorIndex * ChunkSize, minorIndex * ChunkSize);
            }
        }
    }

    public void GenChunk(int majorIndex, int minorIndex) {
        if (!Chunks.ContainsKey(majorIndex)) {
            Chunks[majorIndex] = new Dictionary<int, ChunkData>();
        }
        Chunks[majorIndex][minorIndex] = WorldDataGenerator.GenerateChunk(majorIndex, minorIndex);
    }

    public ChunkData GetChunk(int majorIndex, int minorIndex) {
        majorIndex = ((majorIndex % MajorTorusSegments) + MajorTorusSegments) % MajorTorusSegments;
        minorIndex = ((minorIndex % MinorTorusSegments) + MinorTorusSegments) % MinorTorusSegments;
        return Chunks[majorIndex][minorIndex];
    }

    public ChunkData GetChunkFromBlock(int x, int z) {
        if (x < 0) x += MajorBlockCount;
        if (z < 0) z += MinorBlockCount;
        int majorIndex = x / ChunkSize;
        int minorIndex = z / ChunkSize;
        return GetChunk(majorIndex, minorIndex);
    }

    public void SetBlock(int x, int y, int z, short block) {
        if (x < 0) x += MajorBlockCount;
        if (z < 0) z += MinorBlockCount;
        int majorIndex = x / ChunkSize;
        int minorIndex = z / ChunkSize;
        GetChunk(majorIndex, minorIndex).SetBlock(x % ChunkSize, y, z % ChunkSize, block);
    }
}