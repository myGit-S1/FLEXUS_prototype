using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float duration = 0.15f;
    [SerializeField] float magnitude = 0.1f;
    [SerializeField] float frequency = 15f;

    Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        float seed = Random.value * 100f;

        while (elapsed < duration)
        {
            float noiseX = Mathf.PerlinNoise(seed, elapsed * frequency) - 0.5f;
            float noiseY = Mathf.PerlinNoise(seed + 1f, elapsed * frequency) - 0.5f;

            Vector3 offset = new Vector3(noiseX, noiseY, 0) * magnitude;
            transform.localPosition = originalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}