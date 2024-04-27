using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이드의 애니메이터를 제어하는 클래스입니다.
/// </summary>
public class EdAnimatorController : MonoBehaviour
{
    // 애니메이션 해시
    readonly int HashGameStart = Animator.StringToHash("GameStart");
    readonly int HashGameOver = Animator.StringToHash("GameOver");

    Animator _animator; // 애니메이터

    void Start()
    {
        // 애니메이터 캐시
        _animator = GetComponent<Animator>();

        // 게임매니저에 이벤트 연결
        GameManager.Instance.OnGamePlayStarted += () => _animator.SetTrigger(HashGameStart);
        GameManager.Instance.OnGameOver += () => _animator.SetTrigger(HashGameOver);
    }
}
