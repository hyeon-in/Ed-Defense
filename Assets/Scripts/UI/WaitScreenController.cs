using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대기 화면을 처리하는 클래스입니다.
/// </summary>
public class WaitScreenController : MonoBehaviour
{
    void Start()
    {
        InputManager.Instance.OnInputDown += HandleInputDown;
    }

    /// <summary>
    /// 입력이 시작될 때 호출되는 메서드입니다.
    /// </summary>
    void HandleInputDown(Vector2 inputPosition)
    {
        // 게임을 시작하고 화면 오브젝트 제거
        GameManager.Instance.CurrentState = GameManager.GameState.Play;
        GameManager.Instance.OnGamePlayStarted?.Invoke();
        InputManager.Instance.OnInputDown -= HandleInputDown;
        Destroy(gameObject);
    }
}
