using UnityEngine;

public class ProjectileSurface : MonoBehaviour
{
    [Header("Ефекти")]
    public bool spawnDecal = true;          // ставити декаль при хіті
    public bool spawnDestroyEffect = true;  // вибух/партикли при знищенні

    [Header("Відскок")]
    [Range(0f, 1f)]
    public float bounceMultiplier = 1f;     // 1 = як завжди, 0.2 = слабкий відскок, 0 = взагалі без відскоку
}