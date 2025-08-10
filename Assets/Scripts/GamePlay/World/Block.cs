using System;
using System.Collections;
using AILand.System.ObjectPoolSystem;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class Block : MonoBehaviour, IPooledObject
    {
        public GameObject GameObject => gameObject;

        public GameObject generatePlatform;
        public Transform cubeHolder;
        public Transform propHolder;
        public GameObject water;
        public Renderer lowTerrainRenderer;
        
        private BlockData m_blockData;
        public BlockData BlockData => m_blockData;
        
        private bool m_isRecreatingMesh = false;

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
            if (water) water.SetActive(active);
            if (lowTerrainRenderer) lowTerrainRenderer.gameObject.SetActive(active && isCreated);
        }
        
        
        public void SetCrystal(GameObject crystal)
        {
            if (generatePlatform)
            {
                Transform crystalSet = generatePlatform.transform.Find("CrystalSet");
                if (crystalSet != null)
                {
                    crystal.transform.SetParent(crystalSet);
                    crystal.transform.localPosition = Vector3.zero;
                }
            }
        }
        
        
        
        public void SetLowTerrainTexture(Texture color, Texture height, Vector3 playerPos, int sight)
        {
            
            color.filterMode = FilterMode.Point;
            height.filterMode = FilterMode.Point;
            if (lowTerrainRenderer)
            {
                lowTerrainRenderer.material.SetTexture("_ColorTex", color);
                lowTerrainRenderer.material.SetTexture("_HeightTex", height);
                // 生成形变后的网格
                if (GameManager.Instance.CurBlockId == m_blockData.BlockID && !m_isRecreatingMesh)
                {
                    StartCoroutine(
                        CreateDeformedMesh(
                            lowTerrainRenderer.GetComponent<MeshFilter>().sharedMesh, height as Texture2D, playerPos, sight)
                    );
                }
            }
        }
        
        
        IEnumerator CreateDeformedMesh(Mesh originalMesh, Texture2D heightTexture, Vector3 playerPos, int sight, int loadFrame = 200)
        {
            m_isRecreatingMesh = true;
            if (heightTexture == null) yield break;
    
            // 创建新的网格实例
            Mesh newMesh = Instantiate(originalMesh);
            Vector3[] vertices = newMesh.vertices;
            Vector2[] uvs = newMesh.uv;
    
            var loadPerFrame = vertices.Length / loadFrame;
            
            // 根据高度图修改顶点位置
            int count = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 uv = uvs[i];
        
                // 采样高度图
                int x = Mathf.FloorToInt(uv.x * (heightTexture.width - 1));
                int y = Mathf.FloorToInt(uv.y * (heightTexture.height - 1));
        
                // 确保坐标在有效范围内
                x = Mathf.Clamp(x, 0, heightTexture.width - 1);
                y = Mathf.Clamp(y, 0, heightTexture.height - 1);
        
                // 获取高度值（假设使用红色通道）
                Color heightColor = heightTexture.GetPixel(x, y);
                float height = heightColor.r * 5 + heightColor.g * 10 + heightColor.b * 35 - 0.5f;

                // 如果在视野范围内，则设置为-0.5f
                int playerX = Mathf.RoundToInt(playerPos.x);
                int playerZ = Mathf.RoundToInt(playerPos.z);
                if (Mathf.Abs(playerX - x) <= sight && Mathf.Abs(playerZ - y) <= sight) height = -0.5f;
                
                
                if (height <= -0.5f) height = -2f; 
                
                
                
                vertices[i].y += height; 
                
                count++;
                if (count >= loadPerFrame)
                {
                    count = 0;
                    yield return null;
                }
            }
    
            // 更新网格数据
            newMesh.vertices = vertices;
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
    
            lowTerrainRenderer.GetComponent<MeshCollider>().sharedMesh = newMesh;
            m_isRecreatingMesh = false;
        }
        
        public void UpdateLowTerrain(Vector3 playerPos, float sight)
        {
            lowTerrainRenderer.material.SetVector("_playerPos",
                new Vector4(playerPos.x - transform.position.x - 100f, playerPos.z - transform.position.z - 100f, sight));

        }

        


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