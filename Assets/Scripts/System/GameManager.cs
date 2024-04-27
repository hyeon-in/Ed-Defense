using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ �����ϴ� �̱��� Ŭ�����Դϴ�.
/// </summary>
public class GameManager : SingletonManager<GameManager>
{
    /// <summary>
    /// ������ ���۵� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    public Action OnGamePlayStarted;

    /// <summary>
    /// ���� �÷��� �ð��� ����� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    public Action<float> OnPlayTimeSecondsUpdated;
    
    /// <summary>
    /// ���� ���ھ ����� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    public Action<float> OnHighScoreUpdated;

    /// <summary>
    /// ���� ���°� ����� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    public Action<GameState> OnGameStateChanged;

    /// <summary>
    /// ���� ������ �� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    public Action OnGameOver;

    /// <summary>
    /// ������ ���¿� ���� ������
    /// </summary>
    public enum GameState
    {
        Wait,
        Play,
        GameOver,
        Upgrade
    }

    float _highScore;               // ���� ���ھ�
    float _playTimeSeconds = 0f;    // ���� �÷��� �ð�
    GameState _currentState;        // ������ ���� ����

    /// <summary>
    /// ������ ���� ���¿� ���� ������Ƽ�Դϴ�.
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
                        // ������ ���� ���°� �Ǹ� timeScale�� 1�� ����
                        Time.timeScale = 1f;
                        break;
                    case GameState.Upgrade:
                        // ���׷��̵� ȭ���� ������ ���°� �Ǹ� timeScale�� 0���� ����
                        Time.timeScale = 0f;
                        break;
                    case GameState.GameOver:
                        // ���� ������ �Ǹ� ���ӿ��� �̺�Ʈ ȣ��
                        OnGameOver?.Invoke();
                        break;
                }
                // ���� ���°� ����� �� �̺�Ʈ ȣ��
                OnGameStateChanged?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// ������ �÷��� �ð��� ���� ������Ƽ�Դϴ�.
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
    /// ���� ���ھ ���� ������Ƽ�Դϴ�.
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
        // �̺�Ʈ ����
        OnGamePlayStarted += () => PlayTimeSeconds = 0f;    // ������ ���۵� �� �÷��� �ð� 0���� �ʱ�ȭ
        OnGameOver += HandleGameOver;

        // ���� ���ھ� �ҷ���
        HighScore = PlayerPrefs.GetFloat("HighScore", HighScore);
    }

    void Update()
    {
        if (CurrentState == GameState.Play)
        {
            // ������ �÷��� ������ �� �ǽð����� �÷��� Ÿ�� ���� 
            PlayTimeSeconds += Time.deltaTime;
        }
    }

    /// <summary>
    /// ���� ������ �Ǹ� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    void HandleGameOver()
    {
        if(HighScore < PlayTimeSeconds)
        {
            // ���� ���� ���ھ ���� �÷��� �ð����� �����ٸ� ���� ���ھ �����ϰ� ����
            HighScore = PlayTimeSeconds;
            PlayerPrefs.SetFloat("HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }
}