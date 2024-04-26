using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 싱글톤 매니저의 기반이 되는 추상 클래스입니다.
/// </summary>
/// <typeparam name="T">싱글톤으로 만들 MonoBehaviour의 유형</typeparam>
public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// 싱글톤 인스턴스에 접근하는 속성 프로퍼티입니다.
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