using UnityEngine;

namespace AILand.GamePlay.World
{
    public class Block : MonoBehaviour
    {
        public GameObject generatePlatform;
        public GameObject cubeHolder;
        public GameObject water;
        public GameObject lowTerrain;
        
        private BlockData m_blockData;
        public BlockData BlockData => m_blockData;
        
        public void SetBlockData(BlockData blockData)
        {
            m_blockData = blockData;
            transform.position = m_blockData.WorldPosition;
            gameObject.name = $"Block_{m_blockData.BlockID}_{m_blockData.WorldIndex.x}_{m_blockData.WorldIndex.y}";
            
            if (generatePlatform)
            {
                generatePlatform.transform.localPosition = m_blockData.GeneratorPosition;
                generatePlatform.SetActive(false);
            }
        }


        public void ShowGeneratePlatform(bool show)
        {
            if (generatePlatform)
            {
                generatePlatform.SetActive(show);
            }
        }
        
    }
}