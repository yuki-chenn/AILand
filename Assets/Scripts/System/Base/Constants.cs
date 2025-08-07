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
    
    // 水晶的最大能量
    public const float EnergyInkTimes = 100;
        
    public static readonly Dictionary<EnergyType, Color> energyColors = new Dictionary<EnergyType, Color>
    {
        { EnergyType.Metal, new Color(1f, 0.8f, 0f, 1f) },    
        { EnergyType.Wood, new Color(0f, 0.8f, 0.2f, 1f) },  
        { EnergyType.Water, new Color(0f, 0.5f, 1f, 1f) },    
        { EnergyType.Fire, new Color(1f, 0.3f, 0f, 1f) },     
        { EnergyType.Earth, new Color(0.6f, 0.4f, 0.2f, 1f) }  
    };

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
