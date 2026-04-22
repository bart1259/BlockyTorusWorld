using UnityEngine;

public class ForestBiome : Biome {

    public ForestBiome(Vector2 normalizedLocation) : base(normalizedLocation) { }

    public override float GetBlockHeight(int x, int z) {
        return (5.0f + 4.0f * Mathf.PerlinNoise(x / 30.0f, z / 30.0f))
        + 8.0f * Mathf.PerlinNoise(x / 90.0f, z / 90.0f);
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
        float treeDensity = Mathf.PerlinNoise(x / 100.0f, z / 100.0f);
        treeDensity *= 10;
        treeDensity += 3;
        if (Random.Range(0, 1000) <= treeDensity) {
            int treeHeight = Random.Range(5, 10);
            ChunkData chunkData = worldData.GetChunkFromBlock(x, z);
            int height = chunkData.GetHeight(x % worldData.ChunkSize, z % worldData.ChunkSize);
            short block = chunkData.GetTopBlock(x % worldData.ChunkSize, z % worldData.ChunkSize);
            if (block != 1)
                return;

            int top = height + treeHeight;
            for (int y = height; y <= top; y++) {
                worldData.SetBlock(x, y, z, 5); // Bark
                if (y > top - 3) {
                    for (int xx = -2; xx <= 2; xx++) {
                        for (int zz = -2; zz <= 2; zz++) {
                            if (xx == 0 && zz == 0) continue;
                            worldData.SetBlock(x + xx, y, z + zz, 6); // Leaves
                        }
                    }
                }
            }
            // Leaf cap above the trunk
            for (int xx = -1; xx <= 1; xx++) {
                for (int zz = -1; zz <= 1; zz++) {
                    worldData.SetBlock(x + xx, top + 1, z + zz, 6); // Leaves
                }
            }
        }
    }
}
