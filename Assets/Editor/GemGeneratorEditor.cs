using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GemGenerator))]
public class GemGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GemGenerator generator = (GemGenerator)target;

        EditorGUILayout.LabelField("宝石生成器", EditorStyles.boldLabel);

        generator.gemType = (GemType)EditorGUILayout.EnumPopup("宝石类型", generator.gemType);
        generator.complexity = EditorGUILayout.IntSlider("复杂度", generator.complexity, 3, 20);
        generator.irregularity = EditorGUILayout.Slider("不规则程度", generator.irregularity, 0f, 1f);
        generator.sharpness = EditorGUILayout.Slider("尖锐度", generator.sharpness, 0f, 1f);
        generator.noiseScale = EditorGUILayout.Slider("噪声强度", generator.noiseScale, 0f, 2f);
        generator.scale = EditorGUILayout.Vector3Field("缩放", generator.scale);
        generator.randomSeed = EditorGUILayout.IntField("随机种子", generator.randomSeed);

        EditorGUILayout.Space();

        if (GUILayout.Button("生成新宝石", GUILayout.Height(30)))
        {
            generator.GenerateGem();
        }

        if (GUILayout.Button("随机参数"))
        {
            generator.RandomizeParameters();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("复杂度: 控制宝石的面数\n不规则程度: 控制形状的随机变化\n尖锐度: 控制边缘的锐利程度\n噪声强度: 控制表面细节", MessageType.Info);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(generator);
        }
    }
}