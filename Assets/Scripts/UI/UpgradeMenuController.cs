using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 업그레이드의 데이터를 담고 있는 클래스입니다.
/// </summary>
[System.Serializable]
public class Upgrade
{
    /// <summary>
    /// 업그레이드 타입에 대한 열거형
    /// </summary>
    public enum UpgradeType
    {
        Reroll,
        HealthIncrease,
        DamageIncrease,
        BatteryIncrease,
        BatteryChargeIncrease,
        BonusCoinIncrease,
        PenetrationCountIncrease
    }

    public UpgradeType type;                    // 업그레이드 타입
    public Sprite iconSprte;                    // 아이콘 스프라이트
    public string description;                  // 업그레이드 설명
    public int defaultCost;                     // 기본 비용
    public int progressiveCost;                 // 업그레이드 할 때마다 증가하는 비용
    [HideInInspector] public int upgradeCount;  // 업그레이드 횟수
    [HideInInspector] public int currentCost;   // 현재 비용
}

/// <summary>
/// 메뉴의 아이템에 대한 데이터를 담고 있는 클래스입니다.
/// </summary>
[System.Serializable]
public class MenuItem
{
    /// <summary>
    /// Submit시 실행되는 액션에 대한 열거형입니다.
    /// </summary>
    public enum SubmitAction
    {
        Upgrade,
        Reroll,
    }

    public Image menuItemImage;                                 // 메뉴 아이템 게임 버튼 이미지
    public SubmitAction submitAction;                           // 메뉴 제출 입력 처리 이벤트
    public Image iconImage;                                     // 아이콘 이미지
    public Text descriptionText;                                // 설명 텍스트
    public Text costText;                                       // 비용에 대한 텍스트
    [SerializeField] Sprite _defaultSprite;                     // 기본 메뉴 스프라이트
    [SerializeField] Sprite _selectedSprite;                    // 선택 메뉴 스프라이트

    [HideInInspector] public Upgrade.UpgradeType currentUpgradeType;     // 현재 업그레이드

    /// <summary>
    /// 메뉴 아이템의 세부 아이템을 업데이트하는 메서드입니다.
    /// </summary>
    /// <param name="description">업그레이드 설명</param>
    /// <param name="cost">업그레이드 비용</param>
    /// <param name="iconSprite">업그레이드 아이콘</param>
    public void UpdateMenuItemDetails(string description, int cost, Sprite iconSprite = null)
    {
        if(iconSprite != null)
        {
            iconImage.sprite = iconSprite; ;
        }
        descriptionText.text = description;
        costText.text = string.Format("◆ {0}", cost);
    }

    /// <summary>
    /// 메뉴가 선택됐음을 시각적으로 처리하기 위한 메서드입니다.
    /// </summary>
    public void SelectMenuItem()
    {
        menuItemImage.sprite = _selectedSprite;
    }

    /// <summary>
    /// 메뉴가 선택되지 않았음을 시각적으로 처리하기 위한 메서드입니다.
    /// </summary>
    public void DeselectMenuItem()
    {
        menuItemImage.sprite = _defaultSprite;
    }
}

/// <summary>
/// 업그레이드 메뉴를 제어하는 클래스입니다.
/// </summary>
public class UpgradeMenuController : MonoBehaviour
{
    [SerializeField] List<MenuItem> _upgradeMenuItems = new List<MenuItem>();   // 업그레이드 메뉴 아이템의 리스트
    [SerializeField] Upgrade[] _upgrades;                                       // 업그레이드의 데이터들을 담아놓은 배열
    [SerializeField] Upgrade _reroll;                                           // 업그레이드 아이템 변경에 대한 정보
    [SerializeField] PlayerController _playerController;                        // 플레이어 컨트롤러
    [SerializeField] PlayerDamage _playerDamage;                                // 플레이어 대미지
    [SerializeField] PlayerCoin _playerCoin;                                    // 플레이어 코인
    [SerializeField] AudioClip _upgradeSound;                                   // 업그레이드 효과음

    int _currentMenuItemIndex = -1;                                                                                 // 현재 메뉴 아이템 인덱스
    Camera _camera;                                                                                                 // 카메라
    Dictionary<Upgrade.UpgradeType, Upgrade> _upgradesDictionary = new Dictionary<Upgrade.UpgradeType, Upgrade>();  // 업그레이드 데이터 딕셔너리

    /// <summary>
    /// 현재 메뉴 아이템 인덱스에 대한 프로퍼티입니다.
    /// </summary>
    public int CurrentMenuItemIndex
    {
        get => _currentMenuItemIndex;
        set
        {
            _currentMenuItemIndex = value;

            // 모든 메뉴 아이템의 이미지를 기본 스프라이트로 되돌린 뒤 현재 인덱스의 메뉴 아이템에 선택 스프라이트 적용
            foreach (var menuItem in _upgradeMenuItems)
            {
                menuItem.DeselectMenuItem();
            }

            // 현재 아이템 메뉴 인덱스가 0 이상일 때 특정 업그레이드 메뉴 아이템을 선택한 메뉴 아이템으로 설정
            if (CurrentMenuItemIndex >= 0)
            {
                _upgradeMenuItems[_currentMenuItemIndex].SelectMenuItem();
            }
        }
    }

    void Start()
    {
        // 카메라 캐시
        _camera = Camera.main.GetComponent<Camera>();

        // 플레이어와 관련된 컴포넌트 중 하나라도 없으면 게임 오브젝트를 불러와서 초기화
        if (_playerController == null || _playerDamage == null || _playerCoin == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _playerController = playerObject.GetComponent<PlayerController>();
                _playerDamage = playerObject.GetComponent<PlayerDamage>();
                _playerCoin = playerObject.GetComponent<PlayerCoin>();
            }
            else
            {
                Debug.LogError("플레이어를 찾을 수 없어요! 플레이어가 존재하는지, 또는 플레이어에게 태그가 제대로 할당됐는지 체크해 주세요.");
            }
        }

        for (int i = 0; i < _upgrades.Length; i++) 
        {
            // 업그레이드에 대한 데이터들을 딕셔너리에 보관
            _upgradesDictionary.Add(_upgrades[i].type, _upgrades[i]);
        }

        // 이벤트 연결
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
        InputManager.Instance.OnInputDown += HandleMenuItemSelection;
        InputManager.Instance.OnInputUp += HandleMenuActionSubmission;

        // 게임을 처음 실행할 때 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 입력이 시작될 때 호출되는 메서드로, 특정 메뉴를 선택하기 위한 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleMenuItemSelection(Vector2 inputPosition)
    {
        if(GameManager.Instance.CurrentState != GameManager.GameState.Upgrade)
        {
            // 현재 상태가 업그레이드 메뉴를 실행한 상태가 아닐 때 입력되는 것을 방지
            return;
        }

        for (int i = 0; i < _upgradeMenuItems.Count; i++)
        {
            // 특정 메뉴 아이템에 대한 입력이 확인되면 해당 메뉴를 입력한 메뉴로 선택하고 메서드를 중단
            if (IsPointerOverButton(inputPosition, _upgradeMenuItems[i].menuItemImage.rectTransform))
            {
                CurrentMenuItemIndex = i;
                return;
            }
        }

        // 입력된 메뉴 아이템이 없으면 인덱스를 -1로 설정
        CurrentMenuItemIndex = -1;
    }

    /// <summary>
    /// 입력이 종료될 때 호출되는 메서드로, 특정 메뉴를 제출하기 위한 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    void HandleMenuActionSubmission(Vector2 inputPosition)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Upgrade)
        {
            // 현재 상태가 업그레이드 메뉴를 실행한 상태가 아닐 때 입력되는 것을 방지
            return;
        }

        if (CurrentMenuItemIndex >= 0)
        {
            // 특정 메뉴 아이템에 대한 입력이 확인되면 업그레이드(혹은 변경) 수행
            if (IsPointerOverButton(inputPosition, _upgradeMenuItems[CurrentMenuItemIndex].menuItemImage.rectTransform))
            {
                PerformUpgrade();
            }
        }

        // 입력된 메뉴 아이템이 없으면 인덱스를 -1로 설정
        CurrentMenuItemIndex = -1;
    }

    /// <summary>
    /// 입력 좌표가 버튼 위에 있음을 체크하기 위한 메서드입니다.
    /// </summary>
    /// <param name="inputPosition">입력 좌표</param>
    /// <param name="_buttonRect">버튼의 RectTransform</param>
    /// <returns>입력 좌표가 버튼 위에 있는지 여부</returns>
    bool IsPointerOverButton(Vector2 inputPosition, RectTransform _buttonRect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(_buttonRect, inputPosition, _camera);
    }

    /// <summary>
    /// 업그레이드를 수행하는 메서드입니다.
    /// </summary>
    void PerformUpgrade()
    {
        Upgrade.UpgradeType upgradeType = _upgradeMenuItems[CurrentMenuItemIndex].currentUpgradeType;

        if (upgradeType != Upgrade.UpgradeType.Reroll)
        {
            // 업그레이드 타입이 Reroll이 아니면 업그레이드 적용
            // 플레이어가 소지한 코인이 비용보다 높으면 업그레이드 적용
            int cost = _upgradesDictionary[upgradeType].currentCost;
            if (_playerCoin.Coin >= cost)
            {
                // 플레이어의 돈 차감
                _playerCoin.Coin -= cost;

                // 현재 실행한 업그레이드의 비용을 높임
                _upgradesDictionary[upgradeType].currentCost += _upgradesDictionary[upgradeType].progressiveCost * (_upgradesDictionary[upgradeType].upgradeCount + 1);
                _upgradesDictionary[upgradeType].upgradeCount++;

                // 업그레이드 타입에 따라 업그레이드 적용
                switch (upgradeType)
                {
                    case Upgrade.UpgradeType.HealthIncrease:
                        // 체력 증가
                        _playerDamage.CurrentHealth++;
                        break;
                    case Upgrade.UpgradeType.DamageIncrease:
                        // 대미지 증가
                        _playerController.CurrentLaserDamage++;
                        break;
                    case Upgrade.UpgradeType.BatteryIncrease:
                        // 배터리 증가
                        _playerController.MaxLaserBattery += 20;
                        break;
                    case Upgrade.UpgradeType.BatteryChargeIncrease:
                        // 배터리 회복 속도 증가
                        _playerController.BatteryCharge++;
                        break;
                    case Upgrade.UpgradeType.BonusCoinIncrease:
                        // 추가 코인 증가
                        BonusCoin.BonusCointCount++;
                        break;
                    case Upgrade.UpgradeType.PenetrationCountIncrease:
                        // 관통 횟수 증가
                        _playerController.PenetrationCount++;
                        break;
                }

                // 동일한 업그레이드 메뉴 아이템의 텍스트 업데이트
                foreach (var upgradeMenuItem in _upgradeMenuItems)
                {
                    if (upgradeMenuItem.submitAction == _upgradeMenuItems[CurrentMenuItemIndex].submitAction)
                    {
                        UpdateMenuItemUpgradeDetails(upgradeMenuItem);
                    }
                }
                // 현재 입력한 버튼의 업그레이드를 무작위로 변경
                AssignRandomUpgradeToMenuItem(CurrentMenuItemIndex);
                // 사운드 재생
                SoundManager.Instance.PlaySFX(_upgradeSound);
            }
        }
        else
        {
            // 업그레이드 타입이 Reroll이면 변경 적용
            // 플레이어가 소지한 코인이 비용보다 높으면 변경 적용
            int cost = _reroll.currentCost;
            if (_playerCoin.Coin >= cost)
            {
                // 플레이어의 돈 차감
                _playerCoin.Coin -= cost;
                // 변경 비용 증가
                _reroll.currentCost += _reroll.progressiveCost * (_reroll.upgradeCount + 1);
                _reroll.upgradeCount++;
                // 모든 메뉴 아이템 변경
                RerollMenuItems();
                // 사운드 재생
                SoundManager.Instance.PlaySFX(_upgradeSound);
            }
        }
    }

    /// <summary>
    /// 변경 버튼을 눌렀을 때 모든 메뉴 아이템의 업그레이드를 변경하는 메서드입니다.
    /// 게임이 시작될 때도 호출됩니다.
    /// </summary>
    void RerollMenuItems()
    {
        for (int i = 0; i < _upgradeMenuItems.Count; i++)
        {
            _upgradeMenuItems[i].DeselectMenuItem();
            if (_upgradeMenuItems[i].submitAction == MenuItem.SubmitAction.Reroll)
            {
                _upgradeMenuItems[i].currentUpgradeType = _reroll.type;
                UpdateMenuItemUpgradeDetails(_upgradeMenuItems[i]);
            }
            else
            {
                // 메뉴 아이템 업그레이드 설정 초기화
                AssignRandomUpgradeToMenuItem(i);
            }
        }
    }

    /// <summary>
    /// 특정 메뉴 아이템에 무작위 업그레이드를 할당하는 메서드입니다.
    /// </summary>
    /// <param name="index">메뉴 아이템의 인덱스 값</param>
    void AssignRandomUpgradeToMenuItem(int index)
    {
        // 메뉴 아이템의 업그레이드 타입을 무작위로 적용
        int upgradeIndex = UnityEngine.Random.Range(0, _upgrades.Length);
        _upgradeMenuItems[index].currentUpgradeType = _upgrades[upgradeIndex].type;

        // 메뉴 아이템의 업그레이드에 대한 세부 사항 적용
        UpdateMenuItemUpgradeDetails(_upgradeMenuItems[index]);
    }

    /// <summary>
    /// 메뉴 아이템의 업그레이드에 대한 세부 사항을 업데이트하는 메서드입니다.
    /// </summary>
    /// <param name="upgradeMenuItem">세부 사항을 업데이트하려는 메뉴 아이템</param>
    void UpdateMenuItemUpgradeDetails(MenuItem upgradeMenuItem)
    {
        var type = upgradeMenuItem.currentUpgradeType;

        Sprite spriteIcon = null;
        string newDescription;
        int newCost;

        if (type != Upgrade.UpgradeType.Reroll)
        {
            spriteIcon = _upgradesDictionary[type].iconSprte;
            newDescription = _upgradesDictionary[type].description;
            newCost = _upgradesDictionary[type].currentCost;
        }
        else
        {
            newDescription = _reroll.description;
            newCost = _reroll.currentCost;
        }
        
        // 가져온 값들을 메뉴 아이템에 적용
        upgradeMenuItem.UpdateMenuItemDetails(newDescription, newCost, spriteIcon);
    }

    /// <summary>
    /// 게임이 시작될 때 호출되는 메서드입니다.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // 초기화
        for (int i = 0; i < _upgrades.Length; i++)
        {
            _upgrades[i].currentCost = _upgrades[i].defaultCost;
        }
        _reroll.currentCost = _reroll.defaultCost;
        RerollMenuItems();
    }
}
