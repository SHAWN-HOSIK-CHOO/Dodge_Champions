using System;
using UnityEngine;

public class FindJoinControl : MonoBehaviour
{
    [SerializeField]
    GameObject _infoElementParent;
    [SerializeField]
    GameObject _infoElementElementPrf;

    public event Action<LobbyInfoElement> _onJoinRequest;

    [HideInInspector]
    public LobbyInfoElement curSelectedLobby;

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void SelecteMode(LobbyInfoElement element)
    {
        if (curSelectedLobby != null)
        {
            curSelectedLobby.UnSelect();
        }
        curSelectedLobby = element;
    }

    public void InvokeJoin(LobbyInfoElement element)
    {
        _onJoinRequest?.Invoke(element);
    }

    public void RemoveAllElement()
    {
        foreach (var element in _infoElementParent.GetComponentsInChildren<LobbyInfoElement>())
        {
            element.transform.SetParent(null);
            Destroy(element.gameObject);
        }
    }
    public LobbyInfoElement AddElement(EOS_LobbySearchResult lobby)
    {
        var item = Instantiate(_infoElementElementPrf).GetComponent<LobbyInfoElement>();
        item.transform.SetParent(_infoElementParent.transform);
        item.Init(this, lobby);
        return item;
    }
}
