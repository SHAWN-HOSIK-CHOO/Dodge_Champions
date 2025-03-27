using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystemGUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text _localTickText;
    [SerializeField]
    TMP_Text _serverTickText;
    [SerializeField]
    TMP_Text _localBufferedTickText;
    [SerializeField]
    TMP_Text _serverBufferedTickText;
    [SerializeField]
    Slider _localBufferedTickSlider;
    [SerializeField]
    Slider _serverBufferedTickSlider;
    [SerializeField]
    NgoManager _ngoManager;
    public void Init()
    {
        _localTickText.text = "LocalTick : 0";
        _serverTickText.text = "ServerTick : 0";

        _localBufferedTickSlider.onValueChanged.AddListener(OnLocalBufferedTick);
        _serverBufferedTickSlider.onValueChanged.AddListener(OnServerBufferedTick);
        _ngoManager.NetworkTickSystem.Tick += OnLocalTick;
        _ngoManager.NetworkTickSystem.ServerTick += OnServerTick;

        _localBufferedTickSlider.maxValue = 30;
        _localBufferedTickSlider.minValue = -30;
        _localBufferedTickSlider.value = 3;


        _serverBufferedTickSlider.maxValue = 30;
        _serverBufferedTickSlider.minValue = -30;
        _serverBufferedTickSlider.value = -3;
    }
    void OnLocalTick()
    {
        _localTickText.text = $"LocalTick : {_ngoManager.NetworkTickSystem.LocalTime.Tick}";
    }
    void OnServerTick()
    {
        _serverTickText.text = $"ServerTick : {_ngoManager.NetworkTickSystem.ServerTime.Tick}";
    }
    void OnLocalBufferedTick(float val)
    {
        _localBufferedTickText.text = $"{_localBufferedTickSlider.value}";
        if (_ngoManager.NetworkTickSystem == null) return;
        _ngoManager._localBufferSec = _localBufferedTickSlider.value / _ngoManager.NetworkTickSystem.TickRate;
        _ngoManager.SetNetworkValue();

    }
    void OnServerBufferedTick(float val)
    {
        _serverBufferedTickText.text = $"{_serverBufferedTickSlider.value}";
        if (_ngoManager.NetworkTickSystem == null) return;
        _ngoManager._serverBufferSec = -_serverBufferedTickSlider.value / _ngoManager.NetworkTickSystem.TickRate;
        _ngoManager.SetNetworkValue();
    }
}
