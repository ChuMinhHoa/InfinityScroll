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
        
#if UNITY_EDITOR       
        public int column;
#endif
        public int groupDataIndex;
        public RectTransform myRectTransform;
        private TData[] _data;
        public List<TSlot> groupSlot = new();
        public Padding padding;
        
        public virtual void InitData(Span<TData> dataChange, int dataIndexStart, int groupIndex)
        {
            _data = dataChange.ToArray();
            groupDataIndex = groupIndex;
            for (var i = 0; i < groupSlot.Count; i++)
            {
                groupSlot[i].InitData(_data[i], dataIndexStart + i);
            }
        }
        
        public virtual void UpdateData(TData dataChange, int dataIndexChange)
        {
            for (var i = 0; i < groupSlot.Count; i++)
            {
                if (dataIndexChange == groupSlot[i].dataIndex)
                {
                    groupSlot[i].InitData(_data[i], i);
                }
            }
        }
        [Button]
        public void ReSizeSlot()
        {
            for (var i = 0; i < groupSlot.Count; i++)
            {
                SetRectSize(groupSlot[i].myRectTransform, i);
            }
        }

        #if UNITY_EDITOR
        

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
