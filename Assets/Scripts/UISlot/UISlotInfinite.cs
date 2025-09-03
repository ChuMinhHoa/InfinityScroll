using System;
using TMPro;
using UICore;
using UnityEngine;

namespace UISlot
{
    public class UISlotInfinite : SlotBase<int>
    {
        [SerializeField] private TextMeshProUGUI txtValue;
        public override void InitData(int dataChange, int dataIndexChange)
        {
            base.InitData(dataChange, dataIndexChange);
            txtValue.text = dataChange.ToString();
        }
    }
}
