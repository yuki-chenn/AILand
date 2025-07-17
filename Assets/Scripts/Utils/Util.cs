using System;
using System.Collections.Generic;
using UnityEngine;

public class Util
{ 
    /// <summary>
    /// 计算距离图，计算map中每个为0的点到其他最近的不为0的点的曼哈顿距离
    /// </summary>
    /// <param name="map"></param>
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
                else
                {
                    distanceMap[sx, sy] = int.MaxValue;
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
    public static float[,] GrayTexture2Array(in Texture2D texture)
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
    
    
    
}
