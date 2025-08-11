using System;
using System.Collections;
using AILand.GamePlay;
using AILand.System.EventSystem;
using UnityEngine;
using UnityEngine.UI;
using AILand.Utils;
using EventType = AILand.System.EventSystem.EventType;
using AILand.GamePlay.World;



namespace AILand.UI
{
    public class DrawIslandShapePanel : BaseUIPanel
    {
        private Button m_btnGenerate;
        private Button m_btnClear;
        private Button m_btnClose;
        private PixelPainter m_ppShape;
        private Slider m_sliderRemainColor;  // 剩余颜色的展示slider
        private Text m_txtAmount;
        private Transform m_smallWarn;
        
        
        private PerlinNoise pn;
        
        

        private int m_blockWidth = Constants.BlockWidth;
        private int m_blockHeight = Constants.BlockHeight;

        // 水晶数据
        private ElementalEnergy m_crystalEnergy;
        
        protected override void Awake()
        {
            base.Awake();
            
            pn = new PerlinNoise(m_blockWidth, m_blockHeight, 42);
            pn.oriAmplitude = 128;
            pn.oriFrequency = 4;
            pn.octaves = 3;
            pn.scale = 120f;
        }

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        protected override void BindUI()
        {
            m_smallWarn = transform.Find("TxtWarn");
            m_btnClose = transform.Find("BtnClose").GetComponent<Button>();
            m_btnGenerate = transform.Find("BtnGenerate").GetComponent<Button>();
            m_btnClear = transform.Find("BtnClear").GetComponent<Button>();
            m_ppShape = transform.Find("Painter/PPShape").GetComponent<PixelPainter>();
            m_sliderRemainColor = transform.Find("RemainColor/SliderRemainColor").GetComponent<Slider>();
            m_txtAmount = transform.Find("RemainColor/SliderRemainColor/TxtAmount").GetComponent<Text>();

            m_btnClose.onClick.AddListener(Hide);
            m_btnGenerate.onClick.AddListener(OnBtnGenerateClick);
            m_btnClear.onClick.AddListener(OnBtnClearClick);
            
            m_ppShape.OnInkConsumptionChanged += OnInkConsumptionChanged;
        }

        protected override void BindListeners()
        {
            EventCenter.AddListener<ElementalEnergy>(EventType.ShowDrawIslandShapePanelUI, OnCrystalDrawShape);
        }

        protected override void UnbindListeners()
        {
            EventCenter.RemoveListener<ElementalEnergy>(EventType.ShowDrawIslandShapePanelUI, OnCrystalDrawShape);
        }

        private void OnCrystalDrawShape(ElementalEnergy energy)
        {
            m_crystalEnergy = energy;
    
            // 初始化墨水数据
            m_ppShape.ClearCanvas();
            var inkData = new InkConsumptionData();
            inkData.AddInk(Color.white, m_crystalEnergy.NormalElement.Sum * Constants.EnergyInkTimes); // 只有白色
            m_ppShape.SetInkData(inkData);
            
            Show();
        }
        
        private void OnInkConsumptionChanged(InkConsumptionData inkData)
        {
            // 更新 UI 显示
            var whiteInk = inkData.GetInkByColor(Color.white);
            if (whiteInk != null)
            {
                m_sliderRemainColor.value = whiteInk.RemainingRatio;
                m_txtAmount.text = $"{whiteInk.RemainingAmount:F0} / {whiteInk.totalAmount:F0}";
        
                // 检查墨水是否耗尽
                if (whiteInk.RemainingAmount <= 0)
                {
                    m_ppShape.canPaint = false;
                }
                else
                {
                    m_ppShape.canPaint = true;
                }
            }
        }

        private void OnBtnClearClick()
        {
            m_ppShape.ClearCanvas();
        }

        private bool m_isWarning = false;
        private void OnBtnGenerateClick()
        {
            // 检查像素点的数量
            var count = Util.CountPixelPoints(m_ppShape.GetPaintTexture(),Color.white);
            Debug.Log($"count:{count}");
            if (count < 2000)
            {
                if(!m_isWarning) StartCoroutine(WarnCoroutine(2));
                return;
            }
            
            var terrainNoiseMap = GenerateTerrainNoiseMap();
            Hide();
            EventCenter.Broadcast(EventType.ShowDrawIslandCellTypePanelUI, terrainNoiseMap, m_crystalEnergy);
        }
        
        IEnumerator WarnCoroutine(int count)
        {
            m_isWarning = true;
            for(int i = 0; i < count; i++)
            {
                m_smallWarn.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                m_smallWarn.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.3f);
            }
            m_smallWarn.gameObject.SetActive(true);
            m_isWarning = false;
        }

        /// <summary>
        /// 生成地形的噪声图
        /// </summary>
        /// <returns>size:blockHeight*blockWidth,value:[0,1]</returns>
        private float[,] GenerateTerrainNoiseMap()
        {
            // 拿到绘制的结果 
            float[,] paintMap = m_ppShape.GetPaintMap();

            // 根据绘制的结果计算出距离图
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
            for (int x = 0; x < m_blockWidth; x++)
            {
                for (int y = 0; y < m_blockHeight; y++)
                {
                    noiseMap[x, y] *= weightMap[x, y];
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y]);
                }
            }

            var resultTex = Util.Array2GrayTexture(noiseMap);

            return noiseMap;
        }

        private float DecayFunction(int distance)
        {
            // y = (a - d) / (1 + (x / c) ^ b) + d
            float a = 1.1000f; // dis=0时的值
            float b = 2.2303f; // 下降速率 > 1 值越大前期越缓慢
            float c = 11.8828f; // 前期的分解，值越大前期持续越长
            float d = -0.1908f; // 最后趋近于的值

            return (a - d) / (1 + Mathf.Pow(distance / c, b)) + d;
        }
    }

}