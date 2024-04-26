using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ���ھ� �ؽ�Ʈ UI�� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class HighScoreText : MonoBehaviour
{
    Text _highScoreText;  // ���� ���ھ� �ؽ�Ʈ

    void Awake()
    {
        // ü�� �ؽ�Ʈ ĳ��
        _highScoreText = GetComponent<Text>();
        // ���� ü���� ���ϸ� UI�� �ؽ�Ʈ ����
        GameManager.Instance.OnHighScoreUpdated += HandleHighScoreUpdated;
    }

    /// <summary>
    /// ���� ���ھ ����� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="highScore">���� ���ھ� ��</param>
    void HandleHighScoreUpdated(float highScore)
    {
        if (highScore == 0)
        {
            // ���� ���ھ 0�̸� �ؽ�Ʈ ���
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