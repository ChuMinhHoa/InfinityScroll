using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace GlobalConfig
{
    [GlobalConfig("Assets/Resources/GlobalConfig/DataIntGlobalConfig")]
    [CreateAssetMenu(fileName = "DataIntGlobalConfig", menuName = "GlobalConfigs/DataIntGlobalConfig")]
    public class DataIntGlobalConfig : GlobalConfig<DataIntGlobalConfig>
    {
        public DataTestConfig[] dataTestConfigs;

        [Button]
        private void SetInt()
        {
            for (var i = 0; i < dataTestConfigs.Length; i++)
            {
                dataTestConfigs[i].dataInt = i;
                dataTestConfigs[i].color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }
        }
    }
    
    [Serializable]
    public class DataTestConfig
    {
        public int dataInt;
        public Color color;
    }
}
