using UnityEngine;
using System.Collections.Generic;

public class WorldDataGenerator {

    public int Seed { get; private set; }
    public int ChunkSize { get; private set; }
    public int ChunkHeight { get; private set; }
    public int MajorTorusSegments { get; private set; } // Chunks across major torus direction
    public int MinorTorusSegments { get; private set; } // Chunks across minor torus direction

    // 0 - Air
    // 1 - Grass
    // 2 - Dirt
    // 3 - Sand
    // 4 - Stone
    // 5 - Bark
    // 6 - Leaves
    // 7 - Snow
    // 8 - Dark Rock
    // 9 - Cactus

    private const float BiomeTransitionBlocks = 100f;

    private BiomeManager biomeManager;

    public WorldDataGenerator(int seed, int chunkSize, int chunkHeight, int majorTorusSegments, int minorTorusSegments)
    {
        Seed = seed;
        ChunkSize = chunkSize;
        ChunkHeight = chunkHeight;
        MajorTorusSegments = majorTorusSegments;
        MinorTorusSegments = minorTorusSegments;

        int majorBlockCount = majorTorusSegments * chunkSize;
        int minorBlockCount = minorTorusSegments * chunkSize;

        biomeManager = new BiomeManager(majorBlockCount, minorBlockCount);
        biomeManager.AddBiome(new DesertBiome(  new Vector2(0,                         0)));
        biomeManager.AddBiome(new MeadowsBiome(    new Vector2(majorBlockCount / 4f,      0)));
        biomeManager.AddBiome(new ForestBiome(    new Vector2(majorBlockCount / 2f,      0)));
        biomeManager.AddBiome(new MountainsBiome( new Vector2(majorBlockCount * 3f / 4f, 0)));
    }

    public ChunkData GenerateChunk(int majorIndex, int minorIndex)
    {
        ChunkData chunkData = new ChunkData(ChunkSize, ChunkHeight, ChunkSize);

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                int worldX = majorIndex * ChunkSize + x;
                int worldZ = minorIndex * ChunkSize + z;
                Vector2 blockLocation = new Vector2(worldX, worldZ);

                BiomeBlend blend = biomeManager.GetBiomeBlend(blockLocation, worldX, worldZ, BiomeTransitionBlocks);
                Biome columnBiome = blend.IsBlended && UnityEngine.Random.value > blend.PrimaryWeight
                    ? blend.Secondary
                    : blend.Primary;
                columnBiome.FillColumn(chunkData, x, z, (int)blend.Height, ChunkHeight);

                int blockX = (ChunkSize * majorIndex) + x;
                int offset = (int)(
                    (Mathf.Sin(blockX * 0.005f) * 60) +
                    (Mathf.Sin(blockX * 0.02f) * 20)
                );
                int halfSize = 6;
                int blockZ = (ChunkSize * minorIndex) + z;  
                if (blockZ > 256 - halfSize + offset && blockZ < 256 + halfSize + offset) {
                    // River
                    for (int y = 0; y < ChunkHeight; y++) {
                        if (y == 0) {
                            chunkData.SetBlock(x, y, z, 3); // Sand
                        } else if (y <= 4) {
                            chunkData.SetBlock(x, y, z, 11); // Water
                        } else {
                            chunkData.SetBlock(x, y, z, 0); // Air
                        }
                    }
                } else if (blockZ > 256 - (2 * halfSize) + offset && blockZ < 256 + (2 * halfSize) + offset) {
                    float riverBankness = 0f;
                    int leftInner = 256 - halfSize + offset;
                    int rightInner = 256 + halfSize + offset;
                    int leftOuter = 256 - (2 * halfSize) + offset;
                    int rightOuter = 256 + (2 * halfSize) + offset;

                    if (blockZ >= leftOuter && blockZ < leftInner)
                        riverBankness = Mathf.Clamp01((float)(blockZ - leftOuter) / (leftInner - leftOuter));
                    else if (blockZ > rightInner && blockZ <= rightOuter)
                        riverBankness = Mathf.Clamp01((float)(rightOuter - blockZ) / (rightOuter - rightInner));
                    else
                        riverBankness = 1f;

                    riverBankness = Mathf.SmoothStep(0f, 1f, riverBankness);
                    float bankHeight = Mathf.Lerp(blend.Height, 0f, riverBankness);
                    int bankTop = Mathf.FloorToInt(bankHeight);

                    for (int y = 0; y < ChunkHeight; y++) {
                        if (y == bankTop && bankTop < 8) {
                            chunkData.SetBlock(x, y, z, 3); // Sand surface
                        } else if (y > bankTop && y <= 4) {
                            chunkData.SetBlock(x, y, z, 11); // Water
                        } else if (y > bankTop) {
                            chunkData.SetBlock(x, y, z, 0); // Air
                        }
                        // y < bankTop: leave biome fill intact
                    }
                }
            }
        }

        return chunkData;
    }

    public void PlaceStructures(WorldData worldData, int chunkX, int chunkZ) {
        for (int x = chunkX; x < chunkX + ChunkSize; x++) {
            for (int z = chunkZ; z < chunkZ + ChunkSize; z++) {
                BiomeBlend blend = biomeManager.GetBiomeBlend(new Vector2(x, z), x, z, BiomeTransitionBlocks);
                blend.Primary.PlaceStructures(worldData, x, z);
            }
        }
    }
}
