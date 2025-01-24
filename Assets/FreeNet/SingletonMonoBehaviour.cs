using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour, Singleton<T>
{
    public static T _instance { get; private set; }
    bool Singleton<T>.Singleton(T instance)
    {
        if (_instance == null)
        {
            _instance = instance;
            DontDestroyOnLoad(this);
            return true;
        }
        return false;
    }
}
