using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

public class World : MonoBehaviour, IController
{
    public int MapSize = 16;
    public int ChunkSize = 16;
    public int ChunkHeight = 100;
    public int StoneThreshold = 30;
    public int WaterThreshold = 50;
    public float NoiseScale = 0.03f;
    public GameObject ChunkPrefab;
    private Vector3[] _directions =
    {
        Vector3.back,
        Vector3.down,
        Vector3.forward,
        Vector3.left,
        Vector3.right,
        Vector3.up
    };

    private Dictionary<Vector3Int, ChunkData> _chunksData = new Dictionary<Vector3Int, ChunkData>();
    private Dictionary<Vector3Int, ChunkRenderer> _chunksInstances = new Dictionary<Vector3Int, ChunkRenderer>();
    private void Start()
    {
        ResKit.Init();
        GenerateWorld();
    }
    /// <summary>
    /// 生成世界
    /// </summary>
    public void GenerateWorld()
    {
        var blockModel = this.GetModel<BlockTextureModel>();
        _chunksData.Clear();
        foreach (var chunk in _chunksInstances.Values)
        {
            Destroy(chunk.gameObject);
        }
        _chunksInstances.Clear();

        for (int x = 0; x < MapSize; x++)
        {
            for (int z = 0; z < MapSize; z++)
            {
                var chunkData = new ChunkData(ChunkSize, ChunkHeight, new Vector3Int(x, 0, z));
                GenerateBlock(chunkData);
                _chunksData.Add(chunkData.Position, chunkData);
                GameObject chunkObject = Instantiate(ChunkPrefab, chunkData.Position, Quaternion.identity);
                // bool isMainMesh = blockType == BlockType.Water ? false : true;
                MeshData meshData = new MeshData(true);
                foreach (var block in chunkData.Blocks)
                {
                    BlockHelper.SetMeshData(meshData, block, blockModel);
                }
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                Debug.Log($"区块位置{chunkData.Position} + 方块类型{chunkRenderer}");
                _chunksInstances.Add(chunkData.Position, chunkRenderer);
                chunkRenderer.InitializeChunk(chunkData);
                chunkRenderer.UpdateChunk(meshData);
            }
        }
    }


    /// <summary>
    /// 生成块
    /// </summary>
    /// <param name="chunkData"></param>
    private void GenerateBlock(ChunkData chunkData)
    {

        float baseX = chunkData.Position.x * NoiseScale;
        float baseZ = chunkData.Position.z * NoiseScale;
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

                    int index = x + ChunkSize * y + z * ChunkSize * ChunkHeight;
                    chunkData.Blocks[index] = new Block(blockType, new Vector3(x, y, z)); // 创建方块
                }
            }
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameEntry.Interface;
    }
}
