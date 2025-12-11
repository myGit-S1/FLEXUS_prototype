using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Pool settings")]
    public T prefab;
    public int initialSize = 20;

    readonly Queue<T> pool = new Queue<T>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    T CreateNew()
    {
        T obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);

        // якщо об’єкт хоче знати свій пул
        if (obj is IPoolableWithPool<T> withPool)
            withPool.SetPool(this);

        pool.Enqueue(obj);
        return obj;
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
            CreateNew();

        T obj = pool.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);

        if (obj is IPoolable poolable)
            poolable.OnTakenFromPool();

        return obj;
    }

    public void Return(T obj)
    {
        if (obj is IPoolable poolable)
            poolable.OnReturnedToPool();

        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}

// опціональний інтерфейс, якщо об'єкт хоче зберігати посилання на свій пул
public interface IPoolableWithPool<T> where T : MonoBehaviour
{
    void SetPool(ObjectPool<T> pool);
}