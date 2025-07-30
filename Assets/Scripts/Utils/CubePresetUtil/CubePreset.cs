using AILand.GamePlay.World;
using UnityEngine;

namespace AILand.Utils
{
    public class CubePreset : MonoBehaviour
    {
        public CubePresetSO presetData;

        [SerializeField][HideInInspector] private Vector3Int m_minPoint; // 左下角点
        [SerializeField][HideInInspector] private Vector3Int m_maxPoint; // 右上角点

        public Vector3Int minPoint
        {
            get
            {
                if (presetData != null)
                    return presetData.minPoint;
                return m_minPoint;
            }
            set
            {
                m_minPoint = value;
                if (presetData != null)
                    presetData.minPoint = value;
            }
        }

        public Vector3Int maxPoint
        {
            get
            {
                if (presetData != null)
                    return presetData.maxPoint;
                return m_maxPoint;
            }
            set
            {
                m_maxPoint = value;
                if (presetData != null)
                    presetData.maxPoint = value;
            }
        }

        
        private void OnValidate()
        {
            // 当在Inspector中修改时同步数据
            if (presetData != null)
            {
                if (presetData.minPoint != m_minPoint)
                {
                    presetData.minPoint = m_minPoint;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(presetData);
#endif
                }
                if (presetData.maxPoint != m_maxPoint)
                {
                    presetData.maxPoint = m_maxPoint;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(presetData);
#endif
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (presetData == null)
                return;

            DrawCubeRange(Color.green, new Color(0.5f, 1f, 0.5f, 0.5f));
        }

        private void OnDrawGizmosSelected()
        {
            if (presetData == null)
                return;

            DrawCubeRange(Color.yellow, new Color(1, 1, 0, 0.5f));
        }

        private void DrawCubeRange(Color borderColor, Color fillColor)
        {
            UnityEngine.Gizmos.color = fillColor;
    
            // 计算实际大小
            Vector3 actualSize = new Vector3(
                maxPoint.x - minPoint.x + 1,
                maxPoint.y - minPoint.y + 1,
                maxPoint.z - minPoint.z + 1
            );
    
            // 中心点
            Vector3 center = transform.position + new Vector3(
                minPoint.x + actualSize.x * 0.5f,
                minPoint.y + actualSize.y * 0.5f,
                minPoint.z + actualSize.z * 0.5f
            );
            
            // 偏移0.5f以确保Gizmos在正确位置
            center += new Vector3(-0.5f, -0.5f, -0.5f);
            
            UnityEngine.Gizmos.DrawCube(center, actualSize);

            UnityEngine.Gizmos.color = borderColor;
            UnityEngine.Gizmos.DrawWireCube(center, actualSize);
        }
    }
}