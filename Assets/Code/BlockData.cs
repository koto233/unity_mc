using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 决定每个 Block 的纹理配置信息
[CreateAssetMenu(fileName = "BlockData", menuName = "Data/Block Data")]
public class BlockData : ScriptableObject
{
    [Header("Texture Settings")]
    [Tooltip("单个纹理在纹理图集上的尺寸 (单位:像素)")]
    public float textureSizeX, textureSizeY;
    
    [Space(10)]
    [Tooltip("每种方块类型的纹理配置")]
    public List<BlockTextureConfig> textureConfigs;
}

[System.Serializable]
public class BlockTextureConfig
{
    [Header("Identification")]
    [Tooltip("方块类型标识")]
    public BlockType blockType;
    
    [Header("Texture Coordinates")]
    [Tooltip("顶部面在纹理图集上的坐标")]
    public Vector2Int topFace;
    
    [Tooltip("底部面在纹理图集上的坐标")]
    public Vector2Int bottomFace;
    
    [Tooltip("侧面在纹理图集上的坐标")]
    public Vector2Int sideFace;
    
    [Header("Physics Properties")]
    [Tooltip("是否为实体方块 (如液体应设为false)")]
    public bool isSolid = true;
    
    [Tooltip("是否生成碰撞体")]
    public bool generateCollider = true;
}