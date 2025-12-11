using UnityEngine;
using UnityEngine.UI;

public class ProjectileLauncher : MonoBehaviour, IShotPowerReceiver
{
    [Header("Посилання")]
    public Transform firePoint;
    public Projectile projectilePrefab;
    public ProjectileTrajectory trajectory;
    public CannonRecoil cannonRecoil;
    public CameraShake cameraShake;
    public ObjectPool<Projectile> projectilePool;


    [Header("Постріл")]
    public float baseForce = 30f;          // базова сила
    public float minShotPower = 0.1f;      // 0–1
    public float maxShotPower = 1f;
    public float minFlightSpeed = 0.5f;
    public float maxFlightSpeed = 2.0f;
    [Range(0f, 1f)]
    public float currentShotPower = 1f;
    [Range(0f, 1f)]
    public float currentFlightSpeed = 1f;

    [Header("Форма кулі")]
    public float minRandomAmplitude = 0.0f;
    public float maxRandomAmplitude = 0.4f;


    public void SetShotPower(float value)
    {
        currentShotPower = Mathf.Clamp01(value);
    }


    void Update()
    {
        // постріл (як було)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        // малюємо траєкторію кожен кадр
        if (trajectory != null)
        {
            UpdateTrajectoryPreview();
        }
    }


    void UpdateTrajectoryPreview()
    {
        if (firePoint == null || projectilePrefab == null)
            return;

        float shotPower01 = Mathf.Clamp01(currentShotPower);
        float flightSpeed01 = Mathf.Clamp01(currentFlightSpeed);

        float shotPower = Mathf.Lerp(minShotPower, maxShotPower, shotPower01);
        float flightSpeed = Mathf.Lerp(minFlightSpeed, maxFlightSpeed, flightSpeed01);

        Vector3 dir = firePoint.forward.normalized;
        Vector3 initialVelocity = dir * baseForce * shotPower;

        // беремо параметри фізики з префаба
        float gravity = projectilePrefab.gravity;

        // trajectory.maxBounces = projectilePrefab.maxBounces;
        trajectory.bounceDamping = projectilePrefab.bounceDamping;

        trajectory.DrawTrajectory(
            firePoint.position,
            initialVelocity,
            gravity,
            flightSpeed
        );
    }



    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        // Нормалізуємо діапазони
        float shotPower01 = Mathf.Clamp01(currentShotPower);
        float flightSpeed01 = Mathf.Clamp01(currentFlightSpeed);

        float shotPower = Mathf.Lerp(minShotPower, maxShotPower, shotPower01);
        float flightSpeed = Mathf.Lerp(minFlightSpeed, maxFlightSpeed, flightSpeed01);
        float randomAmplitude = Mathf.Lerp(minRandomAmplitude, maxRandomAmplitude, shotPower01);

        // Напрямок – куди дивиться гармата / ствол
        Vector3 dir = firePoint.forward.normalized;

        Vector3 initialVelocity = dir * baseForce * shotPower;

       Projectile proj;

if (projectilePool != null)
{
    proj = projectilePool.Get(firePoint.position, firePoint.rotation);
}
else
{
    proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
}


        proj.Init(
            firePoint.position,
            initialVelocity,
            flightSpeed,
            randomAmplitude
        );

        //effects
        cannonRecoil.PlayRecoil();

        cameraShake.Shake();
    }
}