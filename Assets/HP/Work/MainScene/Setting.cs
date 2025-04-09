using UnityEngine;
using UnityEngine.EventSystems;

namespace MainScene
{
    public class Setting : MonoBehaviour
    {
        [SerializeField]
        UIImgButton _GameSettingTab;
        [SerializeField]
        GameObject _GameSettingBG;
        [SerializeField]
        UIImgButton _VidSoundSettingTab;
        [SerializeField]
        GameObject _VidSoundSettingBG;

        [SerializeField]
        UIImgButton _SettingGear;
        [SerializeField]
        UIImgButton _EpicLogo;

        [SerializeField]
        UIImgButton _closeSetting;
        [SerializeField]
        ResolutionSelect _resolution;

        [SerializeField]
        UIImgButton _Confirm;
        [SerializeField]
        UIImgButton _Cancle;

        [SerializeField]
        GameObject _SettingUI;
        [SerializeField]
        GameObject _GameSettingUI;
        [SerializeField]
        GameObject _VidSoundSettingUI;


        private void Start()
        {
            _EpicLogo.OnPointerClickAction += OnEpicLogoClick;
            _Cancle.OnPointerClickAction += OnCancleClick;
            _Confirm.OnPointerClickAction += OnConfirmClick;
            _VidSoundSettingTab.OnPointerClickAction += OnVidSoundSettingTabClick;
            _GameSettingTab.OnPointerClickAction += OnGameSettingTabClick;
            _SettingGear.OnPointerClickAction += OnSettingGearClick;
            _closeSetting.OnPointerClickAction += OnCloseSetting;
        }

        void OnVidSoundSettingTabClick(BaseEventData data)
        {
            _GameSettingBG.gameObject.SetActive(false);
            _VidSoundSettingBG.gameObject.SetActive(true);
        }

        void OnGameSettingTabClick(BaseEventData data)
        {
            _GameSettingBG.gameObject.SetActive(true);
            _VidSoundSettingBG.gameObject.SetActive(false);
        }

        void OnEpicLogoClick(BaseEventData data)
        {

        }


        void OnCloseSetting(BaseEventData data)
        {
            _SettingUI.SetActive(false);
        }

        void OnSettingGearClick(BaseEventData data)
        {
            _SettingUI.SetActive(true);
        }

        void OnConfirmClick(BaseEventData data)
        {
            // Confirm Setting Values
            _resolution.ChangeResolution();
            _resolution.SaveResolution();

        }
        void OnCancleClick(BaseEventData data)
        {
            _SettingUI.SetActive(false);
        }
    }
}