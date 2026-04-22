
public class ChunkData {
    public short[,,] Blocks { get; private set; }

    public ChunkData(int sizeX, int sizeY, int sizeZ)
    {
        Blocks = new short[sizeX, sizeY, sizeZ];
    }

    public short GetBlock(int x, int y, int z)
    {
        return Blocks[x, y, z];
    }

    public void SetBlock(int x, int y, int z, short block)
    {
        Blocks[x, y, z] = block;
    }

    // This could probably be cached somehow
    public short GetTopBlock(int x, int z)
    {
        for (int y = Blocks.GetLength(1) - 1; y >= 0; y--)
        {
            if (GetBlock(x, y, z) != 0)
            {
                return GetBlock(x, y, z);
            }
        }
        return 0;
    }

    public short GetHeight(int x, int z)
    {
        for (short y = 0; y < Blocks.GetLength(1); y++)
        {
            if (GetBlock(x, y, z) == 0)
            {
                return y;
            }
        }
        return 0;
    }
}