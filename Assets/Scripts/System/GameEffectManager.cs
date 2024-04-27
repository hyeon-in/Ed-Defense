using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 게임 이펙트를 관리하는 싱글톤 클래스입니다.
/// </summary>
public class GameEffectManager : SingletonManager<GameEffectManager>
{
    Vector3 _cameraOrigin;                  // 카메라 원점
    Transform _cameraTransform;             // 카메라 트랜스폼
    PixelPerfectCamera _pixelPerfectCamera; // 픽셀 퍼펙트 카메라

    Coroutine _cameraShakeEffectCoroutine = null;   // 카메라가 흔들리는 이펙트를 처리하기 위한 코루틴 객체
    Coroutine _frameFreezeEffectCoroutine = null;   // 프레임이 멈추는 이펙트를 처리하기 위한 코루틴 객체

    void Start()
    {
        // 카메라 트랜스폼 및 픽셀 퍼펙트 카메라 캐시
        _cameraTransform = Camera.main.GetComponent<Transform>();
        _pixelPerfectCamera = _cameraTransform.GetComponent<PixelPerfectCamera>();
        // 카메라 원점 좌표를 현재 카메라 좌표로 설정
        _cameraOrigin = _cameraTransform.position;
    }

    /// <summary>
    /// 카메라를 흔드는 이펙트를 실행하는 메서드입니다.
    /// </summary>
    /// <param name="shakeIntensity">카메라가 흔들리는 강도</param>
    /// <param name="duration">카메라가 흔들리는 시간</param>
    public void StartCameraShakeEffect(float shakeIntensity, float duration)
    {
        // 기존 이펙트 중단
        StopCameraShake();
        // 카메라를 흔드는 이펙트 코루틴 실행
        _cameraShakeEffectCoroutine = StartCoroutine(CameraShakeEffectCoroutine(shakeIntensity, duration));
    }

    /// <summary>
    /// 카메라를 흔드는 이펙트를 처리하는 코루틴입니다.
    /// </summary>
    /// <param name="shakeIntensity">카메라가 흔들리는 강도</param>
    /// <param name="duration">카메라가 흔들리는 시간</param>
    IEnumerator CameraShakeEffectCoroutine(float shakeIntensity, float duration)
    {
        // 이펙트 경과 시간
        float elapsedTime = 0f;

        // 이펙트의 지속 시간까지 카메라를 흔드는 이펙트 실행
        while (elapsedTime < duration)
        {
            // 카메라를 흔드는 강도는 시간이 지날수록 줄어듬
            float currentIntensity = shakeIntensity * (1f - elapsedTime / duration);

            // 카메라 좌표를 강도에 따라 무작위 설정
            float x = Random.Range(-currentIntensity, currentIntensity);
            float y = Random.Range(-currentIntensity, currentIntensity);
            Vector3 newPosition = _cameraOrigin + new Vector3(x, y, 0);
            _cameraTransform.position = _pixelPerfectCamera.RoundToPixel(newPosition);  // 카메라 좌표를 픽셀에 맞춰 조정

            // 1프레임 대기 및 경과 시간 증가
            yield return null;
            elapsedTime += Time.deltaTime;
            
            while(GameManager.Instance.CurrentState == GameManager.GameState.Upgrade)
            {
                // 업그레이드 화면이 실행된 상태이면 대기
                yield return null;
            }
        }

        // 카메라를 흔드는 이펙트 중단
        StopCameraShake();
    }

    /// <summary>
    /// 카메라를 흔드는 이펙트를 중단하는 메서드입니다.
    /// </summary>
    public void StopCameraShake()
    {
        if (_cameraShakeEffectCoroutine != null)
        {
            // 코루틴 중단
            StopCoroutine(_cameraShakeEffectCoroutine);
            _cameraShakeEffectCoroutine = null;
            // 카메라 좌표를 원점으로 설정
            _cameraTransform.position = _cameraOrigin;
        }
    }
    
    /// <summary>
    /// 시간이 순간적으로 멈추는 이펙트를 실행하는 메서드입니다.
    /// </summary>
    /// <param name="duration">이펙트 지속 시간</param>
    public void StartFrameFreezeEffect(float duration)
    {
        // 기존 이펙트 중단
        StopFrameFreezeEffect();
        // 시간이 멈추는 이펙트 코루틴 실행
        _frameFreezeEffectCoroutine = StartCoroutine(FrameFreezeEffectCoroutine(duration));
    }

    /// <summary>
    /// 시간이 순간적으로 멈추는 이펙트를 처리하는 코루틴입니다.
    /// </summary>
    /// <param name="duration">이펙트 지속 시간</param>
    IEnumerator FrameFreezeEffectCoroutine(float duration)
    {
        // timeScale을 0으로 설정한 뒤 지속 시간이 지나면 이펙트 중단
        Time.timeScale = 0f;
        yield return YieldInstructionCache.WaitForSecondsRealtime(duration);
        StopFrameFreezeEffect();
    }

    /// <summary>
    /// 시간이 순간적으로 멈추는 이펙트를 중단하는 메서드입니다.
    /// </summary>
    public void StopFrameFreezeEffect()
    {
        if (_frameFreezeEffectCoroutine != null)
        {
            // 코루틴 중단
            StopCoroutine(_frameFreezeEffectCoroutine);
            _frameFreezeEffectCoroutine = null;
            // 현재 상태가 업그레이드를 하고 있는 상태가 아니라면 timeScale을 1로 되돌림 
            if (GameManager.Instance.CurrentState != GameManager.GameState.Upgrade)
            {
                Time.timeScale = 1f;
            }
        }
    }
}
