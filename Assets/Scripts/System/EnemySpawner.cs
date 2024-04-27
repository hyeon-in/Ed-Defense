using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ó���� ����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    const float SpawnIntervalDecreaseRate = 0.1f;   // 1�� ���� ���� ���� ������ �����ϴ� ������ ��Ÿ���ϴ�.
    const float MaxSpawnIntervalDecrease = 1f;      // ���� ���� ������ �ִ� ���ҷ��Դϴ�.
    const float SpawnDistance = 16f;                // ���� �����Ǵ� �Ÿ��Դϴ�.

    /// <summary>
    /// �� ������ ���� ������ Ŭ�����Դϴ�.
    /// </summary>
    [System.Serializable]
    public class EnemySpawnData
    {
        public Pool pool;                                 // �� ������Ʈ Ǯ
        public float spawnStartTime;                      // ���� ó������ �����ϴ� �ð�
        public float spawnInterval;                       // ���� ���� ����
        [HideInInspector] public float lastSpawnTime;     // ������ ���� �ð�
    }

    [SerializeField] EnemySpawnData[] enemiesSpawnData; // �� ���� �����͵��� ��Ƴ��� �迭�Դϴ�.

    void Start()
    {
        foreach(var enemySpawnData in enemiesSpawnData)
        {
            // �� ���� �������� Ǯ�� ������Ʈ Ǯ �Ŵ����� ����
            ObjectPoolManager.Instance.AddPool(enemySpawnData.pool);
        }

        // �̺�Ʈ ����
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Play)
        {
            // ���� �÷��� ���°� �ƴϸ� �������� ����
            return;
        }

        // 1�� ������ ���� ���� ����
        int elapsedMinutes = Mathf.FloorToInt((GameManager.Instance.PlayTimeSeconds % 3600f) / 60f);
        float spawnIntervalDecrease = Mathf.FloorToInt(SpawnIntervalDecreaseRate * elapsedMinutes);
        spawnIntervalDecrease = Mathf.Clamp(spawnIntervalDecrease, 0f, MaxSpawnIntervalDecrease);

        // �ǽð����� ���� �����͸� ��ȯ�ϸ� ���� �ð��� �Ǹ� ���� ����
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
    /// Ư�� ���� �����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="pool">���� ������Ʈ Ǯ������</param>
    void EnemySpawn(Pool pool)
    {
        Vector2 spawnPosition = Random.insideUnitCircle.normalized * SpawnDistance;
        ObjectPoolManager.Instance.SpawnFromPool(pool.poolName, spawnPosition);
    }

    /// <summary>
    /// ������ ���۵� �� ȣ��Ǵ� �ݹ� �޼����Դϴ�.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // ������ ������ ���� �ð� �ʱ�ȭ
        foreach (var enemySpawnData in enemiesSpawnData)
        {
            enemySpawnData.lastSpawnTime = 0f;
        }
    }
}
