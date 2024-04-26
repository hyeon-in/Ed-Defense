using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 플레이 시간 텍스트를 관리하는 클래스입니다.
/// </summary>
public class PlayTimeText : MonoBehaviour
{
    Text _playTimeText;

    void Start()
    {
        _playTimeText = GetComponent<Text>();
        GameManager.Instance.OnPlayTimeSecondsUpdated += OnPlayTimeSecondsUpdated;
    }

    /// <summary>
    /// 게임 플레이 시간이 변경될 때 호출되는 메서드입니다.
    /// </summary>
    /// <param name="playTimeSeconds">현재 게임 플레이 시간</param>
    void OnPlayTimeSecondsUpdated(float playTimeSeconds)
    {
        // 게임 플레이 시간을 텍스트 포맷에 맞춰 변환
        int hours = Mathf.FloorToInt(playTimeSeconds / 3600f);
        int minutes = Mathf.FloorToInt((playTimeSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(playTimeSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((playTimeSeconds * 100) % 100);

        if (hours > 0)
        {
            _playTimeText.text = string.Format("{0}:{1:D2}:{2:D2}.{3:D2}", hours, minutes, seconds, milliseconds);
        }
        else
        {
            _playTimeText.text = string.Format("{0:D2}:{1:D2}.{2:D2}", minutes, seconds, milliseconds);
        }
    }
}
