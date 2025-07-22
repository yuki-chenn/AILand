using AILand.GamePlay.World;
using UnityEngine;

namespace AILand.Gizmos
{
    public class BlockBorderGizmos : MonoBehaviour
    {
        [Header("Gizmos Settings")]
        public bool showGizmos = true;
        
        [Header("Border Colors")]
        public Color borderColor = Color.green;
        public Color gridColor = Color.blue;
        public Color selectedColor = Color.yellow;
        
        [Header("Text Settings")]
        public bool showText = true;
        public Color textColor = Color.white;
        public int fontSize = 12;
        public Vector2 textOffset = Vector2.zero; // XZ偏移
        public float textHeight = 5f; // Y高度偏移
        
        [Header("Display Options")]
        public bool showWorldIndex = true;
        public bool showBlockID = true;
        
        private BlockData blockData
        {
            get
            {
                return GetComponent<Block>().BlockData;
            }
        }
        
        private void OnDrawGizmos()
        {
            DrawGizmos(false);
        }
        
        private void OnDrawGizmosSelected()
        {
            DrawGizmos(true);
        }
        
        public void DrawGizmos(bool isSelected = false)
        {
            if (!showGizmos) return;
            
            DrawBlockBounds(isSelected);
        }
        
        private void DrawBlockBounds(bool isSelected)
        {
            // 绘制外边框
            UnityEngine.Gizmos.color = isSelected ? selectedColor : borderColor;
            
            Vector3 blockCenter = transform.position + new Vector3(Constants.BlockWidth * 0.5f, 0, Constants.BlockHeight * 0.5f);
            Vector3 blockSize = new Vector3(Constants.BlockWidth, 1, Constants.BlockHeight);
            
            // 绘制线框立方体
            UnityEngine.Gizmos.DrawWireCube(blockCenter, blockSize);
            
            // 绘制地面网格
            UnityEngine.Gizmos.color = gridColor;
            DrawGroundGrid(transform.position);
            
            // 绘制文本信息
            if (showText)
            {
                DrawTextLabel(blockCenter);
            }
        }
        
        private void DrawGroundGrid(Vector3 blockPos)
        {
            int gridSize = 20; // 每20单位画一条线
            
            // 绘制垂直线（跳过边界线，避免与边框重合）
            for (int x = gridSize; x < Constants.BlockWidth; x += gridSize)
            {
                Vector3 start = blockPos + new Vector3(x, 0, 0);
                Vector3 end = blockPos + new Vector3(x, 0, Constants.BlockHeight);
                UnityEngine.Gizmos.DrawLine(start, end);
            }
            
            // 绘制水平线（跳过边界线，避免与边框重合）
            for (int z = gridSize; z < Constants.BlockHeight; z += gridSize)
            {
                Vector3 start = blockPos + new Vector3(0, 0, z);
                Vector3 end = blockPos + new Vector3(Constants.BlockWidth, 0, z);
                UnityEngine.Gizmos.DrawLine(start, end);
            }
        }
        
        private void DrawTextLabel(Vector3 blockCenter)
        {
            if (blockData == null) return;
            
            // 应用XZ偏移和Y高度
            Vector3 labelPosition = blockCenter + new Vector3(textOffset.x, textHeight, textOffset.y);
            
            // 构建显示文本
            string labelText = "";
            if (showWorldIndex && showBlockID)
            {
                labelText = $"({blockData.WorldIndex.x}, {blockData.WorldIndex.y})\nID: {blockData.BlockID}";
            }
            else if (showWorldIndex)
            {
                labelText = $"({blockData.WorldIndex.x}, {blockData.WorldIndex.y})";
            }
            else if (showBlockID)
            {
                labelText = $"ID: {blockData.BlockID}";
            }
            
            if (string.IsNullOrEmpty(labelText)) return;
            
            #if UNITY_EDITOR
            UnityEditor.Handles.color = textColor;
            UnityEditor.Handles.Label(labelPosition, labelText, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = textColor },
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            });
            #endif
        }
    }
}