using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 플레이어의 제어를 위한 클래스입니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 현재 레이저 배터리, 혹은 최대 레이저 배터리가 업데이트되면 호출되는 이벤트입니다.
    /// </summary>
    public Action<int, int> OnLaserBatteryUpdated;

    // 상수
    const float LLELRotationSpeed = 25f;                    // 나타의 회전 속도(1 / 0.04와 동일)
    const float LaserAttackDistance = 16f;                  // 레이저 공격 거리
    const float BatteryChargeDelay = 0.02f;                 // 배터리 차지 딜레이
    const float MobileInputDeadZone = 20f;                  // 모바일 입력 데드존
    const float MobileAutoTargetingRange = 0.5f;            // 모바일 자동 조준 범위
    const float MinimumAimCorrectionDistanceSquared = 1.5f; // 모바일에서 조준 보정시 적과 너무 가까우면 조준되지 않게 하기 위한 상수

    // SerializeField
    [Header("나타")]
    [SerializeField] Transform _llelTransform;      // 나타의 트랜스폼
    [SerializeField] float _llelDistance = 2f;      // 나타의 플레이어와의 거리
    [Header("레이저")]
    [SerializeField] int _defaultLaserDamage = 1;                         // 기본 레이저 대미지
    [SerializeField] float _laserAttackDelay = 0.02f;                     // 레이저 대미지 간격
    [SerializeField] int _defaultMaxLaserBattery = 100;                   // 기본 최대 레이저 배터리 
    [SerializeField] int _defaultBatteryCharge = 1;                       // 레이저 배터리 회복량
    [SerializeField] Transform _laserTransform;                           // 레이저 트랜스폼
    [SerializeField] SpriteRenderer _laserMuzzleEffectSpriteRenderer;     // 레이저가 시작되는 이펙트의 스프라이트 렌더러
    [SerializeField] Transform _laserHitEffectTransform;                  // 레이저 적중 이펙트의 트랜스폼
    [SerializeField] LayerMask _enemyLayer;                               // 공격하려는 적의 레이어

    // private
    int _currentLaserBattery;           // 현재 레이저 배터리
    int _maxLaserBattery;               // 최대 레이저 배터리
    float _deltaTime;                   // 델타 타임 캐시
    float _lastLaserAttackTime;         // 마지막으로 레이저가 공격한 시간
    float _lastBatteryChargeTime = 0f;  // 마지막으로 배터리가 충전된 시간

    Vector2 _llelDirection;                 // 나타 방향
    Vector2 _aimDirection;                  // 조준 방향

    Vector2 _startInputPosition;            // 입력이 시작된 좌표
    Vector2 _currentInputPosition;          // 현재 입력 좌표

    Transform _transform;                   // 플레이어의 트랜스폼 캐시
    Camera _camera;                         // 카메라 캐시
    PixelPerfectCamera _pixelPerfectCamera; // 카메라의 픽셀 퍼펙트 카메라
    AudioSource _laserSoundPlayer;          // 레이저 사운드를 재생하기 위한 오디오 소스

    // EnemyDamage 클래스를 보관하는 딕셔너리
    Dictionary<GameObject, EnemyDamage> _enemyDamageCache = new Dictionary<GameObject, EnemyDamage>();

    /// <summary>
    /// 현재 레이저 대미지에 대한 프로퍼티입니다.
    /// </summary>
    public int CurrentLaserDamage { get; set; }

    /// <summary>
    /// 배터리의 충전 량에 대한 프로퍼티입니다.
    /// </summary>
    public int BatteryCharge { get; set; }

    /// <summary>
    /// 레이저가 적을 최대한 관통할 수 있는 수에 대한 프로퍼티입니다.
    /// </summary>
    public int PenetrationCount { get; set; }

    /// <summary>
    /// 공격 중인지 체크하기 위한 프로퍼티입니다.
    /// </summary>
    public bool IsAttacking { get; private set; }

    /// <summary>
    /// 현재 레이저 배터리에 대한 프로퍼티입니다.
    /// </summary>
    public int CurrentLaserBattery
    {
        get => _currentLaserBattery;
        private set
        {
            if(_currentLaserBattery != value)
            {
                // 레이저 배터리가 최대 레이저 배터리를 넘지 않게 조정
                _currentLaserBattery = Mathf.Clamp(value, 0, MaxLaserBattery);
                // 레이저 배터리 업데이트 이벤트 호출
                OnLaserBatteryUpdated?.Invoke(_currentLaserBattery, MaxLaserBattery);
            }
        }
    }

    /// <summary>
    /// 최대 레이저 배터리에 대한 프로퍼티입니다.
    /// </summary>
    public int MaxLaserBattery
    {
        get => _maxLaserBattery;
        set
        {
            if (_maxLaserBattery != value)
            {
                _maxLaserBattery = value;
                // 레이저 배터리 업데이트 이벤트 호출
                OnLaserBatteryUpdated?.Invoke(_maxLaserBattery, MaxLaserBattery);
            }
        }
    }


    void Start()
    {
        // 컴포넌트 캐시
        _camera = Camera.main.GetComponent<Camera>();
        _pixelPerfectCamera = _camera.GetComponent<PixelPerfectCamera>();
        _transform = GetComponent<Transform>();
        _laserSoundPlayer = GetComponent<AudioSource>();

        if (_llelTransform == null)
        {
            var llelObject = transform.Find("LLEL");
            if (llelObject != null)
            {
                _llelTransform = llelObject.GetComponent<Transform>();
            }
            else
            {
                Debug.LogError("플레이어 오브젝트 안에 나타가 없습니다!");
            }
        }

        if (_laserTransform == null)
        {
            var laserObject = transform.Find("LaserSprite");
            if (laserObject != null)
            {
                _laserTransform = laserObject.GetComponent<Transform>();
            }
            else
            {
                Debug.LogError("플레이어 오브젝트 안에 레이저 스프라이트가 없습니다!");
            }
        }

        if (_laserMuzzleEffectSpriteRenderer == null)
        {
            var laserMuzzleEffectObject = _llelTransform.transform.Find("LaserMuzzleEffect");
            if (laserMuzzleEffectObject != null)
            {
                _laserMuzzleEffectSpriteRenderer = laserMuzzleEffectObject.GetComponent<SpriteRenderer>();
            }
            else
            {
                Debug.LogError("플레이어 오브젝트 안에 레이저 총구 이펙트 스프라이트가 없습니다!");
            }
        }

        if (_laserHitEffectTransform == null)
        {
            var laserHitEffectObject = transform.Find("LaserHitEffect");
            if (laserHitEffectObject != null)
            {
                _laserHitEffectTransform = laserHitEffectObject.GetComponent<Transform>();
            }
            else
            {
                Debug.LogError("플레이어 오브젝트 안에 레이저 적중 이펙트 스프라이트가 없습니다!");
            }
        }

        // 게임 시작 이벤트와 게임 상태 변경 이벤트 연결
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        // 입력 이벤트 연결
        InputManager.Instance.OnInputDown += HandleInputDown;
        InputManager.Instance.OnInput += HandleInput;
        InputManager.Instance.OnInputUp += HandleInputUp;
    }

    void Update()
    {
        // 게임 플레이 상태가 아니면 반환
        if (GameManager.Instance.CurrentState != GameManager.GameState.Play) return;

        // 델타 타임 캐시
        _deltaTime = Time.deltaTime;

        // 나타 방향
        _llelDirection = (_llelTransform.position - _transform.position).normalized;

        if (IsAttacking)
        {
            // 공격하고 있으면
            // 조준 방향 설정
            _aimDirection = CalculateAimDirection();
            if (CurrentLaserBattery > 0)
            {
                // 레이저 사운드 재생
                _laserSoundPlayer.Play();
                // 레이저 공격 처리
                HandleLaserAttack();
                // 레이저 총구 이펙트가 보이게 설정
                _laserMuzzleEffectSpriteRenderer.color = new Color32(255, 255, 255, 255);
                // 레이저 공격 시간이 될 때마다 레이저 공격 시간 재설정 및 배터리 감소
                if (GameManager.Instance.PlayTimeSeconds >= _lastLaserAttackTime + _laserAttackDelay)
                {
                    _lastLaserAttackTime = GameManager.Instance.PlayTimeSeconds;
                    CurrentLaserBattery--;
                }
            }
            else
            {
                // 레이저 배터리가 남아있지 않으면 공격하고 있지 않은 것으로 처리
                HandleNonAttackState();
            }
            // 나타 좌표 설정
            AdjustLLELPosition();
        }
        else
        {
            // 공격하고 있지 않으면 공격하고 있지 않은 것으로 처리 및 배터리 충전
            HandleNonAttackState();
            BatteryChage();
        }
    }

    /// <summary>
    /// 공격하지 않고 있는 상황을 처리하는 메서드입니다.
    /// </summary>
    void HandleNonAttackState()
    {
        // 레이저 사운드 재생 중단 및 레이저 이펙트 설정
        _laserSoundPlayer.Pause();
        _laserTransform.localScale = new Vector3(0, 1, 1);
        _laserMuzzleEffectSpriteRenderer.color = new Color32(255, 255, 255, 0);
        _laserHitEffectTransform.position = new Vector2(1000, 1000);
    }
    
    /// <summary>
    /// 공격을 하고 있지 않을 경우 빠르게 배터리를 채우는 메서드입니다.
    /// </summary>
    void BatteryChage()
    {
        if(GameManager.Instance.PlayTimeSeconds >= _lastBatteryChargeTime + BatteryChargeDelay)
        {
            CurrentLaserBattery += BatteryCharge;
            _lastBatteryChargeTime = GameManager.Instance.PlayTimeSeconds;
        }
    }

    /// <summary>
    /// 나타의 좌표를 조정하는 메서드입니다.
    /// </summary>
    void AdjustLLELPosition()
    {
        float currentAngle = Mathf.Atan2(_llelDirection.y, _llelDirection.x) * Mathf.Rad2Deg;
        float targetAngle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;

        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, _deltaTime * LLELRotationSpeed);
        newAngle *= Mathf.Deg2Rad;

        Vector2 newDirection = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
        Vector2 newPosition = (Vector2)_transform.position + newDirection * _llelDistance;

        _llelTransform.position = newPosition;
    }

    /// <summary>
    /// 레이저 공격을 처리하는 메서드입니다.
    /// </summary>
    void HandleLaserAttack()
    {
        // 레이저 길이를 레이저 공격 길이로 초기화
        float laserLength = LaserAttackDistance;
        // 레이저 적중 처리
        var hits = Physics2D.RaycastAll(_llelTransform.position, _llelDirection, laserLength, _enemyLayer);
        if (hits.Length > 0)
        {
            // 적중 횟수는 항상 최대 관통 횟수 이내로 설정
            int hitCount = Mathf.Clamp(hits.Length, 0, PenetrationCount + 1);
            for(int i = 0; i < hitCount; i++)
            {
                if (GameManager.Instance.PlayTimeSeconds >= _lastLaserAttackTime + _laserAttackDelay)
                {
                    // 게임 플레이 시간이 레이저 어택 딜레이를 넘어야 공격 처리
                    var enemyObject = hits[i].collider.gameObject;
                    if (!_enemyDamageCache.TryGetValue(enemyObject, out EnemyDamage enemyDamage))
                    {
                        // 해당 적이 캐시된 적이 없다면 캐시에 추가
                        enemyDamage = enemyObject.GetComponent<EnemyDamage>();
                        _enemyDamageCache[enemyObject] = enemyDamage;
                    }
                    // 대미지 처리
                    enemyDamage.TakeDamage(CurrentLaserDamage);
                }
            }
            if (hitCount >= PenetrationCount)
            {
                // 적중 횟수가 최대 관통 횟수 이상이면
                // 레이저의 길이를 마지막으로 적중한 적의 위치만큼으로 설정
                laserLength = (hits[hitCount - 1].point - (Vector2)_llelTransform.position).magnitude;
            }
            // 레이저 적중 이펙트를 마지막으로 적중한 적의 위치로 설정
            _laserHitEffectTransform.position = hits[hitCount - 1].point;
        }
        else
        {
            // 레이저가 적중하지 않은 상태이면 레이저 적중 이펙트를 화면 밖으로 보냄
            _laserHitEffectTransform.position = new Vector2(1000, 1000);
        }

        // 레이저 스프라이트 업데이트
        UpdateLaserSprite(laserLength);
    }

    /// <summary>
    /// 레이저 스프라이트를 업데이트하는 메서드입니다.
    /// </summary>
    /// <param name="laserLength">레이저 길이</param>
    void UpdateLaserSprite(float laserLength)
    {
        // 레이저 좌표를 나타의 좌표로 설정
        _laserTransform.position = _llelTransform.position;
        // 레이저 각도 설정
        float spriteAngle = Mathf.Atan2(_llelDirection.y, _llelDirection.x) * Mathf.Rad2Deg;
        _laserTransform.rotation = Quaternion.Euler(0f, 0f, spriteAngle);
        // 레이저 길이 설정
        _laserTransform.localScale = new Vector3(laserLength * _pixelPerfectCamera.assetsPPU, 1, 1);
    }

    /// <summary>
    /// 플레이어의 공격 조준 방향을 계산하는 메서드입니다.
    /// </summary>
    /// <param name="aimDirection">조준 방향</param>
    public Vector2 CalculateAimDirection()
    {
        if(InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            // 마우스 입력 시 이드의 위치와 입력 위치로 조준 방향 계산
            return (_currentInputPosition - (Vector2)_transform.position).normalized;
        }
        else
        {
            // 모바일 입력 시 입력 시작 위치와 현재 입력 위치로 조준 방향 계산
            Vector2 aimDirection = (_currentInputPosition - _startInputPosition).normalized;
            // 조준 보정 적용
            aimDirection = MobileAimCorrection(aimDirection);

            return aimDirection;
        }
    }

    /// <summary>
    /// 모바일 조작 시 조준을 보정해주는 메서드입니다.
    /// </summary>
    /// <param name="aimDirection">조준 방향</param>
    /// <returns>보정 된 조준 방향</returns>
    Vector2 MobileAimCorrection(Vector2 aimDirection)
    {
        var hit = Physics2D.CircleCast((Vector2)_llelTransform.position, MobileAutoTargetingRange, aimDirection, LaserAttackDistance, _enemyLayer);
        if (hit)
        {
            // 조준하고 있는 방향으로 적이 감지되면 조준 방향을 해당 적의 방향으로 조정
            // 초근접 상황에서는 조준이 튀는 것을 방지하기 위해 조준 보정을 하지 않음
            Vector2 hitPointToLLEL = hit.point - (Vector2)_llelTransform.position;
            if (hitPointToLLEL.sqrMagnitude > MinimumAimCorrectionDistanceSquared)
            {
                aimDirection = hitPointToLLEL.normalized;
            }
        }
        return aimDirection;
    }

    /// <summary>
    /// 게임이 시작될 때 호출되는 이벤트입니다.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // 초기화
        CurrentLaserDamage = _defaultLaserDamage;
        MaxLaserBattery = _defaultMaxLaserBattery;
        CurrentLaserBattery = MaxLaserBattery;
        BatteryCharge = _defaultBatteryCharge;
        PenetrationCount = 0;
        _lastBatteryChargeTime = 0f;
        _lastLaserAttackTime = 0f;
    }

    /// <summary>
    /// 게임 상태가 변경될 때 호출되는 이벤트입니다.
    /// </summary>
    /// <param name="currentState">현재 상태</param>
    void HandleGameStateChanged(GameManager.GameState currentState)
    {
        if (currentState != GameManager.GameState.Play)
        {
            // 게임 상태가 플레이가 아니면 레이저 사운드를 중단
            _laserSoundPlayer.Pause();
        }
    }

    /// <summary>
    /// 입력이 시작되면 호출되는 이벤트입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleInputDown(Vector2 inputPosition)
    {
        if(InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            // 마우스 입력이면 입력 좌표를 World 좌표로 변환
            inputPosition = _camera.ScreenToWorldPoint(inputPosition);
        }
        // 입력 시작 좌표 설정
        _startInputPosition = inputPosition;
    }

    /// <summary>
    /// 입력 중이면 호출되는 이벤트입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleInput(Vector2 inputPosition)
    {
        if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            // 마우스 입력이면 입력 좌표를 World 좌표로 변환
            inputPosition = _camera.ScreenToWorldPoint(inputPosition);
        }
        else
        {
            // 입력이 시작된 좌표와 현재 입력 좌표의 거리를 비교
            // 데드존을 넘지 않았으면 반환
            float distance = (inputPosition - _startInputPosition).sqrMagnitude;
            if (distance < MobileInputDeadZone * MobileInputDeadZone)
            {
                return;
            }
        }
        // 공격하고 있는 상태로 설정
        IsAttacking = true;
        // 현재 입력 좌표 설정
        _currentInputPosition = inputPosition;
    }

    /// <summary>
    /// 입력이 종료되면 호출되는 이벤트입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleInputUp(Vector2 inputPosition)
    {
        // 입력 종료시 공격 중단
        IsAttacking = false;
    }
}
