using System;
using GlobalConfig;
using Sirenix.OdinInspector;
using UICore;
using UISlot;
using UnityEngine;

public class UITestMainContent : MonoBehaviour
{
    public int[] testData;
    public MainContentBase<UISlotInfinite, int> mainContent;
    public MainContentGridBase<SlotGridDemo, DataTestConfig, UISlotDataTest> gridContent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testData = new int[97];
        for (var i = 0; i < testData.Length; i++)
        {
            testData[i] = i;
        }

        mainContent.Start();
        mainContent.InitData(testData.AsSpan());
     
    }

    [Button]
    private void InitData()
    {
        var data = DataIntGlobalConfig.Instance.dataTestConfigs;
        gridContent.Start();
        gridContent.InitData(data.AsSpan());
    }

}
