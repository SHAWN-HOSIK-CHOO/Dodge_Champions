
using HP;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class EmptySceneManager : MonoBehaviour
{
    public PlayerInput input;
    WASD_MouseBinding WASD_MouseBinding;
    void Start()
    {

        WASD_MouseBinding = new WASD_MouseBinding();
        WASD_MouseBinding.Enable(true);
        WASD_MouseBinding._onMoveInputChanged += OnMoveInputChanged;

        input.actions["test"].performed += test;
    }

    void test(CallbackContext ctx)
    {
        Debug.Log(ctx);
    }

    void OnMoveInputChanged(Vector3 val)
    {
        Debug.Log(val);
    }
}
