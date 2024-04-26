using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// �÷��̾��� ��� ���� Ŭ�����Դϴ�.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// ���� ������ ���͸�, Ȥ�� �ִ� ������ ���͸��� ������Ʈ�Ǹ� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    public Action<int, int> OnLaserBatteryUpdated;

    // ���
    const float LLELRotationSpeed = 25f;                    // ��Ÿ�� ȸ�� �ӵ�(1 / 0.04�� ����)
    const float LaserAttackDistance = 16f;                  // ������ ���� �Ÿ�
    const float BatteryChargeDelay = 0.02f;                 // ���͸� ���� ������
    const float MobileInputDeadZone = 20f;                  // ����� �Է� ������
    const float MobileAutoTargetingRange = 0.5f;            // ����� �ڵ� ���� ����
    const float MinimumAimCorrectionDistanceSquared = 1.5f; // ����Ͽ��� ���� ������ ���� �ʹ� ������ ���ص��� �ʰ� �ϱ� ���� ���

    // SerializeField
    [Header("��Ÿ")]
    [SerializeField] Transform _llelTransform;      // ��Ÿ�� Ʈ������
    [SerializeField] float _llelDistance = 2f;      // ��Ÿ�� �÷��̾���� �Ÿ�
    [Header("������")]
    [SerializeField] int _defaultLaserDamage = 1;                         // �⺻ ������ �����
    [SerializeField] float _laserAttackDelay = 0.02f;                     // ������ ����� ����
    [SerializeField] int _defaultMaxLaserBattery = 100;                   // �⺻ �ִ� ������ ���͸� 
    [SerializeField] int _defaultBatteryCharge = 1;                       // ������ ���͸� ȸ����
    [SerializeField] Transform _laserTransform;                           // ������ Ʈ������
    [SerializeField] SpriteRenderer _laserMuzzleEffectSpriteRenderer;     // �������� ���۵Ǵ� ����Ʈ�� ��������Ʈ ������
    [SerializeField] Transform _laserHitEffectTransform;                  // ������ ���� ����Ʈ�� Ʈ������
    [SerializeField] LayerMask _enemyLayer;                               // �����Ϸ��� ���� ���̾�

    // private
    int _currentLaserBattery;           // ���� ������ ���͸�
    int _maxLaserBattery;               // �ִ� ������ ���͸�
    float _deltaTime;                   // ��Ÿ Ÿ�� ĳ��
    float _lastLaserAttackTime;         // ���������� �������� ������ �ð�
    float _lastBatteryChargeTime = 0f;  // ���������� ���͸��� ������ �ð�

    Vector2 _llelDirection;                 // ��Ÿ ����
    Vector2 _aimDirection;                  // ���� ����

    Vector2 _startInputPosition;            // �Է��� ���۵� ��ǥ
    Vector2 _currentInputPosition;          // ���� �Է� ��ǥ

    Transform _transform;                   // �÷��̾��� Ʈ������ ĳ��
    Camera _camera;                         // ī�޶� ĳ��
    PixelPerfectCamera _pixelPerfectCamera; // ī�޶��� �ȼ� ����Ʈ ī�޶�
    AudioSource _laserSoundPlayer;          // ������ ���带 ����ϱ� ���� ����� �ҽ�

    // EnemyDamage Ŭ������ �����ϴ� ��ųʸ�
    Dictionary<GameObject, EnemyDamage> _enemyDamageCache = new Dictionary<GameObject, EnemyDamage>();

    /// <summary>
    /// ���� ������ ������� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int CurrentLaserDamage { get; set; }

    /// <summary>
    /// ���͸��� ���� ���� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int BatteryCharge { get; set; }

    /// <summary>
    /// �������� ���� �ִ��� ������ �� �ִ� ���� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int PenetrationCount { get; set; }

    /// <summary>
    /// ���� ������ üũ�ϱ� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public bool IsAttacking { get; private set; }

    /// <summary>
    /// ���� ������ ���͸��� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int CurrentLaserBattery
    {
        get => _currentLaserBattery;
        private set
        {
            if(_currentLaserBattery != value)
            {
                // ������ ���͸��� �ִ� ������ ���͸��� ���� �ʰ� ����
                _currentLaserBattery = Mathf.Clamp(value, 0, MaxLaserBattery);
                // ������ ���͸� ������Ʈ �̺�Ʈ ȣ��
                OnLaserBatteryUpdated?.Invoke(_currentLaserBattery, MaxLaserBattery);
            }
        }
    }

    /// <summary>
    /// �ִ� ������ ���͸��� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int MaxLaserBattery
    {
        get => _maxLaserBattery;
        set
        {
            if (_maxLaserBattery != value)
            {
                _maxLaserBattery = value;
                // ������ ���͸� ������Ʈ �̺�Ʈ ȣ��
                OnLaserBatteryUpdated?.Invoke(_maxLaserBattery, MaxLaserBattery);
            }
        }
    }


    void Start()
    {
        // ������Ʈ ĳ��
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
                Debug.LogError("�÷��̾� ������Ʈ �ȿ� ��Ÿ�� �����ϴ�!");
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
                Debug.LogError("�÷��̾� ������Ʈ �ȿ� ������ ��������Ʈ�� �����ϴ�!");
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
                Debug.LogError("�÷��̾� ������Ʈ �ȿ� ������ �ѱ� ����Ʈ ��������Ʈ�� �����ϴ�!");
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
                Debug.LogError("�÷��̾� ������Ʈ �ȿ� ������ ���� ����Ʈ ��������Ʈ�� �����ϴ�!");
            }
        }

        // ���� ���� �̺�Ʈ�� ���� ���� ���� �̺�Ʈ ����
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        // �Է� �̺�Ʈ ����
        InputManager.Instance.OnInputDown += HandleInputDown;
        InputManager.Instance.OnInput += HandleInput;
        InputManager.Instance.OnInputUp += HandleInputUp;
    }

    void Update()
    {
        // ���� �÷��� ���°� �ƴϸ� ��ȯ
        if (GameManager.Instance.CurrentState != GameManager.GameState.Play) return;

        // ��Ÿ Ÿ�� ĳ��
        _deltaTime = Time.deltaTime;

        // ��Ÿ ����
        _llelDirection = (_llelTransform.position - _transform.position).normalized;

        if (IsAttacking)
        {
            // �����ϰ� ������
            // ���� ���� ����
            _aimDirection = CalculateAimDirection();
            if (CurrentLaserBattery > 0)
            {
                // ������ ���� ���
                _laserSoundPlayer.Play();
                // ������ ���� ó��
                HandleLaserAttack();
                // ������ �ѱ� ����Ʈ�� ���̰� ����
                _laserMuzzleEffectSpriteRenderer.color = new Color32(255, 255, 255, 255);
                // ������ ���� �ð��� �� ������ ������ ���� �ð� �缳�� �� ���͸� ����
                if (GameManager.Instance.PlayTimeSeconds >= _lastLaserAttackTime + _laserAttackDelay)
                {
                    _lastLaserAttackTime = GameManager.Instance.PlayTimeSeconds;
                    CurrentLaserBattery--;
                }
            }
            else
            {
                // ������ ���͸��� �������� ������ �����ϰ� ���� ���� ������ ó��
                HandleNonAttackState();
            }
            // ��Ÿ ��ǥ ����
            AdjustLLELPosition();
        }
        else
        {
            // �����ϰ� ���� ������ �����ϰ� ���� ���� ������ ó�� �� ���͸� ����
            HandleNonAttackState();
            BatteryChage();
        }
    }

    /// <summary>
    /// �������� �ʰ� �ִ� ��Ȳ�� ó���ϴ� �޼����Դϴ�.
    /// </summary>
    void HandleNonAttackState()
    {
        // ������ ���� ��� �ߴ� �� ������ ����Ʈ ����
        _laserSoundPlayer.Pause();
        _laserTransform.localScale = new Vector3(0, 1, 1);
        _laserMuzzleEffectSpriteRenderer.color = new Color32(255, 255, 255, 0);
        _laserHitEffectTransform.position = new Vector2(1000, 1000);
    }
    
    /// <summary>
    /// ������ �ϰ� ���� ���� ��� ������ ���͸��� ä��� �޼����Դϴ�.
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
    /// ��Ÿ�� ��ǥ�� �����ϴ� �޼����Դϴ�.
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
    /// ������ ������ ó���ϴ� �޼����Դϴ�.
    /// </summary>
    void HandleLaserAttack()
    {
        // ������ ���̸� ������ ���� ���̷� �ʱ�ȭ
        float laserLength = LaserAttackDistance;
        // ������ ���� ó��
        var hits = Physics2D.RaycastAll(_llelTransform.position, _llelDirection, laserLength, _enemyLayer);
        if (hits.Length > 0)
        {
            // ���� Ƚ���� �׻� �ִ� ���� Ƚ�� �̳��� ����
            int hitCount = Mathf.Clamp(hits.Length, 0, PenetrationCount + 1);
            for(int i = 0; i < hitCount; i++)
            {
                if (GameManager.Instance.PlayTimeSeconds >= _lastLaserAttackTime + _laserAttackDelay)
                {
                    // ���� �÷��� �ð��� ������ ���� �����̸� �Ѿ�� ���� ó��
                    var enemyObject = hits[i].collider.gameObject;
                    if (!_enemyDamageCache.TryGetValue(enemyObject, out EnemyDamage enemyDamage))
                    {
                        // �ش� ���� ĳ�õ� ���� ���ٸ� ĳ�ÿ� �߰�
                        enemyDamage = enemyObject.GetComponent<EnemyDamage>();
                        _enemyDamageCache[enemyObject] = enemyDamage;
                    }
                    // ����� ó��
                    enemyDamage.TakeDamage(CurrentLaserDamage);
                }
            }
            if (hitCount >= PenetrationCount)
            {
                // ���� Ƚ���� �ִ� ���� Ƚ�� �̻��̸�
                // �������� ���̸� ���������� ������ ���� ��ġ��ŭ���� ����
                laserLength = (hits[hitCount - 1].point - (Vector2)_llelTransform.position).magnitude;
            }
            // ������ ���� ����Ʈ�� ���������� ������ ���� ��ġ�� ����
            _laserHitEffectTransform.position = hits[hitCount - 1].point;
        }
        else
        {
            // �������� �������� ���� �����̸� ������ ���� ����Ʈ�� ȭ�� ������ ����
            _laserHitEffectTransform.position = new Vector2(1000, 1000);
        }

        // ������ ��������Ʈ ������Ʈ
        UpdateLaserSprite(laserLength);
    }

    /// <summary>
    /// ������ ��������Ʈ�� ������Ʈ�ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="laserLength">������ ����</param>
    void UpdateLaserSprite(float laserLength)
    {
        // ������ ��ǥ�� ��Ÿ�� ��ǥ�� ����
        _laserTransform.position = _llelTransform.position;
        // ������ ���� ����
        float spriteAngle = Mathf.Atan2(_llelDirection.y, _llelDirection.x) * Mathf.Rad2Deg;
        _laserTransform.rotation = Quaternion.Euler(0f, 0f, spriteAngle);
        // ������ ���� ����
        _laserTransform.localScale = new Vector3(laserLength * _pixelPerfectCamera.assetsPPU, 1, 1);
    }

    /// <summary>
    /// �÷��̾��� ���� ���� ������ ����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="aimDirection">���� ����</param>
    public Vector2 CalculateAimDirection()
    {
        if(InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            // ���콺 �Է� �� �̵��� ��ġ�� �Է� ��ġ�� ���� ���� ���
            return (_currentInputPosition - (Vector2)_transform.position).normalized;
        }
        else
        {
            // ����� �Է� �� �Է� ���� ��ġ�� ���� �Է� ��ġ�� ���� ���� ���
            Vector2 aimDirection = (_currentInputPosition - _startInputPosition).normalized;
            // ���� ���� ����
            aimDirection = MobileAimCorrection(aimDirection);

            return aimDirection;
        }
    }

    /// <summary>
    /// ����� ���� �� ������ �������ִ� �޼����Դϴ�.
    /// </summary>
    /// <param name="aimDirection">���� ����</param>
    /// <returns>���� �� ���� ����</returns>
    Vector2 MobileAimCorrection(Vector2 aimDirection)
    {
        var hit = Physics2D.CircleCast((Vector2)_llelTransform.position, MobileAutoTargetingRange, aimDirection, LaserAttackDistance, _enemyLayer);
        if (hit)
        {
            // �����ϰ� �ִ� �������� ���� �����Ǹ� ���� ������ �ش� ���� �������� ����
            // �ʱ��� ��Ȳ������ ������ Ƣ�� ���� �����ϱ� ���� ���� ������ ���� ����
            Vector2 hitPointToLLEL = hit.point - (Vector2)_llelTransform.position;
            if (hitPointToLLEL.sqrMagnitude > MinimumAimCorrectionDistanceSquared)
            {
                aimDirection = hitPointToLLEL.normalized;
            }
        }
        return aimDirection;
    }

    /// <summary>
    /// ������ ���۵� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // �ʱ�ȭ
        CurrentLaserDamage = _defaultLaserDamage;
        MaxLaserBattery = _defaultMaxLaserBattery;
        CurrentLaserBattery = MaxLaserBattery;
        BatteryCharge = _defaultBatteryCharge;
        PenetrationCount = 0;
        _lastBatteryChargeTime = 0f;
        _lastLaserAttackTime = 0f;
    }

    /// <summary>
    /// ���� ���°� ����� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    /// <param name="currentState">���� ����</param>
    void HandleGameStateChanged(GameManager.GameState currentState)
    {
        if (currentState != GameManager.GameState.Play)
        {
            // ���� ���°� �÷��̰� �ƴϸ� ������ ���带 �ߴ�
            _laserSoundPlayer.Pause();
        }
    }

    /// <summary>
    /// �Է��� ���۵Ǹ� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleInputDown(Vector2 inputPosition)
    {
        if(InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            // ���콺 �Է��̸� �Է� ��ǥ�� World ��ǥ�� ��ȯ
            inputPosition = _camera.ScreenToWorldPoint(inputPosition);
        }
        // �Է� ���� ��ǥ ����
        _startInputPosition = inputPosition;
    }

    /// <summary>
    /// �Է� ���̸� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleInput(Vector2 inputPosition)
    {
        if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            // ���콺 �Է��̸� �Է� ��ǥ�� World ��ǥ�� ��ȯ
            inputPosition = _camera.ScreenToWorldPoint(inputPosition);
        }
        else
        {
            // �Է��� ���۵� ��ǥ�� ���� �Է� ��ǥ�� �Ÿ��� ��
            // �������� ���� �ʾ����� ��ȯ
            float distance = (inputPosition - _startInputPosition).sqrMagnitude;
            if (distance < MobileInputDeadZone * MobileInputDeadZone)
            {
                return;
            }
        }
        // �����ϰ� �ִ� ���·� ����
        IsAttacking = true;
        // ���� �Է� ��ǥ ����
        _currentInputPosition = inputPosition;
    }

    /// <summary>
    /// �Է��� ����Ǹ� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleInputUp(Vector2 inputPosition)
    {
        // �Է� ����� ���� �ߴ�
        IsAttacking = false;
    }
}
