using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {

    // The world is split into sectors (angular slices of the torus)
    //  and each sector is split into chunks (square regions on the surface of the sector)
    // Depending on the distance to the camera, the sector will either be loaded as a
    //  mesh or as a set of chunk meshes. This class performs that orchestration.s

    public int TorusSectors = 16;
    public float DistanceToChunkLoadSector = 512;

    public int Seed = 0;
    public int ChunkSize = 16;
    public int ChunkHeight = 64;
    public int MajorTorusSegments = 32;
    public int MinorTorusSegments = 32;
    public int MajorRadius = 10;
    public int MinorRadius = 1;

    public int UnloadedMajorResolution = 64;
    public int UnloadedMinorResolution = 64;

    public Material DistantSectorMaterial = null;
    public Material LoadedSectorMaterial = null;

    private Dictionary<int, bool> SectorLoaded = new Dictionary<int, bool>();
    private List<GameObject> Sectors = new List<GameObject>();

    private WorldDataGenerator WorldDataGenerator = null;
    private WorldData WorldData = null;
    private TextureMapper TextureMapper = null;
    private ChunkMeshGenerator ChunkMeshGenerator = null;

    public void Awake()
    {
        SetupWorld();
    }

    public void SetupWorld() {
        WorldDataGenerator = new WorldDataGenerator(Seed, ChunkSize, ChunkHeight, MajorTorusSegments, MinorTorusSegments);
        WorldData = new WorldData(WorldDataGenerator, MajorRadius, MinorRadius, MajorTorusSegments, MinorTorusSegments, ChunkSize, ChunkHeight, TorusSectors);
        TextureMapper = new TextureMapper(4, 4);
        TextureMapper.SetTextureCoordinates(1, new Vector2Int(0, 3));
        TextureMapper.SetTextureCoordinates(2, new Vector2Int(1, 3));
        TextureMapper.SetTextureCoordinates(3, new Vector2Int(2, 3));
        TextureMapper.SetTextureCoordinates(4, new Vector2Int(3, 3));
        TextureMapper.SetTextureCoordinates(5, new Vector2Int(0, 2));
        TextureMapper.SetTextureCoordinates(6, new Vector2Int(1, 2));
        TextureMapper.SetTextureCoordinates(7, new Vector2Int(2, 2));
        TextureMapper.SetTextureCoordinates(8, new Vector2Int(3, 2));
        TextureMapper.SetTextureCoordinates(9, new Vector2Int(0, 1));
        TextureMapper.SetTextureCoordinates(10, new Vector2Int(1, 1));
        TextureMapper.SetTextureCoordinates(11, new Vector2Int(2, 1));
        TextureMapper.SetTextureCoordinates(12, new Vector2Int(3, 1));
        TextureMapper.SetTextureCoordinates(13, new Vector2Int(0, 0)); // Flower
        TextureMapper.SetTextureCoordinates(14, new Vector2Int(1, 0)); // Dead Bush
        TextureMapper.SetTextureCoordinates(15, new Vector2Int(2, 0)); // Emerald Ore

        
        ChunkMeshGenerator = new ChunkMeshGenerator(WorldData, TextureMapper);

        for (int i = 0; i < TorusSectors; i++) {
            Debug.Log("Setting up sector " + i + " / " + TorusSectors);
            GameObject sector = new GameObject("Sector_" + i);
            sector.transform.parent = transform;
            sector.transform.position = new Vector3(
                MajorRadius * Mathf.Cos(2 * Mathf.PI * i / TorusSectors),
                MajorRadius * Mathf.Sin(2 * Mathf.PI * i / TorusSectors),
                0
            );
            SetupSector(i, sector);
            Sectors.Add(sector);
            SectorLoaded[i] = false;
        }
    }

    public void UpdatePlayerPosition(Vector3 position) {
        for (int i = 0; i < TorusSectors; i++) {
            float distance = Vector3.Distance(position, Sectors[i].transform.position);
            if (distance < DistanceToChunkLoadSector) {
                if (!SectorLoaded[i]) {
                    LoadSector(i);
                    SectorLoaded[i] = true;
                }
            }
            else {
                if (SectorLoaded[i]) {
                    UnloadSector(i);
                    SectorLoaded[i] = false;
                }
            }
        }
    }

    public void SetupSector(int sectorIndex, GameObject sector) {
        //// UNLOADED SECTOR ////

        GameObject unloadedSector = new GameObject("UnloadedSector_" + sectorIndex);
        unloadedSector.transform.parent = sector.transform;
        unloadedSector.transform.position = Vector3.zero;

        DistantSectorMeshGenerator distantSectorMeshGenerator = new DistantSectorMeshGenerator(WorldData, TextureMapper, LoadedSectorMaterial.mainTexture as Texture2D);
        Mesh mesh = distantSectorMeshGenerator.GenerateSectorMesh(sectorIndex, UnloadedMajorResolution, UnloadedMinorResolution);
        unloadedSector.AddComponent<MeshFilter>().sharedMesh = mesh;
        // Make a copy of material
        Material material = new Material(DistantSectorMaterial);
        material.mainTexture = distantSectorMeshGenerator.GetSectorTexture(sectorIndex);
        unloadedSector.AddComponent<MeshRenderer>().material = material;


        //// LOADED SECTOR ////

        GameObject loadedSector = new GameObject("LoadedSector_" + sectorIndex);
        loadedSector.transform.parent = sector.transform;
        loadedSector.transform.position = Vector3.zero;
        for (int x = 0; x < MajorTorusSegments / TorusSectors; x++) {
            for (int y = 0; y < MinorTorusSegments; y++) {
                int ChunkX = x + sectorIndex * (MajorTorusSegments / TorusSectors);
                int ChunkY = y;
                GameObject chunk = new GameObject("Chunk_" + ChunkX + "_" + ChunkY);
                chunk.transform.parent = loadedSector.transform;

                ChunkData chunkData = WorldData.GetChunk(ChunkX, ChunkY);
                Mesh chunkMesh = ChunkMeshGenerator.GenerateMesh(ChunkX, ChunkY);
                Mesh collisionMesh = ChunkMeshGenerator.GenerateMesh(ChunkX, ChunkY, true);
                chunk.AddComponent<MeshFilter>().sharedMesh = chunkMesh;
                chunk.AddComponent<MeshCollider>().sharedMesh = collisionMesh;
                chunk.AddComponent<MeshRenderer>().material = LoadedSectorMaterial;
            }
        }

        loadedSector.SetActive(false);
    }

    public void LoadSector(int sectorIndex) {
        // Make chunk meshes for all chunks in the sector
        GameObject loadedSector = Sectors[sectorIndex].transform.Find("LoadedSector_" + sectorIndex).gameObject;
        GameObject unloadedSector = Sectors[sectorIndex].transform.Find("UnloadedSector_" + sectorIndex).gameObject;
        unloadedSector.SetActive(false);
        loadedSector.SetActive(true);
    }

    public void UnloadSector(int sectorIndex) {
        // Destory all chunk meshes in the sector
        GameObject loadedSector = Sectors[sectorIndex].transform.Find("LoadedSector_" + sectorIndex).gameObject;
        GameObject unloadedSector = Sectors[sectorIndex].transform.Find("UnloadedSector_" + sectorIndex).gameObject;
        unloadedSector.SetActive(true);
        loadedSector.SetActive(false);
    }

    public void SetBlock(Vector3 position, short block) {
        // Undo torus wrapping.
        Vector3 localPosition = ChunkMeshGenerator.WorldToLocalPosition(position, 0, 0);
        localPosition += new Vector3(0.5f, 0.5f, 0.5f);
        int majorIndex = (int)localPosition.x / WorldData.ChunkSize;
        int minorIndex = (int)localPosition.z / WorldData.ChunkSize;

        majorIndex = ((majorIndex % WorldData.MajorTorusSegments) + WorldData.MajorTorusSegments) % WorldData.MajorTorusSegments;
        minorIndex = ((minorIndex % WorldData.MinorTorusSegments) + WorldData.MinorTorusSegments) % WorldData.MinorTorusSegments;

        WorldData.SetBlock((int)localPosition.x, (int)localPosition.y, (int)localPosition.z, block);
        Debug.Log("Set block at " + position + " to " + block);

        // Find chunk
        string chunkName = "Chunk_" + majorIndex + "_" + minorIndex;
        Debug.Log("Chunk name: " + chunkName);
        GameObject chunk = RecursiveFindChild(transform, chunkName).gameObject;
        if (chunk != null) {
            chunk.GetComponent<MeshFilter>().sharedMesh = ChunkMeshGenerator.GenerateMesh(majorIndex, minorIndex);
            chunk.GetComponent<MeshCollider>().sharedMesh = ChunkMeshGenerator.GenerateMesh(majorIndex, minorIndex, true);
        }
    }

    Transform RecursiveFindChild(Transform parent, string name) {
        foreach (Transform child in parent) {
            if (child.name == name) return child;
            Transform found = RecursiveFindChild(child, name);
            if (found != null) return found;
        }
        return null;
    }

}