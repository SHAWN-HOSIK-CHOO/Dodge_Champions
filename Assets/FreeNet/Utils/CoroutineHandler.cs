using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
public class CoroutineHandler
{
    MonoBehaviour _owner;
    Coroutine _coroutine;
    bool _IsStart;
    public CoroutineHandler(MonoBehaviour owner)
    {
        _owner = owner;
        _coroutine = null;
        _IsStart = false;
    }
    public void StopCoroutine()
    {
        if (_coroutine != null)
        {
            _owner.StopCoroutine(_coroutine);
            _coroutine = null;
            _IsStart = false;
        }
    }
    public void StartCoroutine(IEnumerator coroutine)
    {
        _coroutine = _owner.StartCoroutine(coroutine);
        _IsStart = true;
    }
    public IEnumerator Wait()
    {
        if(_IsStart)
        {
            yield return null;
        }
    }

}
public class CoroutineHandler<T> : CoroutineHandler where T : struct
{
    public T _flag { get; private set; }
    public void SetFlag(T flag)
    {
        _flag = flag;
    }
    public CoroutineHandler(MonoBehaviour owner) : base(owner)
    {
        _flag = default(T);
    }
}

public class CoroutineHandler<key,value> : CoroutineHandler
{
    public Dictionary<key, value> _params;
    public void SetParams(key key, value val)
    {
        if (_params.TryGetValue(key, out var _))
        {
            _params[key] = val;
        }
        else
        {
            _params.Add(key, val);
        }
    }
    public CoroutineHandler(MonoBehaviour owner) : base(owner)
    {
        _params = new Dictionary<key, value>();
    }
}
