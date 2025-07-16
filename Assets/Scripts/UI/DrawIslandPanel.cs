using UnityEngine;
using UnityEngine.UI;

public class DrawIslandPanel : MonoBehaviour
{
    private Button m_btnGenerate;
    private Button m_btnClear;
    private PixelPainter m_pixelPainter;

    PerlinNoise pn ;
    private void Awake()
    {
        BindUI();
        pn = new PerlinNoise(150, 150,42);
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
        pn.SetSeed(-1);
    }

    private void OnBtnGenerateClick()
    {
        
        pn.oriAmplitude = 128;
        pn.oriFrequency = 4;
        pn.octaves = 3;
        pn.scale = 70f;
        var noise = pn.NextNoiseTexture();
        m_pixelPainter.SetTexture(noise);
        
    }
}
