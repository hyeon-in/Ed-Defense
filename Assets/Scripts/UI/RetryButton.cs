using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������� ó���ϱ� ���� ��ư Ŭ�����Դϴ�.
/// </summary>
public class RetryButton : BaseButton
{
    [SerializeField] Sprite _default;   // �⺻ ��������Ʈ
    [SerializeField] Sprite _click;     // Ŭ�� ��������Ʈ

    /// <summary>
    /// ��ư Ŭ���� ó���ϴ� �޼����Դϴ�.
    /// </summary>
    protected override void HandleButtonClick()
    {
        // ���� ���°� ���� ���� ������ �� Ŭ���ϸ� ������ �簳�մϴ�.
        if (GameManager.Instance.CurrentState == GameManager.GameState.GameOver)
        {
            GameManager.Instance.CurrentState = GameManager.GameState.Play;
            GameManager.Instance.OnGamePlayStarted?.Invoke();
        }
    }

    /// <summary>
    /// ��ư ��������Ʈ�� ������Ʈ�ϴ� �޼����Դϴ�.
    /// </summary>
    protected override void UpdateButtonSprite()
    {
        buttonImage.sprite = isClicked ? _click : _default;
    }
}
