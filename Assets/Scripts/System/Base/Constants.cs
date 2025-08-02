using System.Collections.Generic;
using AILand.GamePlay.World;
using AILand.Utils;
using UnityEngine;

public class Constants
{
    // blockID的base
    public const int BlockIDBase = 10000;
    // 玩家初始刷新的block
    public static readonly Vector2Int FirstBlockIndex = Vector2Int.zero;
    public static readonly int FirstBlockID = Util.GetBlockID(FirstBlockIndex); 


    // 区块大小
    public const int BlockWidth = 200;  
    public const int BlockHeight = 200; 
    public const int BuildMaxHeight = 64; // 最大高度

    // 可见的Cube类型
    public static readonly List<CubeType?> VisibleCubeTypes = new List<CubeType?>
    {
        CubeType.None,
        CubeType.Air,
        CubeType.AirBlock
    };
}
