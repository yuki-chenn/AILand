using System;
using UnityEngine;

namespace AILand.GamePlay.World
{


    public struct NormalElement
    {
        public int[] fiveElement;
        public NormalElement(int defaultValue)
        {
            fiveElement = new int[5];
            for (int i = 0; i < 5; i++) fiveElement[i] = defaultValue;
        }
        public NormalElement(int[] elements)
        {
            if (elements.Length != 5)
            {
                Debug.LogWarning($"elements length must be 5, but got {elements.Length}. Fill or cut array");
            }

            fiveElement = new int[5];
            for (int i = 0; i < elements.Length; i++) fiveElement[i] = elements[i];
        }
        public NormalElement(int metal, int wood, int water, int fire, int earth)
        {
            fiveElement = new[] { metal, wood, water, fire, earth };
        }
        #region 访问方式
        public int this[int index]
        {
            get => fiveElement[index];
            set => fiveElement[index] = value;
        }
        public int Metal
        {
            get => fiveElement[0];
            set => fiveElement[0] = value;
        }

        public int Wood
        {
            get => fiveElement[1];
            set => fiveElement[1] = value;
        }

        public int Water
        {
            get => fiveElement[2];
            set => fiveElement[2] = value;
        }

        public int Fire
        {
            get => fiveElement[3];
            set => fiveElement[3] = value;
        }

        public int Earth
        {
            get => fiveElement[4];
            set => fiveElement[4] = value;
        }
        public int Sum
        {
            get => fiveElement[0] + fiveElement[1] + fiveElement[2] + fiveElement[3] + fiveElement[4];
        }
        #endregion
    }

    public class ElementalEnergy
    {
        // 基础元素：金木水火土
        public NormalElement NormalElement { get; set; } = new(0);
        
        public void AddEnergy(EnergyType energyType, int amount)
        {
            var ne = NormalElement;
            switch (energyType)
            {
                case EnergyType.Metal:
                    ne.Metal += amount;
                    break;
                case EnergyType.Wood:
                    ne.Wood += amount;
                    break;
                case EnergyType.Water:
                    ne.Water += amount;
                    break;
                case EnergyType.Fire:
                    ne.Fire += amount;
                    break;
                case EnergyType.Earth:
                    ne.Earth += amount;
                    break;
            }
        }
        
        public bool ReduceEnergy(EnergyType energyType, int amount)
        {
            var ne = NormalElement;
            switch (energyType)
            {
                case EnergyType.Metal:
                    if (ne.Metal < amount) return false;
                    ne.Metal -= amount;
                    break;
                case EnergyType.Wood:
                    if (ne.Wood < amount) return false;
                    ne.Wood -= amount;
                    break;
                case EnergyType.Water:
                    if (ne.Water < amount) return false;
                    ne.Water -= amount;
                    break;
                case EnergyType.Fire:
                    if (ne.Fire < amount) return false;
                    ne.Fire -= amount;
                    break;
                case EnergyType.Earth:
                    if (ne.Earth < amount) return false;
                    ne.Earth -= amount;
                    break;
            }
            return true;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc">true</param>
        /// <returns></returns>
        public int[] GetSortedIndex(bool desc=true)
        {
            // 将元素排序，并将原先的索引返回
            int[] sortedIndex = new int[5];
            for (int i = 0; i < 5; i++)
            {
                sortedIndex[i] = i;
            }
            if (desc)
            {
                Array.Sort(sortedIndex, (a, b) => NormalElement[b].CompareTo(NormalElement[a]));
            }
            else
            {
                Array.Sort(sortedIndex, (a, b) => NormalElement[a].CompareTo(NormalElement[b]));
            }
            return sortedIndex;
        }
        
    }
}
