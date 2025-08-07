using System;
using AILand.System.EventSystem;
using System.Threading.Tasks;
using AILand.Utils;
using GamePlay.InventorySystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.GamePlay.World.Prop
{
    public class MagicCrystal : BaseProp
    {
        public override PropType PropType => PropType.MagicCrystal;
        
        private FloatAndRotate m_frScript;
        
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

        private ElementalEnergy m_energy;
        public ElementalEnergy Energy => m_energy;

        
        private string[] shaderColorProperties = 
        {
            "_ParallaxColor", 
            "_TangentSpaceMapColor", 
            "_Color", 
            "_RimLightColor", 
            "_GlitterColor"
        };
        
        private float[] shaderColorIntensities = 
        {
            2.5f, // Parallax Color
            1f, // Tangent Space Map Color
            2f, // Color 
            3f, // Rim Light Color
            3f // Glitter Color
        };

        protected override void Awake()
        {
            base.Awake();
            m_isNatural = true; // 默认是天然生成的
            m_material = GetComponent<Renderer>().material;
            m_frScript = GetComponent<FloatAndRotate>();
            if(m_frScript == null)
            {
                m_frScript = gameObject.AddComponent<FloatAndRotate>();
            }
        }

        private void Start()
        {
            UpdateCrystalMaterial();
        }


        /// <summary>
        /// 注入能量
        /// </summary>
        public void Charge(NormalElement element)
        {
            if(m_energy == null) m_energy = new ElementalEnergy();
            m_energy.NormalElement = element;
            m_isCharged = true;
            m_frScript.enable = true;
            UpdateCrystalMaterial();
        }
        
        public void Charge(EnergyType energyType, int amount=1)
        {
            if (m_energy == null) m_energy = new ElementalEnergy();
            m_energy.AddEnergy(energyType, amount);
            m_isCharged = true;
            m_frScript.enable = true;
            UpdateCrystalMaterial();
        }

        private void UpdateCrystalMaterial()
        {
            if (!m_isCharged || m_energy == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    m_material.SetColor(shaderColorProperties[i], Color.gray);
                }
                return;
            }
            
            var sortedIndex = m_energy.GetSortedIndex();



            for (int i = 0; i < 5; i++)
            {
                var count = m_energy.NormalElement[sortedIndex[i]];
                if(count == 0) break;
                
                var energyRatio = Mathf.Clamp01(count / 100f);
                var energyType = Util.GetSelectedEnergyType(sortedIndex[i]);
                var energyColor = Constants.energyColors[energyType];

                var t = shaderColorIntensities[i];
                float hdrIntensity = -t + energyRatio * 2 * t; 
                Color hdrColor = energyColor;
                hdrColor *= Mathf.Pow(2f, hdrIntensity);
                m_material.SetColor(shaderColorProperties[i], hdrColor);
            }
        }

        #region 交互接口

        public override void OnFocus()
        {
            if (GameManager.Instance.GetCurrentSelectItem().config.itemType != ItemType.InfiniteGauntlet)
            {
                OnLostFocus();
                return;
            }
            base.OnFocus();
        }
        

        public override void Interact()
        {
            
            if(GameManager.Instance.GetCurrentSelectItem().config.itemType != ItemType.InfiniteGauntlet) return;
            Debug.Log("Interact with Magic Crystal");
            
            if (!m_isCharged)
            {
                Debug.Log("Magic Crystal is not charged yet.");
                return;
            }

            if (!m_isActive)
            {
                // 激活水晶，创建岛屿
                EventCenter.Broadcast(EventType.ShowDrawIslandShapePanelUI, m_energy);
            }
            else
            {
                // 回满血,并设置出生点
                DataManager.Instance.PlayerData.RestoreAllHp();
                DataManager.Instance.PlayerData.SetRebirthPosition(GameManager.Instance.player.transform.position + new Vector3(0,5,0));
            }

            
        }
        #endregion

        public override void OnGetFromPool()
        {
            base.OnGetFromPool();
            var magicCrystalComp = WorldManager.Instance?.GetBlockMagicCrystalInstance();
            if (magicCrystalComp == null)
            {
                // Debug.LogError($"MagicCrystal component not found in Block instance.");
                return;
            }
            
            m_isNatural = magicCrystalComp.IsNatural;
            m_isCharged = magicCrystalComp.IsCharged;
            m_isActive = true;
            m_energy = magicCrystalComp.Energy;
            m_frScript.enable = m_isCharged;
            UpdateCrystalMaterial();
        }

        public override void OnReleaseToPool()
        {
            base.OnReleaseToPool();
            m_isNatural = true; // 重置为天然生成
            m_isCharged = false; // 重置为未充能
            m_isActive = false; // 重置为未激活
            m_energy = null; // 清空能量
            m_frScript.enable = false; // 停止浮动和旋转
            UpdateCrystalMaterial(); // 更新材质以反映状态变化
        }
    }
}