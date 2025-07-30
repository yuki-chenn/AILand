namespace AILand.GamePlay.World
{
    public enum CubeType
    {
        None = 0,
        AirBlock,  // 空气墙，只有碰撞没有渲染
        Air,       // 空气，完全没有碰撞和渲染，占位，不允许建造
        
        Sand,
        Dirt,
        Stone,
        Grass,
        Snow,
        
        
        
        
        
        
        Wood,
        Leaf,

        HardStone,    // 硬石，无法被破坏
        
    }
}