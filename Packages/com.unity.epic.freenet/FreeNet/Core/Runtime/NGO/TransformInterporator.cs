using Unity.Netcode;
using UnityEngine;

public class TransformInterporator : NetworkBehaviour
{
    [SerializeField] Transform subject;
    [SerializeField] Transform target;
    [SerializeField] Camera _camera;

    [SerializeField] float positionSmoothTime = 0.2f;
    [SerializeField] float rotationSmoothTime = 0.2f;
    private Vector3 _velocity;

    public override void OnNetworkSpawn()
    {
        _velocity = Vector3.zero;
        if (IsOwner)
        {
            _camera.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        _camera.transform.SetParent(subject);
        subject.position = Vector3.SmoothDamp(subject.position, target.position, ref _velocity, positionSmoothTime);
        subject.rotation = Quaternion.Slerp(subject.rotation, target.rotation, 1 - Mathf.Exp(-Time.deltaTime * rotationSmoothTime));
        _camera.transform.SetParent(gameObject.transform);
    }
}
