using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameLobby.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("UI References")]
        public Button       confirmButton;
        
        [Space(5)] [Header("Select Buttons")] 
        public Button[] characterSelectButtons = new Button[6];

        private int _selectedIndex = 0;

        private void Start()
        {
            confirmButton.onClick.AddListener(OnConfirmSelection);
            
            for (int i = 0; i < characterSelectButtons.Length; i++)
            {
                int index = i;
                characterSelectButtons[i].onClick.AddListener(() =>Callback_Btn_OnCharacterSelected(index));
            }
        }

        private void OnDestroy()
        {
            // Remove button click event
            confirmButton.onClick.RemoveListener(OnConfirmSelection);
        }

        private void OnConfirmSelection()
        {
            // Send selected values to PlayerSelectionManager
            PlayerSelectionManager.Instance.SetPlayerSelection(_selectedIndex);
        }
        
        private void Callback_Btn_OnCharacterSelected(int index)
        {
            _selectedIndex = index;
        }
    }
}
