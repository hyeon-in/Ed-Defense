using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 업그레이드 메뉴를 실행하거나 종료하는 버튼 클래스입니다.
/// </summary>
public class ToggleUpgradeMenuButton : BaseButton
{
    [SerializeField] GameObject _upgradeMenu;   // 업그레이드 메뉴 게임 오브젝트
    [SerializeField] Sprite _menuOpen;          // 메뉴를 오픈할 수 있는 상태일 때 스프라이트
    [SerializeField] Sprite _menuOpenClick;     // 메뉴를 오픈할 수 있는 상태일 때 클릭 스프라이트
    [SerializeField] Sprite _menuClose;         // 메뉴를 종료할 수 있는 상태일 때 스프라이트
    [SerializeField] Sprite _menuCloseClick;    // 메뉴를 종료할 수 있는 상태일 때 종료 스프라이트

    /// <summary>
    /// 버튼 클릭을 처리하는 메서드입니다.
    /// </summary>
    protected override void HandleButtonClick()
    {
        if(_upgradeMenu == null)
        {
            Debug.LogError("토글 버튼이 업그레이드 메뉴를 찾을 수 없습니다!");
            return;
        }

        // 업그레이드 메뉴가 비활성화 된 상태이고 게임을 플레이하고 있는 상태이면 메뉴 오픈
        // 업그레이드 메뉴가 활성화 된 상태라면 메뉴 종료
        if (!_upgradeMenu.activeSelf)
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.Play)
            {
                OpenUpgradeMenu();
            }
        }
        else
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.Upgrade)
            {
                CloseUpgradeMenu();
            }
        }
    }

    void OnApplicationFocus()
    {
        // 업그레이드 메뉴가 비활성화 된 상태이고 게임을 플레이하고 있는 상태이면 메뉴 오픈
        if (!_upgradeMenu.activeSelf && GameManager.Instance.CurrentState == GameManager.GameState.Play)
        {
            OpenUpgradeMenu();
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // 업그레이드 메뉴가 비활성화 된 상태이고 게임을 플레이하고 있는 상태이면 메뉴 오픈
            if (!_upgradeMenu.activeSelf && GameManager.Instance.CurrentState == GameManager.GameState.Play)
            {
                OpenUpgradeMenu();
            }
        }
    }

    /// <summary>
    /// 업그레이드 메뉴를 활성화하는 메서드입니다.
    /// </summary>
    void OpenUpgradeMenu()
    {
        _upgradeMenu.SetActive(true);
        GameManager.Instance.CurrentState = GameManager.GameState.Upgrade;
        UpdateButtonSprite();
    }

    /// <summary>
    /// 업그레이드 메뉴를 비활성화하는 메서드입니다.
    /// </summary>
    void CloseUpgradeMenu()
    {
        _upgradeMenu.SetActive(false);
        GameManager.Instance.CurrentState = GameManager.GameState.Play;
        UpdateButtonSprite();
    }

    /// <summary>
    /// 버튼 스프라이트를 업데이트하는 메서드입니다.
    /// </summary>
    protected override void UpdateButtonSprite()
    {
        if (_upgradeMenu.activeSelf)
        {
            buttonImage.sprite = isClicked ? _menuCloseClick : _menuClose;
        }
        else
        {
            buttonImage.sprite = isClicked ? _menuOpenClick : _menuOpen;
        }
    }
}
