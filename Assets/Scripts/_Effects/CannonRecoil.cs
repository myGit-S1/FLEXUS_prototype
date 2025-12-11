using UnityEngine;

public class CannonRecoil : MonoBehaviour
{
    [Header("Налаштування віддачі")]
    public Transform recoilTarget;   // що рухається (ствол)
    public Transform firePoint;      // точка пострілу (на кінці ствола)
    public float recoilDistance = 0.3f;
    public float recoilTime = 0.05f;
    public float returnTime = 0.1f;

    Vector3 _startPos;
    Vector3 _localForwardDir;   // напрямок ствола в локальних координатах
    bool _isRecoiling;

    void Awake()
    {
        if (recoilTarget == null)
            recoilTarget = transform;

        if (firePoint != null)
        {
            // напрямок ВЗДОВЖ ствола (від бази до дула) в локальних координатах
            Vector3 worldDir = (firePoint.position - recoilTarget.position).normalized;
            _localForwardDir = recoilTarget.InverseTransformDirection(worldDir);
        }
        else
        {
            // запасний варіант – вважаємо, що ствол дивиться по forward
            _localForwardDir = recoilTarget.InverseTransformDirection(recoilTarget.forward);
        }
    }

    public void PlayRecoil()
    {
        if (!_isRecoiling)
            StartCoroutine(RecoilRoutine());
    }

    System.Collections.IEnumerator RecoilRoutine()
    {
        _isRecoiling = true;

        // фіксуємо поточну стартову позицію (на випадок рухомої гармати)
        _startPos = recoilTarget.position;

        // отримаємо поточний світовий напрямок ствола
        Vector3 worldForwardDir = recoilTarget.TransformDirection(_localForwardDir);

        // віддача в протилежний бік
        Vector3 recoilPos = _startPos - worldForwardDir * recoilDistance;

        // рух назад
        float t = 0f;
        while (t < recoilTime)
        {
            t += Time.deltaTime;
            float k = t / recoilTime;
            recoilTarget.position = Vector3.Lerp(_startPos, recoilPos, k);
            yield return null;
        }

        // повернення вперед
        t = 0f;
        while (t < returnTime)
        {
            t += Time.deltaTime;
            float k = t / returnTime;
            recoilTarget.position = Vector3.Lerp(recoilPos, _startPos, k);
            yield return null;
        }

        recoilTarget.position = _startPos;
        _isRecoiling = false;
    }
}