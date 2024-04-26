using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� �Է��� ó���ϱ� ���� �̱��� Ŭ�����Դϴ�.
/// </summary>
public class InputManager : SingletonManager<InputManager>
{
    public Action<Vector2> OnInputDown; // �Է��� ���۵� �� ȣ��Ǵ� �̹�Ʈ
    public Action<Vector2> OnInput;     // �Է� ���̸� ȣ��Ǵ� �̺�Ʈ
    public Action<Vector2> OnInputUp;   // �Է��� �ߴܵǸ� ȣ��Ǵ� �̺�Ʈ

    // ��Ʈ�� Ÿ�� ������
    public enum ControlType
    {
        None,
        Touch,
        Mouse
    }

    /// <summary>
    /// ���� ��Ʈ�� Ÿ�Կ� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public ControlType CurrentControlType { get; private set; } = ControlType.None;

    /// <summary>
    /// �Է��� ���۵� ��ǥ�� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public Vector2 CurrentPosition { get; private set; }

    void Update()
    {
        if (CurrentControlType == ControlType.None)
        {
            // ��Ʈ�� Ÿ���� �������� ���� �����̸� ������ �Է����� ��Ʈ�� Ÿ�� ����
            DetectControlType();
        }
        // �Է� ó��
        HandleInput();
    }

    /// <summary>
    /// ó�� �Է¿� ���� ��Ʈ�� Ÿ���� �����ϴ� �޼����Դϴ�.
    /// </summary>
    void DetectControlType()
    {
        if (Input.touchCount > 0)
        {
            // ��ġ �Է� �� ��� ��Ʈ�� Ÿ���� ��ġ�� ����
            CurrentControlType = ControlType.Touch;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // ���콺 �Է� �� ��Ʈ�� Ÿ���� ���콺�� ����
            CurrentControlType = ControlType.Mouse;
        }
    }

    /// <summary>
    /// �Է��� ó���ϴ� �޼����Դϴ�.
    /// </summary>
    void HandleInput()
    {
        Vector2 inputPosition = Vector2.zero;

        // ��Ʈ�� Ÿ�Կ� ���� �Է� ó��
        switch (CurrentControlType)
        {
            case ControlType.Mouse:
                HandleMouseInput(ref inputPosition);
                break;
            case ControlType.Touch:
                HandleTouchInput(ref inputPosition);
                break;
        }
        
        // ���� ��ǥ�� �Էµ� ��ǥ�� �����մϴ�.
        CurrentPosition = inputPosition;
    }

    /// <summary>
    /// ���콺 �Է��� ó���ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleMouseInput(ref Vector2 inputPosition)
    {
        // �Է� ��ǥ�� ���콺 ��ǥ�� ����
        inputPosition = Input.mousePosition;

        // ���콺 �Է¿� ���� �̺�Ʈ ȣ��
        if (Input.GetMouseButtonDown(0))
        {
            OnInputDown?.Invoke(inputPosition);
        }
        else if (Input.GetMouseButton(0))
        {
            OnInput?.Invoke(inputPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnInputUp?.Invoke(inputPosition);
        }
    }

    /// <summary>
    /// ��ġ �Է��� ó���ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleTouchInput(ref Vector2 inputPosition)
    {
        // ��ġ �Է��� �����Ǿ����� Ȯ��
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            // �Է� ��ǥ�� ��ġ ��ǥ�� ����
            inputPosition = touch.position;

            // ��ġ �Է¿� ���� �̺�Ʈ ȣ��
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnInputDown?.Invoke(inputPosition);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    OnInput?.Invoke(inputPosition);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnInputUp?.Invoke(inputPosition);
                    break;
            }
        }
    }
}