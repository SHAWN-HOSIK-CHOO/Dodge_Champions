using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NetworkScene", menuName = "Scriptable Objects/NetworkScene")]
public class NetworkSceneNames : ScriptableObject
{
    public List<string> sceneNames;
}
