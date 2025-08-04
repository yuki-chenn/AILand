using System;
using AILand.GamePlay;
using AILand.System.EventSystem;
using UnityEngine;
using UnityEngine.UI;
using AILand.Utils;
using EventType = AILand.System.EventSystem.EventType;
using AILand.GamePlay.World;
using UnityEngine.EventSystems;

namespace AILand.UI
{
    public class DrawIslandCellTypePanel : BaseUIPanel
    {
        private Button m_btnGenerate;
        private Button m_btnClear;
        private PixelPainter m_ppCellType;

        private Slider[] m_slidersRemainColor;

        // 地形噪声
        private Texture2D m_terrainTex;
        private float[,] m_terrainNoise;

        // 水晶数据
        private ElementalEnergy m_crystalEnergy;

        // 五种能量对应的颜色
        private Color[] m_energyColors = new Color[]
        {
            Color.magenta,   // 金元素
            Color.green,    // 木元素
            Color.blue,     // 水元素
            Color.red,      // 火元素
            Color.yellow      // 土元素
        };

        private int m_currentSelectedEnergy = 0; // 当前选中的能量类型

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        protected override void BindUI()
        {
            m_btnGenerate = transform.Find("BtnGenerate").GetComponent<Button>();
            m_btnClear = transform.Find("BtnClear").GetComponent<Button>();
            m_ppCellType = transform.Find("PPCellType").GetComponent<PixelPainter>();

            m_slidersRemainColor = new Slider[5];
            var sliderRoot = transform.Find("SliderRemainColorRoot");
            for (int i = 0; i < m_slidersRemainColor.Length; i++)
            {
                m_slidersRemainColor[i] = sliderRoot.GetChild(i).GetComponent<Slider>();

                // 添加点击切换事件
                int index = i;
                var eventTrigger = m_slidersRemainColor[i].gameObject.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                {
                    eventTrigger = m_slidersRemainColor[i].gameObject.AddComponent<EventTrigger>();
                }

                var pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener((data) => OnSliderClick(index));
                eventTrigger.triggers.Add(pointerDown);
            }

            m_btnGenerate.onClick.AddListener(OnBtnGenerateClick);
            m_btnClear.onClick.AddListener(OnBtnClearClick);

            // 绑定墨水消耗事件
            m_ppCellType.OnInkConsumptionChanged += OnInkConsumptionChanged;
        }

        protected override void BindListeners()
        {
            EventCenter.AddListener<float[,], ElementalEnergy>(EventType.ShowDrawIslandCellTypePanelUI, OnCrystalDrawCellType);
        }

        protected override void UnbindListeners()
        {
            EventCenter.RemoveListener<float[,], ElementalEnergy>(EventType.ShowDrawIslandCellTypePanelUI, OnCrystalDrawCellType);
        }

        private void OnCrystalDrawCellType(float[,] terrainNoise, ElementalEnergy energy)
        {
            // 赋值
            m_terrainNoise = terrainNoise;
            m_terrainTex = Util.Array2GrayTexture(terrainNoise);
            m_crystalEnergy = energy;

            // 设置mask
            m_ppCellType.SetGrayMask(m_terrainTex);

            // 初始化墨水数据
            InitializeInkData();
            UpdateSlidersRemainColor();
            SelectEnergyColor(0);

            Show();
        }

        private void InitializeInkData()
        {
            var inkData = new InkConsumptionData();

            // 为每种能量添加对应的墨水
            for (int i = 0; i < 5; i++)
            {
                float energyAmount = m_crystalEnergy.NormalElement[i];
                inkData.AddInk(m_energyColors[i], energyAmount);
            }

            m_ppCellType.SetInkData(inkData);
        }

        private void OnSliderClick(int energyIndex)
        {
            SelectEnergyColor(energyIndex);
        }

        // 选择使用的能量，并更新选择显示
        private void SelectEnergyColor(int energyIndex)
        {
            // 记录 没画的地方表示0
            m_ppCellType.RecordPaintData(energyIndex + 1);
            
            m_currentSelectedEnergy = energyIndex;

            m_ppCellType.brushColor = m_energyColors[energyIndex];

            UpdateSliderSelectState();

            // 检查是否可以绘制
            var inkData = m_ppCellType.GetInkData();
            var ink = inkData.GetInkByColor(m_energyColors[energyIndex]);
            m_ppCellType.canPaint = ink != null && ink.RemainingAmount > 0;
        }

        // 更新选中显示
        private void UpdateSliderSelectState()
        {
            for (int i = 0; i < m_slidersRemainColor.Length; i++)
            {
                // TODO: 选中状态还需要修改
                
                // 更新 Slider 背景色或边框来表示选中状态
                var fillImage = m_slidersRemainColor[i].fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = i == m_currentSelectedEnergy ? m_energyColors[i] : Color.white;
                }
                
            }
        }

        private void OnInkConsumptionChanged(InkConsumptionData inkData)
        {
            // 更新滑条显示
            UpdateSlidersRemainColor();

            // 检查是否可以绘制
            var currentInk = inkData.GetInkByColor(m_energyColors[m_currentSelectedEnergy]);
            m_ppCellType.canPaint = currentInk != null && currentInk.RemainingAmount > 0;
        }

        private void UpdateSlidersRemainColor()
        {
            var inkData = m_ppCellType.GetInkData();
            
            for (int i = 0; i < 5; i++)
            {
                var totalEnergy = m_crystalEnergy.NormalElement[i];
                var ink = inkData.GetInkByColor(m_energyColors[i]);
                
                if (totalEnergy > 0 && ink != null)
                {
                    float remainingRatio = ink.RemainingAmount / totalEnergy;
                    m_slidersRemainColor[i].value = remainingRatio;
                }
                else
                {
                    m_slidersRemainColor[i].value = 0f;
                }
            }
        }

        private void OnBtnClearClick()
        {
            m_ppCellType.ClearCanvas();

            // 重新初始化
            InitializeInkData();
            
            UpdateSlidersRemainColor();

            SelectEnergyColor(0);
        }

        private void OnBtnGenerateClick()
        {
            var paintData = m_ppCellType.StopRecordPaintData();
            
            // 用剩余最多的填充所有值为0的
            if (paintData == null || paintData.Length == 0)
            {
                Debug.LogError("No paint data recorded. Cannot generate island cell types.");
                return;
            }
            
            // 计算原有能量中最多的能量
            int maxEnergyIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                if (m_crystalEnergy.NormalElement[i] > m_crystalEnergy.NormalElement[maxEnergyIndex])
                {
                    maxEnergyIndex = i;
                }
            }
            
            // 计算现存能量中最多的能量
            var ink = m_ppCellType.GetInkData();
            var currentMaxEnergyIndex = -1;
            var maxRemain = -1f;
            for (int i = 0; i < 5; i++)
            {
                var inkData = ink.GetInkByColor(m_energyColors[i]);
                if (inkData.totalAmount > 0)
                {
                    var remain = inkData.RemainingAmount;
                    if (maxRemain < remain)
                    {
                        maxRemain = remain;
                        currentMaxEnergyIndex = i;
                    }
                }
            }
            
            // 如果没有剩余能量，则使用原有能量中最多的
            var fillEnergyIndex = currentMaxEnergyIndex >= 0 ? currentMaxEnergyIndex + 1 : maxEnergyIndex + 1;
            
            // 填充所有值为0的
            for(int x= 0; x < paintData.GetLength(0); x++)
            {
                for (int y = 0; y < paintData.GetLength(1); y++)
                {
                    if (paintData[x, y] == 0)
                    {
                        paintData[x, y] = fillEnergyIndex;
                    }
                }
            }
            
            var blockId = GameManager.Instance.CurBlockId;
            Hide();
            EventCenter.Broadcast(EventType.PlayerCreateIsland, blockId, m_terrainNoise, paintData);
        }
    }
}