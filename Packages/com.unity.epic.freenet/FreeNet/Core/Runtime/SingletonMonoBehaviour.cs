using System.Collections;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour
{
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
    protected static T _instance;

    public static T Instance => _instance;

    protected bool SingletonSpawn(T instance)
    {
        if (Instance == null)
        {
            _instance = instance;
            _Spwaned = true;
            DontDestroyOnLoad(gameObject);
            return true;
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
        return false;
    }
    protected void SingletonInitialize()
    {
        _Initialied = true;
    }
    public virtual void OnRelease()
    {
        _Spwaned = false;
        _Initialied = false;
    }
}
