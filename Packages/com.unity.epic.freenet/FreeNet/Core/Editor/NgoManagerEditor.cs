using Unity.Netcode;
using Unity.Netcode.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NgoManager), true)]
public class NgoManagerEditor : NetworkManagerEditor
{
    // Serialized Properties
    private SerializedProperty _localBufferSec;
    private SerializedProperty _serverBufferSec;
    private SerializedProperty _useEpicOnlineTransport;

    private SerializedProperty _networkSpawnerPref;
    private SerializedProperty _networkScene;

    private new void OnEnable()
    {
        _localBufferSec = serializedObject.FindProperty("_localBufferSec");
        _serverBufferSec = serializedObject.FindProperty("_serverBufferSec");
        _useEpicOnlineTransport = serializedObject.FindProperty("_useEpicOnlineTransport");
        _networkSpawnerPref = serializedObject.FindProperty("_networkSpawnerPref");
        _networkScene = serializedObject.FindProperty("_networkScene");
    }

    protected override void DisplayCallToActionButtons()
    {
        if (!base.m_NetworkManager.IsServer && !m_NetworkManager.IsClient)
        {
            string buttonDisabledReasonSuffix = "";

            if (!EditorApplication.isPlaying)
            {
                buttonDisabledReasonSuffix = ". This can only be done in play mode";
                GUI.enabled = false;
            }

            if (m_NetworkManager.NetworkConfig.NetworkTopology == NetworkTopologyTypes.ClientServer)
            {
                if (GUILayout.Button(new GUIContent("Start NGO Host", "Starts a host instance" + buttonDisabledReasonSuffix)))
                {
                    (m_NetworkManager as NgoManager).StartHost();
                }

                if (GUILayout.Button(new GUIContent("Start NGO Server", "Starts a server instance" + buttonDisabledReasonSuffix)))
                {
                    (m_NetworkManager as NgoManager).StartServer();
                }

                if (GUILayout.Button(new GUIContent("Start NGO Client", "Starts a client instance" + buttonDisabledReasonSuffix)))
                {
                    (m_NetworkManager as NgoManager).StartClient();
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Start NGO Client", "Starts a distributed authority client instance" + buttonDisabledReasonSuffix)))
                {
                    (m_NetworkManager as NgoManager).StartClient();
                }
            }


            if (!EditorApplication.isPlaying)
            {
                GUI.enabled = true;
            }
        }
        else
        {
            string instanceType = string.Empty;

            if (m_NetworkManager.IsHost)
            {
                instanceType = "Host";
            }
            else if (m_NetworkManager.IsServer)
            {
                instanceType = "Server";
            }
            else if (m_NetworkManager.IsClient)
            {
                instanceType = "Client";
            }

            EditorGUILayout.HelpBox("You cannot edit the NetworkConfig when a " + instanceType + " is running.", MessageType.Info);

            if (GUILayout.Button(new GUIContent("Stop " + instanceType, "Stops the " + instanceType + " instance.")))
            {
                m_NetworkManager.Shutdown();
            }
        }
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
        EditorGUILayout.PropertyField(_networkScene);

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
