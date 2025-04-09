using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyElement : MonoBehaviour
{
    [SerializeField]
    TMP_Text _mode;
    [SerializeField]
    TMP_Text _owner;
    [SerializeField]
    TMP_Text _name;
    [SerializeField]
    TMP_Text _member;
    [SerializeField]
    TMP_Text _security;
    [SerializeField]
    Image _image;
    [SerializeField]
    UISelectElement _UISelectElement;
    public EOS_LobbySearchResult _lobbySearch;

    private void OnDestroy()
    {
        _lobbySearch.Release();
    }

    public void Init(EOS_LobbySearchResult lobby)
    {
        _lobbySearch = lobby;
        if (LobbyAttributeExtenstion.GetLobbyMode(_lobbySearch._attribute, out var mode))
        {
            _mode.text = mode;
        }
        if (LobbyAttributeExtenstion.GetLobbyName(_lobbySearch._attribute, out var name))
        {
            _name.text = name;
        }
        if (LobbyAttributeExtenstion.GetOwner(_lobbySearch._attribute, out var owner))
        {
            _owner.text = owner;
        }
        if (LobbyAttributeExtenstion.GetLobbySecurity(_lobbySearch._attribute, out var security))
        {
            _security.text = security.ToString();
        }
        _member.text = $"{_lobbySearch._info.AvailableSlots} / {_lobbySearch._info.MaxMembers}";
    }
}
