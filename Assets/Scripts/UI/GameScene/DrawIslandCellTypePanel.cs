using System;
using AILand.GamePlay;
using AILand.System.EventSystem;
using UnityEngine;
using UnityEngine.UI;
using AILand.Utils;
using EventType = AILand.System.EventSystem.EventType;
using AILand.GamePlay.World;



namespace AILand.UI
{
    public class DrawIslandCellTypePanel : BaseUIPanel
    {
        private Button m_btnGenerate;
        private Button m_btnClear;
        private PixelPainter m_ppCellType;

        // 地形噪声
        private Texture2D m_terrainTex;
        private float[,] m_terrainNosie;

        // 水晶数据
        private ElementalEnergy m_crystalEnergy;

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        protected override void BindUI()
        {
            m_btnGenerate = transform.Find("BtnGenerate").GetComponent<Button>();
            m_btnClear = transform.Find("BtnClear").GetComponent<Button>();
            m_ppCellType = transform.Find("PPCelltype").GetComponent<PixelPainter>();

            m_btnGenerate.onClick.AddListener(OnBtnGenerateClick);
            m_btnClear.onClick.AddListener(OnBtnClearClick);
        }

        protected override void BindListeners()
        {
            EventCenter.AddListener<float[,], ElementalEnergy>(EventType.ShowDrawIslandCellTypePanelUI, OnCrystallDrawCellType);
        }

        protected override void UnbindListeners()
        {
            EventCenter.RemoveListener<float[,], ElementalEnergy>(EventType.ShowDrawIslandCellTypePanelUI, OnCrystallDrawCellType);
        }

        private void OnCrystallDrawCellType(float[,] terrainNosie, ElementalEnergy energy)
        {
            m_terrainNosie = terrainNosie;
            m_terrainTex = Util.Array2GrayTexture(terrainNosie);
            m_crystalEnergy = energy;
            m_ppCellType.SetGrayMask(m_terrainTex);
            //m_ppCellType.SetTexture(m_terrainTex);
            Show();
        }

        private void OnBtnClearClick()
        {
            m_ppCellType.ClearCanvas();
        }

        private void OnBtnGenerateClick()
        {
            EventCenter.Broadcast(EventType.PlayerCreateIsland, m_terrainNosie);
        }
       
    }

}