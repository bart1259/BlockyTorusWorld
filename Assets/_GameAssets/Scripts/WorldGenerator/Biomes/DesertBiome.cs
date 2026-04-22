using UnityEngine;

public class DesertBiome : Biome {

    public DesertBiome(Vector2 normalizedLocation) : base(normalizedLocation) { }

    public override float GetBlockHeight(int x, int z) {
        return 5.0f + 5.0f * Mathf.PerlinNoise(x / 30.0f, z / 30.0f);
    }

    public override void FillColumn(ChunkData chunkData, int x, int z, int height, int chunkHeight) {
        bool isCactus = Random.Range(0, 1000) < 2;
        bool isDeadBush = Random.Range(0, 1000) < 2;
        if (isCactus)
            isDeadBush = false;

        int cactusHeight = Random.Range(2, 4);
        for (int y = 0; y < chunkHeight; y++) {
            if (y == height)
                chunkData.SetBlock(x, y, z, 3); // Sand
            else if (y < height)
                chunkData.SetBlock(x, y, z, 4); // Stone
            else if (isCactus && y > height && y <= height + cactusHeight)
                chunkData.SetBlock(x, y, z, 9); // Cactus
            else if (isDeadBush && y > height && y <= height + 1)
                chunkData.SetBlock(x, y, z, 14); // Dead Bush
            else
                chunkData.SetBlock(x, y, z, 0); // Air

            if (y == 0)
                chunkData.SetBlock(x, y, z, 8); // Dark Stone
        }
    }

    public override void PlaceStructures(WorldData worldData, int x, int z) {
        // Place structures
    }
}
