using UnityEngine;

public class TransformInterporator : MonoBehaviour
{
    [SerializeField] Transform subject;
    [SerializeField] Transform target;
    [SerializeField] float positionSmoothTime = 0.2f;
    [SerializeField] float rotationSmoothTime = 5.0f; 
    private Vector3 velocity = Vector3.zero;
    private void Update()
    {
        if (subject == null || target == null) return;

        // 위치 보간
        subject.position = Vector3.SmoothDamp(subject.position, target.position, ref velocity, positionSmoothTime);

        // 회전 보간 (자연스럽게 감속)
        subject.rotation = Quaternion.Slerp(subject.rotation, target.rotation, 1 - Mathf.Exp(-Time.deltaTime * rotationSmoothTime));
    }

}
