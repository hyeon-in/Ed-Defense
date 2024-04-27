using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 데이터를 담고 있는 스크립터블 오브젝트 클래스입니다.
/// </summary>
[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data", order = 1)]
public class EnemyDataSO : ScriptableObject
{
    public int defaultHealth = 10;              // 적의 기본 체력
    public float defaultMoveSpeed = 1f;         // 적의 기본 이동속도
    public int coinDropAmount = 3;              // 드롭하는 코인의 량
    public Material whiteFlashEffectMaterial;   // 흰색으로 번쩍이는 이펙트를 처리하는 머테리얼
    public AudioClip hitSound;                  // 공격 받을 때 사운드
    public AudioClip weekendFarmSound;          // 주말농장으로 갔을 때 사운드
}
