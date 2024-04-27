using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �÷��� �ð� �ؽ�Ʈ�� �����ϴ� Ŭ�����Դϴ�.
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
    /// ���� �÷��� �ð��� ����� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="playTimeSeconds">���� ���� �÷��� �ð�</param>
    void OnPlayTimeSecondsUpdated(float playTimeSeconds)
    {
        // ���� �÷��� �ð��� �ؽ�Ʈ ���˿� ���� ��ȯ
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
