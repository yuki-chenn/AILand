using AILand.GamePlay.World;
using UnityEngine;

namespace AILand.Utils
{
    public class CubePreset : MonoBehaviour
    {
        public CubePresetSO presetData;

        [SerializeField][HideInInspector] private Vector3Int m_size;
        
        public Vector3Int size
        {
            get 
            { 
                if (presetData != null)
                    return presetData.size;
                return m_size;
            }
            set 
            { 
                m_size = value;
                if (presetData != null)
                    presetData.size = value;
            }
        }

        private void OnValidate()
        {
            // 当在Inspector中修改时同步数据
            if (presetData != null && presetData.size != m_size)
            {
                presetData.size = m_size;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(presetData);
#endif
            }
        }
        
        private void OnDrawGizmos()
        {
            if (presetData == null || presetData.size == Vector3Int.zero)
                return;
            
            DrawCubeRange(Color.green, new Color(0.5f, 1f, 0.5f, 0.5f), presetData.size);
        }

        private void OnDrawGizmosSelected()
        {
            if (presetData == null || presetData.size == Vector3Int.zero)
                return;
            
            DrawCubeRange(Color.yellow, new Color(1, 1, 0, 0.5f), presetData.size);
            
        }
        
        private void DrawCubeRange(Color borderColor, Color fillColor, Vector3Int size)
        {
            UnityEngine.Gizmos.color = fillColor;
            Vector3 gizmosSize = new Vector3(size.x, size.y, size.z);
            Vector3 center = transform.position + new Vector3(-0.5f,-0.5f,-0.5f) + new Vector3(gizmosSize.x * 0.5f, gizmosSize.y * 0.5f, gizmosSize.z * 0.5f);
    
            UnityEngine.Gizmos.DrawCube(center, gizmosSize);
    
            UnityEngine.Gizmos.color = borderColor;
            UnityEngine.Gizmos.DrawWireCube(center, gizmosSize);
        }
        
    }
}