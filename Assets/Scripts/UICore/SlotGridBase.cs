using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UICore
{
    [System.Serializable]
    public class SlotGridBase<TData, TSlot> : MonoBehaviour  where TSlot : SlotBase<TData>
    {
        
#if UNITY_EDITOR       
        public int column;
#endif
        public RectTransform myRectTransform;
        private TData[] _data;
        public List<TSlot> slots = new();
        public Padding padding;
        
        public virtual void InitData(Span<TData> dataChange)
        {
            _data = dataChange.ToArray();
            for (var i = 0; i < slots.Count; i++)
            {
                slots[i].InitData(_data[i], i);
            }
        }
        
        public virtual void UpdateData(TData dataChange, int dataIndexChange)
        {
            for (var i = 0; i < slots.Count; i++)
            {
                if (dataIndexChange == slots[i].dataIndex)
                {
                    slots[i].InitData(_data[i], i);
                }
            }
        }

        #if UNITY_EDITOR
        [Button]
        private void ReSizeSlot()
        {
            for (var i = 0; i < slots.Count; i++)
            {
                SetRectSize(slots[i].myRectTransform, i);
            }
        }

        private void SetRectSize(RectTransform rect, int index)
        {
            var width = (myRectTransform.rect.width - padding.spacing.rValue.Value.x * (column - 1) -
                        padding.left.rValue.Value - padding.right.rValue.Value) / column;
            rect.sizeDelta = new Vector2(width, rect.rect.height);
            Debug.Log("Size: " + rect.sizeDelta);
            var pos = rect.anchoredPosition;
            pos.x = index * rect.rect.width + padding.left.rValue.Value +
                    rect.rect.width / 2f + padding.spacing.rValue.Value.x * index;
            rect.anchoredPosition = pos;
            Debug.Log("pos x: " + pos.x);
            Debug.Log("================");
        }

        public void SetColumn(int col)
        {
            column = col;
        }
        #endif
    }
}
