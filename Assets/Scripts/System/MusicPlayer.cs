using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip _music;  // ���� ����
    void Start()
    {
        // �̺�Ʈ ����
        // ������ ���۵Ǹ� ������ �����ϸ�, ���� ������ �Ǹ� ������ �ߴ�
        GameManager.Instance.OnGamePlayStarted += () => SoundManager.Instance.PlayBGM(_music);
        GameManager.Instance.OnGameOver += () => SoundManager.Instance.PuaseBGM();
    }
}
