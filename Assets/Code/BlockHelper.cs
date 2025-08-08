using System.Collections.Generic;
using QFramework;
using UnityEngine;

public static class BlockHelper
{
    private static readonly Dictionary<BlockFace, Vector3[]> _faceVertices = new()
{
    { BlockFace.Back, new [] {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f,  0.5f, -0.5f),
        new Vector3( 0.5f,  0.5f, -0.5f),
        new Vector3( 0.5f, -0.5f, -0.5f)
    }},
    { BlockFace.Forward, new [] {
        new Vector3( 0.5f, -0.5f,  0.5f),
        new Vector3( 0.5f,  0.5f,  0.5f),
        new Vector3(-0.5f,  0.5f,  0.5f),
        new Vector3(-0.5f, -0.5f,  0.5f)
    }},
    { BlockFace.Left, new [] {
        new Vector3(-0.5f, -0.5f,  0.5f),
        new Vector3(-0.5f,  0.5f,  0.5f),
        new Vector3(-0.5f,  0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f)
    }},
    { BlockFace.Right, new [] {
        new Vector3( 0.5f, -0.5f, -0.5f),
        new Vector3( 0.5f,  0.5f, -0.5f),
        new Vector3( 0.5f,  0.5f,  0.5f),
        new Vector3( 0.5f, -0.5f,  0.5f)
    }},
    { BlockFace.Down, new [] {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3( 0.5f, -0.5f, -0.5f),
        new Vector3( 0.5f, -0.5f,  0.5f),
        new Vector3(-0.5f, -0.5f,  0.5f)
    }},
    { BlockFace.Up, new [] {
        new Vector3(-0.5f,  0.5f,  0.5f),
        new Vector3( 0.5f,  0.5f,  0.5f),
        new Vector3( 0.5f,  0.5f, -0.5f),
        new Vector3(-0.5f,  0.5f, -0.5f)
    }}
};

    private static readonly Dictionary<BlockFace, Vector3Int> _position = new Dictionary<BlockFace, Vector3Int>()
    {
        {BlockFace.Down, Vector3Int.down},
        {BlockFace.Up, Vector3Int.up},
        {BlockFace.Left,Vector3Int.left},
        {BlockFace.Right,Vector3Int.right},
        {BlockFace.Forward,Vector3Int.forward},
        {BlockFace.Back,Vector3Int.back}
    };
    private static BlockFace[] _directions = new BlockFace[]
    {
        BlockFace.Down,
        BlockFace.Up,
        BlockFace.Back,
        BlockFace.Forward,
        BlockFace.Left,
        BlockFace.Right
    };
    public static void SetMeshData(ChunkData chunkData, MeshData meshData, BlockTextureModel model, Dictionary<Vector3Int, ChunkData> chunks)
    {
        foreach (var block in chunkData.Blocks)
        {
            foreach (var face in _directions)
            {
                // Debug.Log($"方块类型{block.blockType}");
                // 如果方块为空或者为空气 则不渲染
                if (block.blockType == BlockType.None || block.blockType == BlockType.Air) continue;
                Block neighborBlock = null;
                var neighborPosition = block.position + _position[face];

                if (neighborPosition.x >= 0 && neighborPosition.x < chunkData.ChunkSize && neighborPosition.y >= 0 && neighborPosition.y < chunkData.ChunkHeight && neighborPosition.z >= 0 && neighborPosition.z < chunkData.ChunkSize)
                {
                    int neighborIndex = GetIndexFromPosition(neighborPosition, chunkData.ChunkSize, chunkData.ChunkHeight);
                    neighborBlock = chunkData.Blocks[neighborIndex];
                    // Debug.Log($"方块邻居{neighborBlock.blockType}");
                    // 如果相邻方块为空或者不为实体 则不渲染
                }
                else
                {
                    var neighborWorldPos = neighborPosition + chunkData.Position;
                    // 找邻居 Chunk 世界坐标（按你的索引规则是 Chunk 左下角的世界方块坐标）
                    Vector3Int neighborChunkWorldPos = new Vector3Int(
                        Mathf.FloorToInt((float)neighborWorldPos.x / chunkData.ChunkSize) * chunkData.ChunkSize,
                        Mathf.FloorToInt((float)neighborWorldPos.y / chunkData.ChunkHeight) * chunkData.ChunkHeight,
                        Mathf.FloorToInt((float)neighborWorldPos.z / chunkData.ChunkSize) * chunkData.ChunkSize
                    );
                    // 如果存在这个 Chunk
                    if (chunks.TryGetValue(neighborChunkWorldPos, out var neighborChunk))
                    {
                        // 转成邻居 Chunk 的局部坐标
                        int nx = ((neighborWorldPos.x % chunkData.ChunkSize) + chunkData.ChunkSize) % chunkData.ChunkSize;
                        int ny = ((neighborWorldPos.y % chunkData.ChunkHeight) + chunkData.ChunkHeight) % chunkData.ChunkHeight;
                        int nz = ((neighborWorldPos.z % chunkData.ChunkSize) + chunkData.ChunkSize) % chunkData.ChunkSize;

                        int np = GetIndexFromPosition(new Vector3Int(nx, ny, nz), chunkData.ChunkSize, chunkData.ChunkHeight);
                        neighborBlock = neighborChunk.Blocks[np];
                    }
                    else
                    {
                        continue;
                    }
                }
                if (neighborBlock != null && model.GetBlockTextureConfig(neighborBlock.blockType).isSolid == true) continue;
                if (block.blockType == BlockType.Water)
                {
                    if (neighborBlock != null && neighborBlock.blockType != BlockType.None && neighborBlock.blockType == BlockType.Air)
                        GetFace(meshData.WaterMesh, model, face, block);
                }
                else
                {
                    GetFace(meshData, model, face, block);
                }


            }
        }
    }


    public static void GetFace(MeshData meshData, BlockTextureModel model, BlockFace face, Block block)
    {
        bool generatesCollider = model.GetBlockTextureConfig(block.blockType).generateCollider;
        foreach (var offset in _faceVertices[face])
        {
            meshData.AddVertex(block.position + offset, generatesCollider);
        }
        Vector2[] UVs = new Vector2[4];
        var textureConfig = model.GetBlockTextureConfig(block.blockType);
        var tilePos = face switch
        {
            BlockFace.Up => textureConfig.topFace,
            BlockFace.Down => textureConfig.bottomFace,
            _ => textureConfig.sideFace
        };

        UVs[0] = new Vector2(model._tileSizeX * tilePos.x + model._tileSizeX - model._textureOffset,
            model._tileSizeY * tilePos.y + model._textureOffset);

        UVs[1] = new Vector2(model._tileSizeX * tilePos.x + model._tileSizeX - model._textureOffset,
            model._tileSizeY * tilePos.y + model._tileSizeY - model._textureOffset);

        UVs[2] = new Vector2(model._tileSizeX * tilePos.x + model._textureOffset,
            model._tileSizeY * tilePos.y + model._tileSizeY - model._textureOffset);

        UVs[3] = new Vector2(model._tileSizeX * tilePos.x + model._textureOffset,
            model._tileSizeY * tilePos.y + model._textureOffset);
        meshData.UV.AddRange(UVs);
        meshData.AddQuadTriangles(generatesCollider);
    }
    public static int GetIndexFromPosition(Vector3Int position, int chunkSize, int chunkHeight)
    {
        return position.x + chunkSize * position.y + chunkSize * chunkHeight * position.z;
    }

    public enum BlockFace
    {
        Back,
        Forward,
        Left,
        Right,
        Down,
        Up
    }

}