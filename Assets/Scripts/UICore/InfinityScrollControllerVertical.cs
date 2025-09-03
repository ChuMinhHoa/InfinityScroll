using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UICore
{
    [Serializable]
    public class InfinityScrollControllerVertical<TSlot, TData> : InfinityScroll<TSlot> where TSlot : SlotBase<TData> 
    {

        // vậy nếu như đã nêu ở dưới thì change padding thì không nhất thiết phải cập nhật lại vị trí của slot?
        // nó chỉ đúng nếu đó là padding top hoặc bottom thôi 
        // padding left hoặc right thì vẫn phải cập nhật lại vị trí của slot vì nó ảnh hưởng đến vị trí x của slot

        public override void InitContent()
        {
            InitContentVertical();
            base.InitContent();
        }

        private void InitContentVertical()
        {
            var contentHeight = Slots[0].myRectTransform.rect.height * DataCount +
                                Padding.spacing.rValue.Value.y * (DataCount - 1) + Padding.top.rValue.Value + Padding.bottom.rValue.Value;
            ScrollRect.content.sizeDelta = new Vector2(ScrollRect.content.sizeDelta.x, contentHeight);
        }
        
        // nếu padding top hoặc bottom thay đổi và slot có vị trí khác vị trí ban đầu của nó thì không cần thiết phải cập nhật lại vị trí của 
        // slot vì nó vẫn đúng vị trí so với content
        // Size của content được cập nhật lại
        public override void InitSlotRectSize()
        {
            var top = Padding.top.rValue.Value;
            
            for (var i = 0; i < Slots.Count; i++)
            {
                var dataIndex = Slots[i].dataIndex;
                var pos =Slots[0].myRectTransform.anchoredPosition;
                pos.y = -1 * (top + dataIndex * (Slots[i].myRectTransform.rect.height + Padding.spacing.rValue.Value.y) +
                              Slots[i].myRectTransform.rect.height / 2);
                pos.x = Mathf.Abs(Padding.left.rValue.Value - Padding.right.rValue.Value) / 2f * (Padding.left.rValue.Value < Padding.right.rValue.Value ? -1 : 1);
                Slots[i].myRectTransform.anchoredPosition = pos;
                
                var size = ScrollRect.content.rect.size;
                if (Padding.controlWidth)
                    size.x -= Padding.left.rValue.Value + Padding.right.rValue.Value;
                else
                    size.x = Slots[i].myRectTransform.rect.width;
                size.y = Slots[i].myRectTransform.rect.height;
                Slots[i].myRectTransform.sizeDelta = size;
            }
        }

        public override void GetWorldCornersViewport(ScrollMainContentType mainContentType)
        {
            base.GetWorldCornersViewport(mainContentType);
            ViewportWorldCorners[0].y -= (Slots[0].myRectTransform.rect.height + Padding.spacing.rValue.Value.y);
            ViewportWorldCorners[1].y += (Slots[0].myRectTransform.rect.height + Padding.spacing.rValue.Value.y);
        }
        
        public override void OnScrollChanged(Vector2 scrollPosition)
        {
            if (ScrollRect != null && ScrollRect.velocity == Vector2.zero)
            {
                return;
            }

            // Your scroll logic here, e.g. check slot visibility and recycle
            float currentScrollPosition = ScrollRect.verticalNormalizedPosition;
            if (currentScrollPosition < LastScrollPosition)
            {
                // Scrolling down
                if (!IsVisibleDown(Slots[0].myRectTransform))
                {
                    Switch(false);
                }
            }
            else if (currentScrollPosition > LastScrollPosition)
            {
                // Scrolling up
                if (!IsVisibleUp(Slots[^1].myRectTransform))
                {
                    Switch(true);
                }
            }

            LastScrollPosition = currentScrollPosition;
        }
        
        private void Switch(bool isUp)
        {
            SwitchAction?.Invoke(isUp, () => SwitchSuccess(isUp));
        }
        
        private bool IsVisibleUp(RectTransform slotRect)
        {
            slotRect.GetWorldCorners(SlotWorldCorners);
            return SlotWorldCorners[0].y > ViewportWorldCorners[0].y; // top > bottom
        }

        private bool IsVisibleDown(RectTransform slotRect)
        {
            slotRect.GetWorldCorners(SlotWorldCorners);
            return SlotWorldCorners[0].y < ViewportWorldCorners[1].y; // top > bottom
        }

        public override void SwitchSuccess(bool isUp)
        {
            var slot = isUp ? Slots[^1] : Slots[0];
            var newPos = isUp ? Slots[0].myRectTransform.anchoredPosition : Slots[^1].myRectTransform.anchoredPosition;
            newPos.y += isUp
                ? slot.myRectTransform.rect.height + Padding.spacing.rValue.Value.y
                : -(slot.myRectTransform.rect.height + Padding.spacing.rValue.Value.y);
            slot.myRectTransform.anchoredPosition = newPos;
            if (isUp)
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
