using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 진행을 관리하는 싱글톤 클래스입니다.
/// </summary>
public class GameManager : SingletonManager<GameManager>
{
    /// <summary>
    /// 게임이 시작될 때 호출되는 이벤트입니다.
    /// </summary>
    public Action OnGamePlayStarted;

    /// <summary>
    /// 게임 플레이 시간이 변경될 때 호출되는 이벤트입니다.
    /// </summary>
    public Action<float> OnPlayTimeSecondsUpdated;
    
    /// <summary>
    /// 하이 스코어가 변경될 때 호출되는 이벤트입니다.
    /// </summary>
    public Action<float> OnHighScoreUpdated;

    /// <summary>
    /// 게임 상태가 변경될 때 호출되는 이벤트입니다.
    /// </summary>
    public Action<GameState> OnGameStateChanged;

    /// <summary>
    /// 게임 오버가 될 때 호출되는 이벤트입니다.
    /// </summary>
    public Action OnGameOver;

    /// <summary>
    /// 게임의 상태에 대한 열거형
    /// </summary>
    public enum GameState
    {
        Wait,
        Play,
        GameOver,
        Upgrade
    }

    float _highScore;               // 하이 스코어
    float _playTimeSeconds = 0f;    // 게임 플레이 시간
    GameState _currentState;        // 게임의 현재 상태

    /// <summary>
    /// 게임의 현재 상태에 대한 프로퍼티입니다.
    /// </summary>
    public GameState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                switch (value)
                {
                    case GameState.Play:
                        // 게임이 시작 상태가 되면 timeScale을 1로 설정
                        Time.timeScale = 1f;
                        break;
                    case GameState.Upgrade:
                        // 업그레이드 화면을 실행한 상태가 되면 timeScale을 0으로 설정
                        Time.timeScale = 0f;
                        break;
                    case GameState.GameOver:
                        // 게임 오버가 되면 게임오버 이벤트 호출
                        OnGameOver?.Invoke();
                        break;
                }
                // 게임 상태가 변경될 때 이벤트 호출
                OnGameStateChanged?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// 게임의 플레이 시간에 대한 프로퍼티입니다.
    /// </summary>
    public float PlayTimeSeconds 
    {
        get => _playTimeSeconds;
        private set
        {
            if(_playTimeSeconds != value)
            {
                _playTimeSeconds = value;
                OnPlayTimeSecondsUpdated?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// 하이 스코어에 대한 프로퍼티입니다.
    /// </summary>
    public float HighScore
    {
        get => _highScore;
        private set
        {
            _highScore = value;
            OnHighScoreUpdated?.Invoke(value);
        }
    }

    void Start()
    {
        // 이벤트 연결
        OnGamePlayStarted += () => PlayTimeSeconds = 0f;    // 게임이 시작될 때 플레이 시간 0으로 초기화
        OnGameOver += HandleGameOver;

        // 하이 스코어 불러옴
        HighScore = PlayerPrefs.GetFloat("HighScore", HighScore);
    }

    void Update()
    {
        if (CurrentState == GameState.Play)
        {
            // 게임이 플레이 상태일 때 실시간으로 플레이 타임 증가 
            PlayTimeSeconds += Time.deltaTime;
        }
    }

    /// <summary>
    /// 게임 오버가 되면 호출되는 메서드입니다.
    /// </summary>
    void HandleGameOver()
    {
        if(HighScore < PlayTimeSeconds)
        {
            // 기존 하이 스코어가 현재 플레이 시간보다 적었다면 하이 스코어를 갱신하고 저장
            HighScore = PlayTimeSeconds;
            PlayerPrefs.SetFloat("HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }
}