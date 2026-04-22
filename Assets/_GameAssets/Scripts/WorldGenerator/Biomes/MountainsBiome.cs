using UnityEngine;

public class MountainsBiome : Biome {

    public MountainsBiome(Vector2 normalizedLocation) : base(normalizedLocation) { }

    // Ridged noise: inverts the valley shape into sharp peaks.
    private static float Ridged(float x, float z, float scale) {
        float n = Mathf.PerlinNoise(x / scale, z / scale);
        return 1f - Mathf.Abs(n * 2f - 1f);
    }

    public override float GetBlockHeight(int x, int z) {
        // Large-scale base elevation.
        float baseShape   = Mathf.PerlinNoise(x / 80.0f + 0.5f, z / 80.0f + 0.5f);
        // Mid-scale ridges, weighted by elevation so only high areas get sharp peaks.
        float ridges      = Ridged(x, z, 32f);
        // Fine surface roughness.
        float detail      = Mathf.PerlinNoise(x / 12.0f + 3.1f, z / 12.0f + 8.9f);
        // Very fine micro-bumps.
        float micro       = Mathf.PerlinNoise(x / 5.0f  + 17.4f, z / 5.0f  + 2.6f);

        return 8f
             + 16f * baseShape
             + 28f * baseShape * ridges   // ridges only stand out on high terrain
             +  4f * detail
             +  1f * micro;
    }

    public override void FillColumn(ChunkData chunkData, int x, int z, int height, int chunkHeight) {
        // Patchy snow line: base altitude + small noise offset.
        float snowNoise = Mathf.PerlinNoise(x / 18.0f + 11.3f, z / 18.0f + 7.2f);
        int snowLine = 34 + (int)(8f * snowNoise);
        bool hasSnow = height >= snowLine;

        for (int y = 0; y < chunkHeight; y++) {
            short block;
            if (y > height) {
                block = 0; // Air
            } else if (y == height) {
                block = hasSnow ? (short)7 : (short)4; // Snow cap or bare Stone
            } else if (y >= height - 2) {
                block = 4;
            } else {
                block = (Random.Range(0, 1000) <= 1) ? (short)15 : (short)4; // Deep Stone
            }

            if (y == 0)
                block = 8; // Dark Stone floor

            chunkData.SetBlock(x, y, z, block);
        }
    }

    public override void PlaceStructures(WorldData worldData, int x, int z) {
        // Place structures
    }
}
