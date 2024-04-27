using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������� ���� �پ��� ó���� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    const float FrameFreezeDuration = 0.025f;           // ȭ�� ���� ����Ʈ 
    const float ShakeIntensity = 0.6f;                  // ȭ�� ���� ����Ʈ ����
    const float ShakeDuration = 0.175f;                 // ȭ�� ���� ����Ʈ �ð�
    const float WhiteFlashEffectDuration = 0.05f;       // ������� ��½�̴� ����Ʈ �ð�
    const float HealthIncreaseRateMultiplier = 2f;      // ���� ü���� �ð��� ������ ���� �����ϴ� ����
    const float DamageSoundDelay = 0.1f;                // ���� ������� �Ծ��� �� ����Ǵ� ���尡 �ٽ� ����� �� ���� ��ٷ��� �ϴ� �ð�
    const string DustEffectWhiteKey = "DustEffectWhite";// �Ͼ� ���� ����Ʈ ������Ʈ Ǯ�� Ű ��
    const string DustEffectKey = "DustEffect";          // ���� ����Ʈ ������Ʈ Ǯ�� Ű ��
    const int DustEffectCount = 3;                      // �����Ϸ��� ���� ����Ʈ ������Ʈ Ǯ�� ��
    const string CoinKey = "Coin";                      // ���� ������Ʈ Ǯ�� Ű ��

    [SerializeField] EnemyDataSO _enemyDataSO;              // �� ������ SO
    [SerializeField] SpriteRenderer _spriteRenderer;        // ��������Ʈ ������

    int _currentHealth;                     // ���� ü��
    float _healthIncreaseRatePerMinutes;    // 1�и��� �����ϴ� ü���� ��
    float _lastHitSoundPlayTime;            // ���������� ���尡 ����� �ð�
    bool _isWeekendFarm;                    // �ָ����忡 ������ üũ�ϴ� �÷��� ����

    Transform _transform;                   // Ʈ������ ĳ��
    Material _defaultMaterial;              // �⺻ ���׸���
    Coroutine _whiteFlashEffectCoroutine;   // ������� ��½�̴� ����Ʈ �ڷ�ƾ

    void Awake()
    {
        // Ʈ������ ĳ��
        _transform = GetComponent<Transform>();

        // ��������Ʈ �������� null���̸� ��������Ʈ �������� ã�� ĳ��
        if (_spriteRenderer == null)
        {
            var spriteObject = transform.Find("Sprite").gameObject;
            if(spriteObject != null)
            {
                if(spriteObject.TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    _spriteRenderer = spriteRenderer;
                }
                else
                {
                    Debug.LogError(name + "�� EnemyDamage Ŭ�������� ��������Ʈ �������� ã�� �� �����ϴ�!");
                }
            }
            else
            {
                Debug.LogError(name + "���� ��������Ʈ ������Ʈ�� �����!");
            }
        }

        // �⺻ ���׸����� ��������Ʈ �������� ���� ���׸���� ����
        _defaultMaterial = _spriteRenderer.material;
        // �� �� ü�� ������ ����
        _healthIncreaseRatePerMinutes = _enemyDataSO.defaultHealth * HealthIncreaseRateMultiplier;
    }

    void OnEnable()
    {
        // �ָ����忡 ���� ���� ���·� ����
        _isWeekendFarm = false;
        // ��������Ʈ �������� ���׸����� �⺻ ���׸���� ����
        _spriteRenderer.material = _defaultMaterial;
        // �� ������ ü�� ����
        int elapsedMinutes = Mathf.FloorToInt((GameManager.Instance.PlayTimeSeconds % 3600f) / 60f);
        _currentHealth = _enemyDataSO.defaultHealth + Mathf.FloorToInt(_healthIncreaseRatePerMinutes * elapsedMinutes);
        // ���������� ��Ʈ ���带 ����� �ð��� 0���� ����
        _lastHitSoundPlayTime = 0f;
    }

    /// <summary>
    /// ������ ������� ������ �޼����Դϴ�.
    /// </summary>
    /// <param name="damage">����� ��</param>
    public void TakeDamage(int damage)
    {
        // ���� �̹� �ָ� ���忡 �� ������ �� �� �ٽ� ������� ���� ���� ����
        if (_isWeekendFarm) return;

        // ���� ü���� �������ŭ ���� ��Ŵ
        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            DamageEffect();
        }
        else
        {
            // ü���� 0�����̸� �ָ����忡 �� ������ ó��
            WeekendFarmEffect();
            // ���� ����
            for (int i = 0; i < _enemyDataSO.coinDropAmount + BonusCoin.BonusCointCount; i++)
            {
                ObjectPoolManager.Instance.SpawnFromPool(CoinKey, _transform.position);
            }
            // ������Ʈ Ǯ ��ȯ
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }
    void DamageEffect()
    {
        // ��� ���� ����Ʈ ����
        ObjectPoolManager.Instance.SpawnFromPool(DustEffectWhiteKey, _transform.position);
        // ���� �޾��� �� ���� ���
        if (GameManager.Instance.PlayTimeSeconds >= _lastHitSoundPlayTime + DamageSoundDelay)
        {
            SoundManager.Instance.PlaySFX(_enemyDataSO.hitSound);
            _lastHitSoundPlayTime = GameManager.Instance.PlayTimeSeconds;
        }
        // ������� ��½�̴� ����Ʈ ����
        StartWhiteFlashEffect();
    }

    /// <summary>
    /// �ָ����忡 ���� ���� ó��
    /// </summary>
    void WeekendFarmEffect()
    {
        // �ָ� �������� �� ������ ó��
        _isWeekendFarm = true;
        // ������� ��½�̴� ����Ʈ �ߴ�
        StopWhiteFlashEffect();
        // ȭ�� ���� ����Ʈ ����
        GameEffectManager.Instance.StartFrameFreezeEffect(FrameFreezeDuration);
        // ȭ�� ���� ����Ʈ ����
        GameEffectManager.Instance.StartCameraShakeEffect(ShakeIntensity, ShakeDuration);
        // �ָ� ���忡 ���� �� ���� ���
        SoundManager.Instance.PlaySFX(_enemyDataSO.weekendFarmSound);
        // ���� ����Ʈ ����
        for (int i = 0; i < DustEffectCount; i++)
        {
            ObjectPoolManager.Instance.SpawnFromPool(DustEffectKey, _transform.position);
        }
    }

    /// <summary>
    /// ���� ���ݹ޾��� �� �Ͼ�� ��½�̴� ����Ʈ�� �����ϴ� �޼����Դϴ�.
    /// </summary>
    void StartWhiteFlashEffect()
    {
        // ���׸����� ������ �������� ����
        if (_enemyDataSO.whiteFlashEffectMaterial == null) return;

        StopWhiteFlashEffect();
        _whiteFlashEffectCoroutine = StartCoroutine(WhiteFlashEffectCoroutine());
    }

    /// <summary>
    /// ���� �Ͼ�� ��½�̴� ����Ʈ�� ó���ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    IEnumerator WhiteFlashEffectCoroutine()
    {
        // ���׸����� ���� ��, ���� �ð� �� �ߴ�
        _spriteRenderer.material = _enemyDataSO.whiteFlashEffectMaterial;
        yield return YieldInstructionCache.WaitForSeconds(WhiteFlashEffectDuration);
        StopWhiteFlashEffect();
    }

    /// <summary>
    /// ���� ������� ��½�̴� ����Ʈ�� �ߴ��ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    void StopWhiteFlashEffect()
    {
        if (_whiteFlashEffectCoroutine != null)
        {
            StopCoroutine(_whiteFlashEffectCoroutine);
            _whiteFlashEffectCoroutine = null;
            _spriteRenderer.material = _defaultMaterial;
        }
    }
}
