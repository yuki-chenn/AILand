using AILand.System.EventSystem;
using System.Threading.Tasks;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.GamePlay.World.Prop
{
    public class MagicCrystal : BaseProp
    {
        public override PropType PropType => PropType.MagicCrystal;
        
        // 是否是天然生成的
        private bool m_isNatural;
        public bool IsNatural
        {
            get => m_isNatural;
            set => m_isNatural = value;
        }

        // 是否已经被充能
        private bool m_isCharged;
        public bool IsCharged
        {
            get => m_isCharged;
            set => m_isCharged = value;
        }

        // 是否已激活（已经创建过岛屿）
        private bool m_isActive;
        public bool IsActive
        {
            get => m_isActive;
            set => m_isActive = value;
        }

        // 水晶的材质
        private Material m_material;
        private Material m_unchargedMaterial;

        private ElementalEnergy m_energy;

        protected override void Awake()
        {
            base.Awake();
            m_isNatural = true; // 默认是天然生成的
            m_material = GetComponent<Renderer>().material;
            m_unchargedMaterial = m_material; // 保存未充能时的材质
        }

        /// <summary>
        /// 注入能量
        /// </summary>
        public void Charge(NormalElement element)
        {
            if(m_energy == null) m_energy = new ElementalEnergy();
            m_energy.NormalElement = element;
            m_isCharged = true;
            UpdateCrystalMaterial();
        }

        private void UpdateCrystalMaterial()
        {
            // parallax color
            // Tagnet Space Map
            // color & emission color(hdr +light)
            // color top & color bottom
            // rim light color & gitter color
        }

        #region 交互接口

        public override void Interact()
        {
            Debug.Log("Interact with Magic Crystal");

            if (!m_isCharged)
            {
                Debug.Log("Magic Crystal is not charged yet.");
                return;
            }

            if (!m_isActive)
            {
                EventCenter.Broadcast(EventType.ShowDrawIslandShapePanelUI, m_energy);
            }

            
        }
        #endregion

    }
}