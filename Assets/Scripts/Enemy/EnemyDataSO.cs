using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �����͸� ��� �ִ� ��ũ���ͺ� ������Ʈ Ŭ�����Դϴ�.
/// </summary>
[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data", order = 1)]
public class EnemyDataSO : ScriptableObject
{
    public int defaultHealth = 10;              // ���� �⺻ ü��
    public float defaultMoveSpeed = 1f;         // ���� �⺻ �̵��ӵ�
    public int coinDropAmount = 3;              // ����ϴ� ������ ��
    public Material whiteFlashEffectMaterial;   // ������� ��½�̴� ����Ʈ�� ó���ϴ� ���׸���
    public AudioClip hitSound;                  // ���� ���� �� ����
    public AudioClip weekendFarmSound;          // �ָ��������� ���� �� ����
}
