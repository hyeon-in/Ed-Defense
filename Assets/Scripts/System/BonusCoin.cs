using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �߰��� ����ϴ� ������ ���� �����ϴ� ���� Ŭ�����Դϴ�.
/// </summary>
public static class BonusCoin
{
    public static int BonusCointCount = 0;  // �߰� ������ ��

    static BonusCoin()
    {
        // ������ ���۵� �� ���� �߰� ������ ���� 0���� �ʱ�ȭ
        GameManager.Instance.OnGamePlayStarted += () => BonusCointCount = 0;
    }
}
