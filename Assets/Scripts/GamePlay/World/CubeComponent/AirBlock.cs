using UnityEngine;

namespace AILand.GamePlay.World.Cube
{
    public class AirBlock : BaseCube
    {
        public override CubeType CubeType => CubeType.AirBlock;

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
            UnityEngine.Gizmos.color = new Color(1,0.92f,0.016f,0.3f);
            UnityEngine.Gizmos.DrawCube(transform.position, Vector3.one);
            UnityEngine.Gizmos.color = Color.yellow;
            UnityEngine.Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
#endif
        
    }
}