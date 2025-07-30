using System.Collections.Generic;
using AILand.GamePlay.World;
using UnityEngine;

namespace AILand.Utils
{
    public static class Util
    {
        /// <summary>
        /// 计算距离图，计算map中每个为0的点到其他最近的不为0的点的曼哈顿距离
        /// </summary>
        /// <param name="map"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static int[,] ComputeDistanceMap(in float[,] map, int width, int height)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);
            
            if(width < mapWidth || height < mapHeight)
            {
                Debug.LogError($"map size {mapWidth}x{mapHeight} is larger than specified size {width}x{height}");
                return null;
            }
            
            // 计算偏移量，使得map在指定的宽高范围内居中
            int offsetX = (width - mapWidth) / 2;
            int offsetY = (height - mapHeight) / 2;
            
            int[] dx = {1, -1, 0, 0};
            int[] dy = {0, 0, 1, -1};

            var queue = new Queue<KeyValuePair<KeyValuePair<int, int>, int>>();
            int[,] distanceMap = new int[width, height];
            bool[,] visited = new bool[width, height];
            
            // 初始化
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    distanceMap[x, y] = int.MaxValue;
                    visited[x, y] = false; 
                }
            }
            
            for(int x = 0; x < mapWidth; x++)
            {
                for(int y = 0; y < mapHeight; y++)
                {
                    var sx = x + offsetX;
                    var sy = y + offsetY;
                    
                    if (map[x, y] > Mathf.Epsilon)
                    {
                        distanceMap[sx, sy] = 0;
                        visited[sx, sy] = true;
                        queue.Enqueue(new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(sx, sy), 0));
                    }
                }
            }
            
            while(queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                int x = current.Key.Key;
                int y = current.Key.Value;
                int dist = current.Value;

                for(int i = 0; i < 4; i++)
                {
                    int sx = x + dx[i];
                    int sy = y + dy[i];
                    
                    if(sx < 0 || sx >= width || sy < 0 || sy >= height || visited[sx, sy]) continue;
                    
                    visited[sx, sy] = true;
                    distanceMap[sx, sy] = dist + 1;
                    queue.Enqueue(new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(sx, sy), dist + 1));
                    
                }
            }

            return distanceMap;

        }
        
        /// <summary>
        /// 灰度纹理转换为二维数组
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static float[,] GrayTexture2Array(Texture2D texture)
        {
            int width = texture.width;
            int height = texture.height;
            float[,] array = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = texture.GetPixel(y, height - 1 - x);
                    array[x, y] = pixelColor.grayscale;
                }
            }

            return array;
        }
        
        /// <summary>
        /// 数组转换为灰度纹理
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Texture2D Array2GrayTexture(in float[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float value = array[x, y];
                    texture.SetPixel(y, height - 1 - x, new Color(value, value, value));
                }
            }
            texture.Apply();
            
            return texture;
        }
        
        
        /// <summary>
        /// 获取block的ID
        /// </summary>
        /// <param name="blockIndexX">block在世界的X index</param>
        /// <param name="blockIndexY">block在世界的Y index</param>
        /// <returns></returns>
        public static int GetBlockID(int blockIndexX, int blockIndexY)
        {
            int offsetX = blockIndexX + Constants.BlockIDBase / 2;
            int offsetY = blockIndexY + Constants.BlockIDBase / 2;
            return offsetX * Constants.BlockIDBase + offsetY;
        }
        public static int GetBlockID(Vector2Int blockIndex)
        {
            return GetBlockID(blockIndex.x, blockIndex.y);
        }
        
        /// <summary>
        /// 根据blockID获取block的index
        /// </summary>
        /// <param name="blockID"></param>
        /// <returns></returns>
        public static Vector2Int GetBlockIndexByID(int blockID)
        {
            int offsetX = blockID / Constants.BlockIDBase;
            int offsetY = blockID % Constants.BlockIDBase;
            
            int blockIndexX = offsetX - Constants.BlockIDBase / 2;
            int blockIndexY = offsetY - Constants.BlockIDBase / 2;
            
            return new Vector2Int(blockIndexX, blockIndexY);
        }
        
        /// <summary>
        /// 给定世界坐标，计算该坐标所在block的Index
        /// </summary>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static Vector2Int GetBlockIndexByWorldPosition(Vector3 worldPos, int blockWidth, int blockHeight)
        {
            // 计算区块的index
            int blockX = Mathf.FloorToInt(worldPos.x / blockWidth);
            int blockY = Mathf.FloorToInt(worldPos.z / blockHeight);
            
            return new Vector2Int(blockX, blockY);
        }
        
        /// <summary>
        /// 给定世界坐标，计算该坐标所在block的ID
        /// </summary>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static int GetBlockIDByWorldPosition(Vector3 worldPos, int blockWidth, int blockHeight)
        {
            return GetBlockID(GetBlockIndexByWorldPosition(worldPos, blockWidth, blockHeight));
        }
        
        
        public static Vector2Int GetBlockIndexByWorldPosition(Vector3 worldPos, int blockWidth, int blockHeight,ref Vector2Int indexInBlock)
        {
            Vector2Int blockIndex = GetBlockIndexByWorldPosition(worldPos, blockWidth, blockHeight);
            
            // block的位置
            Vector2 blockPos = blockIndex * new Vector2(blockWidth, blockHeight);
            
            // block内相对位置
            Vector2 relativePos = new Vector2(worldPos.x, worldPos.z) - blockPos;
            
            indexInBlock = new Vector2Int(
                Mathf.FloorToInt(relativePos.x),
                Mathf.FloorToInt(relativePos.y)
            );

            return blockIndex;
        }
        
        /// <summary>
        /// 计算某个blockID在世界中的位置
        /// </summary>
        /// <param name="blockID"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static Vector3 GetBlockPositionByID(int blockID, int blockWidth, int blockHeight)
        {
            Vector2Int blockIndex = GetBlockIndexByID(blockID);
            float x = blockIndex.x * blockWidth;
            float z = blockIndex.y * blockHeight;
            return new Vector3(x, 0, z);
        }


        /// <summary>
        /// 获取指定blockID周围的blockID列表
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="range">周围圈数，1就是周围8个，2就是周围24个</param>
        /// <param name="res"></param>
        public static void GetAroundBlockID(int blockId, int range,ref List<int> res)
        {
            res.Clear();
            
            Vector2Int blockIndex = GetBlockIndexByID(blockId);
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    int newBlockId = GetBlockID(blockIndex.x + dx, blockIndex.y + dy);
                    res.Add(newBlockId);
                }
            }
        }


        /// <summary>
        /// 获取某个Cube周围6个Cube的数据
        /// </summary>
        /// <param name="cube"></param>
        /// <returns></returns>
        public static List<CubeData> Get6CubesAround(CubeData cube)
        {
            List<CubeData> cubes = new List<CubeData>();
            CellData cell = cube.CellData;
            BlockData block = cell.BlockData;
            Vector2Int index = cell.Index;
            int y = cube.YHeight;

            // 上下左右前后
            cubes.Add(cell.GetCubeData(y + 1)); // 上
            cubes.Add(cell.GetCubeData(y - 1)); // 下
            cubes.Add(block.GetCellData(index.x - 1,index.y)?.GetCubeData(y)); // 左
            cubes.Add(block.GetCellData(index.x + 1,index.y)?.GetCubeData(y)); // 右
            cubes.Add(block.GetCellData(index.x,index.y - 1)?.GetCubeData(y)); // 前
            cubes.Add(block.GetCellData(index.x,index.y + 1)?.GetCubeData(y)); // 后
            
            return cubes;
        }



        #region Random
        
        public static T GetRandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            int index = Random.Range(0, list.Count);
            return list[index];
        }
        
        public static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        
        #endregion
        
        
    }
}
