public struct BiomeBlend {

    public readonly Biome Primary;
    public readonly Biome Secondary;
    public readonly float PrimaryWeight;
    public readonly float Height;

    public bool IsBlended => Secondary != null && PrimaryWeight < 1f;

    public BiomeBlend(Biome primary, float height) {
        Primary = primary;
        Secondary = null;
        PrimaryWeight = 1f;
        Height = height;
    }

    public BiomeBlend(Biome primary, Biome secondary, float primaryWeight, float height) {
        Primary = primary;
        Secondary = secondary;
        PrimaryWeight = primaryWeight;
        Height = height;
    }
}
