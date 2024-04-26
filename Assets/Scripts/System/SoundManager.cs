using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내의 사운드 재생을 관리하는 싱글톤 클래스입니다.
/// </summary>
public class SoundManager : SingletonManager<SoundManager>
{
    static AudioSource _bgmAudioSource = new AudioSource(); // 음악 재생 오디오 소스
    static AudioSource _sfxAudioSource = new AudioSource(); // 효과음 재생 오디오 소스

    /// <summary>
    /// 사운드 매니저 초기화
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // BGM 오디오 소스를 설정
        GameObject newBGMAudioSource = new GameObject { name = "BGM" };
        newBGMAudioSource.transform.parent = transform;
        _bgmAudioSource = newBGMAudioSource.AddComponent<AudioSource>();
        _bgmAudioSource.loop = true;

        // SFX 오디오 소스를 설정
        GameObject newSFXAudioSource = new GameObject { name = "SFX" };
        newSFXAudioSource.transform.parent = transform;
        _sfxAudioSource = newSFXAudioSource.AddComponent<AudioSource>();

        // 3D 환경 음향 제거
        _sfxAudioSource.dopplerLevel = _bgmAudioSource.dopplerLevel = 0f;
        _sfxAudioSource.reverbZoneMix = _bgmAudioSource.reverbZoneMix = 0f;
    }

    /// <summary>
    /// 효과음을 재생하는 메서드입니다.
    /// </summary>
    /// <param name="sound">효과음 AudioClip</param>
    public void PlaySFX(AudioClip sound)
    {
        // 매개 변수가 null이면 중단
        if (sound == null) return;

        _sfxAudioSource.PlayOneShot(sound);
    }

    /// <summary>
    /// 배경 음악을 재생하는 메서드입니다.
    /// </summary>
    /// <param name="sound">배경 음악 AudioClip</param>
    public void PlayBGM(AudioClip sound)
    {
        // 매개 변수가 null이면 중단
        if (sound == null) return;

        _bgmAudioSource.Stop();
        _bgmAudioSource.clip = sound;
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// 음악 재생을 정지하는 메서드입니다.
    /// </summary>
    public void PuaseBGM()
    {
        _bgmAudioSource.Pause();
    }
}