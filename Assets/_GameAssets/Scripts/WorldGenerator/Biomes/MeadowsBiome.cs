using UnityEngine;

public class MeadowsBiome : Biome {

    public MeadowsBiome(Vector2 normalizedLocation) : base(normalizedLocation) { }

    public override float GetBlockHeight(int x, int z) {
        return 5.0f + 5.0f * Mathf.PerlinNoise(x / 30.0f, z / 30.0f);
    }

    public override void FillColumn(ChunkData chunkData, int x, int z, int height, int chunkHeight) {
        
        bool isTallGrass = Random.Range(0, 100) <= 10;
        for (int y = 0; y < chunkHeight; y++) {
            if (y == height)
                chunkData.SetBlock(x, y, z, 1); // Grass
            else if (y < height)
                chunkData.SetBlock(x, y, z, 2); // Dirt
            else if (isTallGrass && y == height + 1)
                chunkData.SetBlock(x, y, z, 12); // Tall Grass
            else
                chunkData.SetBlock(x, y, z, 0); // Air

            if (y == 0)
                chunkData.SetBlock(x, y, z, 8); // Dark Stone
        }
    }

    public override void PlaceStructures(WorldData worldData, int x, int z) {
        // Place structures
        if (Mathf.PerlinNoise(x / 50.0f, z / 50.0f) <= 0.3f && Random.Range(0, 100) <= 5) {
            ChunkData chunkData = worldData.GetChunkFromBlock(x, z);
            short block = chunkData.GetTopBlock(x % worldData.ChunkSize, z % worldData.ChunkSize);
            if (block != 1)
                return;

            int height = chunkData.GetHeight(x % worldData.ChunkSize, z % worldData.ChunkSize);
            worldData.SetBlock(x, height, z, 13); // Flower
        }
    }
}
