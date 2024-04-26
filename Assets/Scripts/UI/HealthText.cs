using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ü�� �ؽ�Ʈ UI�� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class HealthText : MonoBehaviour
{
    [SerializeField] PlayerDamage _playerDamage;    // �÷��̾� ����� ĳ��
    Text _healthText;                               // ü�� �ؽ�Ʈ

    void Start()
    {
        // ü�� �ؽ�Ʈ ĳ��
        _healthText = GetComponent<Text>();

        // ������ �÷��̾� ������Ʈ�� ã�Ƽ� PlayerDamage �Ҵ�
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerDamage = playerObject.GetComponent<PlayerDamage>();
        }
        else
        {
            Debug.LogError("ü�� �ؽ�Ʈ�� �÷��̾ ã�� �� �����! �÷��̾ �����ϴ���, �Ǵ� �÷��̾�� �±װ� ����� �Ҵ�ƴ��� üũ�� �ּ���.");
        }

        // ���� ü���� ���ϸ� UI�� �ؽ�Ʈ ����
        _playerDamage.OnCurrentHealthChanged += (int currentHealth) => _healthText.text = currentHealth.ToString();
    }
}
