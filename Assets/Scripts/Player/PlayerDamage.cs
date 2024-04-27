using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� ������� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class PlayerDamage : MonoBehaviour
{
    /// <summary>
    /// �÷��̾��� ���� ü���� ����Ǹ� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    public Action<int> OnCurrentHealthChanged;

    const float FrameFreezeDuration = 0.2f;           // ȭ�� ���� ����Ʈ �ð�
    const float WhiteFlashEffectDuration = 0.05f;     // ������� ��½�̴� ����Ʈ �ð�
    const float ShakeIntensity = 0.7f;                // ȭ�� ���� ����Ʈ ����
    const float ShakeDuration = 0.2f;                 // ȭ�� ���� ����Ʈ �ð�

    [SerializeField] int _defaultHealth = 5;                // �÷��̾� �⺻ ü��
    [SerializeField] SpriteRenderer _edSpriteRenderer;      // �̵� ��������Ʈ ������
    [SerializeField] Material _whiteFlashEffectMaterial;    // ������� ��½�̴� ����Ʈ�� ó���ϴ� ���׸���
    [SerializeField] AudioClip _damageSound;                // ���� ���� �� ����
    [SerializeField] float _damageRange = 1.5f;             // ���� �޴� ����
    [SerializeField] LayerMask _enemyLayer;                 // �� ���̾�

    int _currentHealth;                     // ���� ü��
    bool _isWeekendFarm;                    // �ָ����忡 �� �������� üũ
    Transform _transform;                   // Ʈ������ ĳ��
    Material _defaultMaterial;              // �⺻ ���׸���
    Coroutine _whiteFlashEffectCoroutine;   // ������� ��½�̴� ����Ʈ �ڷ�ƾ

    /// <summary>
    /// ���� ü�¿� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            if(_currentHealth != value)
            {
                _currentHealth = value;
                if(_currentHealth < 0)
                {
                    // ü���� 0 ���Ϸ� �������� �ʰ� ����
                    _currentHealth = 0;
                }
                // �̺�Ʈ ȣ��
                OnCurrentHealthChanged?.Invoke(value);
            }
        }
    }

    void Start()
    {
        // Ʈ������ ĳ��
        _transform = GetComponent<Transform>();

        if (_edSpriteRenderer == null)
        {
            var edSpriteObject = transform.Find("Ed").gameObject;
            if (edSpriteObject != null)
            {
                if (edSpriteObject.TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    _edSpriteRenderer = spriteRenderer;
                }
                else
                {
                    Debug.LogError("PlayerDamage Ŭ�������� �̵��� ��������Ʈ �������� ã�� �� �����ϴ�!");
                }
            }
            else
            {
                Debug.LogError("�÷��̾�� �̵��� ��������Ʈ ������Ʈ�� �����!");
            }
        }
        // �̵��� �ʱ� ���׸����� �⺻ ���׸���� ����
        _defaultMaterial = _edSpriteRenderer.material;
        // ������ ���۵� �� ȣ��Ǵ� �޼��带 ����
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
    }

    void Update()
    {
        // �̹� �ָ� ���忡 �� �����̸� ó������ ����
        if (_isWeekendFarm) return;

        // ���� �浹 ������ ���� �浹 ó���� �����մϴ�.
        var hit = Physics2D.OverlapCircle(_transform.position, _damageRange, _enemyLayer);
        if(hit)
        {
            // ����� ó��
            HandleDamage();
            // ȭ�� ���� ����Ʈ ����
            GameEffectManager.Instance.StartFrameFreezeEffect(FrameFreezeDuration);
            // ȭ�� ���� ����Ʈ ����
            GameEffectManager.Instance.StartCameraShakeEffect(ShakeIntensity, ShakeDuration);
            // ȿ���� ���
            SoundManager.Instance.PlaySFX(_damageSound);
            // ������� ��½�̴� ����Ʈ ����
            StartWhiteFlashEffect();
            // �浹�� ���� ������Ʈ Ǯ �Ŵ����� ��������
            ObjectPoolManager.Instance.ReturnToPool(hit.gameObject);
        }
    }

    /// <summary>
    /// �÷��̾��� ������� ó���ϴ� �޼����Դϴ�.
    /// </summary>
    void HandleDamage()
    {
        CurrentHealth--;
        if (CurrentHealth <= 0)
        {
            // ü���� 0 ���Ϸ� �������� ���ӿ��� ���·� ����
            _isWeekendFarm = true;
            GameManager.Instance.CurrentState = GameManager.GameState.GameOver;
        }
    }

    /// <summary>
    /// ���� ���ݹ޾��� �� �Ͼ�� ��½�̴� ����Ʈ�� �����ϴ� �޼����Դϴ�.
    /// </summary>
    void StartWhiteFlashEffect()
    {
        StopWhiteFlashEffect();
        _whiteFlashEffectCoroutine = StartCoroutine(WhiteFlashEffectCoroutine());
    }

    /// <summary>
    /// ���� �Ͼ�� ��½�̴� ����Ʈ�� ó���ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    IEnumerator WhiteFlashEffectCoroutine()
    {
        _edSpriteRenderer.material = _whiteFlashEffectMaterial;
        yield return YieldInstructionCache.WaitForSeconds(WhiteFlashEffectDuration);
        StopWhiteFlashEffect();

    }

    void StopWhiteFlashEffect()
    {
        if (_whiteFlashEffectCoroutine != null)
        {
            StopCoroutine(_whiteFlashEffectCoroutine);
            _whiteFlashEffectCoroutine = null;
            _edSpriteRenderer.material = _defaultMaterial;
        }
    }

    /// <summary>
    /// ������ ���۵� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // �ʱ�ȭ
        _isWeekendFarm = false;
        CurrentHealth = _defaultHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageRange);
    }
}