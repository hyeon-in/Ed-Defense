using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 조준 좌표를 표시하기 위한 UI 클래스입니다.
/// </summary>
public class Crosshair : MonoBehaviour
{
    // 크로스헤어를 보이거나 숨기기 위한 색상 상수
    readonly Color32 VisibleColor = Color.white;
    readonly Color32 InvisibleColor = new Color32(0, 0, 0, 0);

    [SerializeField] Transform _playerTransform;         // 플레이어 트랜스폼 캐시
    [SerializeField] PlayerController _playerController; // 플레이어 컨트롤러 캐시
    [SerializeField] float _mobileInputDistance = 10f;   // 모바일 입력 시 UI 표시 거리

    Vector2 _startInputPosition;    // 입력 시작 좌표

    Image _crossHairImage;          // 조준점 이미지
    Camera _camera;                 // 카메라
    RectTransform _rectTransform;   // 조준점의 RectTransform

    void Start()
    {
        // 초기화
        _camera = Camera.main.GetComponent<Camera>();
        _crossHairImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();

        // 조준점의 색을 보이지 않는 색으로 설정
        _crossHairImage.color = InvisibleColor;

        // 씬에서 플레이어 오브젝트를 찾아서 Transform 할당
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerTransform = playerObject.GetComponent<Transform>();
        }
        else
        {
            Debug.LogError("조준점이 플레이어를 찾을 수 없어요! 플레이어가 존재하는지, 또는 플레이어에게 태그가 제대로 할당됐는지 체크해 주세요.");
        }
        _playerController = _playerTransform.GetComponent<PlayerController>();

        // 이벤트 연결 
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        InputManager.Instance.OnInputDown += HandleInputDown;
        InputManager.Instance.OnInput += HandleInput;
        InputManager.Instance.OnInputUp += HandleInputUp;
    }

    void Update()
    {
        // 컨트롤 타입이 마우스이면 실시간으로 조준점이 마우스 좌표를 따라다님
        if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
        {
            Vector2 inputPosition = _camera.ScreenToWorldPoint(InputManager.Instance.CurrentPosition);
            _rectTransform.position = inputPosition;
        }
    }

    /// <summary>
    /// 게임 상태가 변경될 때 호출되는 메서드입니다.
    /// </summary>
    /// <param name="currentGameState">현재 게임 상태</param>
    void HandleGameStateChanged(GameManager.GameState currentGameState)
    {
        if (currentGameState == GameManager.GameState.Play)
        {
            // 컨트롤 타입이 마우스이면 게임 플레이가 시작될 때 마우스를 보이게 하고 커서를 숨김
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Mouse)
            {
                _crossHairImage.color = VisibleColor;
                Cursor.visible = false;
            }
        }
        else
        {
            // 혅재 상태가 게임 플레이 상태가 아니면 조준점을 숨기고 마우스 커서를 보이게 설정
            _crossHairImage.color = InvisibleColor;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// 입력이 시작될 때 호출되는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleInputDown(Vector2 inputPosition)
    {
        // 입력이 시작될 때
        // 게임이 플레이 상태이고 컨트롤 타입이 터치이면
        // 조준점을 보이게 설정하고 입력 시작 좌표를 현재 좌표로 설정
        if (GameManager.Instance.CurrentState == GameManager.GameState.Play)
        {
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Touch)
            {
                _startInputPosition = inputPosition;
            }
        }
    }

    /// <summary>
    /// 입력 중일 때 호출되는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleInput(Vector2 inputPosition)
    {
        // 입력 중일 때
        // 게임이 플레이 상태이고 컨트롤 타입이 터치이면
        if (GameManager.Instance.CurrentState == GameManager.GameState.Play)
        {
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Touch)
            {
                // 플레이어의 공격 상태에 따라 보이고 안 보이고 설정
                _crossHairImage.color = _playerController.IsAttacking ? VisibleColor : InvisibleColor;

                // 플레이어가 조준하고 있는 방향으로 조준점 위치 설정
                Vector2 direction = (inputPosition - _startInputPosition).normalized;
                _rectTransform.position = (Vector2)_playerTransform.position + (direction * _mobileInputDistance);
            }
        }
    }

    /// <summary>
    /// 입력이 종료될 때 호출되는 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleInputUp(Vector2 inputPosition)
    {
        // 입력이 종료될 때
        // 게임이 플레이 상태이고 컨트롤 타입이 터치이면
        // 조준점을 숨깁니다.
        if (GameManager.Instance.CurrentState == GameManager.GameState.Play)
        {
            if (InputManager.Instance.CurrentControlType == InputManager.ControlType.Touch)
            {
                _crossHairImage.color = InvisibleColor;
            }
        }
    }
}
