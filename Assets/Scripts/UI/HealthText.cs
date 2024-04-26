using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 체력 텍스트 UI를 관리하는 클래스입니다.
/// </summary>
public class HealthText : MonoBehaviour
{
    [SerializeField] PlayerDamage _playerDamage;    // 플레이어 대미지 캐시
    Text _healthText;                               // 체력 텍스트

    void Start()
    {
        // 체력 텍스트 캐시
        _healthText = GetComponent<Text>();

        // 씬에서 플레이어 오브젝트를 찾아서 PlayerDamage 할당
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerDamage = playerObject.GetComponent<PlayerDamage>();
        }
        else
        {
            Debug.LogError("체력 텍스트가 플레이어를 찾을 수 없어요! 플레이어가 존재하는지, 또는 플레이어에게 태그가 제대로 할당됐는지 체크해 주세요.");
        }

        // 현재 체력이 변하면 UI의 텍스트 변경
        _playerDamage.OnCurrentHealthChanged += (int currentHealth) => _healthText.text = currentHealth.ToString();
    }
}
