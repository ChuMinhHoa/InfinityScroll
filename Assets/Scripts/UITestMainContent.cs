using System;
using UICore;
using UISlot;
using UnityEngine;

public class UITestMainContent : MonoBehaviour
{
    public int[] testData;
    public MainContentBase<UISlotInfinite, int> mainContent;
    public MainContentGridBase<SlotGridDemo, int, UISlotInfinite> gridContent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testData = new int[1000];
        for (var i = 0; i < testData.Length; i++)
        {
            testData[i] = i;
        }
        mainContent.Start();
        mainContent.InitData(testData.AsSpan());
        gridContent.Start();
        gridContent.InitData(testData.AsSpan());
    }
}
