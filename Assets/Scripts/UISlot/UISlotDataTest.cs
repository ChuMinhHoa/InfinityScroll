using GlobalConfig;
using TMPro;
using UICore;
using UnityEngine.UI;

namespace UISlot
{
    public class UISlotDataTest : SlotBase<DataTestConfig>
    {
        public TextMeshProUGUI txtInt;
        public Image imgBg;
        public override void InitData(DataTestConfig dataChange, int dataIndexChange)
        {
            base.InitData(dataChange, dataIndexChange);
            txtInt.text = dataChange.dataInt.ToString();
            imgBg.color = dataChange.color;
        }
    }
}
