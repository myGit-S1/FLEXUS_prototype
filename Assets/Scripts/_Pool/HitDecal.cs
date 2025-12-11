using UnityEngine;
using System.Collections;

public class HitDecal : MonoBehaviour, IPoolable, IPoolableWithPool<HitDecal>
{
    [Tooltip("Скільки секунд жити декалі (0 = вічно)")]
    public float lifeTime = 8f;

    private ObjectPool<HitDecal> pool;
    private Coroutine lifeRoutine;

    public void SetPool(ObjectPool<HitDecal> pool)
    {
        this.pool = pool;
    }

    public void OnTakenFromPool()
    {
        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
        }

        if (lifeTime > 0f)
            lifeRoutine = StartCoroutine(LifeCoroutine());
    }

    IEnumerator LifeCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);

        if (pool != null)
            pool.Return(this);
    }

    public void OnReturnedToPool()
    {
        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
        }
    }
}