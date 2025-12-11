using UnityEngine;

public class ProjectileEffectsManager : MonoBehaviour
{
    [Header("Хіт-декаль (спрайт у місці удару)")]
    public HitDecalPool decalPool;
    public float decalOffset = 0.01f;
    public bool randomizeRotation = false;

    [Header("Партикли при зникненні")]
    public PooledParticlesPool destroyParticlesPool;

    void OnEnable()
    {
        Projectile.OnAnyHit += HandleHit;
        Projectile.OnAnyDestroyedWithEffects += HandleDestroyed;
    }

    void OnDisable()
    {
        Projectile.OnAnyHit -= HandleHit;
        Projectile.OnAnyDestroyedWithEffects -= HandleDestroyed;
    }

    private void HandleHit(RaycastHit hit, bool spawnDecal)
    {
        if (!spawnDecal || decalPool == null)
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

        // трохи від поверхні
        t.position += hit.normal * decalOffset;
    }

    private void HandleDestroyed(RaycastHit hit)
    {
        if (destroyParticlesPool == null)
            return;

        var effect = destroyParticlesPool.Get(hit.point, Quaternion.LookRotation(hit.normal));
    }
}