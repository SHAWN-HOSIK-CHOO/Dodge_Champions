using UnityEngine;
using UnityEngine.InputSystem;


namespace HP
{
    public class InputBinding : MonoBehaviour
    {
        [SerializeField]
        string _actionMap;

        HP.InputManager _inputManager;
        WASD_MouseBinding _WASD_MouseBinding;

        void Start()
        {
            _inputManager = GetComponent<InputManager>();
            var keyboardMouseScheme = _inputManager.AddControlScheme("Default");
            _inputManager.AddControlScheme(keyboardMouseScheme, InputSystemNaming.Device.Keyboard);
            _inputManager.AddControlScheme(keyboardMouseScheme, InputSystemNaming.Device.Mouse);
            var actionMap = _inputManager.AddActionMap(_actionMap);
            _WASD_MouseBinding = new WASD_MouseBinding(_inputManager, actionMap);
            actionMap.Enable();
            _inputManager.Assign();
        }


        private void OnDestroy()
        {
            _WASD_MouseBinding.Dispose();
        }
    }
}