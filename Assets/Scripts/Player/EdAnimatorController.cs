using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̵��� �ִϸ����͸� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class EdAnimatorController : MonoBehaviour
{
    // �ִϸ��̼� �ؽ�
    readonly int HashGameStart = Animator.StringToHash("GameStart");
    readonly int HashGameOver = Animator.StringToHash("GameOver");

    Animator _animator; // �ִϸ�����

    void Start()
    {
        // �ִϸ����� ĳ��
        _animator = GetComponent<Animator>();

        // ���ӸŴ����� �̺�Ʈ ����
        GameManager.Instance.OnGamePlayStarted += () => _animator.SetTrigger(HashGameStart);
        GameManager.Instance.OnGameOver += () => _animator.SetTrigger(HashGameOver);
    }
}
