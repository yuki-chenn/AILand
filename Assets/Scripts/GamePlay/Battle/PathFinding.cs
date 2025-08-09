using System.Collections.Generic;
using AILand.GamePlay.World;
using UnityEngine;

namespace AILand.GamePlay.Battle
{
    public class PathFinding
    {
        private Vector3[] directions = new Vector3[8]
        {
            Vector3.right,    // (1,0,0)
            Vector3.left,     // (-1,0,0)
            Vector3.forward,  // (0,0,1)
            Vector3.back,      // (0,0,-1)
            Vector3.right + Vector3.forward,   // (1,0,1)
            Vector3.right + Vector3.back,      // (1,0,-1)
            Vector3.left + Vector3.forward,    // (-1,0,1)
            Vector3.left + Vector3.back         // (-1,0,-1)
        };

        private List<Vector3> sortedDirections = new List<Vector3>();

        public struct MovementResult
        {
            public Vector3 direction;
            public bool shouldJump;
        }

        /// <summary>
        /// 获取移动方向和跳跃判断
        /// </summary>
        /// <param name="currentPos">当前位置</param>
        /// <param name="targetPos">目标位置</param>
        /// <param name="checkHeight">是否检查高度差来判断跳跃</param>
        /// <returns>移动结果（方向和是否跳跃）</returns>
        public MovementResult GetMovement(Vector3 currentPos, Vector3 targetPos, bool checkHeight = true)
        {
            MovementResult result = new MovementResult();
            
            // 计算到目标的方向（忽略Y轴）
            Vector3 targetDirection = targetPos - currentPos;
            targetDirection.y = 0f;
            
            if (targetDirection.magnitude < 0.1f)
            {
                result.direction = Vector3.zero;
                result.shouldJump = false;
                return result;
            }
            
            // 找到最佳移动方向
            int dy = 0;
            (result.direction,dy) = FindBestDirection(currentPos, targetDirection.normalized);
            
            // 判断是否需要跳跃
            result.shouldJump = dy > 0;
            return result;
        }

        private (Vector3 dir, int dy) FindBestDirection(Vector3 currentPos, Vector3 targetDirection)
        {
            // 根据与目标方向的角度排序
            sortedDirections.Clear();
            sortedDirections.AddRange(directions);
            sortedDirections.Sort((a, b) => 
                Vector3.Angle(a, targetDirection).CompareTo(Vector3.Angle(b, targetDirection)));

            // 返回第一个可通行的方向
            foreach (Vector3 dir in sortedDirections)
            {
                for (int dy = 0; dy < 3; ++dy)
                {
                    if (IsPathClear(currentPos, dir + new Vector3(0,dy,0)))
                    {
                        return (dir,dy);
                    }
                }
            }

            return (Vector3.zero,0); // 没有可通行路径
        }

        private bool IsPathClear(Vector3 startPos, Vector3 direction)
        {
            Vector3 checkPos = startPos + direction;
            return !WorldManager.Instance.HasCollideCube(checkPos);
        }

        // private bool ShouldJump(Vector3 currentPos, Vector3 nextPos)
        // {
        //     
        //     
        //     return false; // 临时返回false，需要实现具体的高度检测
        // }
    }
}