using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 버튼의 기반이 되는 추상 클래스입니다.
/// </summary>
public abstract class BaseButton : MonoBehaviour
{
    RectTransform _rectTransform;   // rectTransform 캐시
    Camera _camera;                 // 카메라 캐시

    protected bool isClicked;       // 클릭한 상태인지 체크
    protected Image buttonImage;    // 버튼으로 사용하는 이미지

    void Start()
    {
        // 초기화
        _camera = Camera.main.GetComponent<Camera>();
        _rectTransform = GetComponent<RectTransform>();
        buttonImage = GetComponent<Image>();

        // 이벤트 연결
        InputManager.Instance.OnInputDown += HandleInputDown;
        InputManager.Instance.OnInputUp += HandleInputUp;
    }

    /// <summary>
    /// 입력이 시작되면 호출되는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleInputDown(Vector2 inputPosition)
    {
        if (IsPointerOverButton(inputPosition))
        {
            // 입력 좌표가 버튼안에 있으면 클릭을 한 것으로 처리하고 버튼 스프라이트 업데이트
            isClicked = true;
            UpdateButtonSprite();
        }
    }

    /// <summary>
    /// 입력이 종료되면 호출되는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표ㅕ</param>
    void HandleInputUp(Vector2 inputPosition)
    {
        if (isClicked)
        {
            // 클릭한 상태에서 입력 좌표가 버튼 안에 있으면 버튼 클릭 메서드 실행
            if (IsPointerOverButton(inputPosition))
            {
                HandleButtonClick();
            }
            // 클릭을 하지 않은 것으로 처리하고 버튼 스프라이트 업데이트
            isClicked = false;
            UpdateButtonSprite();
        }
    }

    /// <summary>
    /// 입력 위치가 버튼 위에 있는지 체크하는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 위치</param>
    /// <returns>입력 위치가 버튼 위에 있으면 true, 아니라면 false를 반환합니다.</returns>
    bool IsPointerOverButton(Vector2 inputPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, inputPosition, _camera);
    }

    /// <summary>
    /// 버튼이 클릭되었을 때 호출되는 추상 메서드입니다.
    /// 파생 클래스에서 구현됩니다.
    /// </summary>
    protected abstract void HandleButtonClick();

    /// <summary>
    /// 버튼 스프라이트를 업데이트하는 추상 메서드입니다.
    /// 파생 클래스에서 구현합니다.
    /// </summary>
    protected abstract void UpdateButtonSprite();
}
