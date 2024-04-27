using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ���� �����ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class EnemyController : MonoBehaviour
{
    const float SpeedIncreaseRateMultiplier = 0.1f; // �ӵ� ���� ����

    [SerializeField] EnemyDataSO _enemyDataSO;      // �� ������ SO

    float _moveSpeedIncreaseRatePerMinutes;         // �� ������ �������� �ӵ� ��
    float _currentMoveSpeed;                        // ���� �̵��ӵ�

    protected Transform enemyTransform;             // �ڽ��� Ʈ������ ĳ��
    protected Transform playerTransform;            // �÷��̾��� Ʈ������ ĳ��

    void Awake()
    {
        // �� ������ SO�� ������ ������ ���
        if (_enemyDataSO == null)
        {
            Debug.LogError(name + "�̶�� ������ EnemyDataSO�� �����!");
        }

        // �ڽ��� Ʈ������ ������Ʈ�� ������
        enemyTransform = transform.GetComponent<Transform>();

        // ������ �÷��̾� ������Ʈ�� �±׷� ã�Ƽ� Ʈ������ �Ҵ�
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.GetComponent<Transform>();
        }
        else
        {
            // �÷��̾� ������Ʈ�� ������ ���� ���
            Debug.LogError("�÷��̾ ã�� �� �����! �÷��̾ �����ϴ���, �Ǵ� �÷��̾�� �±װ� ����� �Ҵ�ƴ��� üũ�� �ּ���.");
        }
        
        // �⺻ �̵� �ӵ��� �ӵ� ���� ������ ���� ���� �� �� �ӵ� ���� ������ ���
        _moveSpeedIncreaseRatePerMinutes = _enemyDataSO.defaultMoveSpeed * SpeedIncreaseRateMultiplier;

        // �̺�Ʈ ����
        GameManager.Instance.OnGameOver += () => _currentMoveSpeed = 0f;    // ���� ������ �Ǹ� �ӵ��� 0�� ��
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
    }

    protected virtual void OnEnable()
    {
        // �� ������ ���� �ӵ� ������Ʈ
        int elapsedMinutes = Mathf.FloorToInt((GameManager.Instance.PlayTimeSeconds % 3600f) / 60f);
        _currentMoveSpeed = _enemyDataSO.defaultMoveSpeed + (_moveSpeedIncreaseRatePerMinutes * elapsedMinutes);
    }

    protected virtual void Update()
    {
        // ���� �̵带 ���� �̵�
        enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, playerTransform.position, _currentMoveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// ������ ���۵��� �� ȣ��Ǵ� �ݹ� �޼����Դϴ�.
    /// </summary>
    void HandleGamePlayStarted()
    {
        if (gameObject.activeSelf)
        {
            // ������ ���۵� �� Ȱ��ȭ �� �����̸� ������Ʈ Ǯ�� ��ȯ
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}
