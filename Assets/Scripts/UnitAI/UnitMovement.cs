using System;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Unit owner;
    private const float SeparationRadius = 1.2f;
    private const float SeparationWeight = 0.6f;

    public bool IsMoving { get; private set; }

    public event Action<bool> OnMovingStateChanged;

    public void Initialize(Unit owner)
    {
        this.owner = owner;
    }

    public void MoveTowards(Vector2 targetPosition, float speed)
    {
        Vector2 currentPos = transform.position;
        Vector2 toTarget = targetPosition - currentPos;

        if (toTarget.sqrMagnitude < 0.001f)
        {
            Stop();
            return;
        }

        Vector2 desiredDir = toTarget.normalized;
        Vector2 separation = ComputeSeparation(currentPos);
        Vector2 moveDir = (desiredDir + separation * SeparationWeight).normalized;

        RotateTowards(currentPos + moveDir);

        transform.position = currentPos + moveDir * (speed * Time.deltaTime);

        if (!IsMoving)
        {
            IsMoving = true;
            OnMovingStateChanged?.Invoke(true);
        }
    }

    private Vector2 ComputeSeparation(Vector2 currentPos)
    {
        Vector2 separationForce = Vector2.zero;
        var units = UnitRegistry.UnitList;
        int count = units.Count;

        for (int i = 0; i < count; i++)
        {
            Unit other = units[i];
            if (other == null || other == owner || other.IsDead)
                continue;

            Vector2 diff = currentPos - (Vector2)other.transform.position;
            float distSqr = diff.sqrMagnitude;

            if (distSqr > 0.0001f && distSqr < SeparationRadius * SeparationRadius)
            {
                float dist = Mathf.Sqrt(distSqr);
                float strength = 1f - (dist / SeparationRadius);
                separationForce += (diff / dist) * strength;
            }
        }

        return separationForce;
    }

    public void RotateTowards(Vector2 targetPosition)
    {
        Vector2 dir = targetPosition - (Vector2)transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
        }
    }

    public void Stop()
    {
        if (IsMoving)
        {
            IsMoving = false;
            OnMovingStateChanged?.Invoke(false);
        }
    }
}
