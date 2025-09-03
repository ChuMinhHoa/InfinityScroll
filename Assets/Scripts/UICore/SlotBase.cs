using UnityEngine;

namespace UICore
{
    public class SlotBase<TData> : MonoBehaviour
    {
        public RectTransform myRectTransform;
        public int dataIndex;
        public TData data;

        public virtual void InitData(TData dataChange, int dataIndexChange)
        {
            data = dataChange;
            dataIndex = dataIndexChange;
        }

        public virtual void UpdateData(TData dataChange, int dataIndexChange)
        {
            InitData(dataChange, dataIndexChange);
        }
    }
}
