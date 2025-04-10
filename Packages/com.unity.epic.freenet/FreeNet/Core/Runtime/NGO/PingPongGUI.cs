using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PingPongGUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text _pingText;
    [SerializeField]
    TMP_Text _jitterText;
    [SerializeField]
    TMP_Text _rttText;
    [SerializeField]
    Toggle _useVirtualRtt;
    [SerializeField]
    Slider _rttSlider;
    [SerializeField]
    Slider _jitterSlider;
    [SerializeField]
    PingPong _pingpong;

    void Start()
    {
        _pingText.text = $"Ping : 0";
        _pingpong.OnRttChanged += OnRttChanged;
        _useVirtualRtt.onValueChanged.AddListener(OnToggleUseVirtualRtt);
        _rttSlider.onValueChanged.AddListener(OnSliderRttSlider);
        _jitterSlider.onValueChanged.AddListener(OnJitterSlider);

        _rttSlider.maxValue = 1000.0f;
        _rttSlider.minValue = 0.0f;
        _rttSlider.value = 0;
        _jitterSlider.maxValue = 1000.0f;
        _jitterSlider.minValue = 0.0f;
        _jitterSlider.value = 0;
    }

    void OnRttChanged()
    {
        if (_pingpong.GetRtt(FreeNet.Instance._ngoManager.GetComponent<EOSNetcodeTransport>().ServerClientId, out var rtt))
        {
            _pingText.text = $"Ping : {(int)rtt}";
        }
    }
    void OnJitterSlider(float val)
    {
        _pingpong.SetJitterRanage(_jitterSlider.value);
        _jitterText.text = $"jitter : {(int)_jitterSlider.value}";
    }
    void OnSliderRttSlider(float val)
    {
        _pingpong.SetVirtualRtt(_useVirtualRtt.isOn, _rttSlider.value);
        _rttText.text = $"vRtt : {(int)_rttSlider.value}";
    }
    void OnToggleUseVirtualRtt(bool isOn)
    {
        _pingpong.SetVirtualRtt(_useVirtualRtt.isOn, _rttSlider.value);
        _rttText.text = $"vRtt : {_rttSlider.value}";
    }
    private void OnDestroy()
    {
        _useVirtualRtt.onValueChanged.RemoveListener(OnToggleUseVirtualRtt);
        _rttSlider.onValueChanged.RemoveListener(OnSliderRttSlider);
    }
}
