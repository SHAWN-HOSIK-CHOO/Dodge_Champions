using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class Talk : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float stayDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 0.3f;


    public void Show(RectTransform rect, string message, Action onComplete)
    {
        _text.text = message;
        CanvasGroup group = GetComponent<CanvasGroup>();
        group.alpha = 0;
        Vector2 Size = rect.rect.size;

        float maxX = (Size.x / 5f);
        float maxY = (Size.y / 5f);

        Vector2 randomPos = new Vector2(
            UnityEngine.Random.Range(-maxX, maxX),
            UnityEngine.Random.Range(-maxY, maxY)
        );

        GetComponent<RectTransform>().anchoredPosition = randomPos;
        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(1f, fadeInDuration))
            .AppendInterval(stayDuration)
            .Append(group.DOFade(0f, fadeOutDuration))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }
}
