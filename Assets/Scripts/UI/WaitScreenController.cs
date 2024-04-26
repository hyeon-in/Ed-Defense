using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ȭ���� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class WaitScreenController : MonoBehaviour
{
    void Start()
    {
        InputManager.Instance.OnInputDown += HandleInputDown;
    }

    /// <summary>
    /// �Է��� ���۵� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    void HandleInputDown(Vector2 inputPosition)
    {
        // ������ �����ϰ� ȭ�� ������Ʈ ����
        GameManager.Instance.CurrentState = GameManager.GameState.Play;
        GameManager.Instance.OnGamePlayStarted?.Invoke();
        InputManager.Instance.OnInputDown -= HandleInputDown;
        Destroy(gameObject);
    }
}
