using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 코인 텍스트 UI를 관리하는 클래스입니다.
/// </summary>
public class CoinText : MonoBehaviour
{
    [SerializeField] PlayerCoin _playerCoin;    // 플레이어 코인
    Text _coinText;                             // 코인 텍스트

    void Start()
    {
        // 코인 텍스트 캐시
        _coinText = GetComponent<Text>();

        // 씬에서 플레이어 오브젝트를 찾아서 PlayerCoin 할당
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerCoin = playerObject.GetComponent<PlayerCoin>();
        }
        else
        {
            Debug.LogError("코인 텍스트가 플레이어를 찾을 수 없어요! 플레이어가 존재하는지, 또는 플레이어에게 태그가 제대로 할당됐는지 체크해 주세요.");
        }

        // 보유한 코인의 수가 변하면 텍스트 업데이트
        _playerCoin.OnPlayerCoinUpdated += (int coin) => _coinText.text = coin.ToString();
    }
}
