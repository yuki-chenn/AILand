using System;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class Block : MonoBehaviour
    {
        public GameObject generatePlatform;
        public GameObject blockHolder;
        public GameObject water;
        public GameObject lowTerrain;
        
        private BlockData m_blockData;
        
        [Header("Gizmos Settings")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmosColor = Color.red;
        
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
        
        

        #region gizmos
        
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            DrawBlockBounds();
        }
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;
            
            // 当选中时用不同颜色显示
            Color originalColor = gizmosColor;
            gizmosColor = Color.yellow;
            DrawBlockBounds();
            gizmosColor = originalColor;
        }
        
        private void DrawBlockBounds()
        {
            // 绘制外边框 - 绿色
            Gizmos.color = gizmosColor;
            
            Vector3 blockCenter = transform.position + new Vector3(Constants.BlockWidth * 0.5f, 0, Constants.BlockHeight * 0.5f);
            Vector3 blockSize = new Vector3(Constants.BlockWidth, 1, Constants.BlockHeight);
            
            // 绘制线框立方体
            Gizmos.DrawWireCube(blockCenter, blockSize);
            
            // 绘制地面网格 - 蓝色
            Gizmos.color = Color.blue;
            DrawGroundGrid();
            
            // 绘制WorldIndex信息
            DrawWorldIndexLabel(blockCenter);
        }
        
        private void DrawWorldIndexLabel(Vector3 blockCenter)
        {
            if (m_blockData != null)
            {
                // 在Scene视图中显示WorldIndex文本
                Vector3 labelPosition = blockCenter + Vector3.up * 5; // 稍微抬高一点显示
                string labelText = $"({m_blockData.WorldIndex.x}, {m_blockData.WorldIndex.y})";
                
                #if UNITY_EDITOR
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(labelPosition, labelText, new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                });
                #endif
            }
        }
        
        private void DrawGroundGrid()
        {
            Vector3 blockPos = transform.position;
            int gridSize = 20; // 每20单位画一条线
            
            // 绘制垂直线（跳过边界线，避免与绿色边框重合）
            for (int x = gridSize; x < Constants.BlockWidth; x += gridSize)
            {
                Vector3 start = blockPos + new Vector3(x, 0, 0);
                Vector3 end = blockPos + new Vector3(x, 0, Constants.BlockHeight);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制水平线（跳过边界线，避免与绿色边框重合）
            for (int z = gridSize; z < Constants.BlockHeight; z += gridSize)
            {
                Vector3 start = blockPos + new Vector3(0, 0, z);
                Vector3 end = blockPos + new Vector3(Constants.BlockWidth, 0, z);
                Gizmos.DrawLine(start, end);
            }
        }

        #endregion
        
    }
}