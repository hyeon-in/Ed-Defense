using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� ������ ���� ���� �� ���� ȹ���� ó���ϴ� Ŭ�����Դϴ�. 
/// </summary>
public class PlayerCoin : MonoBehaviour
{
    // �÷��̾ ������ ������ ���� ���ϸ� ȣ��Ǵ� �̺�Ʈ
    public Action<int> OnPlayerCoinUpdated;

    [SerializeField] AudioClip _coinSound;  // ���� ����
    [SerializeField] LayerMask _coinLayer;  // ���� ���̾�

    int _coin;              // ������ ����
    Transform _transform;   // Ʈ������ ĳ��

    /// <summary>
    /// �÷��̾ ������ ���ο� ���� ������Ƽ�Դϴ�.
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
        // Ʈ������ ĳ��
        _transform = GetComponent<Transform>();
        
        // ������ ���۵� �� ������ ������ ���� 0���� ����
        GameManager.Instance.OnGamePlayStarted += () => Coin = 0;
    }

    void Update()
    {
        // ���� ������Ʈ�� �浹 ���� ó��
        var coinCollider = Physics2D.OverlapCircle(_transform.position, 1f, _coinLayer);
        if (coinCollider)
        {
            Coin += 1;
            SoundManager.Instance.PlaySFX(_coinSound);
            ObjectPoolManager.Instance.ReturnToPool(coinCollider.gameObject);
        }
    }
}
