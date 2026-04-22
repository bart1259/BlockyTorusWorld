using UnityEngine;
using System.Collections.Generic;

public class BiomeManager {

    public List<Biome> Biomes { get; private set; }

    private readonly int majorBlockCount;
    private readonly int minorBlockCount;

    public BiomeManager(int majorBlockCount, int minorBlockCount) {
        Biomes = new List<Biome>();
        this.majorBlockCount = majorBlockCount;
        this.minorBlockCount = minorBlockCount;
    }

    public void AddBiome(Biome biome) {
        Biomes.Add(biome);
    }

    // Accounts for torus wrapping on both axes by testing all four offset combinations.
    private float TorusDistance(Vector2 a, Vector2 b) {
        float dx = Mathf.Abs(a.x - b.x);
        dx = Mathf.Min(dx, majorBlockCount - dx);
        float dz = Mathf.Abs(a.y - b.y);
        dz = Mathf.Min(dz, minorBlockCount - dz);
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    public Biome GetNearestBiome(Vector2 blockLocation) {
        Biome nearestBiome = null;
        float nearestDistance = float.MaxValue;
        foreach (Biome biome in Biomes) {
            float distance = TorusDistance(blockLocation, biome.Location);
            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearestBiome = biome;
            }
        }
        return nearestBiome;
    }

    // transitionDistance is expressed in blocks
    public BiomeBlend GetBiomeBlend(Vector2 blockLocation, int worldX, int worldZ, float transitionDistance) {
        float[] distances = new float[Biomes.Count];
        for (int i = 0; i < Biomes.Count; i++)
            distances[i] = TorusDistance(blockLocation, Biomes[i].Location);

        int firstClosest = 0, secondClosest = 0;
        float minDist = float.MaxValue, secondMinDist = float.MaxValue;
        for (int i = 0; i < distances.Length; i++) {
            if (distances[i] < minDist) {
                secondClosest = firstClosest;
                secondMinDist = minDist;
                firstClosest = i;
                minDist = distances[i];
            } else if (distances[i] < secondMinDist) {
                secondClosest = i;
                secondMinDist = distances[i];
            }
        }

        float firstHeight = Biomes[firstClosest].GetBlockHeight(worldX, worldZ);
        float diff = secondMinDist - minDist;
        if (diff < transitionDistance) {
            float t = Mathf.SmoothStep(0f, 1f, diff / transitionDistance);
            float primaryWeight = 0.5f + 0.5f * t;
            float secondHeight = Biomes[secondClosest].GetBlockHeight(worldX, worldZ);
            float blendedHeight = primaryWeight * firstHeight + (1f - primaryWeight) * secondHeight;
            return new BiomeBlend(Biomes[firstClosest], Biomes[secondClosest], primaryWeight, blendedHeight);
        }
        return new BiomeBlend(Biomes[firstClosest], firstHeight);
    }
}
