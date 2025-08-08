using System.Collections.Generic;
using UnityEngine;

public static class BlockHelper
{
    private static readonly Dictionary<BlockFace, Vector3[]> faceVertices = new()
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
    private static BlockFace[] _directions = new BlockFace[]
    {
        BlockFace.Down,
        BlockFace.Up,
        BlockFace.Back,
        BlockFace.Forward,
        BlockFace.Left,
        BlockFace.Right
    };
    public static void SetMeshData(MeshData meshData, Block block, BlockTextureModel model)
    {
        foreach (var face in _directions) // _directions 存 BlockFace 而不是 Vector3
        {
            bool generatesCollider = false;
            foreach (var offset in faceVertices[face])
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
            meshData.AddQuadTriangles(false);
        }
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