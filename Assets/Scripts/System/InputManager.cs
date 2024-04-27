using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 입력을 처리하기 위한 싱글톤 클래스입니다.
/// </summary>
public class InputManager : SingletonManager<InputManager>
{
    public Action<Vector2> OnInputDown; // 입력이 시작될 때 호출되는 이밴트
    public Action<Vector2> OnInput;     // 입력 중이면 호출되는 이벤트
    public Action<Vector2> OnInputUp;   // 입력이 중단되면 호출되는 이벤트

    // 컨트롤 타입 열거형
    public enum ControlType
    {
        None,
        Touch,
        Mouse
    }

    /// <summary>
    /// 현재 컨트롤 타입에 대한 프로퍼티입니다.
    /// </summary>
    public ControlType CurrentControlType { get; private set; } = ControlType.None;

    /// <summary>
    /// 입력이 시작된 좌표에 대한 프로퍼티입니다.
    /// </summary>
    public Vector2 CurrentPosition { get; private set; }

    void Update()
    {
        if (CurrentControlType == ControlType.None)
        {
            // 컨트롤 타입이 설정되지 않은 상태이면 감지된 입력으로 컨트롤 타입 설정
            DetectControlType();
        }
        // 입력 처리
        HandleInput();
    }

    /// <summary>
    /// 처음 입력에 따라 컨트롤 타입을 설정하는 메서드입니다.
    /// </summary>
    void DetectControlType()
    {
        if (Input.touchCount > 0)
        {
            // 터치 입력 시 경우 컨트롤 타입을 터치로 설정
            CurrentControlType = ControlType.Touch;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // 마우스 입력 시 컨트롤 타입을 마우스로 설정
            CurrentControlType = ControlType.Mouse;
        }
    }

    /// <summary>
    /// 입력을 처리하는 메서드입니다.
    /// </summary>
    void HandleInput()
    {
        Vector2 inputPosition = Vector2.zero;

        // 컨트롤 타입에 따라 입력 처리
        switch (CurrentControlType)
        {
            case ControlType.Mouse:
                HandleMouseInput(ref inputPosition);
                break;
            case ControlType.Touch:
                HandleTouchInput(ref inputPosition);
                break;
        }
        
        // 현재 좌표를 입력된 좌표로 설정합니다.
        CurrentPosition = inputPosition;
    }

    /// <summary>
    /// 마우스 입력을 처리하는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleMouseInput(ref Vector2 inputPosition)
    {
        // 입력 좌표를 마우스 좌표로 설정
        inputPosition = Input.mousePosition;

        // 마우스 입력에 따라 이벤트 호출
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
    /// 터치 입력을 처리하는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleTouchInput(ref Vector2 inputPosition)
    {
        // 터치 입력이 감지되었는지 확인
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            // 입력 좌표를 터치 좌표로 설정
            inputPosition = touch.position;

            // 터치 입력에 따라 이벤트 호출
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