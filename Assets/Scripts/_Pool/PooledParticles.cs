using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticles : MonoBehaviour, IPoolable, IPoolableWithPool<PooledParticles>
{
    public float extraLifetime = 0.2f;

    private ObjectPool<PooledParticles> pool;
    private ParticleSystem ps;
    private Coroutine lifeRoutine;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void SetPool(ObjectPool<PooledParticles> pool)
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

        if (ps == null) ps = GetComponent<ParticleSystem>();

        ps.Clear(true);
        ps.Play(true);

        lifeRoutine = StartCoroutine(LifeCoroutine());
    }

    IEnumerator LifeCoroutine()
    {
        if (ps == null)
            yield break;

        var main = ps.main;
        float duration = main.duration;

        var startLifetime = main.startLifetime;
        float maxLifetime = startLifetime.constantMax;

        float waitTime = duration + maxLifetime + extraLifetime;

        yield return new WaitForSeconds(waitTime);

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

        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}