using DG.Tweening;
using UnityEngine;
public class DGTween : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        DOTween.Init(true, true, LogBehaviour.Verbose);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
        DOTween.Clear(true);
    }
}
