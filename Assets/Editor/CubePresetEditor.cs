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

            // 手动编辑区域范围
            EditorGUI.BeginChangeCheck();
            Vector3Int newMinPoint = EditorGUILayout.Vector3IntField("左下角点", cubePreset.minPoint);
            Vector3Int newMaxPoint = EditorGUILayout.Vector3IntField("右上角点", cubePreset.maxPoint);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(preset, "修改预设区域");
                cubePreset.minPoint = newMinPoint;
                cubePreset.maxPoint = newMaxPoint;
                EditorUtility.SetDirty(preset);
                EditorUtility.SetDirty(cubePreset);
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("从场景中构建预设"))
            {
                BuildPresetFromScene(preset, cubePreset);
                AutoCalculateRange(preset, cubePreset);
            }

            if (GUILayout.Button("在场景中预览"))
            {
                PreviewInScene(preset);
            }

            if (GUILayout.Button("自动计算区域"))
            {
                AutoCalculateRange(preset, cubePreset);
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
                int rotation = (Mathf.RoundToInt(cube.transform.rotation.eulerAngles.y / 90f) % 4 + 4) % 4;
                preset.AddCube(localPos, cube.CubeType, rotation);
            }

            EditorUtility.SetDirty(preset);
            Debug.Log($"预设构建完成，包含 {preset.cubes.Count} 个方块");
        }

        private void AutoCalculateRange(CubePresetSO preset, CubePreset cubePreset)
        {
            if (preset.cubes.Count == 0)
            {
                Debug.LogWarning("没有方块数据，无法计算区域");
                return;
            }

            // 计算边界
            Vector3Int minPos = preset.cubes[0].position;
            Vector3Int maxPos = preset.cubes[0].position;

            foreach (var cubeData in preset.cubes)
            {
                minPos = Vector3Int.Min(minPos, cubeData.position);
                maxPos = Vector3Int.Max(maxPos, cubeData.position);
            }
            
            Undo.RecordObject(preset, "自动计算预设区域");
            cubePreset.minPoint = minPos;
            cubePreset.maxPoint = maxPos;
            
            EditorUtility.SetDirty(preset);
            EditorUtility.SetDirty(cubePreset);
            SceneView.RepaintAll();
            
            Vector3Int size = maxPos - minPos + Vector3Int.one;
            Debug.Log($"自动计算区域完成: 左下角{minPos}, 右上角{maxPos}, 大小{size}");
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