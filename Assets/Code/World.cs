using System.Collections;
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
        StartCoroutine(GenerateWorld());
        GenerateWorld();
    }


    /// <summary>
    /// 生成世界
    /// </summary>
    public IEnumerator GenerateWorld()
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
                var chunkData = new ChunkData(ChunkSize, ChunkHeight, new Vector3Int(x * ChunkSize, 0, z * ChunkSize));
                GenerateBlock(chunkData);
                _chunksData.Add(chunkData.Position, chunkData);
                Debug.Log($"生成区块{chunkData.Position}");
                yield return new WaitForEndOfFrame();
                // bool isMainMesh = blockType == BlockType.Water ? false : true;
            }
        }

        foreach (var chunk in _chunksData.Values)
        {
            GameObject chunkObject = Instantiate(ChunkPrefab, new Vector3(chunk.Position.x, 0, chunk.Position.z), Quaternion.identity);
            MeshData meshData = new MeshData(true);
            BlockHelper.SetMeshData(chunk, meshData, blockModel, _chunksData);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            _chunksInstances.Add(chunk.Position, chunkRenderer);
            chunkRenderer.InitializeChunk(chunk);
            chunkRenderer.UpdateChunk(meshData);
            yield return new WaitForEndOfFrame();
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
                    chunkData.Blocks[index] = new Block(blockType, new Vector3Int(x, y, z)); // 创建方块
                    // Debug.Log($"{index}方块位置{x}，{y}，{z}  创建成功");
                }
            }
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameEntry.Interface;
    }


}
