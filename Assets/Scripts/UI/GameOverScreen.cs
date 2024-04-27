using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ȭ���� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class GameOverScreen : MonoBehaviour
{
    void Start()
    {
        // ���� ������ �Ǹ� ���� ���� ȭ���� ���̰� ����
        GameManager.Instance.OnGameOver += () => gameObject.SetActive(true);
        // ������ ���۵Ǹ� ���� ���� ȭ���� ������ �ʰ� ����
        GameManager.Instance.OnGamePlayStarted += () => gameObject.SetActive(false);
        
        // ������ ó�� ����� �� �׻� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
