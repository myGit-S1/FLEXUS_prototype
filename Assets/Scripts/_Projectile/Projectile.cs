using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Projectile : MonoBehaviour, IPoolable, IPoolableWithPool<Projectile>
{
    public static event Action<RaycastHit, bool> OnAnyHit;                  // RaycastHit, spawnDecal
    public static event Action<RaycastHit> OnAnyDestroyedWithEffects;

    [Header("Фізика")]
    public float gravity = 9.81f;
    public float speedMultiplier = 1f;
    public float bounceDamping = 0.8f;
    public int maxBounces = 5;
    public float lifeTime = 10f;

    [Header("Колізії")]
    public LayerMask collisionMask = ~0;
    public float skin = 0.01f;

    [Header("Генерація меша")]
public float baseSize = 0.5f;
public float randomAmplitude = 0.2f;

// Стан фізики виносимо в одну структуру
private ProjectilePhysics.State physicsState;
    private float lifeTimer;

    private MeshFilter meshFilter;
    private RaycastHit lastHit;
    private bool hasLastHit = false;

    private bool destroyed = false;

    private ObjectPool<Projectile> pool;

public void SetPool(ObjectPool<Projectile> pool)
{
    this.pool = pool;
}

public void OnTakenFromPool()
{
    // тут можна скидати стани, якщо треба
}

public void OnReturnedToPool()
{
    // тут очищення, якщо треба
}

    public void Init(Vector3 startPos, Vector3 initialVelocity, float speedMul, float randomAmp)
    {
        physicsState = new ProjectilePhysics.State
        {
            position = startPos,
            velocity = initialVelocity,
            bounces = 0
        };

        transform.position = physicsState.position;

        speedMultiplier = speedMul;
        randomAmplitude = randomAmp;

        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        GenerateRandomCubeMesh();

        lifeTimer = 0f;
        destroyed = false;
        hasLastHit = false;
    }

    private void Update()
{
    if (destroyed)
        return;

    float dt = Time.deltaTime * speedMultiplier;
    lifeTimer += Time.deltaTime;

    if (lifeTimer >= lifeTime)
    {
        // час життя вийшов – без ефектів зникнення
        DestroyProjectile(false);
        return;
    }

    // Один крок через спільну фізику
    var result = ProjectilePhysics.Step(
        ref physicsState,
        dt,
        gravity,
        collisionMask,
        skin,
        bounceDamping,
        maxBounces
    );

    // оновлюємо позицію трансформа
    transform.position = physicsState.position;

    // якщо хіта не було – далі нічого не робимо
    if (!result.hit)
        return;

    // --- ЛОГІКА ПОВЕРХНІ ---

    ProjectileSurface surface = result.hitInfo.collider.GetComponent<ProjectileSurface>();

    bool spawnDecal = surface == null ? true  : surface.spawnDecal;
    bool spawnDestroyEffect = surface == null ? true  : surface.spawnDestroyEffect;
    float surfaceBounceMultiplier = surface == null ? 1f : surface.bounceMultiplier;

    // 🔹 тепер просто шлемо подію, а НЕ спавнимо декаль напряму
    OnAnyHit?.Invoke(result.hitInfo, spawnDecal);

    lastHit = result.hitInfo;
    hasLastHit = true;

    // Якщо поверхня "поглинає" снаряд (bounceMultiplier == 0) — просто знищуємо
    if (surfaceBounceMultiplier <= 0f)
    {
        DestroyProjectile(spawnDestroyEffect);
        return;
    }

    // Додатково масштабуємо швидкість під матеріал поверхні
    physicsState.velocity *= surfaceBounceMultiplier;

    // Якщо перевищили кількість рикошетів — знищуємо, з урахуванням налаштувань поверхні
    if (result.exceededBounceLimit)
    {
        DestroyProjectile(spawnDestroyEffect);
        return;
    }
}

    private void GenerateRandomCubeMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "RandomCubeProjectile";

        float s = baseSize;

        // 8 вертексів куба
        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(-s, -s, -s);
        vertices[1] = new Vector3( s, -s, -s);
        vertices[2] = new Vector3( s,  s, -s);
        vertices[3] = new Vector3(-s,  s, -s);
        vertices[4] = new Vector3(-s, -s,  s);
        vertices[5] = new Vector3( s, -s,  s);
        vertices[6] = new Vector3( s,  s,  s);
        vertices[7] = new Vector3(-s,  s,  s);

        // Рандомно розтягуємо вертекси

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 dir = vertices[i].normalized;
            float scale = 1f + UnityEngine.Random.Range(-randomAmplitude, randomAmplitude);
            vertices[i] = dir * baseSize * scale;
        }

        int[] triangles = new int[]
        {
            // front (z+): 4,5,6, 4,6,7
            4,5,6,
            4,6,7,
            // back (z-): 0,2,1, 0,3,2
            0,2,1,
            0,3,2,
            // left (x-): 0,4,7, 0,7,3
            0,4,7,
            0,7,3,
            // right (x+): 1,2,6, 1,6,5
            1,2,6,
            1,6,5,
            // top (y+): 2,3,7, 2,7,6
            2,3,7,
            2,7,6,
            // bottom (y-): 0,1,5, 0,5,4
            0,1,5,
            0,5,4
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }

    private void DestroyProjectile(bool isNeedEffects)
    {
        if (destroyed) return;
        destroyed = true;

        if (isNeedEffects && hasLastHit)
        {
            OnAnyDestroyedWithEffects?.Invoke(lastHit);
        }

        if (pool != null)
        {
            pool.Return(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}