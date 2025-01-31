using System.Collections;
using UnityEngine;
using static EOSWrapper;

public class GameSceneManager : MonoBehaviour
{
    TransitionUI _transitionUI;
    BasicUI _basicUI;

    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<TransitionUI>.WaitInitialize();
        _transitionUI = TransitionUI._instance;
        _basicUI = _transitionUI.GetRootUI().GetComponentInChildren<BasicUI>();
        _basicUI._waitInfoDetail.text = "LoadGame Success";
        _transitionUI.MakeTransitionEnd("LoadGame");
    }
}
