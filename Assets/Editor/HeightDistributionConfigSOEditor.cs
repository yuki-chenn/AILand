#if UNITY_EDITOR
using AILand.GamePlay.World;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeightDistributionConfigSO))]
public class HeightDistributionConfigSOEditor : Editor
{
    // 方便编辑时手动刷新预览
    bool showPreview = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();  // 先绘制默认字段

        GUILayout.Space(8);
        showPreview = EditorGUILayout.Foldout(showPreview, "分布预览");
        if (showPreview)
        {
            DrawDistributionBar();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawDistributionBar()
    {
        var so = target as HeightDistributionConfigSO;
        if (so.distributions == null || so.distributions.Count == 0)
            return;

        // 计算总权重
        float total = 0f;
        foreach (var d in so.distributions) total += Mathf.Max(0f, d.weight);

        // 条形图区域
        Rect rect = GUILayoutUtility.GetRect(10, 20, GUILayout.ExpandWidth(true));
        float x = rect.x;

        // 为每个 CubeType 随机分配一个颜色（可根据需要改成固定映射）
        System.Func<int, Color> PickColor = idx =>
        {
            // 简易配色
            Color[] cols = { Color.green, Color.gray, Color.yellow, Color.magenta, Color.cyan };
            return cols[idx % cols.Length];
        };

        for (int i = 0; i < so.distributions.Count; i++)
        {
            var d = so.distributions[i];
            float w = Mathf.Max(0f, d.weight);
            float wRatio = total > 0 ? w / total : 1f / so.distributions.Count;
            float width = rect.width * wRatio;

            var slice = new Rect(x, rect.y, width, rect.height);
            EditorGUI.DrawRect(slice, PickColor(i));
            // 在颜色块上标注权重
            GUI.Label(slice, $"{d.cubeType}:{w:F2}", EditorStyles.whiteLabel);

            x += width;
        }
    }
}
#endif
