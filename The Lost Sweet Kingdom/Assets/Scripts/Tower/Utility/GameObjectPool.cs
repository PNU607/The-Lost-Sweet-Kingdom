using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : MonoBehaviour
{
    private readonly Stack<T> pool = new Stack<T>();
    private readonly T prefab;
    private readonly Transform parent;
    private readonly int initialSize;

    public GameObjectPool(T prefab, int initialSize = 0, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.initialSize = initialSize;

        //for (int i = 0; i < initialSize; i++)
        //    pool.Push(CreateInstance());

        IncreaseIntrance(initialSize);
    }

    private T CreateInstance()
    {
        T instance = Object.Instantiate(prefab, parent);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public T Spawn(Vector3 position = default, Quaternion rotation = default)
    {
        if (pool.Count <= 0)
        {
            IncreaseIntrance(initialSize / 10);
        }

        T instance = pool.Pop();
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.gameObject.SetActive(true);
        return instance;
    }

    private void IncreaseIntrance(int count)
    {
        for (int i = 0; i < count; i++)
            pool.Push(CreateInstance());
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
