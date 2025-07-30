using UnityEngine;

namespace AILand.GamePlay.World.Cube
{
    public class Air : BaseCube
    {
        public override CubeType CubeType => CubeType.Air;

        public override void OnFocus()
        {
            
        }

        public override void OnLostFocus()
        {
            
        }
        
        public override void Interact()
        {
          
        }
        
#if UNITY_EDITOR
        // 方便在Scene中查看
        private void OnDrawGizmos()
        {
            UnityEngine.Gizmos.color = new Color(0,1,1,0.3f);
            UnityEngine.Gizmos.DrawCube(transform.position, Vector3.one);
            UnityEngine.Gizmos.color = Color.cyan;
            UnityEngine.Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
#endif
        
    }
}