using UnityEngine;

namespace AILand.GamePlay.World
{
    [CreateAssetMenu(fileName ="IslandConfig_" ,menuName ="创建岛屿配置",order =0)]
    public class IslandConfigSO : ScriptableObject
    {
        [SerializeField] private int width = 200;
        [SerializeField] private int height = 200;
        [SerializeField] private CellType[] cellTypesArray;

        public int Width => width;
        public int Height => height;

        // 二维数组属性访问器
        public CellType GetCellType(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return CellType.None;
            if (cellTypesArray == null) InitializeArray();
            return cellTypesArray[y * width + x];
        }

        public void SetCellType(int x, int y, CellType cellType)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;
            if (cellTypesArray == null) InitializeArray();
            cellTypesArray[y * width + x] = cellType;
        }

        public void ResizeArray(int newWidth, int newHeight)
        {
            var newArray = new CellType[newWidth * newHeight];
            
            if (cellTypesArray != null)
            {
                // 复制旧数据
                for (int y = 0; y < Mathf.Min(height, newHeight); y++)
                {
                    for (int x = 0; x < Mathf.Min(width, newWidth); x++)
                    {
                        newArray[y * newWidth + x] = cellTypesArray[y * width + x];
                    }
                }
            }

            width = newWidth;
            height = newHeight;
            cellTypesArray = newArray;
        }

        private void InitializeArray()
        {
            cellTypesArray = new CellType[width * height];
        }

        private void OnValidate()
        {
            if (cellTypesArray == null || cellTypesArray.Length != width * height)
            {
                InitializeArray();
            }
        }
    }
}