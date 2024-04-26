using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 하이 스코어 텍스트 UI를 관리하는 클래스입니다.
/// </summary>
public class HighScoreText : MonoBehaviour
{
    Text _highScoreText;  // 하이 스코어 텍스트

    void Awake()
    {
        // 체력 텍스트 캐시
        _highScoreText = GetComponent<Text>();
        // 현재 체력이 변하면 UI의 텍스트 변경
        GameManager.Instance.OnHighScoreUpdated += HandleHighScoreUpdated;
    }

    /// <summary>
    /// 하이 스코어가 변경될 때 호출되는 메서드입니다.
    /// </summary>
    /// <param name="highScore">하이 스코어 값</param>
    void HandleHighScoreUpdated(float highScore)
    {
        if (highScore == 0)
        {
            // 하이 스코어가 0이면 텍스트 비움
            _highScoreText.text = "";
            return;
        }

        int hours = Mathf.FloorToInt(highScore / 3600f);
        int minutes = Mathf.FloorToInt((highScore % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(highScore % 60f);
        int milliseconds = Mathf.FloorToInt((highScore * 100) % 100);

        if (hours > 0)
        {
            _highScoreText.text = string.Format("{0}:{1:D2}:{2:D2}.{3:D2}", hours, minutes, seconds, milliseconds);
        }
        else
        {
            _highScoreText.text = string.Format("{0:D2}:{1:D2}.{2:D2}", minutes, seconds, milliseconds);
        }
    }
}