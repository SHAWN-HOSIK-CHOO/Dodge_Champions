using UnityEngine;

public class Resolution : MonoBehaviour
{
    [SerializeField]
    int _screenWidth;
    [SerializeField]
    int _screenHeight;
    [SerializeField]
    bool _fullScreen;
    void Start()
    {
        ChangeResolution(_screenWidth, _screenHeight, _fullScreen);
    }
    public void ChangeResolution(int width, int height, bool fullscreen)
    {
        _screenWidth = width;
        _screenHeight = height;
        _fullScreen = fullscreen;
        Screen.SetResolution(width, height, fullscreen);
    }

    public void SaveResolution()
    {
        PlayerPrefs.SetInt("resolution_width", _screenWidth);
        PlayerPrefs.SetInt("resolution_height", _screenHeight);
        PlayerPrefs.SetInt("fullscreen", _fullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }
    public bool ApplySavedResolution()
    {
        if (PlayerPrefs.HasKey("resolution_width") &&
        PlayerPrefs.HasKey("resolution_height") &&
        PlayerPrefs.HasKey("fullscreen"))
        {
            int width = PlayerPrefs.GetInt("resolution_width", Screen.currentResolution.width);
            int height = PlayerPrefs.GetInt("resolution_height", Screen.currentResolution.height);
            bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
            Screen.SetResolution(width, height, isFullscreen);
            return true;
        }
        return false;
    }
}
