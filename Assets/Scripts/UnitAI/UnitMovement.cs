using System;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Unit owner;

    public bool IsMoving { get; private set; }

    public event Action<bool> OnMovingStateChanged;

    public void Initialize(Unit owner)
    {
        this.owner = owner;
    }

    public void MoveTowards(Vector2 targetPosition, float speed)
    {
        Vector2 currentPos = transform.position;
        Vector2 dir = targetPosition - currentPos;

        if (dir.sqrMagnitude > 0.001f)
        {
            RotateTowards(targetPosition);
        }

        transform.position = Vector2.MoveTowards(currentPos, targetPosition, speed * Time.deltaTime);

        if (!IsMoving)
        {
            IsMoving = true;
            OnMovingStateChanged?.Invoke(true);
        }
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
