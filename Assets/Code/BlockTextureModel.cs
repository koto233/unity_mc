using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class BlockTextureModel : AbstractModel
{
    public float _tileSizeX, _tileSizeY;
    public float _textureOffset;
    private ResLoader _resLoader = ResLoader.Allocate();
    private Dictionary<BlockType, BlockTextureConfig> blockTextureDataDictionary = new();
    protected override void OnInit()
    {

        // 通过 AssetBundleName 和 资源名搜索并加载资源（更精确）
        var textureData = _resLoader.LoadSync<BlockDataSO>("blockdata_asset", "BlockData");

        foreach (var item in textureData.textureConfigs)
        {
            if (blockTextureDataDictionary.TryAdd(item.blockType, item) == false)
            {
                Debug.LogError($"重复的方块类型:{item.blockType}");
            }
        }
        _tileSizeX = textureData.textureSizeX;
        _tileSizeY = textureData.textureSizeY;
    }

    public BlockTextureConfig GetBlockTextureConfig(BlockType type)
    {
        if (blockTextureDataDictionary.TryGetValue(type, out var config))
        {
            return config;
        }
        else
        {
            return null;
        }
    }
}