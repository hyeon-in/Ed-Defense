using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����Ʈ�� ó���ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class DustEffect : MonoBehaviour
{
    const float MinSpeed = 3f;      // �ּ� �ӵ�
    const float MaxSpeed = 8f;      // �ִ� �ӵ�
    const float Duration = 0.25f;   // ���� ����Ʈ ���� �ð�

    float _speed;           // �ӵ�
    float _elapsedTime;     // ���� �ð�
    Vector2 _moveDirection; // ���ư��� ����
    Transform _transform;   // ĳ�� Ʈ������

    void Awake()
    {
        // Ʈ������ ĳ��
        _transform = GetComponent<Transform>();
    }

    void OnEnable()
    {
        // Ȱ��ȭ �� ������ �ʱ�ȭ
        _elapsedTime = 0f;
        _speed = Random.Range(MinSpeed, MaxSpeed);
        _moveDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        // ��Ÿ Ÿ�� ĳ��
        float deltaTime = Time.deltaTime;
        // �ð��� �������� ũ�� ����
        _transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (_elapsedTime / Duration));  
        // ������ ������ �������� ���ư�
        _transform.Translate(_moveDirection * _speed * deltaTime);
        if(_elapsedTime >= Duration)
        {
            // ���� �ð��� ������ ������Ʈ Ǯ�� ��ȯ
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
        _elapsedTime += deltaTime;
    }
}
