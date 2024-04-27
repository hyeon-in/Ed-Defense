using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 풀 대상 오브젝트에 대한 정보를 담아놓은 클래스입니다. 
/// </summary>
[System.Serializable]
public class Pool
{
    public string poolName;
    public GameObject prefab;
    public int initialSize = 10;

    public Pool(string poolName, GameObject prefab, int initialSize, bool isUIElement = false)
    {
        this.poolName = poolName;
        this.prefab = prefab;
        this.initialSize = initialSize;
    }
}

/// <summary>
/// 오브젝트 풀을 관리하기 위한 싱글톤 클래스입니다.
/// </summary>
public class ObjectPoolManager : SingletonManager<ObjectPoolManager>
{
    // 오브젝트 풀 리스트
    [SerializeField] List<Pool> _poolList;

    // 풀 오브젝트 딕셔너리
    Dictionary<string, GameObject> _parentObjectDictionary = new Dictionary<string, GameObject>();
    Dictionary<string, Stack<GameObject>> _poolObjectDictionary = new Dictionary<string, Stack<GameObject>>();
    Dictionary<GameObject, string> _poolNameByGameObject = new Dictionary<GameObject, string>();

    /// <summary>
    /// 오브젝트 풀 매니저 초기화
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < _poolList.Count; i++)
        {
            // 풀 리스트에 있는 오브젝트 풀 생성
            CreateObjectPool(_poolList[i]);
        }
    }

    /// <summary>
    /// 오브젝트 풀에 새로운 오브젝트를 생성합니다.
    /// </summary>
    /// <param name="pool">생성하려는 풀 오브젝트</param>
    public void AddPool(Pool pool)
    {
        if (!_poolObjectDictionary.ContainsKey(pool.poolName))
        {
            // 딕셔너리를 체크해서 아직 없는 오브젝트 풀이면 새로 생성
            _poolList.Add(pool);
            CreateObjectPool(pool);
        }
        else
        {
            // 이미 있는 오브젝트 풀이면 추가 생성
            var parentObject = _parentObjectDictionary[pool.poolName];

            for (int i = 0; i < pool.initialSize; i++)
            {
                var obj = Instantiate(pool.prefab, parentObject.transform);
                obj.SetActive(false);
                _poolNameByGameObject.Add(obj, pool.poolName);
                _poolObjectDictionary[pool.poolName].Push(obj);
            }
        }
    }


    /// <summary>
    /// 풀로부터 새로운 풀 오브젝트를 생성함
    /// </summary>
    void CreateObjectPool(Pool pool)
    {
        // 풀 오브젝트를 보관할 부모 오브젝트 추가
        GameObject parentObject = new GameObject() {  name = pool.poolName };
        parentObject.transform.parent = transform;

        // 오브젝트 풀 새로 생성
        Stack<GameObject> objectPool = new Stack<GameObject>();

        for (int i = 0; i < pool.initialSize; i++)
        {
            var obj = Instantiate(pool.prefab, parentObject.transform);
            obj.SetActive(false);
            objectPool.Push(obj);
            _poolNameByGameObject.Add(obj, pool.poolName);
        }

        _poolObjectDictionary.Add(pool.poolName, objectPool);
        _parentObjectDictionary.Add(pool.poolName, parentObject);
    }

    /// <summary>
    /// 오브젝트 풀에서 특정한 게임 오브젝트를 가져옵니다.
    /// </summary>
    public GameObject SpawnFromPool(string poolName, Vector2 position, float angle = 0f)
    {
        if(_poolObjectDictionary.ContainsKey(poolName))
        {
            // 풀 오브젝트 딕셔너리안에 키 값이 존재하면
            if (_poolObjectDictionary[poolName].Count == 0)
            {
                // 오브젝트 풀이 비어있다면 추가 생성
#if UNITY_EDITOR
                Debug.Log("오브젝트 풀 " + poolName + "이 비어있습니다. 새로운 오브젝트를 추가합니다.");
#endif
                Pool pool = _poolList.Find(p => p.poolName == poolName);
                if(pool != null)
                {
                    var parentObject = _parentObjectDictionary[pool.poolName];

                    for (int i = 0; i < pool.initialSize; i++)
                    {
                        GameObject newObj = Instantiate(pool.prefab, parentObject.transform);
                        newObj.SetActive(false);
                        _poolNameByGameObject.Add(newObj, pool.poolName);
                        _poolObjectDictionary[pool.poolName].Push(newObj);
                    }
                }
                else
                {
                    // 풀 리스트에 동일한 풀 오브젝트가 없다면 에러를 출력하고 null 반환
#if UNITY_EDITOR
                    Debug.LogError("오브젝트 풀에 " + poolName + "이라는 오브젝트는 존재하지 않습니다.");
#endif
                    return null;
                }
            }

            // 풀 오브젝트를 딕셔너리에서 가져옴
            GameObject spawnObject = _poolObjectDictionary[poolName].Pop();


            if (spawnObject.activeSelf)
            {
#if UNITY_EDITOR
                // 현재 활동하고 있는 오브젝트이면 에러를 출력하고 null 반환
                Debug.LogError(spawnObject.name + "은 이미 생성된 오브젝트임에도 불러오려고 하고 있습니다! 해당 오브젝트가 정상적으로 오브젝트 풀에 반환됐는지 확인 해 주세요!");
#endif
                return null;
            }

            // 설정 및 활성화 후 object 반환
            spawnObject.transform.position = position;
            spawnObject.transform.rotation = Quaternion.Euler(0, 0, angle);
            spawnObject.SetActive(true);
            return spawnObject;
        }

#if UNITY_EDITOR
        Debug.LogError("오브젝트 풀에 " + poolName + "이라는 오브젝트는 존재하지 않습니다.");
#endif
        return null;
    }

    /// <summary>
    /// 가져갔던 오브젝트를 오브젝트 풀에 다시 반환하는 메서드입니다.
    /// </summary>
    public void ReturnToPool(GameObject returnObject)
    {
#if UNITY_EDITOR
        if (!_poolNameByGameObject.ContainsKey(returnObject))
        {
            Destroy(returnObject);
            Debug.LogError("오브젝트 풀에 " + returnObject.name + "이라는 오브젝트는 존재하지 않습니다.");
        }
#endif
        string poolName = _poolNameByGameObject[returnObject];

        if (_poolObjectDictionary.ContainsKey(poolName))
        {
            // 오브젝트를 비활성화하고 오브젝트 풀에 Push
            returnObject.SetActive(false);
            _poolObjectDictionary[poolName].Push(returnObject);
        }
#if UNITY_EDITOR
        else
        {
            Destroy(returnObject);
            Debug.LogError("오브젝트 풀에 " + poolName + "이라는 오브젝트는 존재하지 않습니다.");
        }
#endif
    }
}