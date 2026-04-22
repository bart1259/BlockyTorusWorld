using UnityEngine;

public abstract class Biome {

    public Vector2 Location { get; private set; }

    public Biome(Vector2 location) {
        Location = location;
    }

    public abstract float GetBlockHeight(int x, int z);
    public abstract void FillColumn(ChunkData chunkData, int x, int z, int height, int chunkHeight);
    public abstract void PlaceStructures(WorldData worldData, int x, int z);
}