
using System.Collections.Generic;
using AILand.Utils;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.World
{
    public class CellData
    {
        
        private BlockData m_blockData;
        public BlockData BlockData => m_blockData;

        // Block内的坐标
        private Vector2Int m_index;
        public Vector2Int Index => m_index;
        
        // Block内的位置
        private Vector3 m_localPosition;
        public Vector3 LocalPosition => m_localPosition;

        // 描述该Cell的边界情况
        private CellWater m_cellWater;
        public CellWater CellWater
        {
            get => m_cellWater;
            set => m_cellWater = value;
        }
        
        // 所有的方块数据
        private List<CubeData> m_cubes;
        public List<CubeData> Cubes => m_cubes;
        
        public CubeData TopCube => m_cubes.Count > 0 ? m_cubes[m_cubes.Count - 1] : null;

        public int Height => m_cubes.Count;

        public CellData(BlockData blockData, Vector2Int index, Vector3 localPosition)
        {
            m_blockData = blockData;
            m_index = index;
            m_localPosition = localPosition;
            m_cellWater = CellWater.None;
            m_cubes = new List<CubeData>();
        }
        
        public CubeData GetCubeData(int yHeight)
        {
            if(yHeight < 0 || yHeight >= m_cubes.Count)
            {
                return null;
            }
            return m_cubes[yHeight];
        }
        
        public void Load()
        {
            foreach (var cube in m_cubes)
            {
                
                if(IsInVisible(cube))
                {
                    cube.Unload();
                }else
                {
                    cube.Load();
                }
            }
        }

        public void Unload()
        {
            foreach (var cube in m_cubes)
            {
                cube.Unload();
            }
        }
        
        private bool IsInVisible(CubeData cube)
        {
            int y = cube.YHeight;

            var upCube = GetCubeData(y + 1);
            bool up = y + 1 < m_cubes.Count && upCube != null && upCube.CubeType != CubeType.None;
            
            var downCube = GetCubeData(y - 1);
            bool down = y == 0 || GetCubeData(y - 1) != null && downCube.CubeType != CubeType.None;
            
            var leftCube = m_blockData.GetCellData(m_index.x - 1, m_index.y)?.GetCubeData(y);
            bool left = leftCube != null && leftCube.CubeType != CubeType.None;
            
            var rightCube = m_blockData.GetCellData(m_index.x + 1, m_index.y)?.GetCubeData(y);
            bool right = rightCube != null && rightCube.CubeType != CubeType.None;
            
            var frontCube = m_blockData.GetCellData(m_index.x, m_index.y - 1)?.GetCubeData(y);
            bool front = frontCube != null && frontCube.CubeType != CubeType.None;
            
            var backCube = m_blockData.GetCellData(m_index.x, m_index.y + 1)?.GetCubeData(y);
            bool back = backCube != null && backCube.CubeType != CubeType.None;
            
            return up && down && left && right && front && back;
        }

        public void DestoryCube(int y, bool needLoad = true)
        {
            if (y < 0 || y >= m_cubes.Count)
            {
                Debug.LogError(
                    $"DestoryCube error : Cube at height {y} does not exist in cell {m_index} of block {m_blockData.BlockID}.");
                return;
            }
            
            m_cubes[y].Change(CubeType.None, 0, needLoad);
            if(needLoad) Load();
        }

        public void AddCube(int y, CubeType cubeType, int rotation, bool needLoad = true)
        {
            if (y < 0 || (y < m_cubes.Count && m_cubes[y].CubeType != CubeType.None))
            {
                Debug.LogError(
                    $"AddCube error : Cannot add cube at height {y} in cell {m_index} of block {m_blockData.BlockID}.");
                return;
            }

            if (y < m_cubes.Count)
            {
                if(m_cubes[y].CubeType != CubeType.None)
                {
                    Debug.LogError(
                        $"AddCube error : Cannot change cube at height {y} in cell {m_index} of block {m_blockData.BlockID}.");
                    return;
                }
                else
                {
                    m_cubes[y].Change(cubeType, rotation, needLoad);
                }
            }
            else
            {
                while(m_cubes.Count < y)
                {
                    m_cubes.Add(new CubeData(this, CubeType.None, 0, m_cubes.Count));
                }
                var newCubeData = new CubeData(this, cubeType,rotation, y);
                m_cubes.Add(newCubeData);
            }

            if(needLoad) Load();
        }

    }
}
