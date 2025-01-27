using Epic.OnlineServices.Lobby;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WaitingTransitionUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup _Canvas;
    [SerializeField]
    TextMeshProUGUI _waitInfo;
    [SerializeField]
    TextMeshProUGUI _waitFor;
    CoroutineHandler<bool> _WaitingUICoroutine;
    private void Awake()
    {
        _WaitingUICoroutine = new CoroutineHandler<bool>(this);
        this.gameObject.SetActive(false);
    }
    public void StartWaitingUICoroutine()
    {
        this.gameObject.SetActive(true);
        _WaitingUICoroutine.SetFlag(false);
        _WaitingUICoroutine.StartCoroutine(WaitingUICoroutine());
    }
    public void StopWaitingUICoroutine()
    {
        _WaitingUICoroutine.SetFlag(true);
    }
    public void UpdateWaitInfo(string waitInfo)
    {
        _waitInfo.text = waitInfo;
    }
    IEnumerator WaitingUICoroutine()
    {
        float elapsedTime = 0;
        _Canvas.alpha = 0;
        while (_Canvas.alpha != 1)
        {
            _Canvas.alpha += Time.deltaTime * 2;
            if (_Canvas.alpha > 1) _Canvas.alpha = 1;
            yield return null;
        }

        while (_WaitingUICoroutine._flag == false)
        {
            elapsedTime += Time.deltaTime;
            int quantinent = (int)(elapsedTime / 0.3);
            if (quantinent > 4) quantinent = 0;
            _waitFor.text = "Wait for" + new string('.', quantinent);
            yield return null;
        }
        _Canvas.alpha = 1;
        while (_Canvas.alpha != 0)
        {
            _Canvas.alpha -= Time.deltaTime;
            if (_Canvas.alpha < 0) _Canvas.alpha = 0;
            yield return null;
        }
        _WaitingUICoroutine.StopCoroutine();
        this.gameObject.SetActive(false);
    }
}
