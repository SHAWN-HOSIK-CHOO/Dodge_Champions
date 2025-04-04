using UnityEngine;

namespace SO.UISettings
{
    [CreateAssetMenu(fileName = "UISettingsSO", menuName = "Scriptable Objects/UISettingsSO")]
    public class UISettingsSO : ScriptableObject
    {
        public Color themeColor;
        public Color ballColor;
        public Color skillColor;
        public string characterName;
        public string ballDesc;
        public string skillDesc;

        public GameObject pfDancer;
    }
}
