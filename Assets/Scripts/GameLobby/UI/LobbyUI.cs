using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameLobby.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_Dropdown ballDropdown;  // TextMeshPro Dropdown for ball selection
        public TMP_Dropdown skillDropdown; // TextMeshPro Dropdown for skill selection
        public Button       confirmButton; // Button to confirm selection

        private void Start()
        {
            confirmButton.onClick.AddListener(OnConfirmSelection);
            
            InitializeDropdownOptions();
        }

        private void OnDestroy()
        {
            // Remove button click event
            confirmButton.onClick.RemoveListener(OnConfirmSelection);
        }

        private void InitializeDropdownOptions()
        {
            // Example options for BallDropdown
            ballDropdown.ClearOptions();
            ballDropdown.AddOptions(new System.Collections.Generic.List<string>
                                    {
                                        "BasicBall","AutoBall","DodgeBall"
                                    });

            // Example options for SkillDropdown
            skillDropdown.ClearOptions();
            skillDropdown.AddOptions(new System.Collections.Generic.List<string>
                                     {
                                         "Dash"
                                     });
        }

        private void OnConfirmSelection()
        {
            // Get selected values from TMP_Dropdowns
            int selectedBallIndex  = ballDropdown.value;
            int selectedSkillIndex = skillDropdown.value;

            // Send selected values to PlayerSelectionManager
            PlayerSelectionManager.Instance.SetPlayerSelection(selectedBallIndex, selectedSkillIndex);

            Debug.Log($"Player selected: BallIndex={selectedBallIndex}, SkillIndex={selectedSkillIndex}");
        }
    }
}
