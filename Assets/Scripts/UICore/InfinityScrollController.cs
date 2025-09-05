using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using R3;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UICore
{
    public enum PaddingType
    {
        Left,
        Right,
        Top,
        Bottom,
        Spacing
    }

    [System.Serializable]
    public class Padding
    {
        public bool controlWidth = true;
        public bool controlHeight = true;
        public PaddingValue<float> left = new(PaddingType.Left);
        public PaddingValue<float> right = new(PaddingType.Right);
        public PaddingValue<float> top = new(PaddingType.Top);
        public PaddingValue<float> bottom = new(PaddingType.Bottom);
        public PaddingValue<Vector2> spacing = new(PaddingType.Spacing);
    }

    [System.Serializable]
    public class PaddingValue<TValueType> where TValueType : struct
    {
        public ReactiveValue<TValueType> rValue;
        public PaddingType paddingType;

#if UNITY_EDITOR
        public class PaddingValueDrawer : OdinValueDrawer<PaddingValue<TValueType>>
        {
            protected override void DrawPropertyLayout(GUIContent label)
            {
                ValueEntry.Property.Children.First().Draw(label);
            }
        }
#endif
        public PaddingValue(PaddingType type)
        {
            paddingType = type;
            rValue = new ReactiveValue<TValueType>();
        }
    }

    [System.Serializable]
    public class InfinityScrollController<TSlot, TData> : InfinityScroll<TSlot> where TSlot : SlotBase<TData> 
    {
        // public ScrollRect scrollRect;
        // public Padding padding;
        // [HideInInspector] public Vector3[] slotWorldCorners = new Vector3[4];
        // [HideInInspector] public Vector3[] viewportWorldCorners = new Vector3[4];
        // [HideInInspector] public float lastScrollPosition;
        // [HideInInspector] public ScrollMainContentType mainContentType;
        // public Action<bool, Action> SwitchAction;
        // public List<TSlot> slots = new();
        // [HideInInspector] public int dataCount;

        public override void InitData(List<TSlot> slots, ScrollMainContentType mainContentType, int dataCountChange)
        {
           
        }

        public override void InitContent()
        {
        }

        public override void InitSlotRectSize()
        {
        }

        private bool IsVisibleUp(RectTransform slotRect)
        {
            slotRect.GetWorldCorners(SlotWorldCorners);
            return SlotWorldCorners[0].y > ViewportWorldCorners[0].y; // top > bottom
        }

        private bool IsVisibleDown(RectTransform slotRect)
        {
            slotRect.GetWorldCorners(SlotWorldCorners);
            return SlotWorldCorners[0].y < ViewportWorldCorners[1].y; // top > bottom
        }

        public override void OnScrollChanged(Vector2 scrollPosition)
        {

        }

        public override void SwitchSuccess(bool isUp)
        {

        }

    }

    public abstract class InfinityScroll<TSlot>
    {
        public ScrollRect ScrollRect;
        public Padding Padding;
        [HideInInspector] public Vector3[] SlotWorldCorners = new Vector3[4];
        [HideInInspector] public Vector3[] ViewportWorldCorners = new Vector3[4];
        [HideInInspector] public float LastScrollPosition;
        [HideInInspector] public ScrollMainContentType MainContentType;
        public Action<bool, Action> SwitchAction;
        public List<TSlot> Slots = new();
        [HideInInspector] public int DataCount;

        public void SetActionSwitch(Action<bool, Action> updateSlotData) => SwitchAction = updateSlotData;


        public virtual void Start()
        {
            Padding.left.rValue.ReactiveProperty.Skip(1).Subscribe(v => ChangePadding(PaddingType.Left, v))
                .AddTo(ScrollRect);
            Padding.right.rValue.ReactiveProperty.Skip(1).Subscribe(v => ChangePadding(PaddingType.Right, v))
                .AddTo(ScrollRect);
            Padding.top.rValue.ReactiveProperty.Skip(1).Subscribe(v => ChangePadding(PaddingType.Top, v))
                .AddTo(ScrollRect);
            Padding.bottom.rValue.ReactiveProperty.Skip(1).Subscribe(v => ChangePadding(PaddingType.Bottom, v))
                .AddTo(ScrollRect);
            Padding.spacing.rValue.ReactiveProperty.Skip(1).Subscribe(v => ChangeSpacing(PaddingType.Spacing, v))
                .AddTo(ScrollRect);
        }

        public virtual void GetWorldCornersViewport(ScrollMainContentType mainContentType)
        {
            MainContentType = mainContentType;
            ScrollRect.viewport.GetWorldCorners(ViewportWorldCorners);
        }

        private void ChangePadding(PaddingType paddingType, float paddingChange)
        {
            InitContent();
        }

        private void ChangeSpacing(PaddingType paddingType, Vector2 spacingChange)
        {
            InitContent();
        }

        public virtual void InitData(List<TSlot> slots, ScrollMainContentType mainContentType, int dataCountChange)
        {
            Slots = slots;
            DataCount = dataCountChange;
            ScrollRect.onValueChanged.AddListener(OnScrollChanged);
            GetWorldCornersViewport(mainContentType);
            InitContent();
        }

        public virtual void InitContent()
        {
            InitSlotRectSize();
        }

        public abstract void InitSlotRectSize();
        public abstract void OnScrollChanged(Vector2 scrollPosition);
        public abstract void SwitchSuccess(bool isUp);
    }
}
