using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
   public class HealthBarUI : MonoBehaviour
   {
      public Transform target;
      public Vector3   offset = new Vector3(0, 1.5f, 0);
      public Image     fillImage;
      public float     distanceScaleFactor = 10f;
      public float     minScale            = 0.2f;

      private Camera        _mainCamera;
      private Canvas        _canvas;
      private RectTransform _rectTransform;

      void Awake()
      {
         _mainCamera    = Camera.main;
         _canvas        = GetComponentInParent<Canvas>();
         _rectTransform = GetComponent<RectTransform>();
         //fillImage      = GetComponent<Image>();
      }

      void LateUpdate()
      {
         if (target == null)
         {
            Destroy(gameObject);
            return;
         }

         Vector3 worldOffset = (_mainCamera.transform.right * offset.x) + (_mainCamera.transform.up * offset.y);

         Vector3 screenPos = _mainCamera.WorldToScreenPoint(target.position + worldOffset);

         if (screenPos.z > 0)
         {
            Vector3 worldPos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                                                                        _canvas.GetComponent<RectTransform>(),
                                                                        screenPos,
                                                                        _canvas.worldCamera,
                                                                        out worldPos))
            {
               _rectTransform.position = worldPos;
            }

            // 1. 카메라와 target 사이 거리
            float distance = Vector3.Distance(_mainCamera.transform.position, target.position);

            // 2. 거리 기반 스케일 조정
            float scaleFactor = Mathf.Clamp(distanceScaleFactor / distance, 0.2f, 1f); // 원하는 범위로 조절
            _rectTransform.localScale = Vector3.one * scaleFactor;
         }
      }

      public void SetFill(float ratio)
      {
         fillImage.fillAmount = ratio;
      }

      public void SetTarget(Transform newTarget)
      {
         target = newTarget;
      }

      public void SetAsPlayer(Color c)
      {
         fillImage.color = c;
         offset          = new Vector3(0, 2.5f, 0); // 내 캐릭터는 조금 더 위에 띄움
      }
   }
}