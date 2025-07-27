using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using AILand.GamePlay.World;

public class IslandMapEditor : EditorWindow
{
    private IslandConfigSO targetConfig;
    private CellType selectedCellType = CellType.None;
    private Vector2 scrollPosition;
    private float cellSize = 20f;
    private bool isDragging = false;
    private Vector2Int dragStart;
    private Vector2Int dragEnd;
    private bool isSelecting = false;
    private HashSet<Vector2Int> selectedCells = new HashSet<Vector2Int>();

    // CellType颜色配置
    private Dictionary<CellType, Color> cellTypeColors = new Dictionary<CellType, Color>
    {
        { CellType.None, Color.white },
        { CellType.Grassland, Color.green },
        { CellType.Water, Color.blue },
        { CellType.Mountain, Color.gray },
        { CellType.Desert, Color.yellow },
        { CellType.Forest, new Color(0f, 0.5f, 0f) }
    };

    [MenuItem("Tools/Island Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<IslandMapEditor>("Island Map Editor");
    }

    private void OnGUI()
    {
        DrawHeader();
        
        if (targetConfig == null) return;

        DrawToolbar();
        DrawGrid();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();
        
        targetConfig = (IslandConfigSO)EditorGUILayout.ObjectField("Island Config", targetConfig, typeof(IslandConfigSO), false);
        
        if (GUILayout.Button("New Config", GUILayout.Width(100)))
        {
            CreateNewConfig();
        }
        
        EditorGUILayout.EndHorizontal();

        if (targetConfig != null)
        {
            EditorGUILayout.BeginHorizontal();
            int newWidth = EditorGUILayout.IntField("Width", targetConfig.Width);
            int newHeight = EditorGUILayout.IntField("Height", targetConfig.Height);
            
            if (newWidth != targetConfig.Width || newHeight != targetConfig.Height)
            {
                if (GUILayout.Button("Resize", GUILayout.Width(60)))
                {
                    targetConfig.ResizeArray(newWidth, newHeight);
                    EditorUtility.SetDirty(targetConfig);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Cell Type:", GUILayout.Width(120));
        selectedCellType = (CellType)EditorGUILayout.EnumPopup(selectedCellType, GUILayout.Width(100));
        
        // 显示选中类型的颜色
        if (cellTypeColors.ContainsKey(selectedCellType))
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = cellTypeColors[selectedCellType];
            GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20));
            GUI.backgroundColor = oldColor;
        }

        GUILayout.FlexibleSpace();

        cellSize = EditorGUILayout.Slider("Cell Size", cellSize, 10f, 50f, GUILayout.Width(200));
        
        EditorGUILayout.EndHorizontal();

        // 操作按钮
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Select All"))
        {
            SelectAll();
        }
        
        if (GUILayout.Button("Clear Selection"))
        {
            selectedCells.Clear();
            Repaint();
        }
        
        if (GUILayout.Button("Fill Selected"))
        {
            FillSelected();
        }
        
        if (GUILayout.Button("Clear Selected"))
        {
            ClearSelected();
        }

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
    }

    private void DrawGrid()
    {
        var rect = GUILayoutUtility.GetRect(targetConfig.Width * cellSize, targetConfig.Height * cellSize);
        
        scrollPosition = GUI.BeginScrollView(
            new Rect(0, rect.y, position.width, position.height - rect.y),
            scrollPosition,
            new Rect(0, 0, targetConfig.Width * cellSize, targetConfig.Height * cellSize)
        );

        HandleGridEvents();
        
        // 绘制网格
        for (int y = 0; y < targetConfig.Height; y++)
        {
            for (int x = 0; x < targetConfig.Width; x++)
            {
                DrawCell(x, y);
            }
        }

        // 绘制选择框
        if (isDragging)
        {
            DrawSelectionRect();
        }

        GUI.EndScrollView();
    }

    private void DrawCell(int x, int y)
    {
        var cellRect = new Rect(x * cellSize, y * cellSize, cellSize, cellSize);
        var cellType = targetConfig.GetCellType(x, y);
        
        // 背景色
        Color bgColor = cellTypeColors.ContainsKey(cellType) ? cellTypeColors[cellType] : Color.white;
        
        // 如果被选中，添加高亮
        if (selectedCells.Contains(new Vector2Int(x, y)))
        {
            bgColor = Color.Lerp(bgColor, Color.red, 0.3f);
        }

        EditorGUI.DrawRect(cellRect, bgColor);
        EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellRect.width, 1), Color.black);
        EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellRect.height), Color.black);
    }

    private void HandleGridEvents()
    {
        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;
        int gridX = Mathf.FloorToInt(mousePos.x / cellSize);
        int gridY = Mathf.FloorToInt(mousePos.y / cellSize);

        if (gridX >= 0 && gridX < targetConfig.Width && gridY >= 0 && gridY < targetConfig.Height)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0) // 左键
                    {
                        if (e.control) // Ctrl+左键 = 矩形选择
                        {
                            StartRectSelection(gridX, gridY);
                        }
                        else if (e.shift) // Shift+左键 = 添加到选择
                        {
                            ToggleSelection(gridX, gridY);
                        }
                        else // 普通左键 = 绘制
                        {
                            PaintCell(gridX, gridY);
                        }
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (isDragging)
                        {
                            UpdateRectSelection(gridX, gridY);
                        }
                        else if (!e.control)
                        {
                            PaintCell(gridX, gridY);
                        }
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 0 && isDragging)
                    {
                        FinishRectSelection();
                        e.Use();
                    }
                    break;
            }
        }
    }

    private void StartRectSelection(int x, int y)
    {
        isDragging = true;
        isSelecting = true;
        dragStart = new Vector2Int(x, y);
        dragEnd = new Vector2Int(x, y);
    }

    private void UpdateRectSelection(int x, int y)
    {
        dragEnd = new Vector2Int(x, y);
        Repaint();
    }

    private void FinishRectSelection()
    {
        isDragging = false;
        
        int minX = Mathf.Min(dragStart.x, dragEnd.x);
        int maxX = Mathf.Max(dragStart.x, dragEnd.x);
        int minY = Mathf.Min(dragStart.y, dragEnd.y);
        int maxY = Mathf.Max(dragStart.y, dragEnd.y);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                selectedCells.Add(new Vector2Int(x, y));
            }
        }
        
        Repaint();
    }

    private void DrawSelectionRect()
    {
        int minX = Mathf.Min(dragStart.x, dragEnd.x);
        int maxX = Mathf.Max(dragStart.x, dragEnd.x);
        int minY = Mathf.Min(dragStart.y, dragEnd.y);
        int maxY = Mathf.Max(dragStart.y, dragEnd.y);

        var rect = new Rect(
            minX * cellSize,
            minY * cellSize,
            (maxX - minX + 1) * cellSize,
            (maxY - minY + 1) * cellSize
        );

        EditorGUI.DrawRect(rect, new Color(1, 0, 0, 0.3f));
    }

    private void PaintCell(int x, int y)
    {
        targetConfig.SetCellType(x, y, selectedCellType);
        EditorUtility.SetDirty(targetConfig);
        Repaint();
    }

    private void ToggleSelection(int x, int y)
    {
        var pos = new Vector2Int(x, y);
        if (selectedCells.Contains(pos))
            selectedCells.Remove(pos);
        else
            selectedCells.Add(pos);
        Repaint();
    }

    private void SelectAll()
    {
        selectedCells.Clear();
        for (int y = 0; y < targetConfig.Height; y++)
        {
            for (int x = 0; x < targetConfig.Width; x++)
            {
                selectedCells.Add(new Vector2Int(x, y));
            }
        }
        Repaint();
    }

    private void FillSelected()
    {
        foreach (var pos in selectedCells)
        {
            targetConfig.SetCellType(pos.x, pos.y, selectedCellType);
        }
        EditorUtility.SetDirty(targetConfig);
        Repaint();
    }

    private void ClearSelected()
    {
        foreach (var pos in selectedCells)
        {
            targetConfig.SetCellType(pos.x, pos.y, CellType.None);
        }
        EditorUtility.SetDirty(targetConfig);
        Repaint();
    }

    private void CreateNewConfig()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Island Config",
            "NewIslandConfig",
            "asset",
            "Choose location for new Island Config"
        );

        if (!string.IsNullOrEmpty(path))
        {
            var newConfig = CreateInstance<IslandConfigSO>();
            AssetDatabase.CreateAsset(newConfig, path);
            AssetDatabase.SaveAssets();
            targetConfig = newConfig;
        }
    }
}