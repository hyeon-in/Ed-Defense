using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 드롭하는 코인을 제어하기 위한 클래스입니다.
/// </summary>
public class CoinController : MonoBehaviour
{
    const float ScatterSpeedMin = 8f;       // 코인이 사방으로 흩어지는 최소 속도
    const float ScatterSpeedMax = 18f;      // 코인이 사방으로 흩어지는 최대 속도
    const float TrackingDelay = 0.1f;       // 코인이 플레이어를 추적하기까지 걸리는 지연 시간
    const float TrackingSpeedBoost = 60f;   // 코인이 플레이어를 추적할 때 가속도

    float _deltaTime;                   // Time.deltaTime 캐시
    float _currentSpeed;                // 현재 속도
    float _scatterSpeedReductionRate;   // 흩어지는 속도의 감속 비율
    float _trackingDelayTimer;          // 추적 지연 타이머

    bool _isTrackingPlayer = false; // 플레이어 추적 상태 여부
    Vector2 _moveDirection;         // 이동 방향

    Transform _transform;       // 코인 트랜스폼 캐시
    Transform _playerTransform; // 플레이어 트랜스폼 캐시

    private void Awake()
    {
        // 트랜스폼 캐시
        _transform = GetComponent<Transform>();

        // 플레이어 오브젝트를 찾아 Transform 컴포넌트를 가져와서 캐시
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject != null)
        {
            _playerTransform = playerObject.GetComponent<Transform>();
        }
        else
        {
            Debug.LogError("코인에서 Player 오브젝트를 찾을 수 없었습니다!");
        }
    }

    private void OnEnable()
    {
        // 활성화 될 때마다 초기화
        _isTrackingPlayer = false;
        _trackingDelayTimer = TrackingDelay;
        _moveDirection = Random.insideUnitCircle.normalized;
        _currentSpeed = Random.Range(ScatterSpeedMin, ScatterSpeedMax);
        _scatterSpeedReductionRate = _currentSpeed * 3f;
    }

    private void Update()
    {
        // Time.deltaTime 캐시
        _deltaTime = Time.deltaTime;

        if (!_isTrackingPlayer)
        {
            // 추적하지 않는 상태이면 사방으로 흩어짐
            ScatterMovement();
        }
        else
        {
            // 추적하고 있ㄴ는 상태이면 플레이어 추적
            TrackPlayerMovement();
        }
    }

    /// <summary>
    /// 코인의 사방으로 흩어지는 이동을 처리하기 위한 메서드입니다.
    /// </summary>
    void ScatterMovement()
    {
        if (_currentSpeed > 1)
        {
            // 현재 속도가 일정 이상이면 이동 처리 및 현재 속도 감소
            _transform.Translate(_moveDirection * _currentSpeed * _deltaTime);
            _currentSpeed -= _scatterSpeedReductionRate * _deltaTime;
        }
        else
        {
            // 현재 속도가 일정 미만이면 플레이어를 추적하는 상태로 설정
            _isTrackingPlayer = true;
        }
    }

    /// <summary>
    /// 코인이 플레이어를 추적하기 위한 메서드입니다.
    /// </summary>
    void TrackPlayerMovement()
    {
        if (_trackingDelayTimer > 0)
        {
            // 플레이어를 추적하기 전에 잠깐 대기
            _trackingDelayTimer -= _deltaTime;
            return;
        }

        // 속도 가속 및, 코인과 플레이어 방향 계산, 계산된 방향으로 코인 이동을 처리합니다.
        _currentSpeed += TrackingSpeedBoost * _deltaTime;
        _moveDirection = (_playerTransform.position - transform.position).normalized;
        transform.Translate(_moveDirection * _currentSpeed * _deltaTime);
    }
}