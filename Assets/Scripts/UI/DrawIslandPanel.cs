using System;
using UnityEngine;
using UnityEngine.UI;

public class DrawIslandPanel : MonoBehaviour
{
    private Button m_btnGenerate;
    private Button m_btnClear;
    private PixelPainter m_pixelPainter;

    private PerlinNoise pn;
    
    private int m_canvasWidth = 150;
    private int m_canvasHeight = 150;

    private int m_blockWidth = 200;
    private int m_blockHeight = 200;
    
    private void Awake()
    {
        BindUI();
        pn = new PerlinNoise(200, 200,42);
        pn.oriAmplitude = 128;
        pn.oriFrequency = 4;
        pn.octaves = 3;
        pn.scale = 70f;
    }
    
    
    private void BindUI()
    {
        m_btnGenerate = transform.Find("BtnGenerate").GetComponent<Button>();
        m_btnClear = transform.Find("BtnClear").GetComponent<Button>();
        m_pixelPainter = transform.Find("PixelPainter").GetComponent<PixelPainter>();
        
        m_btnGenerate.onClick.AddListener(OnBtnGenerateClick);
        m_btnClear.onClick.AddListener(OnBtnClearClick);
    }
    
    private void OnBtnClearClick()
    {
        m_pixelPainter.ClearCanvas();
    }

    private void OnBtnGenerateClick()
    {
        // 拿到绘制的结果 
        float[,] paintMap = m_pixelPainter.GetPaintMap();
        var distanceMap = Util.ComputeDistanceMap(paintMap, m_blockWidth, m_blockHeight);
        
        // 根据距离图计算出权重图
        float[,] weightMap = new float[m_blockWidth, m_blockHeight];
        
        for (int x = 0; x < m_blockWidth; x++)
        {
            for (int y = 0; y < m_blockHeight; y++)
            {
                weightMap[x, y] = DecayFunction(distanceMap[x, y]);
            }
        }
        
        // 再叠加
        var noiseMap = pn.NextNoiseMap();
        for (int x = 0; x < m_blockWidth ; x++)
        {
            for(int y = 0; y < m_blockHeight; y++)
            {
                noiseMap[x, y] *= weightMap[x, y];
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y]);
            }
        }
        
        
        // 结果应该是一个二维float数组
        var resultTex = Util.Array2GrayTexture(noiseMap);
        m_pixelPainter.SetTexture(resultTex);
        
    }

    private float DecayFunction(int distance)
    {
        // y = (a - d) / (1 + (x / c) ^ b) + d
        float a =  1.1000f;  // dis=0时的值
        float b =  2.2303f;  // 下降速率 > 1 值越大前期越缓慢
        float c = 11.8828f;  // 前期的分解，值越大前期持续越长
        float d = -0.1908f;  // 最后趋近于的值
        
        return (a - d) / (1 + Mathf.Pow(distance / c, b)) + d;
    }
}
