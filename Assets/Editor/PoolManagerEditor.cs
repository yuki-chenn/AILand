#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AILand.System.ObjectPoolSystem;

namespace AILand.System.ObjectPoolSystem
{
    [CustomEditor(typeof(PoolManager))]
    public class PoolManagerEditor : Editor
    {
        private PoolManager poolManager;
        
        private void OnEnable()
        {
            poolManager = (PoolManager)target;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Refresh Pooled Objects", GUILayout.Height(30)))
            {
                poolManager.RefreshPooledObjectsList();
                EditorUtility.SetDirty(poolManager);
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pool Settings", EditorStyles.boldLabel);
            
            var poolSettingsList = serializedObject.FindProperty("poolSettingsList");
            
            for (int i = 0; i < poolSettingsList.arraySize; i++)
            {
                var element = poolSettingsList.GetArrayElementAtIndex(i);
                var prefab = element.FindPropertyRelative("prefab");
                var defaultCapacity = element.FindPropertyRelative("defaultCapacity");
                var maxSize = element.FindPropertyRelative("maxSize");
                
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(prefab, GUIContent.none);
                
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    poolSettingsList.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
                
                if (prefab.objectReferenceValue != null)
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Default Capacity", GUILayout.Width(120));
                    defaultCapacity.intValue = EditorGUILayout.IntField(defaultCapacity.intValue);
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Max Size", GUILayout.Width(120));
                    maxSize.intValue = EditorGUILayout.IntField(maxSize.intValue);
                    EditorGUILayout.EndHorizontal();
                    
                    // 运行时显示池状态
                    if (Application.isPlaying)
                    {
                        var pooledObject = (prefab.objectReferenceValue as GameObject).GetComponent<IPooledObject>();
                        if (pooledObject != null)
                        {
                            var type = pooledObject.GetType();
                            var stats = poolManager.GetPoolStats<IPooledObject>();
                            EditorGUILayout.LabelField($"Active: {stats.active}, Inactive: {stats.inactive}, Total: {stats.total}");
                        }
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            if (GUILayout.Button("Add New Pool Setting"))
            {
                poolSettingsList.arraySize++;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif