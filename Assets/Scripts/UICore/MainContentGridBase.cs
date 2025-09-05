using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UICore
{
    [System.Serializable]
    public class MainContentGridBase<TSlotGrid, TData, TSlot> where TSlotGrid : SlotGridBase<TData, TSlot> where TSlot : SlotBase<TData>
    { 
        public List<TSlotGrid> groupSlot = new();
        public TSlotGrid groupPref;
        public Transform parents;
        public int totalDataCount;
        private TData[] _dataArray;
        [BoxGroup("Infinity Scroll Controller")]
        [InfoBox("Đặt anchor preset vào center cho slot", InfoMessageType.Warning)]
        [HideLabel]
        public InfinityScrollControllerGrid<TSlotGrid, TData, TSlot> infiniteScrollGridController;
        
        public void Start()
        {
            SpawnFirstSlot();
        }
        
        private void SpawnFirstSlot()
        {
            var sizeHeight = infiniteScrollGridController.ScrollRect.viewport.rect.height;
            var slotHeight = groupPref.myRectTransform.rect.height;
            var totalGroup = Mathf.Ceil(sizeHeight / slotHeight);
            //var totalGroup = (int)(sizeHeight / slotHeight);
            if (sizeHeight % slotHeight != 0)
                totalGroup += 1;

            for (var i = 0; i < totalGroup; i++)
            {
                var group = Object.Instantiate(groupPref, parents);
                groupSlot.Add(group);
            }
            
        }

        public virtual void InitData(Span<TData> data)
        {
            SetDataToGridSlot(data);
            infiniteScrollGridController.SetActionSwitch(SwitchGridSlot);
            infiniteScrollGridController.InitData(groupSlot, ScrollMainContentType.InfiniteGrid, data.Length);
            for (var i = 0; i < groupSlot.Count; i++)
            {
                groupSlot[i].ReSizeSlot();
            }
        }
        
        private void SetDataToGridSlot(Span<TData> data)
        {
            if(data.Length==0) return;
            totalDataCount = data.Length;
            _dataArray = data.ToArray();
            var dataIndex = 0;
            for (var i = 0; i < groupSlot.Count && i < data.Length; i++)
            {
                groupSlot[i].SetColumn(infiniteScrollGridController.column);
                var end = Mathf.Min(dataIndex + infiniteScrollGridController.column, data.Length);
                
                var dataSpan = data.Slice(dataIndex, end - dataIndex);
                groupSlot[i].InitData(dataSpan, dataIndex, i);
                if (end == data.Length)
                {
                    DeActiveGroupSlot(i);
                    break;
                }
                dataIndex += infiniteScrollGridController.column;
            }
        }

        private void DeActiveGroupSlot(int startIndex)
        {
            for (var i = groupSlot.Count - 1; i > startIndex ; i--)
            {
                Object.Destroy(groupSlot[i].gameObject);
                groupSlot.RemoveAt(i);
            }
        }

        private void SwitchGridSlot(bool isUp, Action onComplete)
        {
            var gSlot = isUp ? groupSlot[^1] : groupSlot[0];
            var dataIndex = isUp ? gSlot.groupDataIndex - groupSlot.Count : gSlot.groupDataIndex + groupSlot.Count;
            
            if (!TryGetSlotData(dataIndex, out var data)) return;
            
            gSlot.InitData(data, dataIndex * infiniteScrollGridController.column, dataIndex);
            
            onComplete?.Invoke();
        }
        
        private bool TryGetSlotData(int dataIndex, out Span<TData> data)
        {
            var totalRow = (int)(totalDataCount/ infiniteScrollGridController.column);
            if (totalDataCount % infiniteScrollGridController.column != 0)
            {
                totalRow += 1;
            }
            if (dataIndex < 0 || dataIndex >= totalRow)
            {
                data = default;
                return false;
            }

            data = GetSlotData(dataIndex);
            return true;
        }

        private Span<TData> GetSlotData(int dataIndex)
        {
            var start = dataIndex * infiniteScrollGridController.column;
            var end = Mathf.Min(start + infiniteScrollGridController.column, totalDataCount);
            return _dataArray.AsSpan().Slice(start, end - start);
        }
    }
}
