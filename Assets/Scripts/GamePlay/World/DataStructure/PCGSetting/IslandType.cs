namespace AILand.GamePlay.World
{
    // 一个IslandType对应一个IslandConfigSO
    public enum IslandType
    {
        None,
        Custom, // 自定义
        Water,  // 纯水域 
        Plain,  // 平原
        Forest, // 森林
        Mountain, // 山地
        Glacier, // 冰川
        Inferno, // 地狱
    }
}