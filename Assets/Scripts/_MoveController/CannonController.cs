using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("References")]
    public Transform baseRoot;    
    public Transform cannonPivot; 

    [Header("Base rotation (Yaw)")]
    public float baseRotateSpeed = 90f; 

    [Header("Cannon rotation (Pitch)")]
    public float cannonRotateSpeed = 60f;
    public float minPitch = -5f;
    public float maxPitch = 60f;

    float currentPitch;
    Quaternion cannonInitialRotation;

    void Awake()
    {
        if (baseRoot == null) baseRoot = transform;

        if (cannonPivot != null)
        {
            // запам’ятовуємо початковий поворот (всі осі)
            cannonInitialRotation = cannonPivot.localRotation;
            currentPitch = 0f; // завжди рахуємо від базового стану
        }
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // ← →
        float vertical   = Input.GetAxis("Vertical");   // ↑ ↓

        RotateBase(horizontal);
        RotateCannon(vertical * -1);
    }

    void RotateBase(float input)
    {
        baseRoot.Rotate(0f, input * baseRotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    void RotateCannon(float input)
    {
        if (cannonPivot == null) return;

        currentPitch += input * cannonRotateSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // додаємо обертання тільки по X до початкового повороту
        cannonPivot.localRotation = cannonInitialRotation * Quaternion.Euler(currentPitch, 0f, 0f);
    }
}