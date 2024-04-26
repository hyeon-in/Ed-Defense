using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� ���� ��ǥ�� ǥ���ϱ� ���� UI Ŭ�����Դϴ�.
/// </summary>
public class Crosshair : MonoBehaviour
{
    // ũ�ν��� ���̰ų� ����� ���� ���� ���
    readonly Color32 VisibleColor = Color.white;
    readonly Color32 InvisibleColor = new Color32(0, 0, 0, 0);

    [SerializeField] Transform _playerTransform;         // �÷��̾� Ʈ������ ĳ��
    [SerializeField] PlayerController _playerController; // �÷��̾� ��Ʈ�ѷ� ĳ��
    [SerializeField] float _mobileInputDistance = 10f;   // ����� �Է� �� UI ǥ�� �Ÿ�

    Vector2 _startInputPosition;    // �Է� ���� ��ǥ

    Image _crossHairImage;          // ������ �̹���
    Camera _camera;                 // ī�޶�
    RectTransform _rectTransform;   // �������� RectTransform

    void Start()
    {
        // �ʱ�ȭ
        _camera = Camera.main.GetComponent<Camera>();
        _crossHairImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();

        // �������� ���� ������ �ʴ� ������ ����
        _crossHairImage.color = InvisibleColor;

        // ������ �÷��̾� ������Ʈ�� ã�Ƽ� Transform �Ҵ�
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerTransform = playerObject.GetComponent<Transform>();
        }
        else
        {
            Debug.LogError("�������� �÷��̾ ã�� �� �����! �÷��̾ �����ϴ���, �Ǵ� �÷��̾�� �±װ� ����� �Ҵ�ƴ��� üũ�� �ּ���.");
        }
        _playerController = _playerTransform.GetComponent<PlayerController>();

        // �̺�Ʈ ���� 
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        InputManager.Instance.OnInputDown += HandleInputDown;
        InputManager.Instance.OnInput += HandleInput;
        InputManager.Instance.OnInputUp += HandleInputUp;
    }

    void Update()
    {
        // ��Ʈ�� Ÿ���� ���콺�̸� �ǽð����� �������� ���콺 ��ǥ�� ����ٴ�
        if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            Vector2 inputPosition = _camera.ScreenToWorldPoint(InputManager.Instance.CurrentPosition);
            _rectTransform.position = inputPosition;
        }
    }

    /// <summary>
    /// ���� ���°� ����� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="currentGameState">���� ���� ����</param>
    void HandleGameStateChanged(GameManager.GameState currentGameState)
    {
        if (currentGameState == GameManager.GameState.Play)
        {
            // ��Ʈ�� Ÿ���� ���콺�̸� ���� �÷��̰� ���۵� �� ���콺�� ���̰� �ϰ� Ŀ���� ����
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
            {
                _crossHairImage.color = VisibleColor;
                Cursor.visible = false;
            }
        }
        else
        {
            // �p�� ���°� ���� �÷��� ���°� �ƴϸ� �������� ����� ���콺 Ŀ���� ���̰� ����
            _crossHairImage.color = InvisibleColor;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// �Է��� ���۵� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleInputDown(Vector2 inputPosition)
    {
        // �Է��� ���۵� ��
        // ������ �÷��� �����̰� ��Ʈ�� Ÿ���� ��ġ�̸�
        // �������� ���̰� �����ϰ� �Է� ���� ��ǥ�� ���� ��ǥ�� ����
        if (GameManager.Instance.CurrentState == GameManager.GameState.Play)
        {
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Touch)
            {
                _startInputPosition = inputPosition;
            }
        }
    }

    /// <summary>
    /// �Է� ���� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleInput(Vector2 inputPosition)
    {
        // �Է� ���� ��
        // ������ �÷��� �����̰� ��Ʈ�� Ÿ���� ��ġ�̸�
        if (GameManager.Instance.CurrentState == GameManager.GameState.Play)
        {
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Touch)
            {
                // �÷��̾��� ���� ���¿� ���� ���̰� �� ���̰� ����
                _crossHairImage.color = _playerController.IsAttacking ? VisibleColor : InvisibleColor;

                // �÷��̾ �����ϰ� �ִ� �������� ������ ��ġ ����
                Vector2 direction = (inputPosition - _startInputPosition).normalized;
                _rectTransform.position = (Vector2)_playerTransform.position + (direction * _mobileInputDistance);
            }
        }
    }

    /// <summary>
    /// �Է��� ����� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleInputUp(Vector2 inputPosition)
    {
        // �Է��� ����� ��
        // ������ �÷��� �����̰� ��Ʈ�� Ÿ���� ��ġ�̸�
        // �������� ����ϴ�.
        if (GameManager.Instance.CurrentState == GameManager.GameState.Play)
        {
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Touch)
            {
                _crossHairImage.color = InvisibleColor;
            }
        }
    }
}
