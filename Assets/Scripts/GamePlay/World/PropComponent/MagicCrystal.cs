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

        private MagicCrystalPropData m_propData;
        public MagicCrystalPropData PropData => m_propData;

        // 水晶的材质
        private Material m_material;
        
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

        public override void SetPropData(PropData propData)
        {
            m_propData = propData as MagicCrystalPropData;
            UpdateCrystalMaterial();
            m_frScript.Reset(propData.LocalPosition);
            m_frScript.enable = m_propData.IsCharged;
        }


        /// <summary>
        /// 注入能量
        /// </summary>
        public void Charge(NormalElement element)
        {
            if(m_propData.Energy == null) m_propData.Energy = new ElementalEnergy();
            m_propData.Energy.NormalElement = element;
            m_propData.IsCharged = true;
            m_frScript.enable = true;
            UpdateCrystalMaterial();
        }
        
        public void Charge(EnergyType energyType, int amount=1)
        {
            if (m_propData.Energy == null) m_propData.Energy = new ElementalEnergy();
            m_propData.Energy.AddEnergy(energyType, amount);
            m_propData.IsCharged = true;
            m_frScript.enable = true;
            UpdateCrystalMaterial();
        }

        private void UpdateCrystalMaterial()
        {
            if (m_propData == null) return;
            
            if (!m_propData.IsCharged || m_propData.Energy == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    m_material.SetColor(shaderColorProperties[i], Color.gray);
                }
                return;
            }
            
            var sortedIndex = m_propData.Energy.GetSortedIndex();



            for (int i = 0; i < 5; i++)
            {
                var count = m_propData.Energy.NormalElement[sortedIndex[i]];
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
            
            if (!m_propData.IsCharged)
            {
                Debug.Log("Magic Crystal is not charged yet.");
                return;
            }

            if (!m_propData.IsActive)
            {
                // 激活水晶，创建岛屿
                EventCenter.Broadcast(EventType.ShowDrawIslandShapePanelUI, m_propData.Energy);
            }
            else
            {
                // 回满血,并设置出生点
                DataManager.Instance.PlayerData.RestoreAllHp();
                DataManager.Instance.PlayerData.SetRebirthPosition(GameManager.Instance.player.transform.position + new Vector3(0,5,0));
            }

            
        }
        #endregion
    }
}