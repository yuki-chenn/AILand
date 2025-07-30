namespace AILand.GamePlay.World
{
    public enum CellWater
    {
        OuterWater, // 外部水域
        InnerWater, // 内部水域
        None,       // 无水域
        BorderWater, // 边界水域 即和陆地相接的水域，边界水域一定是外部水域
    }
}