using System.Collections;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour
{
    /* 사용 예시
        void Awake()
        {
            if (SingletonSpawn(this))
            {
                base.Init();
                Init();
                SingletonInitialize();
            }
        }
        protected void Init()
        {

        }
    */

    /* Note
     * 싱글톤 객체 간의 참조가 생기는 경우
     * IEnumerator은 Awake에서 사용할 수 없음으로 싱글톤의 Start에서 다른 싱글톤의 초기화를 기다려야 하는데 
     * 차라리 사용할때마다 인자로 전달하는 편이 좋은 것 같다.
    */

    public static IEnumerator WaitSpawn()
    {
        while (!_Spwaned)
        {
            yield return null;
        }
    }
    public static IEnumerator WaitInitialize()
    {
        while (!_Initialied)
        {
            yield return null;
        }
    }
    public static bool _Spwaned { get; private set; } = false;
    public static bool _Initialied { get; private set; } = false;
    public static T _instance { get; private set; }
    protected bool SingletonSpawn(T instance)
    {
        if (_instance == null)
        {
            _instance = instance;
            _Spwaned = true;
            DontDestroyOnLoad(this);
            return true;
        }
        Destroy(this);
        return false;
    }
    protected void SingletonInitialize()
    {
        _Initialied = true;
    }
    private void OnDestroy()
    {
        _Spwaned = false;
        _Initialied = false;
    }
}
