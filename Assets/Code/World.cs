using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int MapSize = 16;
    public int ChunkSize = 16;
    public int ChunkHeight = 100;
    public int StoneThreshold = 30;
    public int WaterThreshold = 50;
    public float NoiseScale = 0.03f;
    public GameObject ChunkPrefab;
    Dictionary<Vector3Int, ChunkData> ChunksData = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> ChunksInstances = new Dictionary<Vector3Int, ChunkRenderer>();
    private void Start()
    {

    }
    /// <summary>
    /// 生成世界
    /// </summary>
    public void GenerateWorld()
    {
        ChunksData.Clear();
        foreach (var chunk in ChunksInstances.Values)
        {
            Destroy(chunk.gameObject);
        }
        ChunksInstances.Clear();

        for (int x = 0; x < MapSize; x++)
        {
            for (int z = 0; z < MapSize; z++)
            {
                var chunkData = new ChunkData(ChunkSize, ChunkHeight, new Vector3Int(x, 0, z));

                ChunksData.Add(chunkData.ChunkPosition, chunkData);
            }
        }
    }
    /// <summary>
    /// 生成块
    /// </summary>
    /// <param name="chunkData"></param>
    private void GenerateBlock(ChunkData chunkData)
    {
        float baseX = chunkData.ChunkPosition.x * NoiseScale;
        float baseZ = chunkData.ChunkPosition.z * NoiseScale;
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise(baseX + x * NoiseScale, baseZ + z * NoiseScale);
                var groundHeight = Mathf.RoundToInt(noiseValue * ChunkHeight); // 地面高度
                for (int y = 0; y < ChunkHeight; y++)
                {
                    BlockType blockType;
                    if (y < groundHeight)
                    {
                        if (y <= StoneThreshold)
                        {
                            blockType = BlockType.Stone;
                        }
                        else
                        {
                            blockType = BlockType.Dirt;
                        }
                    }
                    else if (y == groundHeight)
                    {
                        if (y < WaterThreshold)
                        {
                            blockType = BlockType.Sand;
                        }
                        else
                        {
                            blockType = BlockType.Grass;
                        }
                    }
                    else
                    {
                        if (y <= WaterThreshold)
                        {
                            blockType = BlockType.Water;
                        }
                        else
                        {
                            blockType = BlockType.Air; // 水面上为空气
                        }
                    }

                }
            }
        }
    }
}
