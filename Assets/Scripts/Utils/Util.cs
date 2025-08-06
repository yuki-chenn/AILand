using AILand.GamePlay.World;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AILand.Utils
{
    public static class Util
    {


        #region 地形相关

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
        /// 使用Perlin噪声生成平滑的随机曲线
        /// </summary>
        /// <param name="width">数组宽度</param>
        /// <param name="height">数组高度</param>
        /// <param name="border">边缘宽度</param>
        /// <param name="curveLength">曲线长度（步数）</param>
        /// <param name="thickness">曲线粗细</param>
        /// <param name="stepSize">每步移动距离</param>
        /// <param name="startX">起始X坐标（-1表示随机）</param>
        /// <param name="startY">起始Y坐标（-1表示随机）</param>
        /// <param name="seed">噪声种子（用于生成可重复的结果）</param>
        /// <returns>绘制了曲线的01矩阵</returns>
        public static float[,] GetRandomTerrainNoiseMap(int width, int height, int border, int curveLength, int thickness,
            float stepSize = 1.5f, int startX = -1, int startY = -1, int seed = -1)
        {
            float[,] drawCurve = DrawSmoothRandomCurve(
                width: width - border,
                height: height - border,
                curveLength: curveLength,
                thickness: thickness,
                stepSize: stepSize,     
                startX: startX,
                startY: startY,
                seed: seed
            );
            var distanceMap = ComputeDistanceMap(drawCurve, width, height);
            float[,] weightMap = new float[width, height];

            float DecayFunction(int distance)
            {
                float a = 1.1000f, b = 2.2303f, c = 11.8828f, d = -0.1908f;
                return (a - d) / (1 + Mathf.Pow(distance / c, b)) + d;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    weightMap[x, y] = DecayFunction(distanceMap[x, y]);
                }
            }
            
            // 再叠加
            var noiseMap = new PerlinNoise(width, height).NextNoiseMap();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseMap[x, y] *= weightMap[x, y];
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y]);
                }
            }
            return noiseMap;
        }
        
        
        
        public static float[,] DrawSmoothRandomCurve(int width, int height, int curveLength, int thickness,
            float stepSize = 1.5f, int startX = -1, int startY = -1, int seed = -1)
        {
            float[,] array = new float[width, height];

            if (seed >= 0)
            {
                Random.InitState(seed);
            }

            // 随机起始点
            if (startX < 0 || startX >= width) startX = Random.Range(thickness, width - thickness);
            if (startY < 0 || startY >= height) startY = Random.Range(thickness, height - thickness);

            float currentX = startX;
            float currentY = startY;
            float currentDirection = Random.Range(0f, 2f * Mathf.PI); 

            List<Vector2> curvePoints = new List<Vector2>();
            curvePoints.Add(new Vector2(currentX, currentY));

            float turnRange = 30f * Mathf.Deg2Rad; // 随机转向范围
            float margin = thickness + 2f; // 边缘检测

            // 曲线路径
            for (int i = 0; i < curveLength; i++)
            {
                float minAngle, maxAngle;

                // 边缘
                bool nearLeftEdge = currentX < margin;
                bool nearRightEdge = currentX > width - margin;
                bool nearBottomEdge = currentY < margin;
                bool nearTopEdge = currentY > height - margin;

                if (nearLeftEdge || nearRightEdge || nearBottomEdge || nearTopEdge)
                {
                    float awayFromEdgeDirection = 0f;
                    
                    if (nearLeftEdge && nearBottomEdge)
                        awayFromEdgeDirection = Mathf.PI * 0.25f; // 右上方
                    else if (nearRightEdge && nearBottomEdge)
                        awayFromEdgeDirection = Mathf.PI * 0.75f; // 左上方
                    else if (nearLeftEdge && nearTopEdge)
                        awayFromEdgeDirection = Mathf.PI * 1.75f; // 右下方
                    else if (nearRightEdge && nearTopEdge)
                        awayFromEdgeDirection = Mathf.PI * 1.25f; // 左下方
                    else if (nearLeftEdge)
                        awayFromEdgeDirection = 0f; // 向右
                    else if (nearRightEdge)
                        awayFromEdgeDirection = Mathf.PI; // 向左
                    else if (nearBottomEdge)
                        awayFromEdgeDirection = Mathf.PI * 0.5f; // 向上
                    else if (nearTopEdge)
                        awayFromEdgeDirection = Mathf.PI * 1.5f; // 向下

                    // 远离边缘方向
                    minAngle = awayFromEdgeDirection - turnRange * 2f;
                    maxAngle = awayFromEdgeDirection + turnRange * 2f;
                }
                else
                {
                    // 当前方向左右30度范围
                    minAngle = currentDirection - turnRange;
                    maxAngle = currentDirection + turnRange;
                }
                
                float newDirection = Random.Range(minAngle, maxAngle);
                
                newDirection = ((newDirection % (2f * Mathf.PI)) + 2f * Mathf.PI) % (2f * Mathf.PI);

                currentDirection = newDirection;
                
                Vector2 direction = new Vector2(Mathf.Cos(currentDirection), Mathf.Sin(currentDirection));
                currentX += direction.x * stepSize;
                currentY += direction.y * stepSize;
                
                currentX = Mathf.Clamp(currentX, thickness, width - thickness - 1);
                currentY = Mathf.Clamp(currentY, thickness, height - thickness - 1);

                curvePoints.Add(new Vector2(currentX, currentY));
            }
            
            for (int i = 0; i < curvePoints.Count - 1; i++)
            {
                DrawThickLine(ref array, width, height, curvePoints[i], curvePoints[i + 1], thickness);
            }

            return array;
        }

        /// <summary>
        /// 绘制指定粗细的直线
        /// </summary>
        private static void DrawThickLine(ref float[,] array, int width, int height, Vector2 start, Vector2 end, int thickness)
        {
            int x0 = Mathf.RoundToInt(start.x);
            int y0 = Mathf.RoundToInt(start.y);
            int x1 = Mathf.RoundToInt(end.x);
            int y1 = Mathf.RoundToInt(end.y);

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            int x = x0;
            int y = y0;

            while (true)
            {
                DrawThickPoint(ref array, width, height, new Vector2Int(x, y), thickness);

                if (x == x1 && y == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }

        /// <summary>
        /// 绘制指定粗细的点（整数版本）
        /// </summary>
        private static void DrawThickPoint(ref float[,] array, int width, int height, Vector2Int center, int thickness)
        {
            int radius = thickness / 2;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int drawX = center.x + x;
                    int drawY = center.y + y;

                    // 边界检查
                    if (drawX >= 0 && drawX < width && drawY >= 0 && drawY < height)
                    {
                        // 圆形粗细
                        float distance = Mathf.Sqrt(x * x + y * y);
                        if (distance <= radius)
                        {
                            array[drawX, drawY] = 1;
                        }
                    }
                }
            }
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

        #endregion


        #region block相关

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
        

        #endregion


        #region Random
        
        public static T GetRandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            int index = Random.Range(0, list.Count);
            return list[index];
        }

        public static int GetRandomInRange(int min, int max)
        {
            return Random.Range(min, max);
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

        /// <summary>
        /// 克隆SO对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="deepLists"></param>
        /// <returns></returns>
        public static T CloneSO<T>(T src, bool deepLists = false) where T : ScriptableObject
        {
            T dst = ScriptableObject.Instantiate(src);

            if (!deepLists) return dst;

            // 深拷贝示例：把所有 IList<> 字段都重新 new
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var f in typeof(T).GetFields(flags))
            {
                if (!typeof(IList).IsAssignableFrom(f.FieldType)) continue;
                var list = f.GetValue(src) as IList;
                if (list == null) continue;

                var cloneList = (IList) global::System.Activator.CreateInstance(f.FieldType);
                foreach (var elem in list) cloneList.Add(elem);
                f.SetValue(dst, cloneList);
            }
            return dst;
        }
    }
}
