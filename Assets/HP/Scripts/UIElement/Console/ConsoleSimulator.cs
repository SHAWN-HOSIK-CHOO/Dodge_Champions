using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleSimulator : MonoBehaviour
{
    [SerializeField]
    CoroutineHandler _coroutineHandler;
    Coroutine _coroutine;
    [SerializeField]
    Scrollbar _scrollbar;
    [SerializeField]
    TMP_InputField _textField;
    [SerializeField]
    Image _circleImage;

    TweenerCore<Color, Color, ColorOptions> _circleFadeTween;
    TweenerCore<float, float, FloatOptions> _circleFillTween;
    int _beginSimulIndex;
    int _endSimulIndex;
    private void Awake()
    { 
        var col = _circleImage.color;
        col.a = 0;
        _circleImage.color = col;
    }
    private void Start()
    {
        _circleFadeTween = _circleImage.DOFade(0f, 1f).SetAutoKill(false).Pause();
        _circleFillTween = _circleImage.DOFillAmount(1, 1.5f).SetEase(Ease.Linear).From(0).SetLoops(-1, LoopType.Yoyo).OnStepComplete(() =>
        {
            _circleImage.fillClockwise = !_circleImage.fillClockwise;
        }).SetAutoKill(false).Pause();
    }
    public void BeginTracking()
    {
        if (_coroutine != null) _coroutineHandler.EndCoroutine(_coroutine);
        _coroutine = null;
        _beginSimulIndex = _textField.textComponent.textInfo.characterCount;
    }
    public void EndTracking()
    {
        _endSimulIndex = _textField.textComponent.textInfo.characterCount;
    }
    public void Simulate(float stepTime, bool reset = true)
    {
        if (_coroutine == null)
        {
            _coroutine = _coroutineHandler.BeginCoroutine(() => SimulateCoroutine(stepTime, reset));
        }
    }

    public IEnumerator WaitUntilComplete()
    {
        if(_coroutine!=null)
        {
            yield return _coroutine;
        }
    }

    public void Reset()
    {
        _textField.textComponent.maxVisibleCharacters = int.MaxValue;
    }
    IEnumerator SimulateCoroutine(float stepTime,bool reset)
    {
        Color color = _circleImage.color;
        color.a = 1f;
        _circleFadeTween.ChangeValues(_circleImage.color, color, 1.0f).Restart();
        _circleFillTween.Restart();
        _textField.textComponent.maxVisibleCharacters = _beginSimulIndex;
        _scrollbar.value = 1f;
        while (_beginSimulIndex < _endSimulIndex)
        {
            if(_beginSimulIndex < _endSimulIndex)
            {
                _beginSimulIndex++;
            }
            _textField.textComponent.maxVisibleCharacters = _beginSimulIndex;
            _scrollbar.value = 1f;
            yield return new WaitForSeconds(stepTime);
        };
        if (reset) _textField.textComponent.maxVisibleCharacters = int.MaxValue;
        color = _circleImage.color;
        color.a = 0f;
        _circleFadeTween.ChangeValues(_circleImage.color, color, 2f).Restart();
        _circleFillTween.Pause();
    }
    private void OnDestroy()
    {
        _circleFillTween.Kill();
        _circleFadeTween.Kill();
    }
}

