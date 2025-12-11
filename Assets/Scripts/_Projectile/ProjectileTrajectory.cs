using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileTrajectory : MonoBehaviour
{
    public int maxPoints = 80;
    public float timeStep = 0.05f;

    public int maxBounces = 5;
    public float bounceDamping = 0.8f;
    public LayerMask collisionMask = ~0;
    public float skin = 0.01f;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    public void Clear()
    {
        if (lr != null)
            lr.positionCount = 0;
    }

    public void DrawTrajectory(Vector3 startPos, Vector3 initialVelocity, float gravity, float speedMultiplier)
    {
        if (lr == null) lr = GetComponent<LineRenderer>();

        // стартовий стан
        ProjectilePhysics.State state = new ProjectilePhysics.State
        {
            position = startPos,
            velocity = initialVelocity,
            bounces = 0
        };

        // перша точка – старт
        lr.positionCount = 1;
        lr.SetPosition(0, state.position);

        int pointIndex = 1;
        int maxPointsClamped = Mathf.Max(2, maxPoints); // мінімум 2 точки

        for (int i = 1; i < maxPointsClamped; i++)
        {
            float dt = timeStep * speedMultiplier;

            var result = ProjectilePhysics.Step(
                ref state,
                dt,
                gravity,
                collisionMask,
                skin,
                bounceDamping,
                maxBounces
            );

            // додаємо нову позицію після кроку
            lr.positionCount = pointIndex + 1;
            lr.SetPosition(pointIndex, state.position);
            pointIndex++;

            // якщо перевищили ліміт – стоп
            if (result.exceededBounceLimit)
                break;

            // якщо швидкість майже нуль – теж стоп
            if (state.velocity.sqrMagnitude <= 0.0001f)
                break;
        }
    }

}