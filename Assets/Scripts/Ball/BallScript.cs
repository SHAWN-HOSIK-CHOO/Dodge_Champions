using System;
using Character;
using UnityEngine;

namespace Ball
{
public class BallScript : MonoBehaviour
{
    private bool _canStart = false;
    
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _throwSpeed;
    private float _maxHeight;
    private float _timeElapsed;
    private bool _isInitialized;

    public int currentCollideCount = 0;
    private int _targetCollideCount = 1;

    private Rigidbody rb;

    [Header("Curve mode")] public bool isHorizontalThrow = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void Initialize(Vector3 target, float speed, float height, 
         bool horizontalThrow = false)
    {
        _startPosition    = Vector3.zero;
        _targetPosition   = target;
        _throwSpeed       = speed;
        _maxHeight        = height;
        _timeElapsed      = 0f;
        isHorizontalThrow = horizontalThrow;
        _isInitialized    = true;
        _canStart         = false;
    }

    public void StartCommand(Vector3 startPosition)
    {
        if (_isInitialized)
        {
            _canStart      = true;
            _startPosition = startPosition;
        }
    }
    
    void Update()
    {
        if (_isInitialized && _canStart)
        {
            _timeElapsed += Time.deltaTime;
            float travelDuration = Vector3.Distance(_startPosition, _targetPosition) / _throwSpeed;
            float t = _timeElapsed / travelDuration;
            
            transform.position = CalculateParabolicPosition(_startPosition, _targetPosition, _maxHeight, t, isHorizontalThrow);
            
            if (t >= 0.8f) 
            {
                _isInitialized = false;
                ActivateRigidbodyWithVelocity(t);
            }
        }
    }

    Vector3 CalculateParabolicPosition(Vector3 start, Vector3 target, float maxHeight, float t, bool horizontal)
    {
        // 수평 또는 수직 궤적에 따라 Lerp 방향 변경
        Vector3 horizontalPosition = Vector3.Lerp(start, target, t);
        float parabolicHeight = 4 * maxHeight * t * (1 - t);

        if (horizontal)
        {
            // 수평 궤적: Z 높이가 아닌 X를 궤적에 포함
            return new Vector3(start.x + parabolicHeight, horizontalPosition.y, horizontalPosition.z);
        }
        else
        {
            // 기본 수직 궤적
            return new Vector3(horizontalPosition.x, start.y + parabolicHeight, horizontalPosition.z);
        }
    }

    void ActivateRigidbodyWithVelocity(float t)
    {
        float nextT = t + 0.01f;
        Vector3 currentPos = transform.position;
        Vector3 nextPos = CalculateParabolicPosition(_startPosition, _targetPosition, _maxHeight, nextT, isHorizontalThrow);
        Vector3 direction = (nextPos - currentPos).normalized;

        rb.isKinematic = false;
        rb.linearVelocity = direction * _throwSpeed;
    }
    
    // private void OnCollisionEnter(Collision other)
    // {
    //     if (other.transform.CompareTag("Player"))
    //     {
    //         Debug.Log("Hit");
    //         
    //         if (this.transform.CompareTag("Fake"))
    //         {
    //             //other.gameObject.GetComponent<CharacterManager>().NotifyHitServerRPC();
    //         }
    //         
    //         currentCollideCount++;
    //
    //         if (currentCollideCount >= _targetCollideCount)
    //         {
    //             Destroy(this.gameObject);
    //         }
    //     }
    // }
    
}

}
