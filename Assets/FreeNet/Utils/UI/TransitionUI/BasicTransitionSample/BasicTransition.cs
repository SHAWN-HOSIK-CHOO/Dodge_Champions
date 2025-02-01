using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicTransition : TransitionUI.Transition
{
    [SerializeField]
    TextMeshProUGUI _waitInfoText;
    float _elapsedTime;
    public BasicTransition(string transitionName, TextMeshProUGUI waitInfoText)
    {
        _waitInfoText = waitInfoText;
        _transitionName = transitionName;
    }
    public override IEnumerator StartTransition()
    {
        while (!_isDone)
        {
            _elapsedTime += Time.deltaTime;
            int quotient = (int)(_elapsedTime / 0.3) + 1;
            if(quotient > 4) quotient = 0;
            _waitInfoText.text = "Wait For " + new string('.', quotient);
            yield return null;
        }
    }
}
