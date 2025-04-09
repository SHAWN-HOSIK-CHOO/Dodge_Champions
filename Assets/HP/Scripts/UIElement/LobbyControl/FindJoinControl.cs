//using System;
//using UnityEngine;

//public class FindJoinControl : MonoBehaviour
//{
//    [SerializeField]
//    GameObject _infoElementParent;
//    [SerializeField]
//    GameObject _infoElementElementPrf;

//    public event Action<LobbyElement> _onJoinRequest;

//    [HideInInspector]
//    public LobbyElement curSelectedLobby;

//    private void OnEnable()
//    {
//        transform.SetAsLastSibling();
//    }

//    public void SelecteMode(LobbyElement element)
//    {
//        if (curSelectedLobby != null)
//        {
//            curSelectedLobby.UnSelect();
//        }
//        curSelectedLobby = element;
//    }

//    public void InvokeJoin(LobbyElement element)
//    {
//        _onJoinRequest?.Invoke(element);
//    }

//    public void RemoveAllElement()
//    {
//        foreach (var element in _infoElementParent.GetComponentsInChildren<LobbyElement>())
//        {
//            element.transform.SetParent(null);
//            Destroy(element.gameObject);
//        }
//    }
//    public LobbyElement AddElement(EOS_LobbySearchResult lobby)
//    {
//        var item = Instantiate(_infoElementElementPrf).GetComponent<LobbyElement>();
//        item.transform.SetParent(_infoElementParent.transform);
//        item.Init(this, lobby);
//        return item;
//    }
//}
