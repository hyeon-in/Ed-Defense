using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 오버 화면을 관리하는 클래스입니다.
/// </summary>
public class GameOverScreen : MonoBehaviour
{
    void Start()
    {
        // 게임 오버가 되면 게임 오버 화면이 보이게 설정
        GameManager.Instance.OnGameOver += () => gameObject.SetActive(true);
        // 게임이 시작되면 게임 오버 화면이 보이지 않게 설정
        GameManager.Instance.OnGamePlayStarted += () => gameObject.SetActive(false);
        
        // 게임이 처음 실행될 때 항상 비활성화
        gameObject.SetActive(false);
    }
}
