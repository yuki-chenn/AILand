using AILand.GamePlay.World;
using System.Collections.Generic;
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

        [Header("Island Type Grid Colors")]
        public Color noneGridColor = Color.gray;
        public Color customGridColor = Color.white;
        public Color waterGridColor = Color.blue;
        public Color plainGridColor = Color.green;
        public Color forestGridColor = Color.cyan;
        public Color mountainGridColor = Color.red;
        public Color glacierGridColor = Color.magenta;
        public Color infernoGridColor = Color.yellow;

        [Header("Cell Water Display")]
        public bool showCellWater = false;
        public Color innerWaterColor = new Color(0.5f, 0.8f, 1f, 0.7f); // 淡蓝色
        public Color outerWaterColor = new Color(0.2f, 0.4f, 0.8f, 0.7f); // 深蓝色
        public Color noneWaterColor = new Color(0.5f, 1f, 0.5f, 0.7f); // 绿色
        public Color borderWaterColor = new Color(1f, 1f, 0.5f, 0.7f); // 黄色

        [Header("Text Settings")]
        public bool showText = true;
        public Color textColor = Color.white;
        public int fontSize = 12;
        public Vector2 textOffset = Vector2.zero; // XZ偏移
        public float textHeight = 5f; // Y高度偏移

        [Header("Display Options")]
        public bool showWorldIndex = true;
        public bool showBlockID = true;
        public bool showIslandType = true;

        private BlockData blockData => GetComponent<Block>().BlockData;

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

            if (showCellWater)
            {
                DrawCellWater();
            }
        }

        private void DrawBlockBounds(bool isSelected)
        {
            // 绘制外边框
            UnityEngine.Gizmos.color = isSelected ? selectedColor : borderColor;

            Vector3 blockCenter = transform.position + new Vector3(Constants.BlockWidth * 0.5f, 0, Constants.BlockHeight * 0.5f);
            Vector3 blockSize = new Vector3(Constants.BlockWidth, 1, Constants.BlockHeight);

            // 绘制线框立方体
            UnityEngine.Gizmos.DrawWireCube(blockCenter, blockSize);

            // 绘制地面网格 - 根据IslandType选择颜色
            Color currentGridColor = GetGridColorForIslandType();
            UnityEngine.Gizmos.color = currentGridColor;
            DrawGroundGrid(transform.position);

            // 绘制文本信息
            if (showText)
            {
                DrawTextLabel(blockCenter);
            }
        }

        private Color GetGridColorForIslandType()
        {
            if (blockData == null) return gridColor;

            return blockData.IslandType switch
            {
                IslandType.None => noneGridColor,
                IslandType.Custom => customGridColor,
                IslandType.Water => waterGridColor,
                IslandType.Plain => plainGridColor,
                IslandType.Forest => forestGridColor,
                IslandType.Mountain => mountainGridColor,
                IslandType.Glacier => glacierGridColor,
                IslandType.Inferno => infernoGridColor,
                _ => gridColor
            };
        }

        private void DrawCellWater()
        {
            if (blockData == null) return;

            Vector3 blockPos = transform.position;
            Vector3 cellSize = new Vector3(1f, 0.1f, 1f); // 每个cell是1x1，高度设为0.1避免z-fighting

            for (int x = 0; x < Constants.BlockWidth; x++)
            {
                for (int z = 0; z < Constants.BlockHeight; z++)
                {
                    // 获取cell的水类型
                    var waterType = GetCellWaterType(x, z);

                    // 根据水类型选择颜色
                    Color cellColor = GetColorForWaterType(waterType);

                    // 计算cell中心位置
                    Vector3 cellCenter = blockPos + new Vector3(x, 0.05f, z);

                    // 绘制cell
                    UnityEngine.Gizmos.color = cellColor;
                    UnityEngine.Gizmos.DrawCube(cellCenter, cellSize);
                }
            }
        }

        private CellWater GetCellWaterType(int x, int z)
        {
            var cell = blockData.GetCellData(x, z);
            if (cell == null) return CellWater.OuterWater;
            return cell.CellWater;
        }

        private Color GetColorForWaterType(CellWater waterType)
        {
            return waterType switch
            {
                CellWater.InnerWater => innerWaterColor,
                CellWater.OuterWater => outerWaterColor,
                CellWater.BorderWater => borderWaterColor,
                CellWater.None => noneWaterColor,
                _ => outerWaterColor
            };
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
            string labelText = BuildLabelText();

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

        private string BuildLabelText()
        {
            List<string> textParts = new List<string>();

            if (showWorldIndex)
            {
                textParts.Add($"({blockData.WorldIndex.x}, {blockData.WorldIndex.y})");
            }

            if (showBlockID)
            {
                textParts.Add($"ID: {blockData.BlockID}");
            }

            if (showIslandType)
            {
                textParts.Add($"Type: {blockData.IslandType}");
            }

            return string.Join("\n", textParts);
        }
    }
}