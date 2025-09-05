using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UICore
{
    [System.Serializable]
    public class InfinityScrollControllerGrid<TGridSlot, TData, TSlot>  : InfinityScroll<TGridSlot> where TGridSlot : SlotGridBase<TData, TSlot> where TSlot : SlotBase<TData>
    {
        public int column;

        public override void InitContent()
        {
            InitContentVertical();
            base.InitContent();
        }

        private void InitContentVertical()
        {
            var rowCount = (DataCount / column);
            if (DataCount % column != 0)
                rowCount += 1;
            var contentHeight = Slots[0].myRectTransform.rect.height * rowCount +
                                Padding.spacing.rValue.Value.y * (rowCount - 1) + Padding.top.rValue.Value + Padding.bottom.rValue.Value;
            ScrollRect.content.sizeDelta = new Vector2(ScrollRect.content.sizeDelta.x, contentHeight);
        }
        
        public override void InitSlotRectSize()
        {
            var top = Padding.top.rValue.Value;
            
            for (var i = 0; i < Slots.Count; i++)
            {
                var dataIndex = Slots[i].groupDataIndex;
                var pos = Slots[0].myRectTransform.anchoredPosition;
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
            //deo hieu sao khong can dong duoi luon :) dang nhe ra no phai co
            //ViewportWorldCorners[1].y += (Slots[0].myRectTransform.rect.height + Padding.spacing.rValue.Value.y);
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
            return SlotWorldCorners[0].y > ViewportWorldCorners[0].y;
        }

        private bool IsVisibleDown(RectTransform slotRect)
        {
            slotRect.GetWorldCorners(SlotWorldCorners);
            return SlotWorldCorners[0].y < ViewportWorldCorners[1].y;
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
