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
    private HashSet<Vector2Int> selectedCells = new HashSet<Vector2Int>();
    private bool hasChanges = false;

    private Dictionary<CellType, Color> cellTypeColors = new Dictionary<CellType, Color>
    {
        { CellType.None, Color.white },
        { CellType.Grassland, Color.green },
        { CellType.Water, Color.blue },
        { CellType.Mountain, Color.gray },
        { CellType.Desert, Color.yellow },
        { CellType.Forest, new Color(0f,0.5f,0f) }
    };

    // 预留区域的最小内边距
    private const float labelPadding = 4f;

    // 动态计算：左侧预留宽度，保证显示最大列号
    private float gridOffsetX
    {
        get
        {
            if (targetConfig == null) return 0f;
            string maxCol = (targetConfig.Width - 1).ToString();
            Vector2 size = EditorStyles.label.CalcSize(new GUIContent(maxCol));
            return size.x + labelPadding * 2;
        }
    }
    // 动态计算：顶部预留高度，保证显示最大行号
    private float gridOffsetY
    {
        get
        {
            if (targetConfig == null) return 0f;
            string maxRow = (targetConfig.Height - 1).ToString();
            Vector2 size = EditorStyles.label.CalcSize(new GUIContent(maxRow));
            return size.y + labelPadding * 2;
        }
    }

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

        if (hasChanges)
        {
            EditorUtility.SetDirty(targetConfig);
            hasChanges = false;
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();
        targetConfig = (IslandConfigSO)EditorGUILayout.ObjectField("Island Config", targetConfig, typeof(IslandConfigSO), false);
        if (GUILayout.Button("New Config", GUILayout.Width(100))) CreateNewConfig();
        if (GUILayout.Button("Save", GUILayout.Width(60))) // 添加保存按钮
        {
            if (targetConfig != null)
            {
                EditorUtility.SetDirty(targetConfig);
                AssetDatabase.SaveAssets();
            }
        }
        EditorGUILayout.EndHorizontal();

        if (targetConfig != null)
        {
            EditorGUILayout.BeginHorizontal();
            int newWidth = EditorGUILayout.IntField("Width", targetConfig.Width);
            int newHeight = EditorGUILayout.IntField("Height", targetConfig.Height);
            if ((newWidth != targetConfig.Width || newHeight != targetConfig.Height) && GUILayout.Button("Resize", GUILayout.Width(60)))
            {
                targetConfig.ResizeArray(newWidth, newHeight);
                hasChanges = true;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Cell Type:", GUILayout.Width(120));
        selectedCellType = (CellType)EditorGUILayout.EnumPopup(selectedCellType, GUILayout.Width(100));
        if (cellTypeColors.TryGetValue(selectedCellType, out Color col))
        {
            var old = GUI.backgroundColor;
            GUI.backgroundColor = col;
            GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20));
            GUI.backgroundColor = old;
        }
        GUILayout.FlexibleSpace();
        cellSize = EditorGUILayout.Slider("Cell Size", cellSize, 10f, 50f, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All")) SelectAll();
        if (GUILayout.Button("Clear Selection")) { selectedCells.Clear(); Repaint(); }
        if (GUILayout.Button("Fill Selected")) { FillSelected(); }
        if (GUILayout.Button("Clear Selected")) { ClearSelected(); }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    private void DrawGrid()
    {
        // 整个内容区需要包括预留的标签空间
        float totalW = targetConfig.Width * cellSize + gridOffsetX;
        float totalH = targetConfig.Height * cellSize + gridOffsetY;
        // 获取一个足够高的 Rect，避免和上面按钮重叠
        var rect = GUILayoutUtility.GetRect(totalW, totalH);

        float viewWidth = position.width;
        float viewHeight = position.height - rect.y;

        // 绘制行/列标签
        DrawLabels(rect);

        // 滚动区域从 (gridOffsetX, gridOffsetY) 开始
        scrollPosition = GUI.BeginScrollView(
            new Rect(rect.x + gridOffsetX, rect.y + gridOffsetY, viewWidth - gridOffsetX, viewHeight - gridOffsetY),
            scrollPosition,
            new Rect(0, 0, targetConfig.Width * cellSize, targetConfig.Height * cellSize)
        );

        // 处理鼠标事件逻辑保持不变
        HandleGridEvents();

        int startX = Mathf.Max(0, Mathf.FloorToInt(scrollPosition.x / cellSize));
        int endX = Mathf.Min(targetConfig.Width, Mathf.CeilToInt((scrollPosition.x + (viewWidth - gridOffsetX)) / cellSize));
        int startY = Mathf.Max(0, Mathf.FloorToInt(scrollPosition.y / cellSize));
        int endY = Mathf.Min(targetConfig.Height, Mathf.CeilToInt((scrollPosition.y + (viewHeight - gridOffsetY)) / cellSize));

        // 绘制单元格
        for (int y = startY; y < endY; y++)
            for (int x = startX; x < endX; x++)
                DrawCell(x, y);

        if (isDragging) DrawSelectionRect();

        GUI.EndScrollView();
    }

    private void DrawLabels(Rect rect)
    {
        // 计算可见行/列范围
        int startX = Mathf.Max(0, Mathf.FloorToInt(scrollPosition.x / cellSize));
        int endX = Mathf.Min(targetConfig.Width, Mathf.CeilToInt((scrollPosition.x + (position.width - gridOffsetX)) / cellSize));
        int startY = Mathf.Max(0, Mathf.FloorToInt(scrollPosition.y / cellSize));
        int endY = Mathf.Min(targetConfig.Height, Mathf.CeilToInt((scrollPosition.y + (position.height - rect.y - gridOffsetY)) / cellSize));

        // 绘制列号
        for (int x = startX; x < endX; x++)
        {
            string label = x.ToString();
            Vector2 size = EditorStyles.label.CalcSize(new GUIContent(label));
            float px = rect.x + gridOffsetX + x * cellSize - scrollPosition.x + (cellSize - size.x) / 2;
            float py = rect.y + (labelPadding / 2);
            GUI.Label(new Rect(px, py, size.x, size.y), label);
        }

        // 绘制行号
        for (int y = startY; y < endY; y++)
        {
            string label = y.ToString();
            Vector2 size = EditorStyles.label.CalcSize(new GUIContent(label));
            float px = rect.x + (labelPadding / 2);
            float py = rect.y + gridOffsetY + y * cellSize - scrollPosition.y + (cellSize - size.y) / 2;
            GUI.Label(new Rect(px, py, size.x, size.y), label);
        }
    }

    private void DrawCell(int x, int y)
    {
        var cellRect = new Rect(x * cellSize, y * cellSize, cellSize, cellSize);
        var cellType = targetConfig.GetCellType(x, y);
        Color bg = cellTypeColors.ContainsKey(cellType) ? cellTypeColors[cellType] : Color.white;
        if (selectedCells.Contains(new Vector2Int(x, y)))
            bg = Color.Lerp(bg, Color.red, 0.3f);

        EditorGUI.DrawRect(cellRect, bg);
        EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellRect.width, 1), Color.black);
        EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellRect.height), Color.black);
    }

    private void HandleGridEvents()
    {
        var e = Event.current;
        var mousePos = e.mousePosition;
        int gridX = Mathf.FloorToInt(mousePos.x / cellSize);
        int gridY = Mathf.FloorToInt(mousePos.y / cellSize);

        if (gridX >= 0 && gridX < targetConfig.Width && gridY >= 0 && gridY < targetConfig.Height)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (e.control) StartRectSelection(gridX, gridY);
                        else if (e.shift) ToggleSelection(gridX, gridY);
                        else PaintCell(gridX, gridY);
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (isDragging) UpdateRectSelection(gridX, gridY);
                        else PaintCell(gridX, gridY);
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0 && isDragging) FinishRectSelection(); e.Use();
                    break;
            }
        }
    }

    private void StartRectSelection(int x, int y) { isDragging = true; dragStart = dragEnd = new Vector2Int(x, y); }
    private void UpdateRectSelection(int x, int y) { dragEnd = new Vector2Int(x, y); Repaint(); }
    private void FinishRectSelection()
    {
        isDragging = false;
        int minX = Mathf.Min(dragStart.x, dragEnd.x), maxX = Mathf.Max(dragStart.x, dragEnd.x);
        int minY = Mathf.Min(dragStart.y, dragEnd.y), maxY = Mathf.Max(dragStart.y, dragEnd.y);
        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
                selectedCells.Add(new Vector2Int(x, y));
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
        hasChanges = true;
        Repaint();
    }

    private void ToggleSelection(int x, int y)
    {
        var pos = new Vector2Int(x, y);
        if (selectedCells.Contains(pos)) selectedCells.Remove(pos);
        else selectedCells.Add(pos);
        Repaint();
    }

    private void SelectAll()
    {
        selectedCells.Clear();
        for (int y = 0; y < targetConfig.Height; y++)
            for (int x = 0; x < targetConfig.Width; x++)
                selectedCells.Add(new Vector2Int(x, y));
        Repaint();
    }

    private void FillSelected()
    {
        foreach (var pos in selectedCells)
            targetConfig.SetCellType(pos.x, pos.y, selectedCellType);
        hasChanges = true;
        Repaint();
    }

    private void ClearSelected()
    {
        foreach (var pos in selectedCells)
            targetConfig.SetCellType(pos.x, pos.y, CellType.None);
        hasChanges = true;
        Repaint();
    }

    private void CreateNewConfig()
    {
        string path = EditorUtility.SaveFilePanelInProject("Create Island Config", "NewIslandConfig", "asset", "Choose location");
        if (!string.IsNullOrEmpty(path))
        {
            var newConfig = CreateInstance<IslandConfigSO>();
            AssetDatabase.CreateAsset(newConfig, path);
            AssetDatabase.SaveAssets();
            targetConfig = newConfig;
        }
    }
}
