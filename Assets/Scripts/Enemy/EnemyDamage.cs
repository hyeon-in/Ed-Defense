using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 대미지에 대한 다양한 처리를 실행하는 클래스입니다.
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    const float FrameFreezeDuration = 0.025f;           // 화면 정지 이펙트 
    const float ShakeIntensity = 0.6f;                  // 화면 흔들기 이펙트 강도
    const float ShakeDuration = 0.175f;                 // 화면 흔들기 이펙트 시간
    const float WhiteFlashEffectDuration = 0.05f;       // 흰색으로 번쩍이는 이펙트 시간
    const float HealthIncreaseRateMultiplier = 2f;      // 적의 체력이 시간이 지남에 따라 증가하는 비율
    const float DamageSoundDelay = 0.1f;                // 적이 대미지를 입었을 때 재생되는 사운드가 다시 재생될 때 까지 기다려야 하는 시간
    const string DustEffectWhiteKey = "DustEffectWhite";// 하얀 먼지 이펙트 오브젝트 풀의 키 값
    const string DustEffectKey = "DustEffect";          // 먼지 이펙트 오브젝트 풀의 키 값
    const int DustEffectCount = 3;                      // 생성하려는 먼지 이펙트 오브젝트 풀의 수
    const string CoinKey = "Coin";                      // 코인 오브젝트 풀의 키 값

    [SerializeField] EnemyDataSO _enemyDataSO;              // 적 데이터 SO
    [SerializeField] SpriteRenderer _spriteRenderer;        // 스프라이트 렌더러

    int _currentHealth;                     // 현재 체력
    float _healthIncreaseRatePerMinutes;    // 1분마다 증가하는 체력의 량
    float _lastHitSoundPlayTime;            // 마지막으로 사운드가 재생된 시간
    bool _isWeekendFarm;                    // 주말농장에 갔는지 체크하는 플래그 변수

    Transform _transform;                   // 트랜스폼 캐시
    Material _defaultMaterial;              // 기본 머테리얼
    Coroutine _whiteFlashEffectCoroutine;   // 흰색으로 번쩍이는 이펙트 코루틴

    void Awake()
    {
        // 트랜스폼 캐시
        _transform = GetComponent<Transform>();

        // 스프라이트 렌더러가 null값이면 스프라이트 렌더러를 찾아 캐시
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
                    Debug.LogError(name + "의 EnemyDamage 클래스에서 스프라이트 렌더러를 찾을 수 없습니다!");
                }
            }
            else
            {
                Debug.LogError(name + "에게 스프라이트 오브젝트가 없어요!");
            }
        }

        // 기본 머테리얼을 스프라이트 렌더러의 현재 머테리얼로 설정
        _defaultMaterial = _spriteRenderer.material;
        // 분 당 체력 증가량 설정
        _healthIncreaseRatePerMinutes = _enemyDataSO.defaultHealth * HealthIncreaseRateMultiplier;
    }

    void OnEnable()
    {
        // 주말농장에 가지 않은 상태로 설정
        _isWeekendFarm = false;
        // 스프라이트 렌더러의 머테리얼을 기본 머테리얼로 설정
        _spriteRenderer.material = _defaultMaterial;
        // 분 단위로 체력 증가
        int elapsedMinutes = Mathf.FloorToInt((GameManager.Instance.PlayTimeSeconds % 3600f) / 60f);
        _currentHealth = _enemyDataSO.defaultHealth + Mathf.FloorToInt(_healthIncreaseRatePerMinutes * elapsedMinutes);
        // 마지막으로 히트 사운드를 재생한 시간을 0으로 설정
        _lastHitSoundPlayTime = 0f;
    }

    /// <summary>
    /// 적에게 대미지를 입히는 메서드입니다.
    /// </summary>
    /// <param name="damage">대미지 값</param>
    public void TakeDamage(int damage)
    {
        // 적이 이미 주말 농장에 간 상태일 때 또 다시 대미지가 들어가는 것을 방지
        if (_isWeekendFarm) return;

        // 현재 체력을 대미지만큼 감소 시킴
        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            DamageEffect();
        }
        else
        {
            // 체력이 0이하이면 주말농장에 간 것으로 처리
            WeekendFarmEffect();
            // 코인 생성
            for (int i = 0; i < _enemyDataSO.coinDropAmount + BonusCoin.BonusCointCount; i++)
            {
                ObjectPoolManager.Instance.SpawnFromPool(CoinKey, _transform.position);
            }
            // 오브젝트 풀 반환
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }
    void DamageEffect()
    {
        // 흰색 먼지 이펙트 생성
        ObjectPoolManager.Instance.SpawnFromPool(DustEffectWhiteKey, _transform.position);
        // 공격 받았을 때 사운드 재생
        if (GameManager.Instance.PlayTimeSeconds >= _lastHitSoundPlayTime + DamageSoundDelay)
        {
            SoundManager.Instance.PlaySFX(_enemyDataSO.hitSound);
            _lastHitSoundPlayTime = GameManager.Instance.PlayTimeSeconds;
        }
        // 흰색으로 번쩍이는 이펙트 실행
        StartWhiteFlashEffect();
    }

    /// <summary>
    /// 주말농장에 갔을 때를 처리
    /// </summary>
    void WeekendFarmEffect()
    {
        // 주말 농장으로 간 것으로 처리
        _isWeekendFarm = true;
        // 흰색으로 번쩍이는 이펙트 중단
        StopWhiteFlashEffect();
        // 화면 정지 이펙트 실행
        GameEffectManager.Instance.StartFrameFreezeEffect(FrameFreezeDuration);
        // 화면 흔들기 이펙트 실행
        GameEffectManager.Instance.StartCameraShakeEffect(ShakeIntensity, ShakeDuration);
        // 주말 농장에 갔을 때 사운드 재생
        SoundManager.Instance.PlaySFX(_enemyDataSO.weekendFarmSound);
        // 먼지 이펙트 생성
        for (int i = 0; i < DustEffectCount; i++)
        {
            ObjectPoolManager.Instance.SpawnFromPool(DustEffectKey, _transform.position);
        }
    }

    /// <summary>
    /// 적이 공격받았을 때 하얗게 번쩍이는 이펙트를 실행하는 메서드입니다.
    /// </summary>
    void StartWhiteFlashEffect()
    {
        // 머테리얼이 없으면 실행하지 않음
        if (_enemyDataSO.whiteFlashEffectMaterial == null) return;

        StopWhiteFlashEffect();
        _whiteFlashEffectCoroutine = StartCoroutine(WhiteFlashEffectCoroutine());
    }

    /// <summary>
    /// 적이 하얗게 번쩍이는 이펙트를 처리하는 코루틴입니다.
    /// </summary>
    IEnumerator WhiteFlashEffectCoroutine()
    {
        // 머테리얼을 변경 후, 일정 시간 후 중단
        _spriteRenderer.material = _enemyDataSO.whiteFlashEffectMaterial;
        yield return YieldInstructionCache.WaitForSeconds(WhiteFlashEffectDuration);
        StopWhiteFlashEffect();
    }

    /// <summary>
    /// 적이 흰색으로 번쩍이는 이펙트를 중단하는 코루틴입니다.
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
