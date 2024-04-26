using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �ؽ�Ʈ UI�� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class CoinText : MonoBehaviour
{
    [SerializeField] PlayerCoin _playerCoin;    // �÷��̾� ����
    Text _coinText;                             // ���� �ؽ�Ʈ

    void Start()
    {
        // ���� �ؽ�Ʈ ĳ��
        _coinText = GetComponent<Text>();

        // ������ �÷��̾� ������Ʈ�� ã�Ƽ� PlayerCoin �Ҵ�
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerCoin = playerObject.GetComponent<PlayerCoin>();
        }
        else
        {
            Debug.LogError("���� �ؽ�Ʈ�� �÷��̾ ã�� �� �����! �÷��̾ �����ϴ���, �Ǵ� �÷��̾�� �±װ� ����� �Ҵ�ƴ��� üũ�� �ּ���.");
        }

        // ������ ������ ���� ���ϸ� �ؽ�Ʈ ������Ʈ
        _playerCoin.OnPlayerCoinUpdated += (int coin) => _coinText.text = coin.ToString();
    }
}
