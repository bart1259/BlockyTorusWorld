using UnityEngine;
using System.Collections.Generic;

public class TextureMapper {

    public int BlockCountX { get; private set; }
    public int BlockCountY { get; private set; }
    public float TextureSizeX { get; private set; }
    public float TextureSizeY { get; private set; }
    public float MarginMultiplier { get; private set; }

    public Dictionary<int, Vector2Int> TextureCoordinates { get; private set; }

    public TextureMapper(int blockCountX, int blockCountY, float marginMultiplier = 1.0f) {
        this.BlockCountX = blockCountX;
        this.BlockCountY = blockCountY;
        this.MarginMultiplier = marginMultiplier;
        TextureSizeX = 1.0f / BlockCountX / MarginMultiplier; // Account for padding of atlas texture
        TextureSizeY = 1.0f / BlockCountY / MarginMultiplier;
        TextureCoordinates = new Dictionary<int, Vector2Int>();
    }

    public void SetTextureCoordinates(int blockId, Vector2Int textureCoordinates) {
        TextureCoordinates[blockId] = textureCoordinates;
    }

    public Bounds GetTextureCoordinates(int blockId) {
        Vector2Int textureCoordinates = TextureCoordinates[blockId];
        Vector2 center = new Vector2(
            (textureCoordinates.x + 0.5f) / BlockCountX,
            (textureCoordinates.y + 0.5f) / BlockCountY
        );
        Vector2 size = new Vector2(TextureSizeX, TextureSizeY);
        return new Bounds(new Vector3(center.x, center.y, 0.0f), new Vector3(size.x, size.y, 0.0f));
    }
}