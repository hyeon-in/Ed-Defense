using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 먼지 이펙트를 처리하기 위한 클래스입니다.
/// </summary>
public class DustEffect : MonoBehaviour
{
    const float MinSpeed = 3f;      // 최소 속도
    const float MaxSpeed = 8f;      // 최대 속도
    const float Duration = 0.25f;   // 먼지 이펙트 지속 시간

    float _speed;           // 속도
    float _elapsedTime;     // 남은 시간
    Vector2 _moveDirection; // 날아가는 방향
    Transform _transform;   // 캐시 트랜스폼

    void Awake()
    {
        // 트랜스폼 캐시
        _transform = GetComponent<Transform>();
    }

    void OnEnable()
    {
        // 활성화 될 때마다 초기화
        _elapsedTime = 0f;
        _speed = Random.Range(MinSpeed, MaxSpeed);
        _moveDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        // 델타 타임 캐시
        float deltaTime = Time.deltaTime;
        // 시간이 지날수록 크기 감소
        _transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (_elapsedTime / Duration));  
        // 먼지가 지정한 방향으로 날아감
        _transform.Translate(_moveDirection * _speed * deltaTime);
        if(_elapsedTime >= Duration)
        {
            // 지속 시간이 지나면 오브젝트 풀에 반환
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
        _elapsedTime += deltaTime;
    }
}
