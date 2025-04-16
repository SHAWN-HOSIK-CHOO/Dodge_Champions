using Unity.Netcode;
using UnityEngine;

public class TestPlayer : NetworkBehaviour
{
    [SerializeField]
    public Camera _camera;

    [SerializeField]
    HP.PlayerInput _input;

    public override void OnNetworkSpawn()
    {
        _camera.gameObject.SetActive(IsOwner);
        if(IsOwner)
        {
            _input.Enable(IsOwner);
            _input._onMouseRightInputChanged += OnMouseRight; 
        }
    }

    void OnMouseRight(float input)
    {
        if (input == 1f)
        {
            Ray ray = _camera.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.CompareTag("Player"))
                {
                    var player = hitObj.GetComponent<TestPlayer>();

                    //클릭한 플레이어의 정보 UI를 띄운다.
                }
            }
        }
    }
}