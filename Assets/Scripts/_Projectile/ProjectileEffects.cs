using UnityEngine;

public class ProjectileEffects : MonoBehaviour
{
    [Header("Хіт-декаль (спрайт у місці удару)")]
    public HitDecalPool decalPool;
    public float decalOffset = 0.01f;
    public bool randomizeRotation = false;

    [Header("Партикли при зникненні")]
    public PooledParticlesPool destroyParticlesPool;

    public void SpawnHitEffects(RaycastHit hit)
    {
        SpawnHitDecal(hit);
    }

    public void SpawnDestroyEffects(RaycastHit hit)
    {
        if (destroyParticlesPool == null)
            return;

        var effect = destroyParticlesPool.Get(hit.point, Quaternion.identity);

        // орієнтуємо по нормалі
        effect.transform.rotation = Quaternion.LookRotation(hit.normal);
    }

    void SpawnHitDecal(RaycastHit hit)
    {
        if (decalPool == null)
            return;

        HitDecal decal = decalPool.Get(hit.point, Quaternion.identity);
        Transform t = decal.transform;

        // орієнтуємо по нормалі
        t.rotation = Quaternion.LookRotation(hit.normal);

        // опційний рандомний поворот
        if (randomizeRotation)
        {
            t.Rotate(hit.normal, Random.Range(0f, 360f), Space.World);
        }

        // зсуваємо трохи від поверхні
        t.position += hit.normal * decalOffset;
    }
}