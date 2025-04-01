using DG.Tweening;
using SO.UISettings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class FancyButtonManager : MonoBehaviour
    {
        [SerializeField] private RectTransform[] buttons;                // 관리할 버튼들
        private Vector3[] _originalPositions;      // 각 버튼의 원래 위치 저장
        private int _activeButtonIndex = -1; // 현재 활성화된 버튼 인덱스
        private Tween[] _hoverTweens;            // Hover 애니메이션 추적

        [SerializeField] private RectTransform[] shakeButtons;
        private Quaternion[] _shakeOriginalRotations;
        private int _shakeButtonIndex = -1;
        private Tween[] _hoverShakeTweens;

        [Space(5)]
        [Header("Character Info")]
        public UISettingsSO[] characterInfos = new UISettingsSO[6];

        public Transform dancingTransform;
        public Camera dancerCamera;
        public Image nameImage;
        public Image ballImage;
        public Image skillImage;
        public TMP_Text nameTMP;
        public TMP_Text ballDescTMP;
        public TMP_Text skillDescTMP;

        private GameObject _currentDancer;

        private void Awake()
        {
            // 각 버튼의 원래 위치 저장
            _originalPositions = new Vector3[buttons.Length];
            _hoverTweens = new Tween[buttons.Length];

            _shakeOriginalRotations = new Quaternion[shakeButtons.Length];
            _hoverShakeTweens = new Tween[shakeButtons.Length];

            for (int i = 0; i < buttons.Length; i++)
            {
                _originalPositions[i] = buttons[i].anchoredPosition;
            }

            for (int i = 0; i < shakeButtons.Length; i++)
            {
                _shakeOriginalRotations[i] = shakeButtons[i].rotation;
            }
        }

        private void Start()
        {
            OnButtonClick(0);
        }

        public void OnButtonHover(int index)
        {
            if (index == _activeButtonIndex) return; // 활성화된 버튼은 무시

            // 기존 Hover 애니메이션 중지
            _hoverTweens[index]?.Kill();

            // 좌우로 흔들리는 애니메이션 (Loop)
            _hoverTweens[index] = buttons[index]
                                .DOAnchorPosX(_originalPositions[index].x + 20f, 0.15f)
                                .SetLoops(-1, LoopType.Yoyo)
                                .SetEase(Ease.InOutQuad);
        }

        public void OnButtonHoverShake(int index)
        {
            if (index == _shakeButtonIndex) return;

            _hoverShakeTweens[index]?.Kill();

            _hoverShakeTweens[index] = shakeButtons[index].DORotate(new Vector3(0f, 0f, 10f), 0.3f).SetLoops(-1, LoopType.Yoyo)
                                                          .SetEase(Ease.InOutSine);
        }

        public void OnButtonExitShake(int index)
        {
            _hoverShakeTweens[index]?.Kill();
            shakeButtons[index].DORotate(_shakeOriginalRotations[index].eulerAngles, 0.2f);
        }

        public void OnButtonExit(int index)
        {
            if (index == _activeButtonIndex) return;

            // Hover 애니메이션 중지 및 원래 위치로 복귀
            _hoverTweens[index]?.Kill();
            buttons[index].DOAnchorPos(_originalPositions[index], 0.2f)
                          .SetEase(Ease.OutQuad);
        }

        public void OnButtonClick(int index)
        {
            // 이미 활성화된 버튼이면 무시
            if (index == _activeButtonIndex) return;

            ButtonClickHandler(index);

            // 이전에 활성화된 버튼이 있으면 원래 위치로 복귀
            if (_activeButtonIndex != -1)
            {
                _hoverTweens[_activeButtonIndex]?.Kill(); // 기존 애니메이션 중지
                buttons[_activeButtonIndex].DOAnchorPos(_originalPositions[_activeButtonIndex], 0.3f)
                                          .SetEase(Ease.OutQuad);
            }

            // 새로 클릭한 버튼을 위로 올리고 활성화
            _activeButtonIndex = index;
            _hoverTweens[index]?.Kill(); // Hover 애니메이션 중지
            buttons[index].DOAnchorPos(_originalPositions[index] + new Vector3(0, 60f, 0), 0.3f)
                          .SetEase(Ease.OutBack);
        }

        private void ButtonClickHandler(int index)
        {
            if (_currentDancer != null)
                Destroy(_currentDancer.gameObject);

            UISettingsSO clickedSO = characterInfos[index];
            dancerCamera.backgroundColor = clickedSO.themeColor;
            _currentDancer = Instantiate(clickedSO.pfDancer, dancingTransform);
            ballImage.color = clickedSO.ballColor;
            skillImage.color = clickedSO.skillColor;
            ballDescTMP.text = clickedSO.ballDesc;
            skillDescTMP.text = clickedSO.skillDesc;
            nameImage.color = clickedSO.themeColor;
            nameTMP.text = clickedSO.characterName;
        }
    }
}
