using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// ���� ����Ʈ�� �����ϴ� �̱��� Ŭ�����Դϴ�.
/// </summary>
public class GameEffectManager : SingletonManager<GameEffectManager>
{
    Vector3 _cameraOrigin;                  // ī�޶� ����
    Transform _cameraTransform;             // ī�޶� Ʈ������
    PixelPerfectCamera _pixelPerfectCamera; // �ȼ� ����Ʈ ī�޶�

    Coroutine _cameraShakeEffectCoroutine = null;   // ī�޶� ��鸮�� ����Ʈ�� ó���ϱ� ���� �ڷ�ƾ ��ü
    Coroutine _frameFreezeEffectCoroutine = null;   // �������� ���ߴ� ����Ʈ�� ó���ϱ� ���� �ڷ�ƾ ��ü

    void Start()
    {
        // ī�޶� Ʈ������ �� �ȼ� ����Ʈ ī�޶� ĳ��
        _cameraTransform = Camera.main.GetComponent<Transform>();
        _pixelPerfectCamera = _cameraTransform.GetComponent<PixelPerfectCamera>();
        // ī�޶� ���� ��ǥ�� ���� ī�޶� ��ǥ�� ����
        _cameraOrigin = _cameraTransform.position;
    }

    /// <summary>
    /// ī�޶� ���� ����Ʈ�� �����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="shakeIntensity">ī�޶� ��鸮�� ����</param>
    /// <param name="duration">ī�޶� ��鸮�� �ð�</param>
    public void StartCameraShakeEffect(float shakeIntensity, float duration)
    {
        // ���� ����Ʈ �ߴ�
        StopCameraShake();
        // ī�޶� ���� ����Ʈ �ڷ�ƾ ����
        _cameraShakeEffectCoroutine = StartCoroutine(CameraShakeEffectCoroutine(shakeIntensity, duration));
    }

    /// <summary>
    /// ī�޶� ���� ����Ʈ�� ó���ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    /// <param name="shakeIntensity">ī�޶� ��鸮�� ����</param>
    /// <param name="duration">ī�޶� ��鸮�� �ð�</param>
    IEnumerator CameraShakeEffectCoroutine(float shakeIntensity, float duration)
    {
        // ����Ʈ ��� �ð�
        float elapsedTime = 0f;

        // ����Ʈ�� ���� �ð����� ī�޶� ���� ����Ʈ ����
        while (elapsedTime < duration)
        {
            // ī�޶� ���� ������ �ð��� �������� �پ��
            float currentIntensity = shakeIntensity * (1f - elapsedTime / duration);

            // ī�޶� ��ǥ�� ������ ���� ������ ����
            float x = Random.Range(-currentIntensity, currentIntensity);
            float y = Random.Range(-currentIntensity, currentIntensity);
            Vector3 newPosition = _cameraOrigin + new Vector3(x, y, 0);
            _cameraTransform.position = _pixelPerfectCamera.RoundToPixel(newPosition);  // ī�޶� ��ǥ�� �ȼ��� ���� ����

            // 1������ ��� �� ��� �ð� ����
            yield return null;
            elapsedTime += Time.deltaTime;
            
            while(GameManager.Instance.CurrentState == GameManager.GameState.Upgrade)
            {
                // ���׷��̵� ȭ���� ����� �����̸� ���
                yield return null;
            }
        }

        // ī�޶� ���� ����Ʈ �ߴ�
        StopCameraShake();
    }

    /// <summary>
    /// ī�޶� ���� ����Ʈ�� �ߴ��ϴ� �޼����Դϴ�.
    /// </summary>
    public void StopCameraShake()
    {
        if (_cameraShakeEffectCoroutine != null)
        {
            // �ڷ�ƾ �ߴ�
            StopCoroutine(_cameraShakeEffectCoroutine);
            _cameraShakeEffectCoroutine = null;
            // ī�޶� ��ǥ�� �������� ����
            _cameraTransform.position = _cameraOrigin;
        }
    }
    
    /// <summary>
    /// �ð��� ���������� ���ߴ� ����Ʈ�� �����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="duration">����Ʈ ���� �ð�</param>
    public void StartFrameFreezeEffect(float duration)
    {
        // ���� ����Ʈ �ߴ�
        StopFrameFreezeEffect();
        // �ð��� ���ߴ� ����Ʈ �ڷ�ƾ ����
        _frameFreezeEffectCoroutine = StartCoroutine(FrameFreezeEffectCoroutine(duration));
    }

    /// <summary>
    /// �ð��� ���������� ���ߴ� ����Ʈ�� ó���ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    /// <param name="duration">����Ʈ ���� �ð�</param>
    IEnumerator FrameFreezeEffectCoroutine(float duration)
    {
        // timeScale�� 0���� ������ �� ���� �ð��� ������ ����Ʈ �ߴ�
        Time.timeScale = 0f;
        yield return YieldInstructionCache.WaitForSecondsRealtime(duration);
        StopFrameFreezeEffect();
    }

    /// <summary>
    /// �ð��� ���������� ���ߴ� ����Ʈ�� �ߴ��ϴ� �޼����Դϴ�.
    /// </summary>
    public void StopFrameFreezeEffect()
    {
        if (_frameFreezeEffectCoroutine != null)
        {
            // �ڷ�ƾ �ߴ�
            StopCoroutine(_frameFreezeEffectCoroutine);
            _frameFreezeEffectCoroutine = null;
            // ���� ���°� ���׷��̵带 �ϰ� �ִ� ���°� �ƴ϶�� timeScale�� 1�� �ǵ��� 
            if (GameManager.Instance.CurrentState != GameManager.GameState.Upgrade)
            {
                Time.timeScale = 1f;
            }
        }
    }
}
