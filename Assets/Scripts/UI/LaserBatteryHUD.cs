using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 레이저 배터리 HUD를 관리하는 클래스입니다.
/// </summary>
public class LaserBatteryHUD : MonoBehaviour
{
    [SerializeField] Image _laserBatteryGauge;
    [SerializeField] Text _batteryText;
    [SerializeField] PlayerController _playerController;

    void Start()
    {
        // 초기화
        if (_playerController == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _playerController = playerObject.GetComponent<PlayerController>();
            }
            else
            {
                Debug.LogError("레이저 배터리 HUD가 플레이어를 찾을 수 없어요! 플레이어가 존재하는지, 또는 플레이어에게 태그가 제대로 할당됐는지 체크해 주세요.");
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
                Debug.LogError("레이저 배터리 HUD 안에 레이저 배터리 게이지 이미지가 없습니다!");
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
                Debug.LogError("레이저 배터리 HUD 안에 레이저 배터리 퍼센트 텍스트가 없습니다!");
            }
        }

        // 이벤트 연결
        _playerController.OnLaserBatteryUpdated += HandleLaserBatteryUpdated;
    }

    /// <summary>
    /// 레이저 배터리가 업데이트되면 호출되는 메서드입니다.
    /// </summary>
    /// <param name="currentBattery">현재 레이저 배터리</param>
    /// <param name="maxBattery">최대 레이저 배터리</param>
    void HandleLaserBatteryUpdated(int currentBattery, int maxBattery)
    {
        _batteryText.text = currentBattery.ToString();
        _laserBatteryGauge.fillAmount = (float)currentBattery / maxBattery;
    }
}
