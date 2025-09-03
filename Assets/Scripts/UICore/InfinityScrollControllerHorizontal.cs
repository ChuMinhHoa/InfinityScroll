using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UICore
{
    [System.Serializable]
    public class InfinityScrollControllerHorizontal<TSlot, TData> : InfinityScroll<TSlot> where TSlot : SlotBase<TData> 
    {
        
        
        public override void InitContent()
        {
            InitContentHorizontal();
            base.InitContent();
        }
        
        private void InitContentHorizontal()
        {
           var contentWidth = Slots[0].myRectTransform.rect.width * DataCount +
                               Padding.spacing.rValue.Value.x * (DataCount - 1) + Padding.left.rValue.Value + Padding.right.rValue.Value;
           ScrollRect.content.sizeDelta = new Vector2(contentWidth, ScrollRect.content.sizeDelta.y);
        }

        //rồi nói thật đoạn này chả khác kì vertial nên là qua copy thôi

        [Button]
        public override void InitSlotRectSize()
        {
            var left = Padding.left.rValue.Value;
            
            for (var i = 0; i < Slots.Count; i++)
            {
                var dataIndex = Slots[i].dataIndex;
                var pos =Slots[0].myRectTransform.anchoredPosition;
                pos.x = left + dataIndex * (Slots[i].myRectTransform.rect.width + Padding.spacing.rValue.Value.x) + Slots[i].myRectTransform.rect.width/2;
                pos.y = Mathf.Abs(Padding.top.rValue.Value - Padding.bottom.rValue.Value) / 2f * (Padding.top.rValue.Value < Padding.bottom.rValue.Value ? -1 : 1);
                Slots[i].myRectTransform.anchoredPosition = pos;
                
                var size = ScrollRect.content.rect.size;
                if (Padding.controlHeight)
                    size.y -= Padding.top.rValue.Value + Padding.bottom.rValue.Value;
                else
                    size.y = Slots[i].myRectTransform.rect.height;
                size.x = Slots[i].myRectTransform.rect.width;
                Slots[i].myRectTransform.sizeDelta = size;
            }
        }
        
        public override void GetWorldCornersViewport(ScrollMainContentType mainContentType)
        {
            base.GetWorldCornersViewport(mainContentType);
            ViewportWorldCorners[0].x -= Slots[0].myRectTransform.rect.width + Padding.spacing.rValue.Value.x;
            ViewportWorldCorners[3].x += Slots[0].myRectTransform.rect.width + Padding.spacing.rValue.Value.x;
        }
        
        public override void OnScrollChanged(Vector2 scrollPosition)
        {
            if (ScrollRect != null && ScrollRect.velocity == Vector2.zero)
            {
                return;
            }

            // Your scroll logic here, e.g. check slot visibility and recycle
            float currentScrollPosition = ScrollRect.horizontalNormalizedPosition;
            if (currentScrollPosition < LastScrollPosition)
            {
                // Scrolling left
                if (!IsVisibleLeft(Slots[0].myRectTransform))
                {
                    Switch(true);
                }
            }
            else if (currentScrollPosition > LastScrollPosition)
            {
                // Scrolling right
                if (!IsVisibleRight(Slots[^1].myRectTransform))
                {
                    Switch(false);
                }
            }

            LastScrollPosition = currentScrollPosition;
        }
        
        private void Switch(bool isLeft)
        {
            SwitchAction?.Invoke(isLeft, () => SwitchSuccess(isLeft));
        }
        
        private bool IsVisibleRight(RectTransform slotRect)
        {
            slotRect.GetWorldCorners(SlotWorldCorners);
            return SlotWorldCorners[0].x > ViewportWorldCorners[3].x; 
        }

        private bool IsVisibleLeft(RectTransform slotRect)
        {
            slotRect.GetWorldCorners(SlotWorldCorners);
            return SlotWorldCorners[0].x < ViewportWorldCorners[0].x; 
        }

        public override void SwitchSuccess(bool isLeft)
        {
            var slot = isLeft ? Slots[^1] : Slots[0];
            var newPos = isLeft ? Slots[0].myRectTransform.anchoredPosition : Slots[^1].myRectTransform.anchoredPosition;
            newPos.x += isLeft
                ? -(slot.myRectTransform.rect.width + Padding.spacing.rValue.Value.x)
                : slot.myRectTransform.rect.width + Padding.spacing.rValue.Value.x;
            slot.myRectTransform.anchoredPosition = newPos;
            if (isLeft)
            {
                slot.transform.SetAsFirstSibling();
                for (var i = Slots.Count - 1; i > 0; i--)
                    Slots[i] = Slots[i - 1];
                Slots[0] = slot;
            }
            else
            {
                slot.transform.SetAsLastSibling();
                for (var i = 0; i < Slots.Count - 1; i++)
                    Slots[i] = Slots[i + 1];
                Slots[^1] = slot;
            }
        }
    }
}
