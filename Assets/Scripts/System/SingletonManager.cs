using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� �̱��� �Ŵ����� ����� �Ǵ� �߻� Ŭ�����Դϴ�.
/// </summary>
/// <typeparam name="T">�̱������� ���� MonoBehaviour�� ����</typeparam>
public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// �̱��� �ν��Ͻ��� �����ϴ� �Ӽ� ������Ƽ�Դϴ�.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj;
                obj = GameObject.Find(typeof(T).Name);
                if (obj == null)
                {
                    obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
                else
                {
                    _instance = obj.GetComponent<T>();
                }
            }
            return _instance;
        }
        set => _instance = value;
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}