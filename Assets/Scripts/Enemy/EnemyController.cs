using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 적을 제어하기 위한 클래스입니다.
/// </summary>
public class EnemyController : MonoBehaviour
{
    const float SpeedIncreaseRateMultiplier = 0.1f; // 속도 증가 비율

    [SerializeField] EnemyDataSO _enemyDataSO;      // 적 데이터 SO

    float _moveSpeedIncreaseRatePerMinutes;         // 분 단위로 빨라지는 속도 값
    float _currentMoveSpeed;                        // 현재 이동속도

    protected Transform enemyTransform;             // 자신의 트랜스폼 캐시
    protected Transform playerTransform;            // 플레이어의 트랜스폼 캐시

    void Awake()
    {
        // 적 데이터 SO가 없으면 에러를 출력
        if (_enemyDataSO == null)
        {
            Debug.LogError(name + "이라는 적에게 EnemyDataSO가 없어요!");
        }

        // 자신의 트랜스폼 컴포넌트를 가져옴
        enemyTransform = transform.GetComponent<Transform>();

        // 씬에서 플레이어 오브젝트를 태그로 찾아서 트랜스폼 할당
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.GetComponent<Transform>();
        }
        else
        {
            // 플레이어 오브젝트가 없으면 에러 출력
            Debug.LogError("플레이어를 찾을 수 없어요! 플레이어가 존재하는지, 또는 플레이어에게 태그가 제대로 할당됐는지 체크해 주세요.");
        }
        
        // 기본 이동 속도에 속도 증가 비율을 곱한 값을 분 당 속도 증가 값으로 사용
        _moveSpeedIncreaseRatePerMinutes = _enemyDataSO.defaultMoveSpeed * SpeedIncreaseRateMultiplier;

        // 이벤트 연결
        GameManager.Instance.OnGameOver += () => _currentMoveSpeed = 0f;    // 게임 오버가 되면 속도가 0이 됨
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
    }

    protected virtual void OnEnable()
    {
        // 분 단위로 적의 속도 업데이트
        int elapsedMinutes = Mathf.FloorToInt((GameManager.Instance.PlayTimeSeconds % 3600f) / 60f);
        _currentMoveSpeed = _enemyDataSO.defaultMoveSpeed + (_moveSpeedIncreaseRatePerMinutes * elapsedMinutes);
    }

    protected virtual void Update()
    {
        // 적이 이드를 향해 이동
        enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, playerTransform.position, _currentMoveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 게임이 시작됐을 때 호출되는 콜백 메서드입니다.
    /// </summary>
    void HandleGamePlayStarted()
    {
        if (gameObject.activeSelf)
        {
            // 게임이 시작될 때 활성화 된 상태이면 오브젝트 풀로 반환
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}
