using System;
using AILand.System.ObjectPoolSystem;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class Block : MonoBehaviour, IPooledObject
    {
        public GameObject generatePlatform;
        public Transform cubeHolder;
        public GameObject water;
        public Renderer lowTerrainRenderer;
        
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
        
        public void SetBlockActive(bool active, bool isCreated)
        {
            if (generatePlatform) generatePlatform.SetActive(active && !isCreated && m_blockData.IsPlayerCreated);
            if (water) water.SetActive(false && active && isCreated);
            if (lowTerrainRenderer) lowTerrainRenderer.gameObject.SetActive(active && isCreated);
        }
        
        public void SetLowTerrainTexture(Texture color, Texture height)
        {
            color.filterMode = FilterMode.Point;
            height.filterMode = FilterMode.Point;
            if (lowTerrainRenderer)
            {
                lowTerrainRenderer.material.SetTexture("_ColorTex", color);
                lowTerrainRenderer.material.SetTexture("_HeightTex", height);
            }
        }
        
        public void UpdateLowTerrain(Vector3 playerPos, float sight)
        {
            lowTerrainRenderer.material.SetVector("_playerPos",
                new Vector4(playerPos.x - transform.position.x - 100f, playerPos.z - transform.position.z - 100f, sight-1f));

        }

        public GameObject GameObject => gameObject;
        public void OnGetFromPool()
        {
            
        }

        public void OnReleaseToPool()
        {
            
        }

        public void OnDestroyPoolObject()
        {
            
        }

        
    }
}