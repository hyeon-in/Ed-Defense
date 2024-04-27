using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 스폰 처리를 담당하는 클래스입니다.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    const float SpawnIntervalDecreaseRate = 0.1f;   // 1분 마다 적의 스폰 간격이 감소하는 정도를 나타냅니다.
    const float MaxSpawnIntervalDecrease = 1f;      // 적의 스폰 간격의 최대 감소량입니다.
    const float SpawnDistance = 16f;                // 적이 스폰되는 거리입니다.

    /// <summary>
    /// 적 스폰에 대한 데이터 클래스입니다.
    /// </summary>
    [System.Serializable]
    public class EnemySpawnData
    {
        public Pool pool;                                 // 적 오브젝트 풀
        public float spawnStartTime;                      // 적이 처음으로 등장하는 시간
        public float spawnInterval;                       // 적의 스폰 간격
        [HideInInspector] public float lastSpawnTime;     // 마지막 스폰 시간
    }

    [SerializeField] EnemySpawnData[] enemiesSpawnData; // 적 스폰 데이터들을 담아놓은 배열입니다.

    void Start()
    {
        foreach(var enemySpawnData in enemiesSpawnData)
        {
            // 적 스폰 데이터의 풀을 오브젝트 풀 매니저에 삽입
            ObjectPoolManager.Instance.AddPool(enemySpawnData.pool);
        }

        // 이벤트 연결
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Play)
        {
            // 게임 플레이 상태가 아니면 실행하지 않음
            return;
        }

        // 1분 단위로 스폰 간격 감소
        int elapsedMinutes = Mathf.FloorToInt((GameManager.Instance.PlayTimeSeconds % 3600f) / 60f);
        float spawnIntervalDecrease = Mathf.FloorToInt(SpawnIntervalDecreaseRate * elapsedMinutes);
        spawnIntervalDecrease = Mathf.Clamp(spawnIntervalDecrease, 0f, MaxSpawnIntervalDecrease);

        // 실시간으로 스폰 데이터를 순환하며 스폰 시간이 되면 적을 스폰
        float playTimeSeconds = GameManager.Instance.PlayTimeSeconds;
        foreach(var enemySpawnData in enemiesSpawnData)
        {
            if (playTimeSeconds >= enemySpawnData.spawnStartTime)
            {
                if(playTimeSeconds - enemySpawnData.lastSpawnTime >= enemySpawnData.spawnInterval - spawnIntervalDecrease)
                {
                    EnemySpawn(enemySpawnData.pool);
                    enemySpawnData.lastSpawnTime = playTimeSeconds;
                }
            }
        }
    }

    /// <summary>
    /// 특정 적을 스폰하는 메서드입니다.
    /// </summary>
    /// <param name="pool">적의 오브젝트 풀데이터</param>
    void EnemySpawn(Pool pool)
    {
        Vector2 spawnPosition = Random.insideUnitCircle.normalized * SpawnDistance;
        ObjectPoolManager.Instance.SpawnFromPool(pool.poolName, spawnPosition);
    }

    /// <summary>
    /// 게임이 시작될 때 호출되는 콜백 메서드입니다.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // 적들의 마지막 스폰 시간 초기화
        foreach (var enemySpawnData in enemiesSpawnData)
        {
            enemySpawnData.lastSpawnTime = 0f;
        }
    }
}
