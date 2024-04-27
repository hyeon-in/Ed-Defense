using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spiral이라는 적을 제어하기 위한 클래스입니다.
/// </summary>
public class Spiral : EnemyController
{
    const float OrbitSpeed = 80.0f; // 궤도 회전 속도
    float _orbitDirection = 1f;

    protected override void OnEnable()
    {
        base.OnEnable();

        // 초기화 때 무작위 회전 방향 결정
        _orbitDirection = Random.value <= 0.5f ? 1f: -1f;
    }

    protected override void Update()
    {
       base.Update();

        // 플레이어와의 방향 및 각도 계산
        Vector2 playerToDirection = enemyTransform.position - playerTransform.position;
        float playerToAngle = Mathf.Atan2(playerToDirection.y, playerToDirection.x) * Mathf.Rad2Deg;

        // 계산 된 각도에 궤도 이동속도 값 대입 후 Radian값으로 변환
        float newAngle = playerToAngle + (_orbitDirection * OrbitSpeed * Time.deltaTime);
        newAngle *= Mathf.Deg2Rad;

        // 각도를 Vector2 값으로 변환 후 움직일 좌표값 계산
        Vector2 newDirection = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
        Vector2 newPosition = (Vector2)playerTransform.position + (newDirection * playerToDirection.magnitude);
        enemyTransform.position = newPosition;
    }
}
