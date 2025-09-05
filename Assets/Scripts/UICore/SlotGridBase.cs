using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace UICore
{
    [System.Serializable]
    public class SlotGridBase<TData, TSlot> : MonoBehaviour  where TSlot : SlotBase<TData>
    {      
        public int column;
        public int groupDataIndex;
        public RectTransform myRectTransform;
        private TData[] _data;
        public List<TSlot> slots = new();
        public TSlot slotPref;
        public Padding padding;
        
        public virtual void InitData(Span<TData> dataChange, int dataIndexStart, int groupIndex)
        {
            _data = dataChange.ToArray();
            groupDataIndex = groupIndex;
            for (var i = 0; i < slots.Count; i++)
            {
                if (i >= dataChange.Length)
                {
                    slots[i].gameObject.SetActive(false);
                    continue;
                }
                slots[i].InitData(_data[i], dataIndexStart + i);
                if (!slots[i].gameObject.activeSelf)
                    slots[i].gameObject.SetActive(true);
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
        
        [Button]
        public void ReSizeSlot()
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
            var pos = rect.anchoredPosition;
            pos.x = index * rect.rect.width + padding.left.rValue.Value +
                    rect.rect.width / 2f + padding.spacing.rValue.Value.x * index;
            rect.anchoredPosition = pos;
        }
        
        [Button]
        public void SetColumn(int col)
        {
            column = col;
            for (var i = 0; i < col; i++)
            {
                var slot = Instantiate(slotPref, myRectTransform);
                slots.Add(slot);
            }
        }
    }
}
