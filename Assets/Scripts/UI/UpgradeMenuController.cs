using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���׷��̵��� �����͸� ��� �ִ� Ŭ�����Դϴ�.
/// </summary>
[System.Serializable]
public class Upgrade
{
    /// <summary>
    /// ���׷��̵� Ÿ�Կ� ���� ������
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

    public UpgradeType type;                    // ���׷��̵� Ÿ��
    public Sprite iconSprte;                    // ������ ��������Ʈ
    public string description;                  // ���׷��̵� ����
    public int defaultCost;                     // �⺻ ���
    public int progressiveCost;                 // ���׷��̵� �� ������ �����ϴ� ���
    [HideInInspector] public int upgradeCount;  // ���׷��̵� Ƚ��
    [HideInInspector] public int currentCost;   // ���� ���
}

/// <summary>
/// �޴��� �����ۿ� ���� �����͸� ��� �ִ� Ŭ�����Դϴ�.
/// </summary>
[System.Serializable]
public class MenuItem
{
    /// <summary>
    /// Submit�� ����Ǵ� �׼ǿ� ���� �������Դϴ�.
    /// </summary>
    public enum SubmitAction
    {
        Upgrade,
        Reroll,
    }

    public Image menuItemImage;                                 // �޴� ������ ���� ��ư �̹���
    public SubmitAction submitAction;                           // �޴� ���� �Է� ó�� �̺�Ʈ
    public Image iconImage;                                     // ������ �̹���
    public Text descriptionText;                                // ���� �ؽ�Ʈ
    public Text costText;                                       // ��뿡 ���� �ؽ�Ʈ
    [SerializeField] Sprite _defaultSprite;                     // �⺻ �޴� ��������Ʈ
    [SerializeField] Sprite _selectedSprite;                    // ���� �޴� ��������Ʈ

    [HideInInspector] public Upgrade.UpgradeType currentUpgradeType;     // ���� ���׷��̵�

    /// <summary>
    /// �޴� �������� ���� �������� ������Ʈ�ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="description">���׷��̵� ����</param>
    /// <param name="cost">���׷��̵� ���</param>
    /// <param name="iconSprite">���׷��̵� ������</param>
    public void UpdateMenuItemDetails(string description, int cost, Sprite iconSprite = null)
    {
        if(iconSprite != null)
        {
            iconImage.sprite = iconSprite; ;
        }
        descriptionText.text = description;
        costText.text = string.Format("�� {0}", cost);
    }

    /// <summary>
    /// �޴��� ���õ����� �ð������� ó���ϱ� ���� �޼����Դϴ�.
    /// </summary>
    public void SelectMenuItem()
    {
        menuItemImage.sprite = _selectedSprite;
    }

    /// <summary>
    /// �޴��� ���õ��� �ʾ����� �ð������� ó���ϱ� ���� �޼����Դϴ�.
    /// </summary>
    public void DeselectMenuItem()
    {
        menuItemImage.sprite = _defaultSprite;
    }
}

/// <summary>
/// ���׷��̵� �޴��� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class UpgradeMenuController : MonoBehaviour
{
    [SerializeField] List<MenuItem> _upgradeMenuItems = new List<MenuItem>();   // ���׷��̵� �޴� �������� ����Ʈ
    [SerializeField] Upgrade[] _upgrades;                                       // ���׷��̵��� �����͵��� ��Ƴ��� �迭
    [SerializeField] Upgrade _reroll;                                           // ���׷��̵� ������ ���濡 ���� ����
    [SerializeField] PlayerController _playerController;                        // �÷��̾� ��Ʈ�ѷ�
    [SerializeField] PlayerDamage _playerDamage;                                // �÷��̾� �����
    [SerializeField] PlayerCoin _playerCoin;                                    // �÷��̾� ����
    [SerializeField] AudioClip _upgradeSound;                                   // ���׷��̵� ȿ����

    int _currentMenuItemIndex = -1;                                                                                 // ���� �޴� ������ �ε���
    Camera _camera;                                                                                                 // ī�޶�
    Dictionary<Upgrade.UpgradeType, Upgrade> _upgradesDictionary = new Dictionary<Upgrade.UpgradeType, Upgrade>();  // ���׷��̵� ������ ��ųʸ�

    /// <summary>
    /// ���� �޴� ������ �ε����� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int CurrentMenuItemIndex
    {
        get => _currentMenuItemIndex;
        set
        {
            _currentMenuItemIndex = value;

            // ��� �޴� �������� �̹����� �⺻ ��������Ʈ�� �ǵ��� �� ���� �ε����� �޴� �����ۿ� ���� ��������Ʈ ����
            foreach (var menuItem in _upgradeMenuItems)
            {
                menuItem.DeselectMenuItem();
            }

            // ���� ������ �޴� �ε����� 0 �̻��� �� Ư�� ���׷��̵� �޴� �������� ������ �޴� ���������� ����
            if (CurrentMenuItemIndex >= 0)
            {
                _upgradeMenuItems[_currentMenuItemIndex].SelectMenuItem();
            }
        }
    }

    void Start()
    {
        // ī�޶� ĳ��
        _camera = Camera.main.GetComponent<Camera>();

        // �÷��̾�� ���õ� ������Ʈ �� �ϳ��� ������ ���� ������Ʈ�� �ҷ��ͼ� �ʱ�ȭ
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
                Debug.LogError("�÷��̾ ã�� �� �����! �÷��̾ �����ϴ���, �Ǵ� �÷��̾�� �±װ� ����� �Ҵ�ƴ��� üũ�� �ּ���.");
            }
        }

        for (int i = 0; i < _upgrades.Length; i++) 
        {
            // ���׷��̵忡 ���� �����͵��� ��ųʸ��� ����
            _upgradesDictionary.Add(_upgrades[i].type, _upgrades[i]);
        }

        // �̺�Ʈ ����
        GameManager.Instance.OnGamePlayStarted += HandleGamePlayStarted;
        InputManager.Instance.OnInputDown += HandleMenuItemSelection;
        InputManager.Instance.OnInputUp += HandleMenuActionSubmission;

        // ������ ó�� ������ �� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �Է��� ���۵� �� ȣ��Ǵ� �޼����, Ư�� �޴��� �����ϱ� ���� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleMenuItemSelection(Vector2 inputPosition)
    {
        if(GameManager.Instance.CurrentState != GameManager.GameState.Upgrade)
        {
            // ���� ���°� ���׷��̵� �޴��� ������ ���°� �ƴ� �� �ԷµǴ� ���� ����
            return;
        }

        for (int i = 0; i < _upgradeMenuItems.Count; i++)
        {
            // Ư�� �޴� �����ۿ� ���� �Է��� Ȯ�εǸ� �ش� �޴��� �Է��� �޴��� �����ϰ� �޼��带 �ߴ�
            if (IsPointerOverButton(inputPosition, _upgradeMenuItems[i].menuItemImage.rectTransform))
            {
                CurrentMenuItemIndex = i;
                return;
            }
        }

        // �Էµ� �޴� �������� ������ �ε����� -1�� ����
        CurrentMenuItemIndex = -1;
    }

    /// <summary>
    /// �Է��� ����� �� ȣ��Ǵ� �޼����, Ư�� �޴��� �����ϱ� ���� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleMenuActionSubmission(Vector2 inputPosition)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Upgrade)
        {
            // ���� ���°� ���׷��̵� �޴��� ������ ���°� �ƴ� �� �ԷµǴ� ���� ����
            return;
        }

        if (CurrentMenuItemIndex >= 0)
        {
            // Ư�� �޴� �����ۿ� ���� �Է��� Ȯ�εǸ� ���׷��̵�(Ȥ�� ����) ����
            if (IsPointerOverButton(inputPosition, _upgradeMenuItems[CurrentMenuItemIndex].menuItemImage.rectTransform))
            {
                PerformUpgrade();
            }
        }

        // �Էµ� �޴� �������� ������ �ε����� -1�� ����
        CurrentMenuItemIndex = -1;
    }

    /// <summary>
    /// �Է� ��ǥ�� ��ư ���� ������ üũ�ϱ� ���� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    /// <param name="_buttonRect">��ư�� RectTransform</param>
    /// <returns>�Է� ��ǥ�� ��ư ���� �ִ��� ����</returns>
    bool IsPointerOverButton(Vector2 inputPosition, RectTransform _buttonRect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(_buttonRect, inputPosition, _camera);
    }

    /// <summary>
    /// ���׷��̵带 �����ϴ� �޼����Դϴ�.
    /// </summary>
    void PerformUpgrade()
    {
        Upgrade.UpgradeType upgradeType = _upgradeMenuItems[CurrentMenuItemIndex].currentUpgradeType;

        if (upgradeType != Upgrade.UpgradeType.Reroll)
        {
            // ���׷��̵� Ÿ���� Reroll�� �ƴϸ� ���׷��̵� ����
            // �÷��̾ ������ ������ ��뺸�� ������ ���׷��̵� ����
            int cost = _upgradesDictionary[upgradeType].currentCost;
            if (_playerCoin.Coin >= cost)
            {
                // �÷��̾��� �� ����
                _playerCoin.Coin -= cost;

                // ���� ������ ���׷��̵��� ����� ����
                _upgradesDictionary[upgradeType].currentCost += _upgradesDictionary[upgradeType].progressiveCost * (_upgradesDictionary[upgradeType].upgradeCount + 1);
                _upgradesDictionary[upgradeType].upgradeCount++;

                // ���׷��̵� Ÿ�Կ� ���� ���׷��̵� ����
                switch (upgradeType)
                {
                    case Upgrade.UpgradeType.HealthIncrease:
                        // ü�� ����
                        _playerDamage.CurrentHealth++;
                        break;
                    case Upgrade.UpgradeType.DamageIncrease:
                        // ����� ����
                        _playerController.CurrentLaserDamage++;
                        break;
                    case Upgrade.UpgradeType.BatteryIncrease:
                        // ���͸� ����
                        _playerController.MaxLaserBattery += 20;
                        break;
                    case Upgrade.UpgradeType.BatteryChargeIncrease:
                        // ���͸� ȸ�� �ӵ� ����
                        _playerController.BatteryCharge++;
                        break;
                    case Upgrade.UpgradeType.BonusCoinIncrease:
                        // �߰� ���� ����
                        BonusCoin.BonusCointCount++;
                        break;
                    case Upgrade.UpgradeType.PenetrationCountIncrease:
                        // ���� Ƚ�� ����
                        _playerController.PenetrationCount++;
                        break;
                }

                // ������ ���׷��̵� �޴� �������� �ؽ�Ʈ ������Ʈ
                foreach (var upgradeMenuItem in _upgradeMenuItems)
                {
                    if (upgradeMenuItem.submitAction == _upgradeMenuItems[CurrentMenuItemIndex].submitAction)
                    {
                        UpdateMenuItemUpgradeDetails(upgradeMenuItem);
                    }
                }
                // ���� �Է��� ��ư�� ���׷��̵带 �������� ����
                AssignRandomUpgradeToMenuItem(CurrentMenuItemIndex);
                // ���� ���
                SoundManager.Instance.PlaySFX(_upgradeSound);
            }
        }
        else
        {
            // ���׷��̵� Ÿ���� Reroll�̸� ���� ����
            // �÷��̾ ������ ������ ��뺸�� ������ ���� ����
            int cost = _reroll.currentCost;
            if (_playerCoin.Coin >= cost)
            {
                // �÷��̾��� �� ����
                _playerCoin.Coin -= cost;
                // ���� ��� ����
                _reroll.currentCost += _reroll.progressiveCost * (_reroll.upgradeCount + 1);
                _reroll.upgradeCount++;
                // ��� �޴� ������ ����
                RerollMenuItems();
                // ���� ���
                SoundManager.Instance.PlaySFX(_upgradeSound);
            }
        }
    }

    /// <summary>
    /// ���� ��ư�� ������ �� ��� �޴� �������� ���׷��̵带 �����ϴ� �޼����Դϴ�.
    /// ������ ���۵� ���� ȣ��˴ϴ�.
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
                // �޴� ������ ���׷��̵� ���� �ʱ�ȭ
                AssignRandomUpgradeToMenuItem(i);
            }
        }
    }

    /// <summary>
    /// Ư�� �޴� �����ۿ� ������ ���׷��̵带 �Ҵ��ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="index">�޴� �������� �ε��� ��</param>
    void AssignRandomUpgradeToMenuItem(int index)
    {
        // �޴� �������� ���׷��̵� Ÿ���� �������� ����
        int upgradeIndex = UnityEngine.Random.Range(0, _upgrades.Length);
        _upgradeMenuItems[index].currentUpgradeType = _upgrades[upgradeIndex].type;

        // �޴� �������� ���׷��̵忡 ���� ���� ���� ����
        UpdateMenuItemUpgradeDetails(_upgradeMenuItems[index]);
    }

    /// <summary>
    /// �޴� �������� ���׷��̵忡 ���� ���� ������ ������Ʈ�ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="upgradeMenuItem">���� ������ ������Ʈ�Ϸ��� �޴� ������</param>
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
        
        // ������ ������ �޴� �����ۿ� ����
        upgradeMenuItem.UpdateMenuItemDetails(newDescription, newCost, spriteIcon);
    }

    /// <summary>
    /// ������ ���۵� �� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    void HandleGamePlayStarted()
    {
        // �ʱ�ȭ
        for (int i = 0; i < _upgrades.Length; i++)
        {
            _upgrades[i].currentCost = _upgrades[i].defaultCost;
        }
        _reroll.currentCost = _reroll.defaultCost;
        RerollMenuItems();
    }
}
