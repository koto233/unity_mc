using UnityEngine;

/// <summary>
/// 区块由多个Block组成
/// </summary>
public class ChunkData
{
    /// <summary>
    /// 区块里的方块
    /// </summary>
    public BlockType[] Blocks;

    /// <summary>
    /// 区块尺寸
    /// </summary>
    public readonly int ChunkSize;

    /// <summary>
    /// 区块高度
    /// </summary>
    public readonly int ChunkHeight;

    /// <summary>
    /// 区块位置
    /// </summary>
    public Vector3Int ChunkPosition;

    public ChunkData(int chunkSize, int chunkHeight, Vector3Int chunkPosition)
    {
        this.ChunkSize = chunkSize;
        this.ChunkHeight = chunkHeight;
        this.ChunkPosition = chunkPosition;
        Blocks = new BlockType[ChunkSize * ChunkHeight * ChunkSize];
    }
}

public enum BlockType
{
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 空气
    /// </summary>
    Air,
    /// <summary>
    /// 草方块
    /// </summary>
    Grass,
    /// <summary>
    /// 泥土
    /// </summary>
    Dirt,
    /// <summary>
    /// 石头
    /// </summary>
    Stone,
    /// <summary>
    /// 水
    /// </summary>
    Water,
    /// <summary>
    /// 沙子
    /// </summary>
    Sand
}