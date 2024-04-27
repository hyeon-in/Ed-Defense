using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����ϴ� ������ �����ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class CoinController : MonoBehaviour
{
    const float ScatterSpeedMin = 8f;       // ������ ������� ������� �ּ� �ӵ�
    const float ScatterSpeedMax = 18f;      // ������ ������� ������� �ִ� �ӵ�
    const float TrackingDelay = 0.1f;       // ������ �÷��̾ �����ϱ���� �ɸ��� ���� �ð�
    const float TrackingSpeedBoost = 60f;   // ������ �÷��̾ ������ �� ���ӵ�

    float _deltaTime;                   // Time.deltaTime ĳ��
    float _currentSpeed;                // ���� �ӵ�
    float _scatterSpeedReductionRate;   // ������� �ӵ��� ���� ����
    float _trackingDelayTimer;          // ���� ���� Ÿ�̸�

    bool _isTrackingPlayer = false; // �÷��̾� ���� ���� ����
    Vector2 _moveDirection;         // �̵� ����

    Transform _transform;       // ���� Ʈ������ ĳ��
    Transform _playerTransform; // �÷��̾� Ʈ������ ĳ��

    private void Awake()
    {
        // Ʈ������ ĳ��
        _transform = GetComponent<Transform>();

        // �÷��̾� ������Ʈ�� ã�� Transform ������Ʈ�� �����ͼ� ĳ��
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject != null)
        {
            _playerTransform = playerObject.GetComponent<Transform>();
        }
        else
        {
            Debug.LogError("���ο��� Player ������Ʈ�� ã�� �� �������ϴ�!");
        }
    }

    private void OnEnable()
    {
        // Ȱ��ȭ �� ������ �ʱ�ȭ
        _isTrackingPlayer = false;
        _trackingDelayTimer = TrackingDelay;
        _moveDirection = Random.insideUnitCircle.normalized;
        _currentSpeed = Random.Range(ScatterSpeedMin, ScatterSpeedMax);
        _scatterSpeedReductionRate = _currentSpeed * 3f;
    }

    private void Update()
    {
        // Time.deltaTime ĳ��
        _deltaTime = Time.deltaTime;

        if (!_isTrackingPlayer)
        {
            // �������� �ʴ� �����̸� ������� �����
            ScatterMovement();
        }
        else
        {
            // �����ϰ� �֤��� �����̸� �÷��̾� ����
            TrackPlayerMovement();
        }
    }

    /// <summary>
    /// ������ ������� ������� �̵��� ó���ϱ� ���� �޼����Դϴ�.
    /// </summary>
    void ScatterMovement()
    {
        if (_currentSpeed > 1)
        {
            // ���� �ӵ��� ���� �̻��̸� �̵� ó�� �� ���� �ӵ� ����
            _transform.Translate(_moveDirection * _currentSpeed * _deltaTime);
            _currentSpeed -= _scatterSpeedReductionRate * _deltaTime;
        }
        else
        {
            // ���� �ӵ��� ���� �̸��̸� �÷��̾ �����ϴ� ���·� ����
            _isTrackingPlayer = true;
        }
    }

    /// <summary>
    /// ������ �÷��̾ �����ϱ� ���� �޼����Դϴ�.
    /// </summary>
    void TrackPlayerMovement()
    {
        if (_trackingDelayTimer > 0)
        {
            // �÷��̾ �����ϱ� ���� ��� ���
            _trackingDelayTimer -= _deltaTime;
            return;
        }

        // �ӵ� ���� ��, ���ΰ� �÷��̾� ���� ���, ���� �������� ���� �̵��� ó���մϴ�.
        _currentSpeed += TrackingSpeedBoost * _deltaTime;
        _moveDirection = (_playerTransform.position - transform.position).normalized;
        transform.Translate(_moveDirection * _currentSpeed * _deltaTime);
    }
}