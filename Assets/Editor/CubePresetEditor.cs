#if UNITY_EDITOR
using AILand.GamePlay.World;
using AILand.GamePlay.World.Cube;
using UnityEditor;
using UnityEngine;

namespace AILand.Utils
{
    [CustomEditor(typeof(CubePreset))]
    public class CubePresetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CubePreset cubePreset = target as CubePreset;
            CubePresetSO preset = cubePreset?.presetData;

            if (preset == null)
            {
                EditorGUILayout.HelpBox("请先分配预设数据", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("预设信息", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"方块数量: {preset.cubes.Count}");

            // 手动编辑size
            EditorGUI.BeginChangeCheck();
            Vector3Int newSize = EditorGUILayout.Vector3IntField("预设大小", preset.size);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(preset, "修改预设大小");
                preset.size = newSize;
                // 同步到CubePreset组件
                cubePreset.size = newSize;
                EditorUtility.SetDirty(preset);
                EditorUtility.SetDirty(cubePreset);
                // 重绘Scene视图
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("从场景中构建预设"))
            {
                BuildPresetFromScene(preset, cubePreset);
                AutoCalculateSize(preset, cubePreset);
            }

            if (GUILayout.Button("在场景中预览"))
            {
                PreviewInScene(preset);
            }

            if (GUILayout.Button("自动计算大小"))
            {
                AutoCalculateSize(preset, cubePreset);
            }
        }

        private void BuildPresetFromScene(CubePresetSO preset, CubePreset cubePreset)
        {
            var parent = cubePreset.transform;

            // 查找该物体下的所有方块
            var cubes = parent.GetComponentsInChildren<BaseCube>();
            
            Undo.RecordObject(preset, "从场景构建预设");
            preset.cubes.Clear();

            if (cubes.Length == 0)
            {
                Debug.LogWarning($"{parent.name} 中没有找到方块");
                return;
            }

            // 添加方块数据
            foreach (var cube in cubes)
            {
                Vector3Int localPos = Vector3Int.RoundToInt(cube.transform.localPosition);
                preset.AddCube(localPos, cube.CubeType);
            }

            EditorUtility.SetDirty(preset);
            Debug.Log($"预设构建完成，包含 {preset.cubes.Count} 个方块");
        }

        private void AutoCalculateSize(CubePresetSO preset, CubePreset cubePreset)
        {
            if (preset.cubes.Count == 0)
            {
                Debug.LogWarning("没有方块数据，无法计算大小");
                return;
            }

            // 计算边界
            Vector3Int min = preset.cubes[0].position;
            Vector3Int max = preset.cubes[0].position;

            foreach (var cubeData in preset.cubes)
            {
                min = Vector3Int.Min(min, cubeData.position);
                max = Vector3Int.Max(max, cubeData.position);
            }

            Vector3Int calculatedSize = max - min + Vector3Int.one;
            
            Undo.RecordObject(preset, "自动计算预设大小");
            preset.size = calculatedSize;
            cubePreset.size = calculatedSize;
            
            EditorUtility.SetDirty(preset);
            EditorUtility.SetDirty(cubePreset);
            SceneView.RepaintAll();
            
            Debug.Log($"自动计算大小完成: {calculatedSize}");
        }

        private void PreviewInScene(CubePresetSO preset)
        {
            var parent = (target as CubePreset)?.transform;

            if (preset == null || preset.cubes.Count == 0)
            {
                Debug.LogWarning("预设数据为空，无法预览");
                return;
            }

            // 清除之前的预览
            var existingCubes = parent.GetComponentsInChildren<BaseCube>();
            foreach (var cube in existingCubes)
            {
                DestroyImmediate(cube.gameObject);
            }

            // 创建新的预览方块
            foreach (var cubeData in preset.cubes)
            {
                string path = $"Assets/Prefabs/Cube/{cubeData.cubeType}.prefab";
                GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (cubePrefab == null)
                {
                    Debug.LogWarning($"未找到方块预设: {path}");
                    continue;
                }

                Vector3 position = new Vector3(cubeData.position.x, cubeData.position.y, cubeData.position.z) + parent.position;
                GameObject cubeInstance = Instantiate(cubePrefab, position, Quaternion.identity, parent);
                cubeInstance.name = $"{cubeData.cubeType} ({position})";
            }
        }
    }
}
#endif