using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace UICore
{
    public enum ScrollMainContentType
    {
        Normal,
        InfiniteVertical,
        InfiniteHorizontal,
        InfiniteGrid
    }

    [System.Serializable]
    public class MainContentBase<TSlot, TData> where TSlot : SlotBase<TData>
    {
        public ScrollMainContentType mainContentType;
        public List<TSlot> slots = new();
        public int totalDataCount;
        private TData[] _dataArray;
      
        [ShowIf(nameof(IsVertical))]
        [BoxGroup("Infinity Scroll Controller")]
        [InfoBox("Đặt anchor preset vào top center cho slot", InfoMessageType.Warning)]
        [HideLabel]
        public InfinityScrollControllerVertical<TSlot,TData> infiniteScrollVerticalController;
        
        [ShowIf(nameof(IsHorizontal))]
        [BoxGroup("Infinity Scroll Controller")]
        [InfoBox("Đặt anchor preset vào left center cho slot", InfoMessageType.Warning)]
        [HideLabel]
        public InfinityScrollControllerHorizontal<TSlot,TData> infiniteScrollHorizontalController;
        
        [ShowIf(nameof(IsGrid))]
        [BoxGroup("Infinity Scroll Controller")]
        [InfoBox("Đặt anchor preset vào center cho slot", InfoMessageType.Warning)]
        [HideLabel]
        public InfinityScrollControllerGrid<TSlot,TData> infiniteScrollGridController;
        
        private bool IsVertical() => mainContentType == ScrollMainContentType.InfiniteVertical;
        private bool IsHorizontal() => mainContentType == ScrollMainContentType.InfiniteHorizontal;
        private bool IsGrid() => mainContentType == ScrollMainContentType.InfiniteGrid;
        
        public void Start()
        {
            if (IsVertical())
                infiniteScrollVerticalController.Start();
            
            if (IsHorizontal())
                infiniteScrollHorizontalController.Start();
            
            if (IsGrid())
                infiniteScrollGridController.Start();
        }
        
        public virtual void InitData(Span<TData> data)
        {
            _dataArray = data.ToArray();
            totalDataCount = data.Length;
            for (var i = 0; i < slots.Count && i < data.Length; i++)
            {
                slots[i].InitData(data[i], i);
            }

            if (IsVertical())
            {
                infiniteScrollVerticalController.SetActionSwitch(SwitchSlot);
                infiniteScrollVerticalController.InitData(slots, mainContentType, data.Length);
            }
            
            if (IsHorizontal())
            {
                infiniteScrollHorizontalController.SetActionSwitch(SwitchSlot);
                infiniteScrollHorizontalController.InitData(slots, mainContentType, data.Length);
            }
            
            if (IsGrid())
            {
                infiniteScrollGridController.SetActionSwitch(SwitchSlot);
                infiniteScrollGridController.InitData(slots, mainContentType, data.Length);
            }
        }

        #region infinite scroll
        // Đảo vi trí slot và cập nhật data mới
        // Lấy được data thì mới đảo vị trí slot
        private void SwitchSlot(bool isUp, Action onComplete)
        {
            var slot = isUp ? slots[^1] : slots[0];
            var dataIndex = isUp ? slot.dataIndex - slots.Count : slot.dataIndex + slots.Count;
            
            if (!TryGetSlotData(dataIndex, out var data)) return;
            
            slot.UpdateData(data, dataIndex);
            
            onComplete?.Invoke();
        }
        // Lấy data theo index
        // index có thể vượt quá tổng số data hoặc nhỏ hơn 0
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

        #endregion
     
    }
}
