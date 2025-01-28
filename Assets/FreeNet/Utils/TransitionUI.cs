using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUI : SingletonMonoBehaviour<TransitionUI>
{
    [SerializeField]
    GameObject _UIRoot;
    [SerializeField]
    CanvasGroup _Canvas;
    CoroutineHandler _transitionUICoroutine;
    LinkedList<Param> _transitions;
    Dictionary<string, Param> _transitionParams;
    public IEnumerator NullTranstion()
    {
        yield return null;
    }
    public IEnumerator NullTranstion(Action action)
    {
        action?.Invoke();
        yield return null;
    }


    public class Param
    {
        public string _transitionName;
        public IEnumerator _enumerator;
        public bool _isDone;
    }
    private void OnDestroy()
    {
        _transitionUICoroutine.StopCoroutine();
        _transitionUICoroutine = null;
    }
    protected void Awake()
    {
        if(SingletonSpawn(this))
        {
            Init();
            SingletonInitialize();
        }
    }
    protected void Init()
    {
        _transitionUICoroutine = new CoroutineHandler(this);
        _transitions = new LinkedList<Param>();
        _transitionParams = new Dictionary<string, Param>();
        SetUIActive(false);
    }
    void SetUIActive(bool active)
    {
        _UIRoot.SetActive(active);
    }
    public void AddNullTransition(string name,Action action)
    {
        AddTransition(name, NullTranstion(action));
    }
    public void AddNullTransition(string name)
    {
        AddTransition(name, NullTranstion());
    }
    public void AddTransition(string name, IEnumerator transition)
    {
        var param = new Param()
        {
            _transitionName = name,
            _enumerator = transition,
            _isDone = false
        };

        if(!_transitionParams.TryAdd(param._transitionName, param))
        {
            UnityEngine.Debug.LogError("SameName Transition Set. The last one will be discarded.");
        }
        else
        {
            if (_transitions.Count == 0)
            {
                _transitions.AddLast(new Param()
                {
                    _transitionName = "Internal Opening",
                    _enumerator = StartTransitionCoroutine(),
                    _isDone = false
                });
                _transitions.AddLast(param);
                _transitions.AddLast(new Param()
                {
                    _transitionName = "Internal Closing",
                    _enumerator = StopTransitionCoroutine(),
                    _isDone = false
                });
                _transitionUICoroutine.StartUniqueCoroutine((int id) => TransitionCoroutine(id), (int id) =>
                {
                    SetUIActive(true);
                    UnityEngine.Debug.Log($"Start {id}");
                });
            }
            else if (_transitions.Count > 0)
            {
                _transitions.AddBefore(_transitions.Last, param);
            }
        }
    }
    public void StopWaitingUICoroutine()
    {
        _transitionUICoroutine.StopCoroutine();
    }
    IEnumerator StartTransitionCoroutine()
    {
        _Canvas.alpha = 0;
        while (_Canvas.alpha != 1)
        {
            _Canvas.alpha += Time.deltaTime * 2;
            if (_Canvas.alpha > 1) _Canvas.alpha = 1;
            yield return null;
        }
    }
    IEnumerator StopTransitionCoroutine()
    {
        _Canvas.alpha = 1;
        while (_Canvas.alpha != 0)
        {
            _Canvas.alpha -= Time.deltaTime;
            if (_Canvas.alpha < 0) _Canvas.alpha = 0;
            yield return null;
        }
        SetUIActive(false);
    }
    IEnumerator TransitionCoroutine(int id)
    {
        while(_transitions.First !=null)
        {
            Param param = _transitions.First.Value;
            _transitions.RemoveFirst();
            while(!param._isDone)
            {
                yield return param._enumerator;
                param._isDone = true;
            }
            _transitionParams.Remove(param._transitionName);
        }
        _transitionUICoroutine.StopCoroutine();
    }
    public void MakeTransitionEnd(string _name)
    {
        if (_transitionParams.TryGetValue(name, out Param param))
        {
            param._isDone = true;
        }
        else
        {
            Debug.LogWarning("No Transition, it may be done.");
        }
    }
}
