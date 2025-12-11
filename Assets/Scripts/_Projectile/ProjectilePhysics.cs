using UnityEngine;

public static class ProjectilePhysics
{
    public struct State
    {
        public Vector3 position;
        public Vector3 velocity;
        public int bounces;
    }

    public struct StepResult
    {
        public bool hit;                 // було зіткнення?
        public RaycastHit hitInfo;       // інфо про хіт (якщо було)
        public bool exceededBounceLimit; // перевищили maxBounces?
    }

    /// <summary>
    /// Один крок симуляції снаряду.
    /// </summary>
    public static StepResult Step(
        ref State state,
        float dt,
        float gravity,
        LayerMask collisionMask,
        float skin,
        float bounceDamping,
        int maxBounces)
    {
        StepResult result = default;

        // гравітація
        state.velocity += Vector3.down * gravity * dt;

        // зміщення
        Vector3 displacement = state.velocity * dt;
        float dist = displacement.magnitude;
        if (dist <= 0.0001f)
            return result; // майже не рухаємось

        Vector3 dir = displacement.normalized;
        Ray ray = new Ray(state.position, dir);

        if (Physics.Raycast(ray, out RaycastHit hit, dist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            // точка удару
            state.position = hit.point + hit.normal * skin;

            // рикошет
            Vector3 reflectedDir = Vector3.Reflect(state.velocity.normalized, hit.normal);
            state.velocity = reflectedDir * state.velocity.magnitude * bounceDamping;

            state.bounces++;

            result.hit = true;
            result.hitInfo = hit;
            result.exceededBounceLimit = state.bounces >= maxBounces;
        }
        else
        {
            // без зіткнення
            state.position += displacement;
        }

        return result;
    }
}