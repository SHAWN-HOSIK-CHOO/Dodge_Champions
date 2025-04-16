using Unity.Netcode;
using UnityEngine;

public class TransformInterporator : NetworkBehaviour
{
    [SerializeField] Transform subject;
    [SerializeField] Transform target;
    [SerializeField] Camera _camera;

    [SerializeField] float positionSmoothTime = 0.2f;
    [SerializeField] float rotationSmooth = 15f;
    private Vector3 _velocity;

    public override void OnNetworkSpawn()
    {
        _velocity = Vector3.zero;
        _camera.gameObject.SetActive(IsOwner);
    }

    private void Update()
    {
        _camera.transform.SetParent(subject);
        float smoothTimeY = positionSmoothTime * 0.5f; // 중력은 더 빠르게 보간
        float newX = Mathf.SmoothDamp(subject.position.x, target.position.x, ref _velocity.x, positionSmoothTime);
        float newY = Mathf.SmoothDamp(subject.position.y, target.position.y, ref _velocity.y, smoothTimeY);
        float newZ = Mathf.SmoothDamp(subject.position.z, target.position.z, ref _velocity.z, positionSmoothTime);
        subject.position = new Vector3(newX, newY, newZ);
        subject.rotation = Quaternion.Slerp(subject.rotation, target.rotation, 1 - Mathf.Exp(-Time.deltaTime * rotationSmooth));
        _camera.transform.SetParent(gameObject.transform);
    }
}
