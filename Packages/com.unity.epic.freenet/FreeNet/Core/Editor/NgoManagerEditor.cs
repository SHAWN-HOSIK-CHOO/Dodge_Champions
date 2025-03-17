using UnityEngine;
using Unity.Netcode.Editor;
using UnityEditor;

[CustomEditor(typeof(NgoManager), true)]
public class NgoManagerEditor : NetworkManagerEditor
{
    // Serialized Properties
    private SerializedProperty _localBufferSec;
    private SerializedProperty _serverBufferSec;
    private SerializedProperty _useEpicOnlineTransport;

    private SerializedProperty _networkSpawnerPref;


    private new void OnEnable()
    {
        _localBufferSec = serializedObject.FindProperty("_localBufferSec");
        _serverBufferSec = serializedObject.FindProperty("_serverBufferSec");
        _useEpicOnlineTransport = serializedObject.FindProperty("_useEpicOnlineTransport");
        _networkSpawnerPref = serializedObject.FindProperty("_networkSpawnerPref");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck(); 

        EditorGUILayout.PropertyField(_localBufferSec);
        EditorGUILayout.PropertyField(_serverBufferSec);
        EditorGUILayout.PropertyField(_useEpicOnlineTransport);
        EditorGUILayout.PropertyField(_networkSpawnerPref);

        if (EditorGUI.EndChangeCheck()) 
        {
            serializedObject.ApplyModifiedProperties();
            OnValueChanged();
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
        }
    }


    private void OnValueChanged()
    {
        NgoManager manager = (NgoManager)target; // target을 캐스팅

        if (manager != null)
        {
            manager.SetNetworkValue();
        }
    }
}
