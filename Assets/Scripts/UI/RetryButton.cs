using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 재시작을 처리하기 위한 버튼 클래스입니다.
/// </summary>
public class RetryButton : BaseButton
{
    [SerializeField] Sprite _default;   // 기본 스프라이트
    [SerializeField] Sprite _click;     // 클릭 스프라이트

    /// <summary>
    /// 버튼 클릭을 처리하는 메서드입니다.
    /// </summary>
    protected override void HandleButtonClick()
    {
        // 현재 상태가 게임 오버 상태일 때 클릭하면 게임을 재개합니다.
        if (GameManager.Instance.CurrentState == GameManager.GameState.GameOver)
        {
            GameManager.Instance.CurrentState = GameManager.GameState.Play;
            GameManager.Instance.OnGamePlayStarted?.Invoke();
        }
    }

    /// <summary>
    /// 버튼 스프라이트를 업데이트하는 메서드입니다.
    /// </summary>
    protected override void UpdateButtonSprite()
    {
        buttonImage.sprite = isClicked ? _click : _default;
    }
}
