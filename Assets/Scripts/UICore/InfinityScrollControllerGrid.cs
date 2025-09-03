using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UICore
{
    [System.Serializable]
    public class InfinityScrollControllerGrid<TSlot, TData>  : InfinityScroll<TSlot> where TSlot : SlotBase<TData> 
    {
        // địt cụ cái này còn cần vài biến phụ nữa
        // số cột , số hàng của slot nữa
        public bool flexSizeWidth;
        [ShowIf("flexSizeWidth", false)]
        public float slotSize;
        public int column;
        
        public override void InitContent()
        {
            CalculateSize();
            InitGridContentSize();
            base.InitContent();
        }

        private void CalculateSize()
        {
            if (flexSizeWidth)
            {
                CalculateSlotSize();
            }
        }
        // mainrect width = slotSize * column + padding left + padding right + spacing * (column -1)
        private void CalculateSlotSize()
        {
            slotSize = (ScrollRect.viewport.rect.width - Padding.left.rValue.Value - Padding.right.rValue.Value -
                        Padding.spacing.rValue.Value.x * (column - 1)) / column;
        }

        // được rồi vấn đề của cái grid này là phải có max width. Nó là cái kích thước của màn hình
        // giowf làm nào?
        
        private void InitGridContentSize()
        {
            
        }

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
           
        }

        public override void SwitchSuccess(bool isUp)
        {
           
        }
    }
}
