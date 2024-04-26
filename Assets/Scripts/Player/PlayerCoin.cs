using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 보유한 코인 관리 및 코인 획득을 처리하는 클래스입니다. 
/// </summary>
public class PlayerCoin : MonoBehaviour
{
    // 플레이어가 보유한 코인의 수가 변하면 호출되는 이벤트
    public Action<int> OnPlayerCoinUpdated;

    [SerializeField] AudioClip _coinSound;  // 코인 사운드
    [SerializeField] LayerMask _coinLayer;  // 코인 레이어

    int _coin;              // 보유한 코인
    Transform _transform;   // 트랜스폼 캐시

    /// <summary>
    /// 플레이어가 보유한 코인에 대한 프로퍼티입니다.
    /// </summary>
    public int Coin
    {
        get => _coin;
        set
        {
            if(_coin != value)
            {
                _coin = value;
                OnPlayerCoinUpdated?.Invoke(value);
            }
        }
    }

    void Start()
    {
        // 트랜스폼 캐시
        _transform = GetComponent<Transform>();
        
        // 게임이 시작될 때 보유한 코인의 수를 0으로 만듬
        GameManager.Instance.OnGamePlayStarted += () => Coin = 0;
    }

    void Update()
    {
        // 코인 오브젝트의 충돌 감지 처리
        var coinCollider = Physics2D.OverlapCircle(_transform.position, 1f, _coinLayer);
        if (coinCollider)
        {
            Coin += 1;
            SoundManager.Instance.PlaySFX(_coinSound);
            ObjectPoolManager.Instance.ReturnToPool(coinCollider.gameObject);
        }
    }
}
