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
    
    // paintData对应的CellType
    public static readonly Dictionary<int,List<CellType>> CellTypeDict = new Dictionary<int, List<CellType>>
    {
        {0, new List<CellType> { CellType.None }},
        {1, new List<CellType> { CellType.Mountain }},
        {2, new List<CellType> { CellType.Forest }},
        {3, new List<CellType> { CellType.Snow }},
        {4, new List<CellType> { CellType.Lava, CellType.Lavaland }},
        {5, new List<CellType> { CellType.Grassland }},
    };
}
