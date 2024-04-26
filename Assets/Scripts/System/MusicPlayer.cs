using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 음악 재생을 관리하는 클래스입니다.
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip _music;  // 게임 음악
    void Start()
    {
        // 이벤트 연결
        // 게임이 시작되면 음악을 실행하며, 게임 오버가 되면 음악을 중단
        GameManager.Instance.OnGamePlayStarted += () => SoundManager.Instance.PlayBGM(_music);
        GameManager.Instance.OnGameOver += () => SoundManager.Instance.PuaseBGM();
    }
}
