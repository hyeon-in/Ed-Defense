using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 대미지를 처리하는 클래스입니다.
/// </summary>
public class PlayerDamage : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 현재 체력이 변경되면 호출되는 이벤트입니다.
    /// </summary>
    public Action<int> OnCurrentHealthChanged;

    const float FrameFreezeDuration = 0.2f;           // 화면 정지 이펙트 시간
    const float WhiteFlashEffectDuration = 0.05f;     // 흰색으로 번쩍이는 이펙트 시간
    const float ShakeIntensity = 0.7f;                // 화면 흔들기 이펙트 강도
    const float ShakeDuration = 0.2f;                 // 화면 흔들기 이펙트 시간

    [SerializeField] int _defaultHealth = 5;                // 플레이어 기본 체력
    [SerializeField] SpriteRenderer _edSpriteRenderer;      // 이드 스프라이트 렌더러
    [SerializeField] Material _whiteFlashEffectMaterial;    // 흰색으로 번쩍이는 이펙트를 처리하는 머테리얼
    [SerializeField] AudioClip _damageSound;                // 공격 받을 때 사운드
    [SerializeField] float _damageRange = 1.5f;             // 공격 받는 범위
    [SerializeField] LayerMask _enemyLayer;                 // 적 레이어

    int _currentHealth;                     // 현재 체력
    bool _isWeekendFarm;                    // 주말농장에 간 상태인지 체크
    Transform _transform;                   // 트랜스폼 캐시
    Material _defaultMaterial;              // 기본 머테리얼
    Coroutine _whiteFlashEffectCoroutine;   // 흰색으로 번쩍이는 이펙트 코루틴

    /// <summary>
    /// 현재 체력에 대한 프로퍼티입니다.
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
                    // 체력이 0 이하로 내려가지 않게 조정
                    _currentHealth = 0;
                }
                // 이벤트 호출
                OnCurrentHealthChanged?.Invoke(value);
            }
        }
    }

    void Start()
    {
        // 트랜스폼 캐시
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
                    Debug.LogError("PlayerDamage 클래스에서 이드의 스프라이트 렌더러를 찾을 수 없습니다!");
                }
            }
            else
            {
                Debug.LogError("플레이어에게 이드의 스프라이트 오브젝트가 없어요!");
            }
        }
        // 이드의 초기 머테리얼을 기본 머테리얼로 설정
        _defaultMaterial = _edSpriteRenderer.material;
        // 게임이 시작될 때 호출되는 메서드를 연결
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
    }

    void Update()
    {
        // 이미 주말 농장에 간 상태이면 처리하지 않음
        if (_isWeekendFarm) return;

        // 적이 충돌 범위에 들어서면 충돌 처리를 진행합니다.
        var hit = Physics2D.OverlapCircle(_transform.position, _damageRange, _enemyLayer);
        if(hit)
        {
            // 대미지 처리
            HandleDamage();
            // 화면 정지 이펙트 실행
            GameEffectManager.Instance.StartFrameFreezeEffect(FrameFreezeDuration);
            // 화면 흔들기 이펙트 실행
            GameEffectManager.Instance.StartCameraShakeEffect(ShakeIntensity, ShakeDuration);
            // 효과음 재생
            SoundManager.Instance.PlaySFX(_damageSound);
            // 흰색으로 번쩍이는 이펙트 실행
            StartWhiteFlashEffect();
            // 충돌한 적을 오브젝트 풀 매니저에 돌려놓음
            ObjectPoolManager.Instance.ReturnToPool(hit.gameObject);
        }
    }

    /// <summary>
    /// 플레이어의 대미지를 처리하는 메서드입니다.
    /// </summary>
    void HandleDamage()
    {
        CurrentHealth--;
        if (CurrentHealth <= 0)
        {
            // 체력이 0 이하로 떨어지면 게임오버 상태로 변경
            _isWeekendFarm = true;
            GameManager.Instance.CurrentState = GameManager.GameState.GameOver;
        }
    }

    /// <summary>
    /// 적이 공격받았을 때 하얗게 번쩍이는 이펙트를 실행하는 메서드입니다.
    /// </summary>
    void StartWhiteFlashEffect()
    {
        StopWhiteFlashEffect();
        _whiteFlashEffectCoroutine = StartCoroutine(WhiteFlashEffectCoroutine());
    }

    /// <summary>
    /// 적이 하얗게 번쩍이는 이펙트를 처리하는 코루틴입니다.
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
    /// 게임이 시작될 때 호출되는 메서드입니다.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // 초기화
        _isWeekendFarm = false;
        CurrentHealth = _defaultHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageRange);
    }
}