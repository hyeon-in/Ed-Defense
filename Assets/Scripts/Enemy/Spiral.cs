using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spiral�̶�� ���� �����ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class Spiral : EnemyController
{
    const float OrbitSpeed = 80.0f; // �˵� ȸ�� �ӵ�
    float _orbitDirection = 1f;

    protected override void OnEnable()
    {
        base.OnEnable();

        // �ʱ�ȭ �� ������ ȸ�� ���� ����
        _orbitDirection = Random.value <= 0.5f ? 1f: -1f;
    }

    protected override void Update()
    {
       base.Update();

        // �÷��̾���� ���� �� ���� ���
        Vector2 playerToDirection = enemyTransform.position - playerTransform.position;
        float playerToAngle = Mathf.Atan2(playerToDirection.y, playerToDirection.x) * Mathf.Rad2Deg;

        // ��� �� ������ �˵� �̵��ӵ� �� ���� �� Radian������ ��ȯ
        float newAngle = playerToAngle + (_orbitDirection * OrbitSpeed * Time.deltaTime);
        newAngle *= Mathf.Deg2Rad;

        // ������ Vector2 ������ ��ȯ �� ������ ��ǥ�� ���
        Vector2 newDirection = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
        Vector2 newPosition = (Vector2)playerTransform.position + (newDirection * playerToDirection.magnitude);
        enemyTransform.position = newPosition;
    }
}
