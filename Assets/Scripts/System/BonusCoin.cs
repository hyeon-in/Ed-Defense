using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 추가로 드롭하는 코인의 량을 관리하는 정적 클래스입니다.
/// </summary>
public static class BonusCoin
{
    public static int BonusCointCount = 0;  // 추가 코인의 량

    static BonusCoin()
    {
        // 게임이 시작될 때 마다 추가 코인의 량을 0으로 초기화
        GameManager.Instance.OnGamePlayStarted += () => BonusCointCount = 0;
    }
}
