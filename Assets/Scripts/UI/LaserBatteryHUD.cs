using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ���͸� HUD�� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class LaserBatteryHUD : MonoBehaviour
{
    [SerializeField] Image _laserBatteryGauge;
    [SerializeField] Text _batteryText;
    [SerializeField] PlayerController _playerController;

    void Start()
    {
        // �ʱ�ȭ
        if (_playerController == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _playerController = playerObject.GetComponent<PlayerController>();
            }
            else
            {
                Debug.LogError("������ ���͸� HUD�� �÷��̾ ã�� �� �����! �÷��̾ �����ϴ���, �Ǵ� �÷��̾�� �±װ� ����� �Ҵ�ƴ��� üũ�� �ּ���.");
            }
        }
        if (_laserBatteryGauge == null)
        {
            var laserBatteryGaugeObject = transform.Find("LaserBatteryGauge").gameObject;
            if (laserBatteryGaugeObject != null )
            {
                _laserBatteryGauge = laserBatteryGaugeObject.GetComponent<Image>();
            }
            else
            {
                Debug.LogError("������ ���͸� HUD �ȿ� ������ ���͸� ������ �̹����� �����ϴ�!");
            }
        }
        if (_batteryText == null)
        {
            var batteryPercentageTextObject = transform.Find("BatteryText").gameObject;
            if (batteryPercentageTextObject != null)
            {
                _batteryText = batteryPercentageTextObject.GetComponent<Text>();
            }
            else
            {
                Debug.LogError("������ ���͸� HUD �ȿ� ������ ���͸� �ۼ�Ʈ �ؽ�Ʈ�� �����ϴ�!");
            }
        }

        // �̺�Ʈ ����
        _playerController.OnLaserBatteryUpdated += HandleLaserBatteryUpdated;
    }

    /// <summary>
    /// ������ ���͸��� ������Ʈ�Ǹ� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="currentBattery">���� ������ ���͸�</param>
    /// <param name="maxBattery">�ִ� ������ ���͸�</param>
    void HandleLaserBatteryUpdated(int currentBattery, int maxBattery)
    {
        _batteryText.text = currentBattery.ToString();
        _laserBatteryGauge.fillAmount = (float)currentBattery / maxBattery;
    }
}
