using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : MonoBehaviour
{
    private readonly Stack<T> pool = new Stack<T>();
    private readonly T prefab;
    private readonly Transform parent;

    public GameObjectPool(T prefab, int initialSize = 0, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
            pool.Push(CreateInstance());
    }

    private T CreateInstance()
    {
        T instance = Object.Instantiate(prefab, parent);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public T Spawn(Vector3 position = default, Quaternion rotation = default)
    {
        T instance = pool.Count > 0 ? pool.Pop() : CreateInstance();
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Push(obj);
    }

    public void Clear()
    {
        while (pool.Count > 0)
            Object.Destroy(pool.Pop().gameObject);
        pool.Clear();
    }
}
