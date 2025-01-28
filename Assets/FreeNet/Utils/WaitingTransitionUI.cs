using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingTransitionUI : SingletonMonoBehaviour<WaitingTransitionUI>
{
    [SerializeField]
    TextMeshProUGUI _waitInfoText;
    [SerializeField]
    TextMeshProUGUI _waitInfoDetailText;

    float _elapsedTime;


    public TransitionUI _transitionUI { get; private set; }
    private void Awake()
    {
        if(SingletonSpawn(this))
        {
            _elapsedTime = 0;
            SingletonInitialize();
        }
    }
    public IEnumerator Start()
    {
        yield return TransitionUI.WaitInitialize();
        _transitionUI = TransitionUI._instance;
    }
    public void UpdateWaitInfo()
    {
        _elapsedTime += Time.deltaTime;
        int quotient = (int)(_elapsedTime / 0.3) +1;
        _waitInfoText.text = "Wait For " + new string('.', quotient);
    }
    public void UpdateWaitInfoDetail(string detail)
    {
        _waitInfoDetailText.text = detail;
    }
}
