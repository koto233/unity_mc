using QFramework;
using UnityEngine;

public class GameEntry : Architecture<GameEntry>
{
    protected override void Init()
    {
        RegisterUtility(new ChunkHelper());

        RegisterModel(new BlockTextureModel());
    }
}
