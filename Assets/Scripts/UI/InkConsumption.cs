using System;
using UnityEngine;
using System.Collections.Generic;

namespace AILand.UI
{
    [Serializable]
    public class InkData
    {
        public Color color;
        public float totalAmount;
        public float usedAmount;
    
        public float RemainingAmount => totalAmount - usedAmount;
        public float RemainingRatio => totalAmount > 0 ? RemainingAmount / totalAmount : 0f;
    
        public InkData(Color color, float totalAmount)
        {
            this.color = color;
            this.totalAmount = totalAmount;
            this.usedAmount = 0f;
        }
    
        public bool CanConsume(float amount)
        {
            return usedAmount + amount <= totalAmount;
        }
    
        public bool TryConsume(float amount)
        {
            if (CanConsume(amount))
            {
                usedAmount += amount;
                return true;
            }
            return false;
        }
    
        public void Reset()
        {
            usedAmount = 0f;
        }
    }
    
    [Serializable]
    public class InkConsumptionData
    {
        public List<InkData> inkList = new List<InkData>();
    
        public InkData GetInkByColor(Color color)
        {
            return inkList.Find(ink => ColorEquals(ink.color, color));
        }
    
        public void AddInk(Color color, float amount)
        {
            var existing = GetInkByColor(color);
            if (existing != null)
            {
                existing.totalAmount += amount;
            }
            else
            {
                inkList.Add(new InkData(color, amount));
            }
        }
    
        public void ResetAll()
        {
            foreach (var ink in inkList)
            {
                ink.Reset();
            }
        }
    
        private bool ColorEquals(Color a, Color b, float threshold = 0.01f)
        {
            return Mathf.Abs(a.r - b.r) < threshold &&
                   Mathf.Abs(a.g - b.g) < threshold &&
                   Mathf.Abs(a.b - b.b) < threshold &&
                   Mathf.Abs(a.a - b.a) < threshold;
        }
    }
}