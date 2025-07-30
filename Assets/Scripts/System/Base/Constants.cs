using System.Collections.Generic;
using AILand.GamePlay.World;

public class Constants
{
    // blockID的base
    public const int BlockIDBase = 10000; 
    
    
    
    // 区块大小
    public const int BlockWidth = 200;  
    public const int BlockHeight = 200; 
    public const int BuildMaxHeight = 64; // 最大高度


    public static readonly List<CubeType?> VisibleCubeTypes = new List<CubeType?>
    {
        CubeType.None,
        CubeType.Air,
        CubeType.AirBlock
    };
}
