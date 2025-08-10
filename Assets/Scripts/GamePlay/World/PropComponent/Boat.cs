using System;
using UnityEngine;

namespace AILand.GamePlay.World.Prop
{
    public class Boat : BaseProp
    {
        public override PropType PropType => PropType.Boat;
        
        public LayerMask closeToIslandLayer;
        public float closeToIslandRadius = 3f; 
        
        private bool m_closeToIsland = false;
        private bool m_isOnBorad = false;
        
        // 防误触的timer
        private float m_interactTimer;

        public override void SetPropData(PropData propData)
        {
            // base.SetPropData(propData);
        }

        private void Update()
        {
            if(m_interactTimer > 0) m_interactTimer -= Time.deltaTime;
            
            if (m_isOnBorad)
            {
                // 在附近的球形范围内检测
                Collider[] hits = Physics.OverlapSphere(transform.position, closeToIslandRadius, closeToIslandLayer);
                m_closeToIsland = hits.Length > 0;
                
                if (m_closeToIsland && m_interactTimer <= 0f && Input.GetKeyDown(KeyCode.F))
                {
                    GameManager.Instance.GetOffBoard(gameObject);
                    
                    if(hits[0].gameObject.tag.Equals("GeneratePlatform"))
                    {
                        gameObject.SetActive(false);
                    }
                    
                    m_isOnBorad = false;
                }
            }
        }
        
        
        public override void Interact()
        {
            // 防误触
            m_interactTimer = 0.5f;
            
            // TODO : 提交材料
            
            // 上船
            m_isOnBorad = GameManager.Instance.GetOnBoard(gameObject);
        }
    }
}