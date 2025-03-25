using UnityEngine;
using DG.Tweening;

namespace GameUI
{
    public class SlidingUpPanel : MonoBehaviour
    {
        public RectTransform panelRect;       // Panel의 RectTransform
        public RectTransform buttonRect;      // Button의 RectTransform
        public float         duration = 0.5f; // 애니메이션 지속 시간

        private Vector2 _panelStartPos;
        private Vector2 _panelEndPos;
        private Vector2 _buttonStartPos;
        private Vector2 _buttonEndPos;

        private bool _isOpen = false; // 현재 패널이 열려 있는지 여부

        void Start()
        {
            // 패널과 버튼의 시작 위치 설정
            _panelStartPos             = new Vector2(0, -panelRect.rect.height);
            panelRect.anchoredPosition = _panelStartPos;

            _buttonStartPos = buttonRect.anchoredPosition;
        
            // 패널이 올라갔을 때의 위치
            _panelEndPos  = new Vector2(0,                 0);
            _buttonEndPos = new Vector2(_buttonStartPos.x, _buttonStartPos.y + panelRect.rect.height);
        }

        public void OnButtonClickTogglePanel()
        {
            if (panelRect == null || buttonRect == null)
            {
                return;
            }
            
            if (_isOpen)
            {
                // 패널 & 버튼을 아래로 슬라이드 (닫기)
                panelRect.DOAnchorPos(_panelStartPos, duration).SetEase(Ease.InCubic);
                buttonRect.DOAnchorPos(_buttonStartPos, duration).SetEase(Ease.InCubic);
            }
            else
            {
                // 패널 & 버튼을 위로 슬라이드 (열기)
                panelRect.DOAnchorPos(_panelEndPos, duration).SetEase(Ease.OutCubic);
                buttonRect.DOAnchorPos(_buttonEndPos, duration).SetEase(Ease.OutCubic);
            }

            _isOpen = !_isOpen; // 상태 변경
        }
    }
}

