using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


public static class SceneManagerWrapper
{
    public static string GetSceneNameFromPath(string scenePath)
    {
        int begin = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
        if (begin == -1) begin = 0;
        int end = scenePath.LastIndexOf(".", StringComparison.Ordinal);
        if (end == -1 || end < begin) end = scenePath.Length;
        return scenePath.Substring(begin, end - begin);
    }
    public static Scene LoadScene(string sceneName, LoadSceneMode mode)
    {
#if UNITY_EDITOR
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (GetSceneNameFromPath(scene.path) == sceneName)
            {
                return EditorSceneManager.LoadSceneInPlayMode(scene.path, new LoadSceneParameters() { loadSceneMode = mode });
            }
        }
#else
        return SceneManager.LoadSceneAsync("LobbyUI", LoadSceneMode.Additive);
#endif
        return default;
    }
    public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode)
    {
#if UNITY_EDITOR
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (GetSceneNameFromPath(scene.path) == sceneName)
            {
                return EditorSceneManager.LoadSceneAsyncInPlayMode(scene.path, new LoadSceneParameters() { loadSceneMode = mode });
            }
        }
#else
        return SceneManager.LoadSceneAsync("LobbyUI", LoadSceneMode.Additive);
#endif
        return null;
    }
    public static string GetScenePathByBuildIndex(int index)
    {
#if UNITY_EDITOR
        var scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        for (int i = 0; i<scenes.Length; i++)
        {
            if (i == index)
            {
                return scenes[i];
            }
        }
#else
        return SceneUtility.GetScenePathByBuildIndex(index);
#endif
        return null;
    }
    public static int GetBuildIndexByScenePath(string scenePath)
    {
#if UNITY_EDITOR
        var scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        for (int i = 0; i < scenes.Length; i++)
        {
            if (GetSceneNameFromPath(scenes[i]) == GetSceneNameFromPath(scenePath))
            {
                return i;
            }
        }
#else
        return SceneUtility.GetBuildIndexByScenePath(scenePath);
#endif
        return -1;
    }
}
