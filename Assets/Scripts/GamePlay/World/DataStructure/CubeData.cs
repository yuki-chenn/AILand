namespace AILand.GamePlay.World
{
    public class CubeData
    {
        // 方块的种类
        private CubeType m_cubeType;
        public CubeType CubeType => m_cubeType;
        
        // 方块所在的高度
        private int m_YHeight;
        public int YHeight => m_YHeight;

        public CubeData(CubeType type, int yHeight)
        {
            m_cubeType = type;
            m_YHeight = yHeight;
        }
    }

}