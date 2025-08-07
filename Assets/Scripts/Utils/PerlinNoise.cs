using UnityEngine;

namespace AILand.Utils
{
    public class PerlinNoise
    {
        // 画布尺寸
        private int m_width;
        private int m_height;

        // 噪声种子
        private int m_seed = -1;
        private const int OffsetRange = 10000;
        private Texture2D m_noiseTexture;
        private float[,] m_noiseMap;

        // 噪声缩放比例
        public float scale = 20f;

        // 倍频设置
        public int octaves = 1;

        // 振幅衰减
        public float persistence = 0.5f;

        // 频率倍增
        public float lacunarity = 2.0f;

        // 初始振幅
        public float oriAmplitude = 1f;

        // 初始频率
        public float oriFrequency = 1f;



        public Texture2D NoiseTexture => m_noiseTexture;
        public float[,] NoiseMap => m_noiseMap;


        public PerlinNoise(int width, int height, int seed = -1)
        {
            m_width = width;
            m_height = height;

            // if(seed != -1) SetSeed(seed);

            m_noiseTexture = new Texture2D(m_width, m_height, TextureFormat.RGBA32, false);
            m_noiseTexture.wrapMode = TextureWrapMode.Clamp;
        }

        // public void SetSeed(int seed)
        // {
        //     m_seed = seed;
        //     Random.InitState(m_seed);
        // }


        private void GenerateNoise()
        {
            if (m_width <= 0 || m_height <= 0) return;

            float offsetX = Random.Range(-OffsetRange, OffsetRange);
            float offsetY = Random.Range(-OffsetRange, OffsetRange);

            m_noiseMap = new float[m_width, m_height];

            float min = float.MaxValue;
            float max = float.MinValue;

            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    float noiseValue = 0f;
                    float amplitude = this.oriAmplitude;
                    float frequency = this.oriFrequency;

                    for (int i = 0; i < octaves; i++)
                    {
                        // 在坐标上加入随机偏移
                        float sampleX = (x + offsetX) / scale * frequency;
                        float sampleY = (y + offsetY) / scale * frequency;
                        float noise = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseValue += noise * amplitude;

                        frequency *= lacunarity;
                        amplitude *= persistence;
                    }

                    m_noiseMap[x, y] = noiseValue;

                    min = Mathf.Min(min, noiseValue);
                    max = Mathf.Max(max, noiseValue);

                }
            }

            // 归一化
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    float value = Mathf.InverseLerp(min, max, m_noiseMap[x, y]);
                    m_noiseMap[x, y] = value;
                    m_noiseTexture.SetPixel(y, m_height - 1 - x, new Color(value, value, value));
                }
            }

        }

        public Texture2D NextNoiseTexture()
        {
            GenerateNoise();
            return m_noiseTexture;
        }

        public float[,] NextNoiseMap()
        {
            GenerateNoise();
            return m_noiseMap;
        }



    }
}
