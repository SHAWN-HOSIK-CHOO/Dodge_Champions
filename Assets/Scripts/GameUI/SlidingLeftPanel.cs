using UnityEngine;
using DG.Tweening;

namespace GameUI
{
   public class SlidingLeftPanel : MonoBehaviour
   {
      public RectTransform panelRect;       // Panel의 RectTransform
      public float         duration = 0.5f; // 애니메이션 지속 시간

      private Vector2 _panelStartPos;
      private Vector2 _panelEndPos;

      private bool _isOpen = false; // 현재 패널이 열려 있는지 여부

      public GameObject   waitingIndicator;
      public GameObject[] offObjects = new GameObject[3];

      void Start()
      {
         // 패널의 시작 위치 설정 (오른쪽으로 숨기기)
         _panelStartPos             = new Vector2(panelRect.rect.width, 0);
         panelRect.anchoredPosition = _panelStartPos;

         // 패널이 왼쪽으로 나왔을 때의 위치
         _panelEndPos = new Vector2(0, 0);
         waitingIndicator.SetActive(false);
      }

      public void OnButtonClickTogglePanel()
      {
         if (panelRect == null)
         {
            return;
         }

         if (_isOpen)
         {
            // 패널을 오른쪽으로 슬라이드 (닫기)
            panelRect.DOAnchorPos(_panelStartPos, duration).SetEase(Ease.InCubic);
            waitingIndicator.SetActive(true);

            foreach (var ob in offObjects)
            {
               ob.SetActive(false);
            }
         }
         else
         {
            // 패널을 왼쪽으로 슬라이드 (열기)
            panelRect.DOAnchorPos(_panelEndPos, duration).SetEase(Ease.OutCubic);
         }

         _isOpen = !_isOpen; // 상태 변경
      }
   }
}