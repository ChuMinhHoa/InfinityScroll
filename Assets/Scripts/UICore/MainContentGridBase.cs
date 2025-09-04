using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace UICore
{
    [System.Serializable]
    public class MainContentGridBase<TSlotGrid, TData, TSlot> where TSlotGrid : SlotGridBase<TData, TSlot> where TSlot : SlotBase<TData>
    { 
        public List<TSlotGrid> groupSlot = new();
        public int totalDataCount;
        private TData[] _dataArray;
        [BoxGroup("Infinity Scroll Controller")]
        [InfoBox("Đặt anchor preset vào center cho slot", InfoMessageType.Warning)]
        [HideLabel]
        public InfinityScrollControllerGrid<TSlotGrid, TData, TSlot> infiniteScrollGridController;
        
        public void Start()
        {
            infiniteScrollGridController.Start();
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
            totalDataCount = data.Length;
            _dataArray = data.ToArray();
            var dataIndex = 0;
            for (var i = 0; i < groupSlot.Count && i < data.Length; i++)
            {
                var end = Mathf.Min(dataIndex + infiniteScrollGridController.column, data.Length);
                var dataSpan = data.Slice(dataIndex, end - dataIndex);
                groupSlot[i].InitData(dataSpan, dataIndex, i);
              
                dataIndex += infiniteScrollGridController.column;
            }
        }
        
        private void SwitchGridSlot(bool isUp, Action onComplete)
        {
            var gSlot = isUp ? groupSlot[^1] : groupSlot[0];
            var dataIndex = isUp ? gSlot.groupDataIndex - groupSlot.Count : gSlot.groupDataIndex + groupSlot.Count;
            
            // if (!TryGetSlotData(dataIndex, out var data)) return;
            //
            // slot.UpdateData(data, dataIndex);
            
            onComplete?.Invoke();
        }
        
        private bool TryGetSlotData(int dataIndex, out TData data)
        {
            if (dataIndex < 0 || dataIndex >= totalDataCount)
            {
                data = default;
                return false;
            }

            data = GetSlotData(dataIndex);
            return true;
        }
        
        private TData GetSlotData(int dataIndex) => _dataArray[dataIndex];
    }
}
