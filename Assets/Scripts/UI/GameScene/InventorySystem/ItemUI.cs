using System.Collections.Generic;
using AILand.GamePlay;
using AILand.GamePlay.InventorySystem;
using AILand.System.EventSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.UI
{
    public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas canvas;
        private GraphicRaycaster graphicRaycaster;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        
        private Image m_imgItemIcon;
        private Text m_txtItemCount;
        
        private Transform originalParent;
        private Vector3 originalPosition;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            canvas = GetComponentInParent<Canvas>();
            graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
            
            m_imgItemIcon = transform.Find("ImgItemIcon").GetComponent<Image>();
            m_txtItemCount = transform.Find("TxtItemCount").GetComponent<Text>();
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 记录原始状态
            originalParent = transform.parent;
            originalPosition = rectTransform.anchoredPosition;

            // 设置为画布的直接子物体，确保在最上层显示
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();

            // 设置透明度和射线检测
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 跟随鼠标移动
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 恢复设置
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            // 射线检测找到目标槽位
            var targetSlot = GetTargetSlot(eventData);
            
            if (targetSlot != null)
            {
                // 交换物品位置
                SwapItems(targetSlot);
            }
            else
            {
                // 没有找到有效目标，返回原位置
                ReturnToOriginalPosition();
            }
        }

        private ItemSlotUI GetTargetSlot(PointerEventData eventData)
        {
            var raycastResults = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, raycastResults);

            foreach (var result in raycastResults)
            {
                var slot = result.gameObject.GetComponent<ItemSlotUI>();
                if (slot != null && slot.transform != originalParent)
                {
                    return slot;
                }
            }

            return null;
        }

        private void SwapItems(ItemSlotUI targetSlotUI)
        {
            var sourceSlot = originalParent.GetComponent<ItemSlotUI>();
            var targetItem = targetSlotUI.GetCurrentItem();

            if (targetItem != null)
            {
                // 目标槽位有物品，执行交换
                targetItem.transform.SetParent(sourceSlot.transform);
                targetItem.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                sourceSlot.SetItem(targetItem);
            }
            else
            {
                // 目标槽位为空，清空源槽位
                sourceSlot.SetItem(null);
            }

            // 将当前物品移动到目标槽位
            transform.SetParent(targetSlotUI.transform);
            rectTransform.anchoredPosition = Vector3.zero;
            targetSlotUI.SetItem(this);
            
            // 更新保存的数据
            var to = targetSlotUI.transform.GetSiblingIndex();
            var from = sourceSlot.transform.GetSiblingIndex();
            EventCenter.Broadcast(EventType.SwitchItemInInventoryData, from, to);
        }

        private void ReturnToOriginalPosition()
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }


        public void UpdateItemInfo(Sprite itemIcon, int itemCount)
        {

            gameObject.SetActive(itemIcon != null && itemCount > 0);
            
            if (m_txtItemCount)
            {
                m_txtItemCount.text = itemCount.ToString();
            }

            if (m_imgItemIcon)
            {
                m_imgItemIcon.sprite = itemIcon;
            }
        }
    }
}