using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ���� ����� �����ϴ� �̱��� Ŭ�����Դϴ�.
/// </summary>
public class SoundManager : SingletonManager<SoundManager>
{
    static AudioSource _bgmAudioSource = new AudioSource(); // ���� ��� ����� �ҽ�
    static AudioSource _sfxAudioSource = new AudioSource(); // ȿ���� ��� ����� �ҽ�

    /// <summary>
    /// ���� �Ŵ��� �ʱ�ȭ
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // BGM ����� �ҽ��� ����
        GameObject newBGMAudioSource = new GameObject { name = "BGM" };
        newBGMAudioSource.transform.parent = transform;
        _bgmAudioSource = newBGMAudioSource.AddComponent<AudioSource>();
        _bgmAudioSource.loop = true;

        // SFX ����� �ҽ��� ����
        GameObject newSFXAudioSource = new GameObject { name = "SFX" };
        newSFXAudioSource.transform.parent = transform;
        _sfxAudioSource = newSFXAudioSource.AddComponent<AudioSource>();

        // 3D ȯ�� ���� ����
        _sfxAudioSource.dopplerLevel = _bgmAudioSource.dopplerLevel = 0f;
        _sfxAudioSource.reverbZoneMix = _bgmAudioSource.reverbZoneMix = 0f;
    }

    /// <summary>
    /// ȿ������ ����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="sound">ȿ���� AudioClip</param>
    public void PlaySFX(AudioClip sound)
    {
        // �Ű� ������ null�̸� �ߴ�
        if (sound == null) return;

        _sfxAudioSource.PlayOneShot(sound);
    }

    /// <summary>
    /// ��� ������ ����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="sound">��� ���� AudioClip</param>
    public void PlayBGM(AudioClip sound)
    {
        // �Ű� ������ null�̸� �ߴ�
        if (sound == null) return;

        _bgmAudioSource.Stop();
        _bgmAudioSource.clip = sound;
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// ���� ����� �����ϴ� �޼����Դϴ�.
    /// </summary>
    public void PuaseBGM()
    {
        _bgmAudioSource.Pause();
    }
}